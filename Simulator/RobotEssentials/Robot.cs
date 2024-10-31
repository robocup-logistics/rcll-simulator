using System.Text.Json;
using System.Text.Json.Serialization;
using LlsfMsgs;
using Simulator.Utility;
using Simulator.MPS;


namespace Simulator.RobotEssentials;
public partial class Robot {
    public string RobotName { get; }
    public string TeamName { get; }
    public uint JerseyNumber { get; }
    public Team TeamColor { get; }
    public CPosition Position { get; set; }
    private readonly MyLogger MyLogger;
    private bool Running;
    public Products? HeldProduct { get; private set; }
    public Products? FutureProduct { get; private set; }
    private RobotState RobotState;
    public CZones? EntryZone;
    public CZones CurrentZone { get; private set; }
    public CZones HomeZone { get; private set; }
    public RobotConfig RobotConfig;
    public Random Random = new Random();
    //if the robot enters a machine, the input/output mutex that gets locked
    //will be stored here as a reference to make sure it releases it on leaving the input/output
    private Mutex? inputOutputMutex = null;
    private Barrier cancelBarrier;
    private bool canceling;
    //True when the robot is moving diagonal (1.41 times longer than normal movement)
    private bool isDiagonal;

    private UdpConnector? BeaconConnector;
    private ConnectorBase? AgentConnector;

    public Mutex TaskMutex;
    //only use the _currentTask if you locked the mutex by hand and need to actually modify the task and not retrieve it .....
    //Otherwise use the CurrentTask property
    private AgentTask? _currentTask;
    private ZonesManager ZonesManager;
    [JsonIgnore]
    public AgentTask? CurrentTask {
        get {
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
                LastTaskMutex.ReleaseMutex();
            }
        }
        set {
            LastTaskMutex.WaitOne();
            if (value != null) {
                FinishedTasks.Add(new CFinishedTask(value.TaskId, value.Successful));
            }
            _lastTask = value;
            LastTaskMutex.ReleaseMutex();
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
                 MpsManager mpsManager, CZones startZone, bool debug = false) {
        Config = config;
        RobotName = robotConfig.Name;
        RobotConfig = robotConfig;
        foreach (var team in Config.Teams) {
            if (team.Color == robotConfig.TeamColor) {
                teamConfig = team;
            }
        }
        if (teamConfig == null) {
            throw new Exception("TeamConfig for Robot not found:" + robotConfig.Name);
        }

        TeamName = teamConfig.Name;
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

        MyManager = manager;
        JerseyNumber = robotConfig.Jersey;
        FinishedTasks = new List<CFinishedTask>();

        MyLogger = new MyLogger(this.JerseyNumber + "_" + this.RobotName);
        MyLogger.Info(RobotName + " is ready for production!");
        Running = true;
        RobotState = RobotState.Active;

        MpsManager = mpsManager;
        ZonesManager = ZonesManager.GetInstance();
        HomeZone = startZone;
        CurrentZone = startZone;
        Position = new CPosition(startZone.X, startZone.Y, 180);
    }

    public void HandleAgentTaskMessage(AgentTask task) {
        if (task.TeamColor != TeamColor || task.RobotId != JerseyNumber) {
            MyLogger.Warn("Got a task thats not for me. I ignore it!");
            return;
        }
        if (RobotState != RobotState.Active) {
            MyLogger.Info("Robot is not active. Ignoring the task");
            return;
        }
        TaskMutex.WaitOne();
        try {
            if (_currentTask != null) {
                if (task.TaskId == _currentTask.TaskId) {
                    MyLogger.Debug("Recived the current task again. Going to ignore that message");
                    return;
                }
                MyLogger.Info("Received a new task!");
                if (!CancelCurrentTask()) {
                    if (CurrentTask != null) {
                        throw new Exception("cancleing task wasn't succesfull but the task wasn't finished eitehr");
                    }
                }
                _currentTask = task;
            }
            else {
                _currentTask = task;
            }
        }
        finally {
            TaskMutex.ReleaseMutex();
        }
    }

