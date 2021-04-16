using System;
using System.Instants;
using System.Threading;
using System.Collections.Concurrent;

namespace System.Dealer
{
    public static class Deallog
    {
        public static IDeallogger DeallogEvent { get; set; }

        private static int _logLevel = 0;

        private static ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();

        private static Thread oThread = new Thread(new ThreadStart(HandleProc));
        
        private static DateTime ClearLogTime = DateTime.Now.AddDays(-1).AddHours(-1).AddMinutes(-1);

        public static bool threadLive = true;

        public static IDeallogger Logger { get { return DeallogEvent; } }  

        public static void Log(int requiredLogLevel, String message, bool format = false)
        {
            try
            {
                if (_logLevel >= requiredLogLevel)
                {
                    string _message = $"{requiredLogLevel.ToString()}#Information#{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}#{DateTime.Now.Millisecond.ToString()}#{message}";
                    logQueue.Enqueue(_message);
                }
            }
            catch
            {

            }
        }
        public static void Log(int requiredLogLevel, Exception exp)
        {
            try
            {
                if (_logLevel >= requiredLogLevel)
                {
                    string message = requiredLogLevel.ToString() + "#Exception#" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "#" + DateTime.Now.Millisecond.ToString()
                                                                 + "#" + exp.Message
                                                                 + "\r\n" + exp.Source
                                                                 + "\r\n" + exp.StackTrace;
                    logQueue.Enqueue(message);
                }
            }
            catch
            {

            }
        }

        private static void HandleProc()
        {
            //LogClear();
            while (threadLive)
            {
                try
                {
                    Thread.Sleep(1000);
                    if (logQueue.Count > 0)
                    {
                        while (logQueue.Count > 0)
                        {
                            string message;
                            if (logQueue.TryDequeue(out message))
                            {
                                if (Logger != null)
                                    Logger.WriteLog(message);
                            }
                        }
                    }
                    //if (DateTime.Now.Day != ClearLogTime.Day)
                    //{
                    //    if (DateTime.Now.Hour != ClearLogTime.Hour)
                    //    {
                    //        if (DateTime.Now.Minute != ClearLogTime.Minute)
                    //        {
                    //            Echolog.LogClear();
                    //            ClearLogTime = DateTime.Now;
                    //        }
                    //    }
                    //}
                }
                catch
                {

                }
            }
        }

        public static void Start(int logLevel)
        {
            _logLevel = logLevel;
            oThread.IsBackground = true;
            oThread.Start();
        }
        public static void Start()
        {
            _logLevel = 2;
            oThread.IsBackground = true;
            oThread.Start();
        }

        public static void Stop()
        {
            threadLive = false;
            oThread.Join();
        }

        public static void LogClear()
        {
            try
            {
                DateTime time = DateTime.Now.AddDays(-7);
            }
            catch (Exception ex)
            {
                Log(1, ex);
            }

        }
    }

    public class DeallogWriter : IDeallogger
    {
        IDeputy writer { get; set; } 

        public DeallogWriter(IDeputy writeevent)
        {
            writer = writeevent;
        }

        public void WriteLog(string message)
        {
            writer.Execute(message);
        }

        
    }
}
