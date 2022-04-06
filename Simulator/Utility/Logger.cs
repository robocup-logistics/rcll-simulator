using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Opc.UaFx;
using Serilog;
using Serilog.Core;

namespace Simulator.Utility
{
    public class MyLogger
    {
        private readonly bool debug_;
        private readonly string prefix_;
        private readonly string filename;
        private readonly string logsFolder;
        private static Mutex mutex = new Mutex();
        private Serilog.Core.Logger? Logger;
        public enum LogTypes { Info, Error, Warning
        }
        public MyLogger(string prefix, bool debug)
        {
            debug_ = debug;
            prefix_ = "[" + prefix +"] ";
            prefix_ = "";
            logsFolder = "logs" + Path.DirectorySeparatorChar;
            filename = logsFolder + prefix + ".txt";
            Logger = new LoggerConfiguration()
                .WriteTo.File(filename, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
        public void Log(OpcNode node)
        {
            if (debug_)
            {
                mutex.WaitOne();
                WriteLine(prefix_, node.Name + " " + node.Id);
                mutex.ReleaseMutex();
            }
        }

        public void Log(string text)
        {
            if (debug_)
            {
                mutex.WaitOne();
                Logger.Information(text);
                mutex.ReleaseMutex();
            }
        }

        public void Log(byte[] text)
        {
            if (debug_)
            {
                mutex.WaitOne();
                foreach (var b in text)
                {
                    Write(b);
                }
                mutex.ReleaseMutex();
            }
        }
        public void Info(string text)
        {
            if (debug_)
            {
                mutex.WaitOne();
                Logger.Information(text);
                mutex.ReleaseMutex();
            }
        }

        private void WriteLine(string prefix, string text)
        {
            mutex.WaitOne();
            using (var w = File.AppendText(filename))
            {
                Log(prefix, text, w);
                //Console.WriteLine(prefix + text);
            }
            mutex.ReleaseMutex();
            /*using (StreamReader r = File.OpenText("log.txt"))
            {
                DumpLog(r);
            }*/
        }
        private void Write(byte text)
        {
            using (var w = File.AppendText(filename))
            {
                Log("", text.ToString(), w);
                //Console.Write(text);
            }

            /*using (StreamReader r = File.OpenText("log.txt"))
            {
                DumpLog(r);
            }*/
        }
        public static void Log(string prefix, string logMessage, TextWriter w)
        {
            //removed prefix for now
            //w.Write($"{prefix}: ");
            //w.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            //w.WriteLine("  :");
            //Logger.Information($"{logMessage}");
            //w.WriteLine("-------------------------------");
        }

        public string GetLines(int lines)
        {
            /*string text = "";
            mutex.WaitOne();
            List<string> debugtext = File.ReadLines(filename).Reverse().Take(lines).Reverse().ToList();
            mutex.ReleaseMutex();

            foreach (var line in debugtext)
            {
                text += line + "\n";
            }
            */
            return "";
        }
        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}