    //RobotInfo and AgetTask can both call this function to prevent race conditons
    private object CancelLock = new Object();
    public bool CancelCurrentTask() {
        lock (CancelLock) {
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
    }

    public void TaskSucceded(AgentTask task) {
        TaskMutex.WaitOne();
        try {
            if (_currentTask == null) {
                throw new Exception("No Task to finish");
            }

            if (_currentTask.TaskId != task.TaskId) {
                throw new Exception("RACECONDITION HAPPENED");
            }

            MyLogger.Info("Task " + task.TaskId + " was successful!");
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

            if (_currentTask.TaskId != task.TaskId) {
                throw new Exception("RACECONDITION HAPPENED");
            }

            _currentTask.Successful = false;
            _currentTask.ErrorCode = errorCode;
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
            MyLogger.Info("Robot " + RobotName + " is starting! Team is " + teamConfig.Name
                        + " with ip " + Config.Refbox.IP + " and port " + port);
            BeaconConnector = new UdpConnector(Config, Config.Refbox.IP, port, this, MyLogger, teamConfig.Keyphrase);
        }

        if (RobotConfig.connectionType == ConnectionType.TCP) {
            AgentConnector = new TcpConnector(Config, this, MyLogger);
        }
        else {
            AgentConnector = new UdpConnector(Config, this, MyLogger);
        }

        MyLogger.Info("Starting " + RobotName + "'s working thread!");
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
        if (info.State == RobotState.Maintenance) {
            RobotState = RobotState.Maintenance;
            CancelCurrentTask();
            HeldProduct = null;
            SetZone(HomeZone);
        }
        else if (info.State == RobotState.Disqualified) {
            RobotState = RobotState.Disqualified;
            CancelCurrentTask();
            HeldProduct = null;
            SetZone(HomeZone);
        }
        else {
            RobotState = RobotState.Active;
        }
    }

    public void HandleActive() {
        AgentTask? task = CurrentTask;
        if (task == null) {
            // MyLogger.Log("No Tasks currently!");
            return;
        }
        MyLogger.Info("#######################################################################");
        MyLogger.Info("The current task = " + task.ToString());
        MyLogger.Info("#######################################################################");

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
                BufferAtStation(task);
                break;
            case TaskEnum.Explore:
                ExploreMachine(task);
                break;
            default:
                MyLogger.Warn("Somehow an empty task was added?");
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
                Thread.Sleep(500);
                break;
            case RobotState.Maintenance:
                Thread.Sleep(500);
                break;
            default:
                throw new Exception("Unknown Robot State");
        }

        Thread.Sleep(500);
    }

    public void SetPositionBack(float distance) {
        // Convert orientation from degrees to radians
        float orientationRadians = Position.Orientation * (float)(Math.PI / 180.0);

        // Calculate the new position
        float newX = Position.X + distance * (float)Math.Sin(orientationRadians);
        float newY = Position.Y + distance * (float)Math.Cos(orientationRadians);

        // Assuming there's a method to set the position, e.g., SetPosition
        Position.SetPosition(newX, newY);
    }


    public void LookAtZone(CZones zone) {
        float deltaX = CurrentZone.X - zone.X;
        float deltaY = CurrentZone.Y - zone.Y;
        float angle = (float)(Math.Atan2(deltaX, deltaY) * (180.0 / Math.PI));
        if (deltaX == 0 || deltaY == 0) {
            isDiagonal = false;
        }
        else {
            isDiagonal = true;
        }
        Position.SetOrientation(angle);
    }

    public void SetZone(CZones zone) {
        CurrentZone = zone;
        Position.SetPosition(zone.X, zone.Y);
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
    }

    public bool RoleTheDice(int percentage)
    {
        int randomValue = Random.Next(0, 100);
        return randomValue < percentage;
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
