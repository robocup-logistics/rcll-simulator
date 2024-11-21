using LlsfMsgs;
using Simulator.Utility;
using Simulator.MPS;
using ErrorCode = LlsfMsgs.AgentTask.Types.ErrorCode;
using System.Text.RegularExpressions;

namespace Simulator.RobotEssentials;
public partial class Robot {

    List<string> machines = new List<string> {
            "C-CS1", "C-CS2", "C-RS1", "C-RS2", "C-DS", "C-BS", "C-SS",
            "M-CS1", "M-CS2", "M-RS1", "M-RS2", "M-DS", "M-BS", "M-SS"
        };

    private void BufferAtStation(AgentTask task) {
        string station = task.Buffer.MachineId;

        Regex pattern = new Regex("(M|C)-CS(1|2)");
        if (!pattern.IsMatch(station)) {
            MyLogger.Warn("The station is not a CapStation!");
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return;
        }

        if (shelf < 1 || shelf > 3) {
            MyLogger.Warn("The shelf number is invalid!");
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return;
        }

        // IF the machine is moved in between the robot is paused and needs to continue
        // then the robot does not need to grab if it helds a product and the agent task retrieve is defined
        if (!(HeldProduct != null && task.Retrieve != null)) {
            AgentTask gripTask = task.Clone();
            gripTask.Retrieve = new Retrieve();
            gripTask.Retrieve.MachineId = station;
            if(task.Buffer.HasShelfNumber) {
                gripTask.Retrieve.MachinePoint = "shelf" + task.Buffer.ShelfNumber;
            } else {
                gripTask.Retrieve.MachinePoint = "any";
            }

            if (!GetFromStation(gripTask, false)) {
                return;
            }
        }
        AgentTask placeTask = task.Clone();
        placeTask.Deliver = new Deliver();
        placeTask.Deliver.MachineId = station;
        placeTask.Deliver.MachinePoint = "input";
        if (!DeliverToStation(placeTask, false)) {
            return;
        }
        TaskSucceded(task);
    }

    private bool HandleMove(AgentTask task) {
        if (CurrentTask == null) {
            MyLogger.Error("Current task is null!");
            return false;
        }
        var Waypoint = task.Move.Waypoint;
        var MachinePoint = task.Move.MachinePoint;
        Zone targetZone = ZonesManager.GetWaypoint(Waypoint, MachinePoint);
        if (targetZone == 0) {
            MyLogger.Warn("Couldn't find the machine position!");
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return false;
        }
        if (EntryZone != null) {
            LookAtZone(EntryZone);
            if (canceling) {
                return false;
            }
            var diagonalTimeFactor = isDiagonal ? 1.4f : 1.0f;
            Thread.Sleep((int)((float)Config.RobotMoveZoneDuration * diagonalTimeFactor));
            if (canceling) {
                return false;
            }
            SetZone(EntryZone);
            EntryZone = null;

            if (inputOutputLock != null) {
                inputOutputLock.Release(this);
                inputOutputLock = null;
            }
        }
        if (Move(targetZone, task)) {
            MyLogger.Info("Finished the move to waypoint successful!");
        }
        else {
            MyLogger.Warn("Finished the move to waypoint unsuccessful!");
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return false;
        }
        if (machines.Contains(task.Move.Waypoint)) {
            bool input = true;
            if (task.Move.MachinePoint.ToLower() == "output") {
                input = false;
            }

            var mps = MpsManager.GetInstance().GetMachineByName(task.Move.Waypoint);
            if (mps == null) {
                MyLogger.Warn("The Machine not Found!");
                TaskFailed(task, (uint)ErrorCode.InvalidTarget);
                return false;
            }

            if (mps.TeamColor != TeamColor) {
                MyLogger.Warn("The Machine is not of the same team color!");
                TaskFailed(task, (uint)ErrorCode.InvalidTarget);
                return false;
            }

            RobotLock Lock = input ? mps.robotAtInput : mps.robotAtOutput;
            while (!Lock.Acquire(this, 500)) {
                MyLogger.Debug("Waiting for the Machine to be free!");
                if (canceling) {
                    return false;
                }
            }

            MyLogger.Debug("Waiting Aquired Lock!");
            inputOutputLock = Lock;
            if (canceling) {
                return false;
            }
            var zone = ZonesManager.GetMachineZone(task.Move.Waypoint);
            if (zone == null) {
                MyLogger.Warn("The Machine Zone not Found!");
                TaskFailed(task, (uint)ErrorCode.UnableToMoveToTarget);
                return false;
            }
            LookAtZone(zone);

            if (canceling) {
                return false;
            }
            var diagonalTimeFactor = isDiagonal ? 1.4f : 1.0f;
            Thread.Sleep((int)((float)Config.RobotMoveZoneDuration * diagonalTimeFactor));
            if (canceling) {
                return false;
            }
            EntryZone = CurrentZone;
            SetZone(zone);
            SetPositionBack(0.5f);
        }
        TaskSucceded(task);
        return true;
    }

