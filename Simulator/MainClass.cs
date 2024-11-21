using Simulator.MPS;
using Simulator.RobotEssentials;
using Simulator.Utility;

namespace Simulator;
internal class MainClass {
    private static MyLogger? MainLogger;
    private static RobotManager? RobotManager;
    private static MpsManager? MachineManager;
    private static Configurations? Config;
    private static TcpConnector? RefboxConnector;
    private static GTMonitor? GTMonitor;


    private static void Main(string[] args) {
        var next = false;
        var path = "";
        foreach (var argument in args) {
            switch (argument.ToLower()) {
                case "-cfg":
                    next = true;
                    continue;
                default: {
                        if (next) {
                            path = argument;
                            next = false;
                        }
                        break;
                    }
            }
        }

        if (path.Equals("")) {
            Console.WriteLine("No path to the Configuration file is given!");
            Console.WriteLine("Please use \"-cfg path\\to\\file\"!");
            Console.WriteLine("Something like \"-cfg cfg\\config.yaml should work if you just pulled the project!");
            return;
        }

        string baseLogFolder = "logs";
        if (!Directory.Exists(baseLogFolder)) {
            Directory.CreateDirectory(baseLogFolder);
        }

        // Directory for actual logs with date-time stamp
        var dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string dateTimeFolder = Path.Combine(baseLogFolder, dateTime);
        if (!Directory.Exists(dateTimeFolder)) {
            Directory.CreateDirectory(dateTimeFolder);
        }

        // Path for the 'latest' symlink
        string latestLinkPath = Path.Combine(baseLogFolder, "latest");

        // Delete old symlink if it exists
        if (File.Exists(latestLinkPath) || Directory.Exists(latestLinkPath)) {
            // Check if it's a directory or file link because behavior can differ on different systems
            FileSystemInfo fileInfo = new FileInfo(latestLinkPath);
            if ((fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                Directory.Delete(latestLinkPath);
            else
                File.Delete(latestLinkPath);
        }

        Directory.CreateSymbolicLink(latestLinkPath, dateTime);
        MyLogger.BaseFolder = dateTimeFolder;

        Config = new Configurations(path);
        MainLogger = new MyLogger("MainClass");
        Console.Write("Starting the Machines ... ");
        MachineManager = new MpsManager(Config);
        Console.WriteLine("done!");
        Console.Write("Starting the Robots ... ");
        RobotManager = new RobotManager(Config, MachineManager);
        Console.WriteLine("done!");
        if (Config.GroundTruthMonitor) {
            GTMonitor = new GTMonitor();
        }
        RefboxConnector = new TcpConnector(Config, Config.Refbox.IP,
                                Config.Refbox.TcpPort, MachineManager,
                                RobotManager, GTMonitor, new MyLogger("RefboxPublic"));

        var web = new WebGui.WebGui(Config, MachineManager, RobotManager);
        Console.WriteLine("Everything is set up! Waiting for connections!");

    }

    public static void CloseApplication() {
        Console.Write("Starting the cleanup ..");
        if (RobotManager != null) {
            foreach (var robot in RobotManager.Robots) {
                MainLogger?.Info("Starting the Cleanup....");
                robot.RobotStop();
                MainLogger?.Info("Finished the Cleanup....");

            }
        }
        Console.Write(".. cleanup done");
        Environment.Exit(0);
    }
}
