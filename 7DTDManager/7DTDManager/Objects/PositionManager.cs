using _7DTDManager.Interfaces;
using _7DTDManager.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public class PositionManager : Singleton<PositionManager> , ISingleton, IPositionManager
    {
        List<IPositionTrackable> trackedObjects = new List<IPositionTrackable>();
        object lockObject = new Object();

        public PositionManager ()
        {
            PlayersManager.Instance.PlayerMoved += Instance_PlayerMoved;
        }

        public void AddTrackableObject(IPositionTrackable trackable)
        {
            lock (lockObject)
            {
                trackedObjects.Add(trackable);
            }
        }

        public void RemoveTrackableObject(IPositionTrackable trackable)
        {
            lock (lockObject)
            {
                trackedObjects.Remove(trackable);
            }
        }

        public bool IsTracked(IPositionTrackable trackable)
        {
            return trackedObjects.Contains(trackable);
        }

        void Instance_PlayerMoved(object sender, PlayerMovementEventArgs e)
        {
            lock (lockObject)
            {
                foreach (var item in trackedObjects)
                {
                    item.TrackPosition(sender as IPlayer, e.OldPosition, e.NewPosition);
                }
            }
        }

        public bool SomeoneNearTrackable()
        {
            var players = (from a in Program.Server.AllPlayers.Players where a.IsOnline select a);
            if ((players == null) || (players.Count() == 0))
                return false;
            foreach (var track in trackedObjects)
            {
                foreach (var p in players)
                {
                    if (track.NeedsTracking(p.CurrentPosition))
                        return true;
                }
            }
            return false;
        }

        void ISingleton.InitInstance()
        {
            
        }
    }
}