    public bool GetFromStation(AgentTask task, bool succedTask = true) {
        MyLogger.Info("Get From Station task!");
        SerializeRobotToJson();
        if (task == null) {
            MyLogger.Error("GetFromStation -> the current task is NULL!");
            return false;
        }
        var machine = task.Retrieve.MachineId;
        var mps = MpsManager.GetMachineByName(task.Retrieve.MachineId);
        var target = task.Retrieve.MachinePoint;
        if(!task.Retrieve.HasMachineId) {
            task.Retrieve.MachineId = "any";
        }
        Zone targetZone = ZonesManager.GetWaypoint(machine, target);
        if (mps == null || targetZone == 0) {
            MyLogger.Warn("Couldnt find the requested target machine!");
            TaskFailed(task, (uint)ErrorCode.MpsNotFound);
            return false;
        }

        if (mps.TeamColor != TeamColor) {
            MyLogger.Warn("The Machine is not of the same team color!");
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return false;
        }

        RobotLock targetLock = mps.robotAtOutput;
        if (target.ToLower() == "input" ||
           target.ToLower() == "left" || target.ToLower() == "right" || target.ToLower() == "middle"
           || target.ToLower() == "shelf1" || target.ToLower() == "shelf2" || target.ToLower() == "shelf3") {
            targetLock = mps.robotAtInput;
        }
        if (mps == null || targetLock != inputOutputLock) {
            MyLogger.Warn("The Robot isn't at the Output of the Machine!");
            TaskFailed(task, (uint)ErrorCode.NotAtPosition);
            return false;
        }

        if (HeldProduct != null) {
            MyLogger.Warn("The Robot already has a product in its grip!");
            TaskFailed(task, (uint)ErrorCode.WorkpieceAlreadyInGripper);
            return false;
        }
        MyLogger.Info("Starting the Grip Action!");
        SerializeRobotToJson();
        if (canceling) {
            return false;
        }
        FutureProduct = mps.RemoveProduct(target, true);
        Thread.Sleep(Config.RobotGrabProductDuration);
        if (canceling) {
            FutureProduct = null;
            return false;
        }
        HeldProduct = mps.RemoveProduct(target);
        FutureProduct = null;

        if (HeldProduct == null) {
            MyLogger.Warn("The Machine didn't have a product to give!");
            TaskFailed(task, (uint)ErrorCode.WorkpieceSensorDisagreement);
            return false;
        }

        MyLogger.Info("Got a new Product!");
        MyLogger.Debug(HeldProduct.ProductDescription());
        if (succedTask) {
            TaskSucceded(task);
        }
        return true;
    }

    private bool DeliverToStation(AgentTask task, bool succedTask = true) {
        MyLogger.Info("DeliverToStation!");
        SerializeRobotToJson();
        if (task == null) {
            return false;
        }
        var machine = task.Deliver.MachineId;
        var mps = MpsManager.GetMachineByName(task.Deliver.MachineId);
        var target = task.Deliver.MachinePoint;
        Zone targetZone = ZonesManager.GetWaypoint(machine, target);
        if (mps == null || targetZone == 0) {
            MyLogger.Warn("Couldnt find the requested target machine!");
            TaskFailed(task, (uint)ErrorCode.MpsNotFound);
            return false;
        }

        if (mps.TeamColor != TeamColor) {
            MyLogger.Warn("The Machine is not of the same team color!");
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return false;
        }

        RobotLock targetLock = mps.robotAtInput;
        if (target.ToLower() == "output") {
            targetLock = mps.robotAtOutput;
        }
        if (mps == null || targetLock != inputOutputLock) {
            MyLogger.Warn("The Robot isn't at the correct Side of the Machine!");
            TaskFailed(task, (uint)ErrorCode.NotAtPosition);
            return false;
        }
        if (!mps.EmptyMachinePoint(target)) {
            MyLogger.Warn("Something went wrong with placing. Seems there is already a product at "
                         + target + " of machine " + mps.Name);
            TaskFailed(task, (uint)ErrorCode.MachinePointOccupied);
            return false;
        }
        SerializeRobotToJson();
        MyLogger.Info("Aligning and starting the place action");
        if (canceling) {
            return false;
        }
        Thread.Sleep(Config.RobotPlaceDuration);
        if (canceling) {
            return false;
        }

        if (HeldProduct == null) {
            MyLogger.Warn("The Robot doesn't have a product in its grip!");
            TaskFailed(task, (uint)ErrorCode.NoWorkpieceInGripper);
            return false;
        }

        mps.PlaceProduct(target, HeldProduct);

        HeldProduct = null;

        if (succedTask) {
            TaskSucceded(task);
        }
        return true;
    }

