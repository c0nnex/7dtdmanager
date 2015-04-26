using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public class AreaDefiniton : IAreaDefiniton
    {
        public Position Center {get;set;}
        public Double SizeX { get; set; }
        public Double SizeZ { get; set; }

        public event AreaEventDelegate AreaEvent;

        public AreaDefiniton()
        {
            Identifier = Guid.NewGuid().ToString("D");
        }

        public AreaDefiniton(IPosition center, double size = 10)
        {
            Center = center as Position;
            SizeX = size;
            SizeZ = size;
        }


        public bool IsInside(IPosition pos)
        {
            double dist = pos.Distance(Center);
            return ( (dist <= SizeX) && (dist<=SizeZ) ) ;
        }

        public bool IsNear(IPosition pos)
        {
            double dist = pos.Distance(Center);
            return dist <= Math.Min(100.0,Program.Config.PositionTrackingRangeFactor * Math.Max(SizeX,SizeZ));
        }

        IPosition IAreaDefiniton.Center
        {
            get { return Center; }
        }


        public void OnPlayerEnter(IPlayer player)
        {
            AreaEventDelegate handler = AreaEvent;
            if (handler != null)
                handler(this, new AreaEventArgs(player, AreaEventType.PlayerEnter));
        }

        public void OnPlayerLeave(IPlayer player)
        {
            AreaEventDelegate handler = AreaEvent;
            if (handler != null)
                handler(this, new AreaEventArgs(player, AreaEventType.PlayerLeave));
        }

        public void OnDestroy()
        {
            AreaEventDelegate handler = AreaEvent;
            if (handler != null)
                handler(this, new AreaEventArgs(null, AreaEventType.Destroyed));
        }

        public string Identifier { get; set; }
        
    }
}
