using LlsfMsgs;

namespace Simulator.Utility;
public class Timer {
    public long Nsec { get; private set; }
    public long Sec { get; private set; }
    public float TimeFactor { get; private set; }

    private MyLogger MyLogger;
    private Mutex TimerMutex;

    private static Timer? Instance;
    private Configurations Config;

    Thread Tickthread;
    /// <returns>
    /// Returns the instance of the Configurations Singleton
    /// </returns>
    public static Timer GetInstance(Configurations config) {
        return Instance ?? (Instance = new Timer(config));
    }
    private Timer(Configurations config) {
        DateTime now = DateTime.Now;
        Sec = (int)(now - DateTime.UnixEpoch).TotalSeconds;
        Nsec = 0;
        Config = config;
        TimeFactor = Config.TimeFactor;
        MyLogger = new MyLogger("Timer", true);
        Tickthread = new Thread(Tick);
        Tickthread.Start();

        TimerMutex = new Mutex();
    }

    public void UpdateTime(Time gameTime) {
        TimerMutex.WaitOne();
        MyLogger.Log("Got Update time message!");
        Sec = gameTime.Sec;
        Nsec = gameTime.Nsec;
        TimerMutex.ReleaseMutex();
    }
    public Time GetTime() {
        var time = new Time();
        TimerMutex.WaitOne();
        time.Sec = Sec;
        time.Nsec = Nsec;
        TimerMutex.ReleaseMutex();
        return time;
    }
    public void Tick() {
        while (true) {
            Nsec += Convert.ToInt64(500 * TimeFactor);
            if (Nsec >= 1000) {
                Sec += Convert.ToInt64(1);
                Nsec -= 1000;
            }
            Thread.Sleep(500);
        }
    }
}
