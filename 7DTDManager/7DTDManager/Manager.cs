using _7DTDManager.Commands;
using _7DTDManager.LineHandlers;
using _7DTDManager.Interfaces;
using NLog;
using PrimS.Telnet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace _7DTDManager
{

    public delegate void ServerLineHandler(string currentLine);

    public class Manager : IServerConnection
    {       
        static Logger logger = LogManager.GetCurrentClassLogger();
     
        Client serverConnection = null;
        public Players allPlayers = new Players();
             
        DateTime lastPayday = DateTime.Now;
        DateTime lastLP = DateTime.Now;

        public Manager()
        {
            LineManager.Init();
            CommandManager.Init();
            allPlayers = Players.Load();
           
        }

        internal bool Connect()
        {
            try
            {
                logger.Info("Trying to connect....");
                serverConnection = new Client(Program.config.ServerHost, Program.config.ServerTelnetPort, new System.Threading.CancellationToken());
            }
            catch (Exception ex)
            {
                logger.Warn("Connection failed");
                logger.Warn(ex.ToString());
                return false;
            }
            return serverConnection.IsConnected;
        }

        internal void ConnectToServer()
        {
            bIsFirst = true;
            serverConnection = null;
            while (!Connect())
                Thread.Sleep(10000);
           
            logger.Info("Connected");
            
        }

        bool bIsFirst = true;
        string line;
        Queue<string> linesToProcess = new Queue<string>();
        internal void Process()
        {
            if ( bIsFirst )
            {
                string prompt = serverConnection.TerminatedRead("password:");
                serverConnection.WriteLine(Program.config.ServerPassword);
                bIsFirst = false;
                serverConnection.WriteLine("lp");
                PublicMessage("Extended Servercommands V1.1 online. See /help");
            }
            line += serverConnection.Read();
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
                    ConnectToServer();
                }
                lastLP = DateTime.Now;
            }
            
        }
                                                        
        private void ProcessLines()
        {
            while ( linesToProcess.Count > 0 )
            {
                string currentLine = linesToProcess.Dequeue();
                LineManager.ProcessLine(this, currentLine);
            }
            allPlayers.Save();
        }

        public void PrivateMessage(IPlayer p,string msg,params object[] args)
        {
            serverConnection.WriteLine(String.Format("pm {0} {1}", p.EntityID, String.Format(msg, args)));
        }

        public void PublicMessage(string msg, params object[] args)
        {
            serverConnection.WriteLine(String.Format("say "+msg,args));
        }

        public void Execute(string cmd,params object[] args)
        {
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

        IPlayers IServerConnection.allPlayers
        {
            get { return allPlayers as IPlayers; }
        }

        
    }
}
