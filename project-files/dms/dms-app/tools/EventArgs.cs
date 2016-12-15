using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.tools
{
    public class EventArgs<T>
    {
        public T Data { get; private set; }
        public EventArgs(T data)
        {
            Data = data;
        }
    }
}
