using System;
namespace _7DTDManager.Interfaces
{
    public interface IAreaDefiniton
    {
        IPosition Center { get; }
        double SizeX { get; }
        double SizeZ { get; }

        bool IsInside(IPosition pos);
    }
}
