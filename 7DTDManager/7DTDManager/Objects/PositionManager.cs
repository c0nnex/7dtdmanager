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

        public static void Init()
        {
            PlayersManager.Instance.PlayerMoved += Instance_PlayerMoved;
        }

        public static void AddTrackableObject(IPositionTrackable trackable)
        {
            trackedObjects.Add(trackable);
        }

        static void Instance_PlayerMoved(object sender, PlayerMovementEventArgs e)
        {
            foreach (var item in trackedObjects)
            {
                item.TrackPosition(sender as IPlayer, e.OldPosition, e.NewPosition);
            }
        }
    }
}
