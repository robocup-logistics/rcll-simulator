using LlsfMsgs;
using Simulator.Utility;
using Simulator.MPS;
using ErrorCode = LlsfMsgs.AgentTask.Types.ErrorCode;

namespace Simulator.RobotEssentials {
    public partial class Robot {

        // TODO make it nicer
        List<string> machines = new List<string> {
            "C-CS1", "C-CS2", "C-RS1", "C-RS2", "C-DS", "C-BS", "C-SS",
            "M-CS1", "M-CS2", "M-RS1", "M-RS2", "M-DS", "M-BS", "M-SS"
        };

        private void HandleMove(AgentTask task) {
            if (CurrentTask == null) {
                MyLogger.Log("Current task is null!");
                return;
            }
            var Waypoint = task.Move.Waypoint;
            var MachinePoint = task.Move.MachinePoint;
            Zone targetZone = ZonesManager.GetInstance().GetWaypoint(Waypoint, MachinePoint);
            if (targetZone == 0) {
                MyLogger.Log("Couldn't find the machine position!");
                TaskFailed(task, (uint)ErrorCode.InvalidTarget);
                return;
            }
            if (Move(targetZone, task)) {
                MyLogger.Log("Finished the move to waypoint successful!");
            }
            else {
                MyLogger.Log("Finished the move to waypoint unsuccessful!");
                TaskFailed(task, (uint)ErrorCode.InvalidTarget);
                return;
            }
            if (machines.Contains(task.Move.Waypoint)) {
                bool input = true;
                if (task.Move.MachinePoint == "output") {
                    input = false;
                }

                var mps = MpsManager.GetInstance().GetMachineByName(task.Move.Waypoint);
                if (mps == null) {
                    MyLogger.Log("The Machine not Found!");
                    TaskFailed(task, (uint)ErrorCode.InvalidTarget);
                    return;
                }
                Mutex mutex = input ? mps.robotAtInput : mps.robotAtOutput;
                while (!mutex.WaitOne(500)) {
                    if (canceling) {
                        return;
                    }
                }
                inputOutputMutex = mutex;

                if (canceling) {
                    return;
                }
                Thread.Sleep(Config.RobotMoveZoneDuration);
                //TODO SET POSE WITH OFFSET TO THAT MACHINE
                if (canceling) {
                    return;
                }
                var zone = ZonesManager.GetInstance().GetMachineZone(task.Move.Waypoint);
                if (zone == null) {
                    MyLogger.Log("The Machine Zone not Found!");
                    TaskFailed(task, (uint)ErrorCode.UnableToMoveToTarget);
                    return;
                }
                SetZone(zone);
            }
            TaskSucceded(task);
        }

        public void GetFromStation(AgentTask task) {
            MyLogger.Log("Get From Station task!");
            SerializeRobotToJson();
            if (task == null) {
                MyLogger.Log("GetFromStation -> the current task is NULL!");
                return;
            }
            var machine = task.Retrieve.MachineId;
            var mps = MpsManager.GetMachineByName(task.Retrieve.MachineId);
            var target = task.Retrieve.MachinePoint;
            Zone targetZone = ZonesManager.GetInstance().GetWaypoint(machine, target);
            if (mps == null || targetZone == 0) {
                MyLogger.Log("Couldnt find the requested target machine!");
                TaskFailed(task, (uint)ErrorCode.MpsNotFound);
                return;
            }
            if (mps == null || mps.robotAtOutput != inputOutputMutex) {
                MyLogger.Log("The Robot isn't at the Output of the Machine!");
                TaskFailed(task, (uint)ErrorCode.NotAtPosition);
                return;
            }

            if (HeldProduct != null) {
                MyLogger.Log("The Robot already has a product in its grip!");
                TaskFailed(task, (uint)ErrorCode.WorkpieceAlreadyInGripper);
            }
            MyLogger.Log("Starting the GRIP Action!");
            SerializeRobotToJson();
            if (canceling) {
                return;
            }
            Thread.Sleep(Config.RobotGrabProductDuration);
            if (canceling) {
                return;
            }
            HeldProduct = mps.RemoveProduct(target);

            if (HeldProduct != null) {
                MyLogger.Log("Got a new Product!");
                MyLogger.Log(HeldProduct.ProductDescription());
                TaskFailed(task, (uint)ErrorCode.WorkpieceSensorDisagreement);
            }

            TaskSucceded(task);
        }

        private void DeliverToStation(AgentTask task) {
            MyLogger.Log("DeliverToStation!");
            SerializeRobotToJson();
            if (task == null) {
                return;
            }
            var machine = task.Deliver.MachineId;
            var mps = MpsManager.GetMachineByName(task.Retrieve.MachineId);
            var target = task.Deliver.MachinePoint;
            Zone targetZone = ZonesManager.GetInstance().GetWaypoint(machine, target);
            if (mps == null || targetZone == 0) {
                MyLogger.Log("Couldnt find the requested target machine!");
                TaskFailed(task, (uint)ErrorCode.MpsNotFound);
                return;
            }
            if (mps == null || mps.robotAtInput != inputOutputMutex) {
                MyLogger.Log("The Robot isn't at the input of the Machine!");
                TaskFailed(task, (uint)ErrorCode.NotAtPosition);
                return;
            }
            if (!mps.EmptyMachinePoint(target)) {
                MyLogger.Log("Something went wrong with placing. Seems there is already a product at "
                             + target + " of machine " + mps.Name);
                TaskFailed(task, (uint)ErrorCode.MachinePointOccupied);
            }
            SerializeRobotToJson();
            MyLogger.Log("Aligning and starting the place action");
            if (canceling) {
                return;
            }
            Thread.Sleep(Config.RobotPlaceDuration);
            if (canceling) {
                return;
            }

            if (HeldProduct == null) {
                MyLogger.Log("The Robot doesn't have a product in its grip!");
                TaskFailed(task, (uint)ErrorCode.NoWorkpieceInGripper);
                return;
            }

            mps.PlaceProduct(target, HeldProduct);

            HeldProduct = null;

            TaskSucceded(task);
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
            foreach (var z in path) {
                MyLogger.Log("Doing a step towards + " + z.ZoneId);
                if (canceling) {
                    return false;
                }
                Thread.Sleep(Config.RobotMoveZoneDuration);
                if (canceling) {
                    return false;
                }
                if (inputOutputMutex != null) {
                    inputOutputMutex.ReleaseMutex();
                    inputOutputMutex = null;
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
} // namespace Simulator.RobotEssentials
