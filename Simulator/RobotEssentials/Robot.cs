using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using LlsfMsgs;
using Simulator.Utility;
using Simulator.MPS;


namespace Simulator.RobotEssentials {
    public partial class Robot {
        public string RobotName { get; }
        public string TeamName { get; }
        public uint JerseyNumber { get; }
        public Team TeamColor { get; }
        public CPosition Position { get; set; }
        private readonly MyLogger MyLogger;
        private bool Running;
        public Products? HeldProduct { get; private set; }
        private RobotState RobotState;
        public Zones CurrentZone { get; private set; }
        public RobotConfig RobotConfig;
        //if the robot enters a machine, the input/output mutex that gets locked
        //will be stored here as a reference to make sure it releases it on leaving the input/output
        private Mutex? inputOutputMutex = null;
        private Barrier cancelBarrier;
        private bool canceling;

        private UdpConnector? BeaconConnector;
        private ConnectorBase? AgentConnector;

        public Mutex TaskMutex;
        //only use the _currentTask if you locked the mutex by hand and need to actually modify the task and not retrieve it .....
        private AgentTask? _currentTask;
        [JsonIgnore]
        public AgentTask? CurrentTask {
            get {
                //TODO MAYBE DEADLOCK IF SOME TASK WANTSTO ACCES IT AND THEN THE MUTEX IST LOCKED BY HAND AND NEVER COMES TO THE BARRIER
                TaskMutex.WaitOne();
                try {
                    return _currentTask;
                }
                finally {
                    TaskMutex.ReleaseMutex();
                }
            }
            set {
                TaskMutex.WaitOne();
                _currentTask = value;
                TaskMutex.ReleaseMutex();
            }
        }
        private Mutex LastTaskMutex;
        private AgentTask? _lastTask;
        [JsonIgnore]
        public AgentTask? LastTask {
            get {
                LastTaskMutex.WaitOne();
                try {
                    return _lastTask;
                }
                finally {
                    TaskMutex.ReleaseMutex();
                }
            }
            set {
                TaskMutex.WaitOne();
                if (value != null) {
                    FinishedTasks.Add(new CFinishedTask(value.TaskId, value.Successful));
                }
                _lastTask = value;
                TaskMutex.ReleaseMutex();
            }
        }

        [JsonIgnore]
        public Thread? WorkingRobotThread { get; set; }
        private RobotManager MyManager;
        private readonly Configurations Config;
        [JsonIgnore]
        private MpsManager MpsManager;
        public List<CFinishedTask> FinishedTasks { get; }

        private string? JsonInformation;
        private TeamConfig teamConfig;
        private enum TaskEnum : int {
            None,
            Move,
            Deliver,
            Retrieve,
            Buffer,
            Explore
        }

        public Robot(Configurations config, RobotConfig robotConfig, RobotManager manager,
                     MpsManager mpsManager, Zone startZone, bool debug = false) {
            Config = config;
            RobotName = robotConfig.Name;
            RobotConfig = robotConfig;
            TeamName = Config.Teams[0].Name;
            TeamColor = robotConfig.TeamColor;
            foreach (var team in Config.Teams) {
                if (team.Name.Equals(TeamName)) {
                    teamConfig = team;
                }
            }
            if (teamConfig == null) {
                throw new Exception("TeamConfig for Robot not found:" + robotConfig.Name);
            }

            LastTaskMutex = new Mutex();
            TaskMutex = new Mutex();
            cancelBarrier = new Barrier(2);

            //TODO BEACON CONNECTION
            Position = new CPosition(5f, 1f, 0);
            MyManager = manager;
            JerseyNumber = robotConfig.Jersey;
            FinishedTasks = new List<CFinishedTask>();

            MyLogger = new MyLogger(this.JerseyNumber + "_" + this.RobotName, debug);
            MyLogger.Log(RobotName + " is ready for production!");
            Running = true;
            RobotState = RobotState.Active;

            MpsManager = mpsManager;
        }

        public void HandleAgentTaskMessage(AgentTask task) {
            if (task.TeamColor != TeamColor || task.RobotId != JerseyNumber) {
                MyLogger.Log("Got a task thats not for me. I ignore it!");
                //TODO TRANSMMIT REJECTION
                return;
            }
            TaskMutex.WaitOne();
            try {
                if (_currentTask != null) {
                    if (task.TaskId == _currentTask.TaskId) {
                        MyLogger.Log("Recived the current task again. Going to ignore that message");
                        return;
                    }
                    MyLogger.Log("Received a new task!");
                    TaskMutex.ReleaseMutex();
                    if (!CancelCurrentTask()) {
                        if (CurrentTask != null) {
                            throw new Exception("cancleing task wasn't succesfull but the task wasn't finished eitehr");
                        }
                    }
                    CurrentTask = task;
                }
            }
            finally {
                TaskMutex.ReleaseMutex();
            }
        }

        public bool CancelCurrentTask() {
            canceling = true;
            cancelBarrier.SignalAndWait();
            TaskMutex.WaitOne();
            // The Robot work thread will be stopped until Signaled Again
            try {
                if (_currentTask == null) {
                    // Task already finished
                    return false;
                }

                _currentTask.Canceled = true;
                _currentTask.Successful = false;
                LastTask = _currentTask;
                return true;
            }
            finally {
                // The Robot work thread can continue now
                canceling = false;
                TaskMutex.ReleaseMutex();
                cancelBarrier.SignalAndWait();
            }
        }

