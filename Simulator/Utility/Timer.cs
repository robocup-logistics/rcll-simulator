using System;
using System.Threading;
using Simulator.RobotEssentials;
using Llsfmsgs;

namespace Simulator.Utility
{
    public class Timer
    {
        // TODO build up TICK system with separate thread to increase the time
        // it might be that only seconds are needed as NSecs are not important.

        public long Nsec { get; private set; }
        public long Sec { get; private set; }
        public bool Paused { get; private set; }
        public float TimeFactor { get; private set; }
        private Thread? Tickthread;
        //private Logger Logger;
        private UdpConnector Refbox;
        private readonly PBMessageFactoryBase FactoryBase;
        private MyLogger MyLogger;
        private Mutex TimerMutex;
        //private member and getter for my singleton configurations class
        private static Timer? Instance;

        /// <returns>
        /// Returns the instance of the Configurations Singleton
        /// </returns>
        public static Timer GetInstance()
        {
            return Instance ?? (Instance = new Timer());
        }
        private Timer()
        {
            Nsec = 0;
            Sec = 0;
            Paused = true;
            TimeFactor = Configurations.GetInstance().TimeFactor;
            MyLogger = new MyLogger("Timer", true);
            FactoryBase = new PBMessageFactoryBase(MyLogger);
            //Refbox = new UdpConnector(null, myLogger);
            //Refbox.StartSendThread();
            //Tickthread = new Thread(Tick);
            //Tickthread.Start();
            
            TimerMutex = new Mutex();
        }

        public void ContinueTicking()
        {
            Paused = !Paused;
        }
        public void UpdateTime(Time gameTime)
        {
            TimerMutex.WaitOne();
            MyLogger.Log("Got Update time message!");
            Sec = gameTime.Sec;
            Nsec = gameTime.Nsec;
            TimerMutex.ReleaseMutex();
        }
        public Time GetTime()
        {
            var time = new Time();
            TimerMutex.WaitOne();
            time.Sec = Sec;
            time.Nsec = Nsec;
            TimerMutex.ReleaseMutex();
            return time;
        }
        public void Tick()
        {
            while (true)
            {
                //Logger.Log("Current time = " + Sec + " and Paused = " + Paused.ToString());
                if (!Paused)
                {
                    Nsec += Convert.ToInt64(500 * TimeFactor);
                    if (Nsec >= 1000)
                    {
                        Sec += Convert.ToInt64(1);
                        Nsec -= 1000;
                    }
                }
                var message = FactoryBase.CreateMessage(PBMessageFactoryBase.MessageTypes.SimSynchTime);
                if(message != null)
                {
                    Refbox.Messages.Enqueue(message);
                }
                Thread.Sleep(500);
            }
        }

        public void Reset()
        {
            Sec = 0;
            Nsec = 0;
        }
    }
}
