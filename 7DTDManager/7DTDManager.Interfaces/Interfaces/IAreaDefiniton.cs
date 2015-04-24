using System;
namespace _7DTDManager.Interfaces
{
    public interface IAreaDefiniton
    {
        IPosition Center { get; }
        double SizeX { get; }
        double SizeZ { get; }

        bool IsInside(IPosition pos);
        bool IsNear(IPosition pos);
    }

    public interface IAreaProtectionEvent
    {
        TimeSpan HowLong { get;  }
        DateTime When { get;  }
        string Who { get;  }
    }

    public interface IAreaProtection : IAreaDefiniton
    {
        DateTime Expires { get; }
        string OwnedBy { get; }
        IPlayer Owner { get;  }
        bool IsExpired { get; }
        int AreaProtectionID { get; }
        IExposedList<IAreaProtectionEvent> RecordedEvents { get; }

        void RecordEvent(AreaProtectionEventType eventType);
        void Update(TimeSpan extend);
        string ToString(IPosition pos);
    }

    public enum AreaProtectionEventType
    {
        PlayerEnter,
        PlayerLeave,
        PlayerStay,
        Destroyed,
        Expire
    }
}
