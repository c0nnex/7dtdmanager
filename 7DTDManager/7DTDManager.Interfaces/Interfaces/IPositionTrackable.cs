using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface IPositionTrackable
    {
        void TrackPosition(IPlayer p, IPosition oldPos, IPosition newPos);
        bool NeedsTracking(IPosition pos);
    }
}
