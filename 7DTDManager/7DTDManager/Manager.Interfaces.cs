using _7DTDManager.Interfaces;
using _7DTDManager.Objects;
using _7DTDManager.Players;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager
{
    public partial class Manager : IServerConnection
    {
        public void PrivateMessage(IPlayer p, string msg, params object[] args)
        {
            if (_Testing)
            {
                logger.Debug("Would send to {0}: {1}", p.Name, String.Format(msg, args));
                return;
            }
            serverConnection.WriteLine(String.Format("pm {0} \"{1}\"", p.EntityID, String.Format(msg, args)));
        }

        public void PublicMessage(string msg, params object[] args)
        {
            if (_Testing)
            {
                logger.Debug("Would send: {0}", String.Format(msg, args));
                return;
            }
            serverConnection.WriteLine(String.Format("say \"" + msg + "\"", args));
        }

        public void Execute(string cmd, params object[] args)
        {
            logger.Info("Execute: {0}", String.Format(cmd, args));
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
            get { return PlayersManager.Instance; }
        }

        public IPosition CreatePosition(string pos)
        {
            return Position.FromString(pos);
        }

        public ICalloutManager CalloutManager
        {
            get { return CalloutManagerImpl.Instance; }
        }


        public IConfiguration Configuration
        {
            get { return Program.Config; }
        }


        public IAreaDefiniton CreateArea(IPlayer p, IPosition pos, double size = 10.0)
        {
            IAreaDefiniton area = new AreaDefiniton { Center = pos as Position, SizeX = size, SizeZ = size };
            Players.PlayersManager.Instance.OnAreaInitialize(area,p);
            return area;
        }


        public string SavePath
        {
            get 
            {
                return PlayersManager.ProfilePath;
            }
        }

        IPositionManager IServerConnection.PositionManager
        {
            get { return PositionManager.Instance; }
        }


        public IShop GlobalShop
        {
            get
            {
                return Program.Config.GlobalShop;
            }
        }
    }
}
