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
        uint shelf = task.Buffer.ShelfNumber;

        Regex pattern = new Regex("(M|C)-CS(1|2)");
        if (!pattern.IsMatch(station)) {
            MyLogger.Log("The station is not a CapStation!");
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return;
        }

        if (shelf < 1 || shelf > 3) {
            MyLogger.Log("The shelf number is invalid!");
            TaskFailed(task, (uint)ErrorCode.InvalidTarget);
            return;
        }

        AgentTask gripTask = task.Clone();
        gripTask.Retrieve = new Retrieve();
        gripTask.Retrieve.MachineId = station;
        gripTask.Retrieve.MachinePoint = "shelf" + shelf;
        if (!GetFromStation(gripTask, false)) {
            return;
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
            MyLogger.Log("Current task is null!");
            return false;
        }
        var Waypoint = task.Move.Waypoint;
        var MachinePoint = task.Move.MachinePoint;
        Zone targetZone = ZonesManager.GetInstance().GetWaypoint(Waypoint, MachinePoint);
        if (targetZone == 0) {
            MyLogger.Log("Couldn't find the machine position!");
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

            if (inputOutputMutex != null) {
                inputOutputMutex.ReleaseMutex();
                inputOutputMutex = null;
            }
        }
        if (Move(targetZone, task)) {
            MyLogger.Log("Finished the move to waypoint successful!");
        }
        else {
            MyLogger.Log("Finished the move to waypoint unsuccessful!");
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
                MyLogger.Log("The Machine not Found!");
                TaskFailed(task, (uint)ErrorCode.InvalidTarget);
                return false;
            }
            Mutex mutex = input ? mps.robotAtInput : mps.robotAtOutput;
            while (!mutex.WaitOne(500)) {
                if (canceling) {
                    return false;
                }
            }
            inputOutputMutex = mutex;
            if (canceling) {
                return false;
            }
            var zone = ZonesManager.GetInstance().GetMachineZone(task.Move.Waypoint);
            if (zone == null) {
                MyLogger.Log("The Machine Zone not Found!");
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
        MyLogger.Log("Get From Station task!");
        SerializeRobotToJson();
        if (task == null) {
            MyLogger.Log("GetFromStation -> the current task is NULL!");
            return false;
        }
        var machine = task.Retrieve.MachineId;
        var mps = MpsManager.GetMachineByName(task.Retrieve.MachineId);
        var target = task.Retrieve.MachinePoint;
        Zone targetZone = ZonesManager.GetInstance().GetWaypoint(machine, target);
        if (mps == null || targetZone == 0) {
            MyLogger.Log("Couldnt find the requested target machine!");
            TaskFailed(task, (uint)ErrorCode.MpsNotFound);
            return false;
        }

        Mutex targetMutex = mps.robotAtOutput;
        if (target.ToLower() == "input" ||
           target.ToLower() == "left" || target.ToLower() == "right" || target.ToLower() == "middle"
           || target.ToLower() == "shelf1" || target.ToLower() == "shelf2" || target.ToLower() == "shelf3") {
            targetMutex = mps.robotAtInput;
        }
        if (mps == null || targetMutex != inputOutputMutex) {
            MyLogger.Log("The Robot isn't at the Output of the Machine!");
            TaskFailed(task, (uint)ErrorCode.NotAtPosition);
            return false;
        }

        if (HeldProduct != null) {
            MyLogger.Log("The Robot already has a product in its grip!");
            TaskFailed(task, (uint)ErrorCode.WorkpieceAlreadyInGripper);
            return false;
        }
        MyLogger.Log("Starting the GRIP Action!");
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
            MyLogger.Log("The Machine didn't have a product to give!");
            TaskFailed(task, (uint)ErrorCode.WorkpieceSensorDisagreement);
            return false;
        }

        MyLogger.Log("Got a new Product!");
        MyLogger.Log(HeldProduct.ProductDescription());
        if (succedTask) {
            TaskSucceded(task);
        }
        return true;
    }

    private bool DeliverToStation(AgentTask task, bool succedTask = true) {
        MyLogger.Log("DeliverToStation!");
        SerializeRobotToJson();
        if (task == null) {
            return false;
        }
        var machine = task.Deliver.MachineId;
        var mps = MpsManager.GetMachineByName(task.Deliver.MachineId);
        var target = task.Deliver.MachinePoint;
        Zone targetZone = ZonesManager.GetInstance().GetWaypoint(machine, target);
        if (mps == null || targetZone == 0) {
            MyLogger.Log("Couldnt find the requested target machine!");
            TaskFailed(task, (uint)ErrorCode.MpsNotFound);
            return false;
        }
        Mutex targetMutex = mps.robotAtInput;
        if (target.ToLower() == "output") {
            targetMutex = mps.robotAtOutput;
        }
        if (mps == null || targetMutex != inputOutputMutex) {
            MyLogger.Log("The Robot isn't at the correct Side of the Machine!");
            TaskFailed(task, (uint)ErrorCode.NotAtPosition);
            return false;
        }
        if (!mps.EmptyMachinePoint(target)) {
            MyLogger.Log("Something went wrong with placing. Seems there is already a product at "
                         + target + " of machine " + mps.Name);
            TaskFailed(task, (uint)ErrorCode.MachinePointOccupied);
            return false;
        }
        SerializeRobotToJson();
        MyLogger.Log("Aligning and starting the place action");
        if (canceling) {
            return false;
        }
        Thread.Sleep(Config.RobotPlaceDuration);
        if (canceling) {
            return false;
        }

        if (HeldProduct == null) {
            MyLogger.Log("The Robot doesn't have a product in its grip!");
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
        var end = ZonesManager.GetInstance().GetZone(TargetZone);
        if (end == null) {
            MyLogger.Log("TargetZone is null!");
            return false;
        }

        var path = ZonesManager.GetInstance().Astar(CurrentZone, end);
        if (path.Count == 0 && CurrentZone.ZoneId == TargetZone) {
            MyLogger.Log("Finished the move as I'm already in place!");
            return true;
        }

        MyLogger.Log(path.Count != 0 ? "Got a Path!" : "No Path could be computed!!");
        if (path.Count == 0) {
            return false;
        }
        foreach (var z in path) {
            MyLogger.Log("Doing a step towards + " + z.ZoneId);
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

        MyLogger.Log("Finishing the move command");
        return true;
    }

    private void ExploreMachine(AgentTask task) {
        throw new NotImplementedException();//TODO
    }

}// class Robot
