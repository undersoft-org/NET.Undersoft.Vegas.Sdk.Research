using System.Diagnostics;
using System.Collections.Generic;
using System.Multemic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Xunit;

namespace Undersoft.Tests.System.Multemic
{
    public class Catalog64Test : SharedAlbumTestHelper
    {
        public static object holder = new object();
        public static int threadCount = 0;
        public Task[] s1 = new Task[8];

        public Catalog64Test() : base()
        {
            registry = new Catalog64<string>();
            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Catalog64_{DateTime.Now.ToFileTime().ToString()}_Test.log";
        }

        private void catalog64_MultiThread_TCallback_Test(Task[] t)
        {
            Debug.WriteLine($"Test Finished");
        }

        private Task catalog64_MultiThread_Test(IList<KeyValuePair<object, string>> collection)
        {
            Action publicTest = () =>
            {
                int c = 0;
                lock (holder)
                    c = threadCount++;

                SharedAlbum_ThreadIntegrated_Test(collection.Skip(c * 10000).Take(10000).ToArray());
            };

            
            for (int i = 0; i < 8; i++)
            {               
                s1[i] = Task.Factory.StartNew(publicTest);                       
            }

            return Task.Factory.ContinueWhenAll(s1, new Action<Task[]>(a => { catalog64_MultiThread_TCallback_Test(a); }));           
        }

        [Fact]
        public async Task Catalog64_StringKeys_Test()
        {
            await catalog64_MultiThread_Test(stringKeyTestCollection).ConfigureAwait(true);
        }

        [Fact]
        public void Album64_StringKeys_Test()
        {
            SharedAlbum_Integrated_Test(stringKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public async Task Catalog64_IntKeys_Test()
        {
            await catalog64_MultiThread_Test(intKeyTestCollection).ConfigureAwait(true);      
        }

        [Fact]
        public void Album64_IntKeys_Test()
        {
            SharedAlbum_Integrated_Test(intKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public async Task Catalog64_LongKeys_Test()
        {
            await catalog64_MultiThread_Test(longKeyTestCollection).ConfigureAwait(true);
        }

        [Fact]
        public void Album64_LongKeys_Test()
        {
            SharedAlbum_Integrated_Test(longKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public async Task Catalog64_IndentifierKeys_TestAsync()
        {
            await catalog64_MultiThread_Test(identifierKeyTestCollection).ConfigureAwait(true);                   
        }

        [Fact]
        public void Album64_IndentifierKeys_Test()
        {
            SharedAlbum_Integrated_Test(identifierKeyTestCollection.Take(100000).ToArray());
        }

    }
}
