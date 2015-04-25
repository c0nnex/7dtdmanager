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
using _7DTDManager.Objects;
using _7DTDManager.Players;
using System.Timers;

namespace _7DTDManager
{

    public delegate void ServerLineHandler(string currentLine);

    public partial class Manager : IServerConnection,IDisposable
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        EventDrivenTCPClient serverConnection = null;
        private PlayersManager allPlayers = new PlayersManager();
        private System.Timers.Timer pollTimer = null;

        DateTime lastPayday = DateTime.Now;
        DateTime lastLP = DateTime.Now;

        // Set to true to have nothing send to the server but lp command
#if DEBUG
        private bool _Testing = false;
#else
        private bool _Testing = false;
#endif

        public Manager()
        {            
            allPlayers = PlayersManager.Load();
            PositionManager.Init();
            pollTimer = new System.Timers.Timer();
            pollTimer.Interval = Program.Config.PollInterval;
            pollTimer.AutoReset = true;
            pollTimer.Elapsed += pollTimer_Elapsed;            
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
            if (status.ConnectionStatus == ConnectionStatus.DisconnectedByHost)
            {
                pollTimer.Stop();
                bIsFirst = true; // Make Sure login again
            }
            if (status.ConnectionStatus == ConnectionStatus.Connected)
            {
                LineManager.Init(this);
                pollTimer.Start();
            }
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
                serverConnection.WriteLine("allitems");
                serverConnection.WriteLine("lp");
                PublicMessage("7DTDManager Servercommands {0} online. See /help for commands", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
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
        }

        void pollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            pollTimer.Interval = Program.Config.PollInterval;

            // Check if someone is online and near a tracked area (e.g. shop )
            if (allPlayers.OnlinePlayers > 0)
            {
                if (PositionManager.SomeoneNearTrackable())
                {
                    pollTimer.Interval = Program.Config.PositionInterval;
                    logger.Trace("Someone near trackable Area new Poll Intervall {0}", Program.Config.PositionInterval);
                }
            }

            if (CalloutManagerImpl.NextCallout <= DateTime.Now)
                CalloutManagerImpl.UpdateCallouts();

            // Check for Short callouts
            double nextCalloutIntervall = (CalloutManagerImpl.NextCallout - DateTime.Now).TotalMilliseconds;
            if ((nextCalloutIntervall > 0) && (nextCalloutIntervall < pollTimer.Interval))
            {
                logger.Info("short callout. new interval {0}", nextCalloutIntervall);
                pollTimer.Interval = nextCalloutIntervall;

            }

            TimeSpan span = DateTime.Now - lastPayday;

            if (span.Minutes > Program.Config.PaydayInterval)
            {
                if (allPlayers.OnlinePlayers > 0)
                {
                    logger.Info("Payday!");
                    allPlayers.Payday();
                }
                lastPayday = DateTime.Now;
            }

            if ( (serverConnection.ConnectionState == ConnectionStatus.Connected) && (allPlayers.OnlinePlayers > 0))
            {
                span = DateTime.Now - lastLP;
                if (span.TotalMilliseconds > Program.Config.PositionInterval)
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
            Program.Config.Save();
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
