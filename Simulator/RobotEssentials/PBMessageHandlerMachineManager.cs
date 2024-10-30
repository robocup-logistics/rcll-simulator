using Google.Protobuf;
using LlsfMsgs;
using Simulator.MPS;
using Simulator.Utility;
using Timer = Simulator.Utility.Timer;

namespace Simulator.RobotEssentials;
class PBMessageHandlerMachineManager : PBMessageHandlerBase {
    private readonly MpsManager mpsManager_;
    private readonly RobotManager robotManager_;
    private readonly ZonesManager zonesManager_;

    public PBMessageHandlerMachineManager(Configurations config, MpsManager mpsManager, RobotManager robotManager, MyLogger log)
        : base(config, log) {
        mpsManager_ = mpsManager;
        robotManager_ = robotManager;
        zonesManager_ = ZonesManager.GetInstance();
    }

    #region Message Handling

    protected override bool ProcessMessage(byte[] stream, int componentId, int messageType, int payloadSize) {
        switch (messageType) {
            case (int)MachineInfo.Types.CompType.MsgType:
                return HandleMachineInfo(stream, componentId, payloadSize);
            case (int)GameState.Types.CompType.MsgType:
                return HandleGameState(stream, payloadSize);
            case (int)RobotInfo.Types.CompType.MsgType:
                return HandleRobotInfo(stream, componentId, payloadSize);
            default:
                MyLogger.Warn($"Unknown MessageType {messageType} for Component {componentId}");
                return false;
        }
    }

    #endregion

    #region Message Type Handlers

    private bool HandleMachineInfo(byte[] stream, int componentId, int payloadSize) {
        if ((int)MachineInfo.Types.CompType.CompId != componentId) {
            MyLogger.Warn($"Component ID mismatch: expected {MachineInfo.Types.CompType.CompId}, found {componentId}");
            return false;
        }

        var machineInfoParser = new MessageParser<MachineInfo>(() => new MachineInfo());
        try {
            var machineInfo = machineInfoParser.ParseFrom(stream, 12, payloadSize - 4);
            MyLogger.Debug("MachineInfo message parsed successfully.");
            MyLogger.Debug($"Parsed message: {machineInfo}");
            // Additional handling logic...
            string msg = machineInfo.ToString();
            MyLogger.Info($"The Parsed message = {msg}");
            //TODO remove
            if (machineInfo.Machines.Count < mpsManager_.Machines.Count) {
                MyLogger.Debug("MachineInfo is not containing all machines!");
                return false;
            }
            zonesManager_.ZoneManagerMutex.WaitOne();
            if (mpsManager_.AllMachineSet) {
                ZonesManager.GetInstance().ZoneManagerMutex.ReleaseMutex();
                return true;
            }
            mpsManager_.PlaceMachines(machineInfo);
            zonesManager_.ZoneManagerMutex.ReleaseMutex();
            return true;
        }
        catch (Exception e) {
            MyLogger.Error($"Parsing error: {e}");
            return false;
        }
    }

    private bool HandleRobotInfo(byte[] stream, int componentId, int payloadSize) {
        var robotInfoParser = new MessageParser<RobotInfo>(() => new RobotInfo());
        var robotInfo = robotInfoParser.ParseFrom(stream, 12, payloadSize - 4);

        robotManager_.HandleRobotInfo(robotInfo);

        MyLogger.Info("GameInfo message parsed successfully.");
        MyLogger.Debug($"Parsed message: {robotInfo}");
        return true;
    }

    private bool HandleGameState(byte[] stream, int payloadSize) {
        var gameStateParser = new MessageParser<GameState>(() => new GameState());
        var gameState = gameStateParser.ParseFrom(stream, 12, payloadSize - 4);
        Timer.GetInstance(Config).UpdateTime(gameState.GameTime);

        if (gameState.HasPointsCyan)
            Config.Teams[0].Points = gameState.PointsCyan;
        if (gameState.HasPointsMagenta)
            Config.Teams[1].Points = gameState.PointsMagenta;

        MyLogger.Info("GameState message parsed successfully.");
        MyLogger.Debug($"Parsed message: {gameState}");
        return true;
    }

    #endregion
}
