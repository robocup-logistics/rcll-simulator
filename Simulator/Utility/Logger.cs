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
        private readonly bool Debug;
        private readonly string Prefix;
        private readonly string Filename;
        private readonly string LogsFolder;
        private readonly Mutex Mutex = new Mutex();
        private Serilog.Core.Logger? Logger;
        public enum LogTypes { Info, Error, Warning
        }
        public MyLogger(string prefix, bool debug)
        {
            Debug = debug;
            Prefix = "[" + prefix +"] ";
            Prefix = "";
            LogsFolder = "logs" + Path.DirectorySeparatorChar;
            Filename = LogsFolder + prefix + ".log";
            //Console.WriteLine(Directory.GetCurrentDirectory() + "Writing to  : " + Filename);
            Logger = new LoggerConfiguration()
                .WriteTo.File(Filename, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            
            Log("------------------------------------------");
            Log("Starting new logging session at " + DateTime.Now);
        }
        public void Log(OpcNode node)
        {
            if (Debug)
            {
                Mutex.WaitOne();
                WriteLine(Prefix, node.Name + " " + node.Id);
                Mutex.ReleaseMutex();
            }
        }

        public void Log(string text)
        {
            if (Debug)
            {
                Mutex.WaitOne();
                Logger.Information(text);
                Mutex.ReleaseMutex();
            }
        }

        public void Log(byte[] text)
        {
            if (Debug)
            {
                Mutex.WaitOne();
                foreach (var b in text)
                {
                    Write(b);
                }
                Mutex.ReleaseMutex();
            }
        }
        public void Info(string text)
        {
            if (Debug)
            {
                Mutex.WaitOne();
                Logger.Information(text);
                Mutex.ReleaseMutex();
            }
        }

        private void WriteLine(string prefix, string text)
        {
            Mutex.WaitOne();
            using (var w = File.AppendText(Filename))
            {
                Log(prefix, text, w);
                //Console.WriteLine(prefix + text);
            }
            Mutex.ReleaseMutex();
            /*using (StreamReader r = File.OpenText("log.txt"))
            {
                DumpLog(r);
            }*/
        }
        private void Write(byte text)
        {
            using (var w = File.AppendText(Filename))
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
