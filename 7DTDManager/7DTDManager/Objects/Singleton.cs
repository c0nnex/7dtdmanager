using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public class Singleton<T>  where T : ISingleton,new()
    {
        public static T instance;

        public Singleton() 
        {            
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public virtual void InitInstance()
        { }

        public static void Init()
        {
            Instance.InitInstance();
        }
    }

    public interface ISingleton
    {
        void InitInstance();
    }
}
