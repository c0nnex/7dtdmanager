using System;
namespace _7DTDManager.Interfaces
{

    public delegate void AreaEventDelegate(IAreaDefiniton area, AreaEventArgs e);

    public interface IAreaDefiniton : IIdentifyable
    {        
        IPosition Center { get; }
        double SizeX { get; }
        double SizeZ { get; }

        bool IsInside(IPosition pos);
        bool IsNear(IPosition pos);

        event AreaEventDelegate AreaEvent;

        void OnPlayerEnter(IPlayer player);
        void OnPlayerLeave(IPlayer player);
        void OnDestroy();
    }

    public class AreaEventArgs : EventArgs
    {
        public IPlayer Player;
        public AreaEventType EventType;

        public AreaEventArgs(IPlayer p, AreaEventType eventType)
        {
            Player = p;
            EventType = eventType;
        }
    }
    
    public enum AreaEventType
    {
        Init,
        PlayerEnter,
        PlayerLeave,
        PlayerStay,
        Destroyed,
        Expire
    }
}