    public bool Move(Zone TargetZone, AgentTask task) {
        var end = ZonesManager.GetZone(TargetZone);
        if (end == null) {
            MyLogger.Error("TargetZone is null!");
            return false;
        }

        var path = ZonesManager.Astar(CurrentZone, end);
        if (path.Count == 0 && CurrentZone.ZoneId == TargetZone) {
            MyLogger.Info("Finished the move as I'm already in place!");
            return true;
        }

        if (path.Count == 0) {
            MyLogger.Error("No Path could be computed!!");
            return false;
        }

        MyLogger.Debug("Got a Path!");
        foreach (var z in path) {
            MyLogger.Debug("Doing a step towards + " + z.ZoneId);
            LookAtZone(z);
            if (canceling) {
                return false;
            }
            var diagonalTimeFactor = isDiagonal ? 1.4f : 1.0f;
            Thread.Sleep((int)((float)Config.RobotMoveZoneDuration * diagonalTimeFactor));
            if (canceling) {
                return false;
            }
            SetZone(z);
        }

        MyLogger.Info("Finishing the move command");
        return true;
    }

    private void ExploreMachine(AgentTask task) {
        MyLogger.Info("Exploring the Machine!");
        if (!task.ExploreMachine.HasWaypoint) {
            MyLogger.Warn("The task has no waypoint!");
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return;
        }

        var waypoint = task.ExploreMachine.Waypoint;

        var targetZone = ZonesManager.GetZone(waypoint);
        if (targetZone == null) {
            MyLogger.Warn("The target zone is not found! Name" + waypoint);
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return;
        }

        if (CurrentZone == targetZone) {
            MyLogger.Info("Already at the target zone!");
            TaskSucceded(task);
            return;
        }

        if (!targetZone.Free()) {
            MyLogger.Info("The target zone is not free!");
            var machine = targetZone.GetZoneString();
            targetZone = ZonesManager.GetZone(ZonesManager.GetZoneNextToMachine(machine));
        }
        if (targetZone == null) {
            MyLogger.Warn("Machine Zone not found: Name" + waypoint);
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return;
        }

        var path = ZonesManager.Astar(CurrentZone, targetZone);

        if (path.Count == 0) {
            MyLogger.Error("No Path could be computed!!");
            TaskFailed(task, (uint)ErrorCode.UnableToMoveToTarget);
        }

        //TODO MIRROR REPORT
        foreach (var z in path) {
            MyLogger.Debug("Doing a step towards + " + z.ZoneId);
            LookAtZone(z);
            if (canceling) {
                return;
            }
            var diagonalTimeFactor = isDiagonal ? 1.4f : 1.0f;
            Thread.Sleep((int)((float)Config.RobotMoveZoneDuration * diagonalTimeFactor));
            if (canceling) {
                return;
            }
            SetZone(z);
            foreach (var zone in CurrentZone.GetNeighborhood()) {
                if (!zone.Free() && !zone.Found(TeamColor)) {
                    if (canceling) {
                        return;
                    }
                    Thread.Sleep(Config.RobotExploreDuration);
                    var hasTag = zone.HasTag();
                    if (!teamConfig.Markerless && !hasTag) {
                        // Only rot and zone can be determined
                        if (!Config.RobotReportDirect) {
                            if (RoleTheDice(Config.ExplorationProbability)) {
                                var report = GetMachineReport(zone.ZoneId, null, (uint)zone.Orientation);
                                AgentConnector?.AppendMachineReport(report);
                                zone.Found(TeamColor, true);
                            }
                        }
                    }
                    else {
                        // All information can be determined
                        if (Config.RobotReportDirect) {
                            if (RoleTheDice(Config.ExplorationProbability)) {
                                var report = GetMachineReport(zone.ZoneId, zone.GetZoneString(), (uint)zone.Orientation);
                                BeaconConnector?.AppendMachineReport(report);
                                zone.Found(TeamColor, true);
                            }
                        }
                        else {
                            string? name = null;
                            uint? orientation = null;
                            if (RoleTheDice(Config.ExplorationProbability)) {
                                name = zone.GetZoneString();
                            }
                            if (RoleTheDice(Config.ExplorationProbability)) {
                                orientation = (uint)zone.Orientation;
                            }
                            if (name != null || orientation != null) {
                                var report = GetMachineReport(zone.ZoneId, name, orientation, teamConfig.Markerless);
                                AgentConnector?.AppendMachineReport(report);
                            }
                        }
                    }
                }
            }
        }
    }

    private List<string> types = new List<string> { "CS", "RS", "SS", "DS", "BS" };

    private MachineReport GetMachineReport(Zone zone, string? name, uint? Orientation, bool markerless = false) {
        var report = new MachineReport();
        string? type = null;
        if (name != null) {
            type = types.FirstOrDefault(t => name.Contains(t));
        }
        report.TeamColor = TeamColor;
        report.Machines.Add(new MachineReportEntry() {
            Zone = zone,
            Type = type
        });

        // Name can be retrieved from the agent through the refbox
        if (!markerless) {
            report.Machines[0].Name = name;
        }
        if (Orientation != null) {
            report.Machines[0].Rotation = (uint)Orientation;
        }
        return report;
    }

}// class Robot
