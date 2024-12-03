using Serilog;

namespace Simulator.Utility;
public class MyLogger {
    public static string BaseFolder = "";
    public static bool debug_ = true;
    public static bool WarnToConsole = false;
    public static bool ErrorToConsole = false;
    private readonly string Prefix;
    private readonly string Filename;
    private readonly Mutex Mutex = new Mutex();
    private Serilog.Core.Logger Logger;

    public MyLogger(string prefix) {
        Prefix = "[" + prefix + "] ";
        Filename = Path.Combine(BaseFolder, prefix + ".log");
        string latest = Path.Combine(BaseFolder, "..", "latest", prefix + ".log");
        string latestSymlink = Path.Combine("..", "..", Filename);

        Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(Filename, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        if (File.Exists(latest) || Directory.Exists(latest)) {
            File.Delete(latest);
        }

        // Create the symlink
        File.CreateSymbolicLink(latest, latestSymlink);

        Info("------------------------------------------");
        Info("Starting new logging session at " + DateTime.Now);
    }

    public void Info(string text) {
        Mutex.WaitOne();
        Logger.Information(text);
        Mutex.ReleaseMutex();
    }

    public void Warn(string text) {
        Mutex.WaitOne();
        Logger.Warning(text);
        Mutex.ReleaseMutex();
        if (WarnToConsole) {
            Console.Error.WriteLineAsync(Prefix + "[WARN] " + text);
        }
    }

    public void Debug(string text) {
        if (debug_) {
            Mutex.WaitOne();
            Logger.Debug(text);
            Mutex.ReleaseMutex();
        }
    }

    public void Error(string text) {
        Mutex.WaitOne();
        Logger.Error(text);
        Mutex.ReleaseMutex();
        if (ErrorToConsole) {
            Console.Error.WriteLineAsync(Prefix + "[ERROR] " + text);
        }
    }

}
