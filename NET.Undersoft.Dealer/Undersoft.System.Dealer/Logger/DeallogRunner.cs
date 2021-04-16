using System;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace System.Dealer
{
    public static class DeallogRunner
    {
        public static void Start()
        {
            Deallog.Start(2);
        }
        public static void Stop()
        {
            Deallog.Stop();
        }
    }
}
