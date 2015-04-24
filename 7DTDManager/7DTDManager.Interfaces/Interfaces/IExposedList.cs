using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface IExposedList<ExposedType>
    {
        IReadOnlyList<ExposedType> Items { get; }
        void Add(ExposedType value);
        void Remove(ExposedType value);
        void RemoveAt(int index);
        void Clear();
    }
}
