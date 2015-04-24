using System;
using System.Collections.Generic;
namespace _7DTDManager.Interfaces
{
    public interface IPosition : IEquatable<IPosition>
    {
        double X { get; }
        double Y { get;  }
        double Z { get; }
        IPosition InvalidPosition { get; }

        IPosition Clone();
       
        bool IsValid {get;}
        string ToCommandString();
        string ToHumanString();
        string ToString();

        double Distance(IPosition other);
        bool IsNear(IPosition pos, double maxDistance);
    }

    
}
