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

        public AreaDefiniton()
        {

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

        IPosition IAreaDefiniton.Center
        {
            get { return Center; }
        }
    }
}
