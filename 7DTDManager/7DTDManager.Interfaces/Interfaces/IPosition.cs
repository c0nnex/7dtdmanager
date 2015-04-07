using System;
namespace _7DTDManager.Interfaces
{
    public interface IPosition
    {
        double X { get; }
        double Y { get;  }
        double Z { get; }
        IPosition InvalidPosition { get; }

        IPosition Clone();
       
        bool IsValid();
        string ToCommandString();
        string ToHumanString();
        string ToString();
        
    }
}
