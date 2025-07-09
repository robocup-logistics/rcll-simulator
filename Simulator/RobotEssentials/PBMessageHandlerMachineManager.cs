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
    private readonly GTMonitor? GTMonitor;

    public PBMessageHandlerMachineManager(Configurations config, MpsManager mpsManager, RobotManager robotManager, GTMonitor? gtMonitor,
                                          MyLogger log)
        : base(config, log) {
        mpsManager_ = mpsManager;
        robotManager_ = robotManager;
        zonesManager_ = ZonesManager.GetInstance();
        GTMonitor = gtMonitor;
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
            case (int)OrderInfo.Types.CompType.MsgType:
            case (int)AttentionMessage.Types.CompType.MsgType:
            case (int)VersionInfo.Types.CompType.MsgType:
                return true;
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
            MyLogger.Info("MachineInfo message parsed successfully.");
            MyLogger.Debug($"Parsed message: {machineInfo}");

            if (GTMonitor != null)
                GTMonitor.Append(machineInfo);

            string msg = machineInfo.ToString();
            MyLogger.Debug($"The Parsed message = {msg}");
            mpsManager_.HandleMachineInfo(machineInfo);
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
        MyLogger.Info("GameState message parsed successfully.");
        MyLogger.Debug($"Parsed message: {gameState}");

        if (CurrentGame.height != gameState.FieldHeight || CurrentGame.width != gameState.FieldWidth) {
            MyLogger.Info($"Field size changed from {CurrentGame.width}x{CurrentGame.height} to {gameState.FieldWidth}x{gameState.FieldHeight}");
            zonesManager_.Resize(gameState.FieldWidth, gameState.FieldHeight);
        }

        if (CurrentGame.GamePhase != gameState.Phase) {
            MyLogger.Info($"Game Phase changed from {CurrentGame.GamePhase} to {gameState.Phase}");
            if (gameState.Phase == GameState.Types.Phase.Setup) {
                mpsManager_.ResetMachines();
                robotManager_.ResetRobots();
            }
            CurrentGame.GamePhase = gameState.Phase;
        }

        CurrentGame.GameState = gameState.State;

        return true;
    }

    #endregion
}
