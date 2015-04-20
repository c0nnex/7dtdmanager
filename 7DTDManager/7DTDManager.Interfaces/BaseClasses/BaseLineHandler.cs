using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public abstract class BaseLineHandler : IServerLineHandler
    {
        protected ILogger Log;

        public virtual void Init(IServerConnection serverConnection, ILogger logger)
        {
            Log = logger;
        }

        public bool PriorityProcess
        {
            get { return _PriorityProcess; }
            set { _PriorityProcess = value; }
        } private bool _PriorityProcess = false;
        
        public bool Exclusive
        {
            get { return _Exclusive; }
            set { _Exclusive = value; } 
        } private bool _Exclusive = true;

        public abstract bool ProcessLine(IServerConnection serverConnection, string currentLine);



        
    }
}
