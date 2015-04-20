using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager.Objects
{
    [Serializable]
    public sealed class Position : IPosition, IEquatable<IPosition>
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        [XmlAttribute]
        public Double X { get; set; }
        [XmlAttribute]
        public Double Y { get; set; }
        [XmlAttribute]
        public Double Z { get; set; }

        public bool IsValid
        {
            get { return ((X != InvalidPosition.X) && (Y != InvalidPosition.Y) && (Z != InvalidPosition.Z)); }
        }

        public static readonly Position InvalidPosition = new Position { X = Int64.MinValue, Y = Int64.MinValue, Z = Int64.MinValue };



        public Position Clone()
        {
            return new Position { X = this.X, Y = this.Y, Z = this.Z };
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", X, Y, Z);
        }
        public string ToHumanString()
        {
            return String.Format("{0}{1}, {2}{3}", Math.Abs(Z), Z < 0 ? "S" : "N", Math.Abs(X), X < 0 ? "W" : "E");
        }

        public string ToCommandString()
        {
            return String.Format("{0} {1} {2}", (int)X+1, (int)Y+1, (int)Z+1);
        }


        IPosition IPosition.InvalidPosition
        {
            get { return Position.InvalidPosition as IPosition; }
        }

        IPosition IPosition.Clone()
        {
            return Clone() as IPosition;
        }

        public bool Equals(IPosition other)
        {
            return ((X == other.X) && (Y == other.Y) && (Z == other.Z));
        }

        public double Distance(IPosition other)
        {
            return Math.Sqrt(((X - other.X) * (X - other.X) + (Z - other.Z) * (Z - other.Z)));
        }

        public static IPosition FromString(string pos)
        {
            try
            {
                string[] p = pos.Split(new char[] { ',' });
                Position CurrentPosition = new Position
                {
                    X = Convert.ToDouble(p[0].Trim().ToLowerInvariant()),
                    Y = Convert.ToDouble(p[1].Trim().ToLowerInvariant()),
                    Z = Convert.ToDouble(p[2].Trim().ToLowerInvariant())
                };
                return CurrentPosition;
            }
			catch ( Exception ex )
            {
                logger.Error("error parsing position '{0}' : {1}", pos, ex.Message);
                return null;
            }
        }
    }

   
}
