using System.Collections.Concurrent;
using Simulator.Utility;
using LlsfMsgs;
using Simulator.RobotEssentials;

namespace Simulator.MPS;
public class MpsManager {
    private ConcurrentDictionary<string, Mps> Machines { get; }
    private ZonesManager ZonesManager;
    private MyLogger myLogger;
    private readonly Configurations Config;
    private static MpsManager? Instance;
    private bool magentaTag = true;
    private bool cyanTag = true;

    public static MpsManager GetInstance() {
        if (Instance == null) {
            throw new NullReferenceException("MPsManager not initialized!");
        }
        return Instance;
    }

    public MpsManager(Configurations config) {
        Instance = this;
        myLogger = new MyLogger("MpsManager");
        Config = config;
        myLogger.Info("Started the Mps Manager!");
        Machines = new ConcurrentDictionary<string, Mps>();
        ZonesManager = ZonesManager.GetInstance();

        foreach (var teamconfig in Config.Teams) {
            if (teamconfig.Color == Team.Magenta) {
                magentaTag = !teamconfig.Markerless;
            }
            if (teamconfig.Color == Team.Cyan) {
                cyanTag = !teamconfig.Markerless;
            }
        }

        if (Config.FixedMPSplacement) {
            CreateMachines();
        }
    }

    private void CreateMachines() {

        Console.WriteLine("Fixed Positions enabled! Placing machines .. ");
        foreach (var mps in Config.MpsConfigs) {
            var hasTag = mps.Name.Contains("M-") ? magentaTag : cyanTag;
            Mps? currentMps;
            Thread? thread;
            switch (mps.Type) {
                case MpsType.BaseStation:
                    var bs = new MPS_BS(Config, mps.Name, mps.Team, hasTag);
                    thread = new Thread(bs.Run);
                    currentMps = bs;
                    break;
                case MpsType.CapStation:
                    var cs = new MPS_CS(Config, mps.Name, mps.Team, hasTag);
                    thread = new Thread(cs.Run);
                    currentMps = cs;
                    break;
                case MpsType.DeliveryStation:
                    var ds = new MPS_DS(Config, mps.Name, mps.Team, hasTag);
                    thread = new Thread(ds.Run);
                    currentMps = ds;
                    break;
                case MpsType.RingStation:
                    var rs = new MPS_RS(Config, mps.Name, mps.Team, hasTag);
                    thread = new Thread(rs.Run);
                    currentMps = rs;
                    break;
                case MpsType.StorageStation:
                    var ss = new MPS_SS(Config, mps.Name, mps.Team, hasTag);
                    thread = new Thread(ss.Run);
                    currentMps = ss;
                    break;
                default:
                    Console.WriteLine("Unknown station type!");
                    thread = null;
                    currentMps = null;
                    break;
            }
            if (currentMps == null || thread == null) {
                continue;
            }

            thread.Name = currentMps.Name + "_workingThread";
            thread.Start();
            Machines.TryAdd(mps.Name, currentMps);
            ZonesManager.PlaceMachine(mps.Zone, (uint)mps.Orientation, currentMps);
        }
    }

    private void CreateMachine(Machine machine) {
        Mps? currentMps;
        var hasTag = machine.TeamColor == Team.Magenta ? magentaTag : cyanTag;
        Thread? thread;
        switch (machine.Type) {
            case "BS":
                var bs = new MPS_BS(Config, machine.Name, machine.TeamColor, hasTag);
                thread = new Thread(bs.Run);
                currentMps = bs;
                break;
            case "CS":
                var cs = new MPS_CS(Config, machine.Name, machine.TeamColor, hasTag);
                thread = new Thread(cs.Run);
                currentMps = cs;
                break;
            case "DS":
                var ds = new MPS_DS(Config, machine.Name, machine.TeamColor, hasTag);
                thread = new Thread(ds.Run);
                currentMps = ds;
                break;
            case "RS":
                var rs = new MPS_RS(Config, machine.Name, machine.TeamColor, hasTag, machine.RingColors[0], machine.RingColors[1]);
                thread = new Thread(rs.Run);
                currentMps = rs;
                break;
            case "SS":
                var ss = new MPS_SS(Config, machine.Name, machine.TeamColor, hasTag);
                thread = new Thread(ss.Run);
                currentMps = ss;
                break;
            default:
                Console.WriteLine("Unknown station type!");
                thread = null;
                currentMps = null;
                break;
        }
        if (currentMps == null || thread == null) {
            return;
        }

        thread.Name = currentMps.Name + "_workingThread";
        thread.Start();
        Machines.TryAdd(machine.Name, currentMps);
        ZonesManager.PlaceMachine(machine.Zone, machine.Rotation, currentMps);
    }

    public Mps? GetMachineByName(string machineId) {
        return Machines.ContainsKey(machineId) ? Machines[machineId] : null;
    }

    public void HandleMachineInfo(MachineInfo machineInfo) {
        foreach (var machine in machineInfo.Machines) {
            if (ZonesManager.GetZone(machine.Zone) == null) {
                myLogger.Warn("Zone not found for machine " + machine.Name);
                continue;
            }
            if (Machines.ContainsKey(machine.Name)) {
                if (!Machines[machine.Name].DeepEquals(machine)) {
                    UpdateMachine(Machines[machine.Name], machine);
                }
            }
            else {
                CreateMachine(machine);
            }
        }
    }

    private void UpdateMachine(Mps machine, Machine newMachine) {
        if (machine.Zone != newMachine.Zone || machine.Rotation != newMachine.Rotation) {
            RobotManager robotManager = RobotManager.GetInstance();
            robotManager.PauseRobots(machine.Name);
            ZonesManager.MoveMachine(machine, newMachine.Zone, newMachine.Rotation);
            robotManager.MoveRobotsToMachine(machine);
            robotManager.ResumeRobots();
        }

        if (machine.TeamColor != newMachine.TeamColor) {
            throw new Exception("Team color change not supported!");
        }

        if (machine.Type == MpsType.RingStation) {
            MPS_RS rs = (MPS_RS)machine;
            if (rs.Ring1 != newMachine.RingColors[0]) {
                myLogger.Warn("Ring 1 color changed on " + rs.Name);
                rs.Ring1 = newMachine.RingColors[0];
            }
            if (rs.Ring2 != newMachine.RingColors[1]) {
                myLogger.Warn("Ring 2 color changed on " + rs.Name);
                rs.Ring2 = newMachine.RingColors[1];
            }
        }

        // if (machine.Type == MpsType.CapStation) {
        //     MPS_CS cs = (MPS_CS)machine;
        //     // TODO IF Cap Color will be send from the refbox
        //     // check if it is equals and add to DeepEqua
        //     // the check for equallity
        // }
    }

    public void ResetMachines() {
        foreach (KeyValuePair<string, Mps> machine in Machines) {
            machine.Value.HardResetMachine();
        }
    }

    public void MoveMachineToNewField(Dictionary<Zone, CZones> Dictionary) {
        foreach (KeyValuePair<string, Mps> machine in Machines) {
            Dictionary[machine.Value.Zone].PlaceMachine(machine.Value, machine.Value.Rotation);
        }
    }

    // Use for webgui only
    public List<Mps> GetAllMachines() {
        return Machines.Values.ToList();
    }
}
