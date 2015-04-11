using _7DTDManager.Commands;
using _7DTDManager.LineHandlers;
using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using _7DTDManager.Network;
using System.Net;

namespace _7DTDManager
{

    public delegate void ServerLineHandler(string currentLine);

    public class Manager : IServerConnection,IDisposable
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        EventDrivenTCPClient serverConnection = null;
        private PlayersManager allPlayers = new PlayersManager();

        DateTime lastPayday = DateTime.Now;
        DateTime lastLP = DateTime.Now;

        // Set to true to have nothing send to the server but lp command
        private bool _Testing = false;

        public Manager()
        {
            LineManager.Init();
            CommandManager.Init();
            allPlayers = PlayersManager.Load();
           
        }

        internal bool Connect()
        {
            try
            {
                bIsFirst = true;
                logger.Info("Trying to connect....");
                IPAddress addr = IPAddress.Loopback;
                if (!IPAddress.TryParse(Program.Config.ServerHost, out addr))
                {
                    IPAddress[] addresses = Dns.GetHostAddresses(Program.Config.ServerHost);
                    if (addresses.Length == 0)
                        throw new InvalidOperationException("Host not found");
                    addr = addresses[0];
                }
                serverConnection = new EventDrivenTCPClient(addr, Program.Config.ServerTelnetPort, true);
                serverConnection.DataReceived += serverConnection_DataReceived;
                serverConnection.ConnectionStatusChanged += serverConnection_ConnectionStatusChanged;
                serverConnection.Connect();
                if (_Testing)
                    CommandsDisabled = true;
            }
            catch (Exception ex)
            {
                logger.Warn("Connection failed");
                logger.Warn(ex.ToString());
                return false;
            }
            return serverConnection.IsConnected;
        }

        void serverConnection_ConnectionStatusChanged(object sender, ConnectionStatusEventArgs status)
        {
            logger.Info("ConnectionStatus: {0}", status.ConnectionStatus.ToString());
        }

        bool bIsFirst = true;
        string line;
        Queue<string> linesToProcess = new Queue<string>();

        void serverConnection_DataReceived(object sender, DataReceivedEventArgs e)
        {

            if (bIsFirst) // First Receive, we assume password. This is lazy i know!
            {
                serverConnection.WriteLine(Program.Config.ServerPassword);
                bIsFirst = false;
                serverConnection.WriteLine("lp");
                PublicMessage("Extended Servercommands {0} online. See /help for commands", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
            line += Convert.ToString(e.Data);

            if (!String.IsNullOrEmpty(line))
            {
                string[] newLines = line.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (!line.EndsWith("\r\n"))
                {
                    line = newLines[newLines.Length - 1];
                    newLines[newLines.Length - 1] = String.Empty;
                }
                else
                    line = String.Empty;
                foreach (var item in newLines)
                {
                    if (!String.IsNullOrEmpty(item))
                        linesToProcess.Enqueue(item);
                }
                //logger.Info("Stack: {0} Rest: {1}", linesToProcess.Count, line);
            }
            ProcessLines();
            TimedActions();
        }

        // No Timer needed here, because Server will trigger us every x seconds with stats, if someone is online
        private void TimedActions()        
        {
            
            TimeSpan span = DateTime.Now - lastPayday;
            if (span.Minutes > 1)
            {
                logger.Info("Payday!");
                allPlayers.Payday();
                lastPayday = DateTime.Now;
            }
            span = DateTime.Now - lastLP;
            if (span.Minutes > 1)
            {
                try
                {
                    serverConnection.WriteLine("lp");
                }
                catch
                {
                    logger.Info("Server Disconnected... Trying to reconnect");
                    allPlayers.Save();                   
                }
                lastLP = DateTime.Now;
            }

        }

        private void ProcessLines()
        {
            while (linesToProcess.Count > 0)
            {
                string currentLine = linesToProcess.Dequeue();
                LineManager.ProcessLine(this, currentLine);
            }
            allPlayers.Save();
        }

        public void PrivateMessage(IPlayer p, string msg, params object[] args)
        {
            if (_Testing)
            {
                logger.Debug("Would send to {0}: {1}", p.Name, String.Format(msg, args));
                return;
            }
            serverConnection.WriteLine(String.Format("pm {0} {1}", p.EntityID, String.Format(msg, args)));
        }

        public void PublicMessage(string msg, params object[] args)
        {
            if (_Testing)
            {
                logger.Debug("Would send: {0}", String.Format(msg, args));
                return;
            }
            serverConnection.WriteLine(String.Format("say " + msg, args));
        }

        public void Execute(string cmd, params object[] args)
        {
            if (_Testing)
            {
                logger.Debug("Execute: {0}", String.Format(cmd, args));
            }
            serverConnection.WriteLine(String.Format(cmd, args));
        }


        public bool IsConnected
        {
            get { return serverConnection.IsConnected; }
        }

        public bool CommandsDisabled
        {
            get { return _CommandsDisabled; }
            set { _CommandsDisabled = value; }
        } private bool _CommandsDisabled;

        public IPlayersManager AllPlayers
        {
            get { return allPlayers as IPlayersManager; }
        }


       protected void Dispose(bool bAll)
        {
            if (serverConnection != null)
            {
                serverConnection.Dispose();
                serverConnection = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
