using Google.Protobuf;
using LlsfMsgs;
using Simulator.MPS;
using Simulator.Utility;
using Timer = Simulator.Utility.Timer;

namespace Simulator.RobotEssentials {
    class PBMessageHandlerMachineManager : PBMessageHandlerBase {
        private readonly MpsManager Manager;

        public PBMessageHandlerMachineManager(Configurations config, MpsManager mpsManager, MyLogger log)
            : base(config, log) {
            Manager = mpsManager;
        }

        #region Message Handling

        public override bool HandleMessage(byte[] stream) {
            if (!IsValidStream(stream))
                return false;

            int payloadSize = BytesToInt(stream, 4, 4);
            int componentId = BytesToInt(stream, 8, 2);
            int messageType = BytesToInt(stream, 10, 2);

            if (payloadSize == 0) {
                MyLogger.Log($"The payload size is {payloadSize}, no further processing.");
                return false;
            }

            return ProcessMessage(stream, componentId, messageType, payloadSize);
        }

        private bool IsValidStream(byte[] stream) {
            /*    Each row is 4 bytes
             * 1. Protocol version, Cipher, Reserved byte1 , reserved byte2
             * 2. Payload size
             * 3. component ID and Message type each 2 bytes. Used to detect the Protobuff message
             * */
            if (stream.Length < 4) {
                MyLogger.Log("The received message is too short to be parsed!");
                return false;
            }

            if (FrameHeader.Version != stream[0]) {
                MyLogger.Log("Version mismatch!");
                return false;
            }

            if (FrameHeader.Cipher != stream[1]) {
                MyLogger.Log("Cipher mismatch!");
                return false;
            }

            // Additional checks for reserved bytes can be implemented if needed.

            return true;
        }

        private bool ProcessMessage(byte[] stream, int componentId, int messageType, int payloadSize) {
            switch (messageType) {
                case (int)Machine.Types.CompType.MsgType:
                    return HandleMachine(stream, payloadSize);
                case (int)MachineInfo.Types.CompType.MsgType:
                    return HandleMachineInfo(stream, componentId, payloadSize);
                case (int)GameState.Types.CompType.MsgType:
                    return HandleGameState(stream, payloadSize);
                default:
                    MyLogger.Log($"Unknown MessageType {messageType} for Component {componentId}");
                    return false;
            }
        }

        #endregion

        #region Message Type Handlers

        private bool HandleMachine(byte[] stream, int payloadSize) {
            var machineParser = new MessageParser<Machine>(() => new Machine());
            var machine = machineParser.ParseFrom(stream, 12, payloadSize - 4);
            MyLogger.Log("MachineInfo message parsed successfully.");
            MyLogger.Log($"Parsed message: {machine}");
            return true;
        }

        private bool HandleMachineInfo(byte[] stream, int componentId, int payloadSize) {
            if ((int)MachineInfo.Types.CompType.CompId != componentId) {
                MyLogger.Log($"Component ID mismatch: expected {MachineInfo.Types.CompType.CompId}, found {componentId}");
                return false;
            }

            var machineInfoParser = new MessageParser<MachineInfo>(() => new MachineInfo());
            try {
                var machineInfo = machineInfoParser.ParseFrom(stream, 12, payloadSize - 4);
                MyLogger.Log("MachineInfo message parsed successfully.");
                MyLogger.Log($"Parsed message: {machineInfo}");
                // Additional handling logic...
                string msg = machineInfo.ToString();
                MyLogger.Log($"The Parsed message = {msg}");
                if (machineInfo.Machines.Count < Manager.Machines.Count) {
                    MyLogger.Log("MachineInfo is not containing all machines!");
                    return false;
                }
                ZonesManager.GetInstance().ZoneManagerMutex.WaitOne();
                if (Manager.AllMachineSet) {
                    ZonesManager.GetInstance().ZoneManagerMutex.ReleaseMutex();
                    return true;
                }
                Manager.PlaceMachines(machineInfo);
                ZonesManager.GetInstance().ZoneManagerMutex.ReleaseMutex();
                return true;
            }
            catch (Exception e) {
                MyLogger.Log($"Parsing error: {e}");
                return false;
            }
        }

        private bool HandleGameState(byte[] stream, int payloadSize) {
            var gameStateParser = new MessageParser<GameState>(() => new GameState());
            var gameState = gameStateParser.ParseFrom(stream, 12, payloadSize - 4);
            Timer.GetInstance(Config).UpdateTime(gameState.GameTime);

            if (gameState.HasPointsCyan)
                Config.Teams[0].Points = gameState.PointsCyan;
            if (gameState.HasPointsMagenta)
                Config.Teams[1].Points = gameState.PointsMagenta;

            MyLogger.Log("GameState message parsed successfully.");
            MyLogger.Log($"Parsed message: {gameState}");
            return true;
        }

        #endregion
    }
}
