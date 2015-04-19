using _7DTDManager.Interfaces;
using _7DTDManager.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public static class PositionManager
    {
        static List<IPositionTrackable> trackedObjects = new List<IPositionTrackable>();
        static object lockObject = new Object();

        public static void Init()
        {
            PlayersManager.Instance.PlayerMoved += Instance_PlayerMoved;
        }

        public static void AddTrackableObject(IPositionTrackable trackable)
        {
            lock (lockObject)
            {
                trackedObjects.Add(trackable);
            }
        }

        public static void RemoveTrackableObject(IPositionTrackable trackable)
        {
            lock (lockObject)
            {
                trackedObjects.Remove(trackable);
            }
        }

        static void Instance_PlayerMoved(object sender, PlayerMovementEventArgs e)
        {
            lock (lockObject)
            {
                foreach (var item in trackedObjects)
                {
                    item.TrackPosition(sender as IPlayer, e.OldPosition, e.NewPosition);
                }
            }
        }

        public static bool SomeoneNearTrackable()
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
    }
}
