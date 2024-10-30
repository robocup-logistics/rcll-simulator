using LlsfMsgs;

namespace Simulator.Utility;
public class Timer {
    public long Nsec { get; private set; }
    public long Sec { get; private set; }
    public float TimeFactor { get; private set; }

    private MyLogger MyLogger;
    private static Mutex TimerMutex = new Mutex();

    private static Timer? Instance;
    private Configurations Config;

    Thread Tickthread;
    private static readonly object _lock = new object();
    /// <returns>
    /// Returns the instance of the Configurations Singleton
    /// </returns>
    public static Timer GetInstance(Configurations config) {
        if (Instance == null) {
            lock (_lock) {
                if (Instance == null) {
                    Instance = new Timer(config);
                }
            }
        }
        return Instance;
    }
    private Timer(Configurations config) {
        DateTime now = DateTime.Now;
        Sec = 0;
        Nsec = 0;
        Config = config;
        TimeFactor = Config.TimeFactor;
        MyLogger = new MyLogger("Timer");
        Tickthread = new Thread(Tick);
        Tickthread.Start();

        TimerMutex = new Mutex();
    }

    public void UpdateTime(Time gameTime) {
        TimerMutex.WaitOne();
        MyLogger.Debug("Got Update time message! " + gameTime.ToString());
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
            Nsec += (long)(500_000_000 * TimeFactor);
            if (Nsec >= 1_000_000_000) {
                Sec += 1;
                Nsec -= 1_000_000_000;
            }
            Thread.Sleep(500);
        }
    }
}
