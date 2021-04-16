using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Instants.Sqlbank
{
    public class NetEcho : Exception
    {
        public NetEcho(string message)
            : base(message)
        {
        }
    }
}
