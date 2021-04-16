using System.Collections.Generic;
using System.Multemic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Xunit;

namespace Undersoft.Tests.System.Multemic
{
    public class Board64_Test : SharedDeckTestHelper
    {
        public object holder = new object();
        public static int threadCount = 0;
        public Task[] s1 = new Task[8];

        public Board64_Test() : base()
        {
            registry = new Board64<string>();
            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Board64_{DateTime.Now.ToFileTime().ToString()}_Test.log";
        }

        private void publicBoard_MultiThread_TCallback_Test(Task[] t)
        {
            Debug.WriteLine($"Test Finished");
        }

        private Task board64_MultiThread_Test(IList<KeyValuePair<object, string>> collection)
        {
            registry = new Board64<string>();
            Action publicTest = () =>
            {
                int c = 0;
                lock (holder)
                    c = threadCount++;

                SharedDeck_ThreadIntegrated_Test(collection.Skip(c * 10000).Take(10000).ToArray());
            };

            for (int i = 0; i < 8; i++)
            {
                s1[i] = Task.Factory.StartNew(publicTest);

            }

            return Task.Factory.ContinueWhenAll(s1, new Action<Task[]>(a => { publicBoard_MultiThread_TCallback_Test(a); }));

        }

        [Fact]
        public async Task Board64_StringKeys_Test()
        {
            Task t = board64_MultiThread_Test(stringKeyTestCollection);
            await t.ConfigureAwait(true);
        }

        [Fact]
        public void Deck64_StringKeys_Test()
        {
            SharedDeck_ThreadIntegrated_Test(stringKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public async Task Board64_IntKeys_Test()
        {
            Task t = board64_MultiThread_Test(intKeyTestCollection);
            await t.ConfigureAwait(true);
        }

        [Fact]
        public void Deck64_IntKeys_Test()
        {
            SharedDeck_ThreadIntegrated_Test(intKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public async Task Board64_LongKeys_Test()
        {
            Task t = board64_MultiThread_Test(longKeyTestCollection);
            await t.ConfigureAwait(true);
        }

        [Fact]
        public void Deck64_LongKeys_Test()
        {
            SharedDeck_ThreadIntegrated_Test(longKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public async Task Board64_IndentifierKeys_Test()
        {
            Task t = board64_MultiThread_Test(identifierKeyTestCollection);
            await t.ConfigureAwait(true);
        }

        [Fact]
        public void Deck64_IndentifierKeys_Test()
        {
            SharedDeck_ThreadIntegrated_Test(identifierKeyTestCollection.Take(100000).ToArray());
        }
    }
}
