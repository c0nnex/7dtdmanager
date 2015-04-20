using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager.Objects
{
    [Serializable]
    public class ExposedList<BaseType,ExposedType> : IExposedList<ExposedType> where BaseType : ExposedType
    {
        public List<BaseType> Items = new List<BaseType>();

        [XmlIgnore]
        IReadOnlyList<ExposedType> IExposedList<ExposedType>.Items
        {
            get { return Items as IReadOnlyList<ExposedType>; }
        }

        void IExposedList<ExposedType>.Add(ExposedType value)
        {
            Items.Add((BaseType)value);
        }

        void IExposedList<ExposedType>.Remove(ExposedType value)
        {
            Items.Remove((BaseType)value);
        }

        void IExposedList<ExposedType>.Clear()
        {
            Items.Clear();
        }
    }
}
