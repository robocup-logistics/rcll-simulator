using LlsfMsgs;
using Simulator.MPS;
using Simulator.Utility;

namespace Simulator.RobotEssentials;
public class RobotManager {
    public List<Robot> Robots { get; }
    private ZonesManager ZonesManager_;
    private MpsManager MpsManager;
    private readonly Configurations Config;
    public static RobotManager? Instance;
    public static RobotManager GetInstance() {
        if (Instance == null) {
            throw new NullReferenceException("RobotManager not initialized!");
        }
        return Instance;
    }

    public RobotManager(Configurations config, MpsManager mpsManager) {
        Instance = this;
        Robots = new List<Robot>();
        ZonesManager_ = ZonesManager.GetInstance();
        MpsManager = mpsManager;
        Config = config;
        CreateRobots();
    }

    private void CreateRobots() {
        var configs = Config.RobotConfigs;
        foreach (var rob in configs) {
            //Position is teamside x: 4 + jersey(i.e. 5,6,7), y: 1
            var zone = (Zone)((rob.TeamColor == Team.Magenta ? 1000 : 0) + (4 + rob.Jersey) * 10 + 1);
            var zones = ZonesManager_.GetZone(zone);
            if (zones == null) {
                throw new Exception("Couldn't find the zone for the robot! "
                                    + "TeamColor: " + rob.TeamColor + " Jersey: " + rob.Jersey);
            }

            var robot = new Robot(Config, rob, this, MpsManager, zones, true);
            robot.WorkingRobotThread = new Thread(() => robot.Run());
            robot.WorkingRobotThread.Name = "Robot" + robot.JerseyNumber + "_working_thread";
            robot.WorkingRobotThread.Start();

            Robots.Add(robot);
        }
    }

    public void HandleRobotInfo(RobotInfo robotInfo) {
        lock (Robots) {
            foreach (var info in robotInfo.Robots) {
                foreach (var robot in Robots) {
                    if (robot.JerseyNumber == info.Number && robot.TeamColor == info.TeamColor) {
                        robot.HandleRobotInfo(info);
                    }
                }
            }
        }
    }

    public void ResetRobots() {
        lock (Robots) {
            foreach (var robot in Robots) {
                robot.Reset();
            }
        }
    }

    public void StopAllRobots() {
        foreach (var robot in Robots) {
            robot.RobotStop();
        }
    }

    private Mutex pauseMutex = new Mutex();
    // Pauses all Robots that want to move to this mps
    public void PauseRobots(String? name = null) {
        pauseMutex.WaitOne();
        foreach (Robot robot in Robots) {
            robot.Pause(name);
        }
    }


    public void ResumeRobots() {
        foreach (Robot robot in Robots) {
            robot.Resume();
        }
        pauseMutex.ReleaseMutex();
    }

    public void HomeRobots() {
        foreach (Robot robot in Robots) {
            robot.Home();
        }
    }

    public void MoveRobotsToMachine(Mps machine) {
        foreach (Robot robot in Robots) {
            if (robot.inputOutputLock == machine.robotAtOutput || robot.inputOutputLock == machine.robotAtInput) {
                CZones? zone = ZonesManager_.GetZone(machine.Zone);
                if(zone == null) {
                    throw new Exception("Couldn't find the zone for the machine! "
                                        + "Machine: " + machine.Name + " Zone: " + machine.Zone);
                }
                robot.SetZone(zone);
            }
        }
    }
}
