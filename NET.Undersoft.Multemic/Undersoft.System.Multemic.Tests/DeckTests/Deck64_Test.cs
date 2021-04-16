using System;
using System.Multemic;
using System.Uniques;
using System.Linq;
using System.Diagnostics;
using Xunit;

namespace Undersoft.Tests.System.Multemic
{
    public class Deck64_Test : DeckTestHelper
    {

        public Deck64_Test() : base()
        {
            registry = new Deck64<string>();
            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Deck64_{DateTime.Now.ToFileTime().ToString()}_Test.log";
        }

        [Fact]
        public void Deck64_StringKeys_Test()
        {
            Deck_Integrated_Test(stringKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Deck64_IntKeys_Test()
        {
            Deck_Integrated_Test(intKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Deck64_LongKeys_Test()
        {
            Deck_Integrated_Test(longKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Deck64_IndentifierKeys_Test()
        {
            Deck_Integrated_Test(identifierKeyTestCollection.Take(100000).ToArray());
        }
        
    }
}