        public void TaskSucceded(AgentTask task) {
            TaskMutex.WaitOne();
            try {
                if (_currentTask == null) {
                    throw new Exception("No Task to finish");
                }
                //TODO REMOVE IF IT NEVER HAPPENS AND REMOVE THIS TASK PASING
                //THROUGH AND JUST USE THE CURRENTTASK GLOBAL VARIABLE
                if (_currentTask.TaskId != task.TaskId) {
                    throw new Exception("RACECONDITION HAPPENED");
                }
                _currentTask.Successful = true;
                LastTask = _currentTask;
                _currentTask = null;
            }
            finally {
                TaskMutex.ReleaseMutex();
            }
        }

        public void TaskFailed(AgentTask task, uint errorCode = 0) {
            TaskMutex.WaitOne();
            try {
                if (_currentTask == null) {
                    throw new Exception("No Task to finish");
                }
                //TODO REMOVE IF IT NEVER HAPPENS AND REMOVE THIS TASK PASING
                //THROUGH AND JUST USE THE CURRENTTASK GLOBAL VARIABLE
                if (_currentTask.TaskId != task.TaskId) {
                    throw new Exception("RACECONDITION HAPPENED");
                }
                _currentTask.Successful = false;
                _currentTask.ErrorCode = errorCode;
                errorCode = (uint)LlsfMsgs.AgentTask.Types.ErrorCode.InternalError;
                LastTask = _currentTask;
                _currentTask = null;
            }
            finally {
                TaskMutex.ReleaseMutex();
            }
        }

        public void Run() {

            var port = TeamColor == Team.Cyan ? Config.Refbox.CyanSendPort : Config.Refbox.MagentaSendPort;
            if (Config.RobotDirectBeaconSignals) {
                MyLogger.Log("Robot " + RobotName + " is starting! Team is " + teamConfig.Name
                            + " with ip " + Config.Refbox.IP + " and port " + port);
                BeaconConnector = new UdpConnector(Config, Config.Refbox.IP, port, this, MyLogger);
            }

            if (RobotConfig.connectionType == ConnectionType.TCP) {
                AgentConnector = new TcpConnector(Config, this, MyLogger);
            }
            else {
                AgentConnector = new UdpConnector(Config, this, MyLogger);
            }

            //TODO INIT CONNECTIOn
            MyLogger.Log("Starting " + RobotName + "'s working thread!");
            SerializeRobotToJson();
            while (Running) {
                if (canceling) {
                    cancelBarrier.SignalAndWait();
                    cancelBarrier.SignalAndWait();
                    // First time to be in sync with the Cancle function
                    // Second time to wait till the Cancle function is done
                }
                Work();
            }

            if (Config.RobotDirectBeaconSignals) {
                BeaconConnector?.Stop();
            }
        }

        public void HandleRobotInfo(LlsfMsgs.Robot info) {
            //TODO
        }

        public void HandleActive() {
            AgentTask? task = CurrentTask;
            if (task == null) {
                MyLogger.Log("No Tasks currently!");
                return;
            }
            MyLogger.Log("#######################################################################");
            MyLogger.Log("The current task = " + task.ToString());
            MyLogger.Log("#######################################################################");

            switch (CheckTaskType(task)) {
                case TaskEnum.Move:
                    HandleMove(task);
                    break;
                case TaskEnum.Retrieve:
                    GetFromStation(task);
                    break;
                case TaskEnum.Deliver:
                    DeliverToStation(task);
                    break;
                case TaskEnum.Buffer:
                    //TODO IMPLEMENT (REPLANISHMENT?)
                    break;
                case TaskEnum.Explore:
                    ExploreMachine(task);
                    break;
                default:
                    MyLogger.Log("Somehow an empty task was added?");
                    break;
            }
            SerializeRobotToJson();
        }


        public void Work() {
            switch (RobotState) {
                case RobotState.Active:
                    HandleActive();
                    break;
                case RobotState.Disqualified:
                    //TODO
                    break;
                case RobotState.Maintenance:
                    //TODO
                    break;
                default:
                    break;
            }

            Thread.Sleep(500);
        }

        public void SetZone(Zones zone) {
            CurrentZone = zone;
            Position = new CPosition(zone.X, zone.Y, 0);
        }

        private TaskEnum CheckTaskType(AgentTask task) {
            if (task?.Move != null)
                return TaskEnum.Move;
            else if (task?.Retrieve != null)
                return TaskEnum.Retrieve;
            else if (task?.Deliver != null)
                return TaskEnum.Deliver;
            else if (task?.Buffer != null)
                return TaskEnum.Buffer;
            else if (task?.ExploreMachine != null)
                return TaskEnum.Explore;
            return TaskEnum.None;
        }

        public void RobotStop() {
            Running = false;
        }

        public void SerializeRobotToJson() {
            JsonInformation = JsonSerializer.Serialize(this);
            //Console.WriteLine(JsonInformation);
        }
    }

    // The Beaconsignal contains a type named FinishedTask thatswhy the C fo custome
    public struct CFinishedTask {
        public uint TaskId { get; }
        public bool Successful { get; }

        public CFinishedTask(uint taskid, bool success) {
            TaskId = taskid;
            Successful = success;
        }
    }
}
