using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface IPositionManager
    {
        void AddTrackableObject(IPositionTrackable trackable);
        bool IsTracked(IPositionTrackable trackable);
        void RemoveTrackableObject(IPositionTrackable trackable);
        bool SomeoneNearTrackable();
    }
}
