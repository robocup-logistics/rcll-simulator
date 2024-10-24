using Simulator.Utility;

namespace Simulator.RobotEssentials {
    class PBMessageHandlerBase {
        public MyLogger MyLogger { get; private set; }
        public Configurations Config { get; private set; }
        public PBMessageHandlerBase(Configurations config, MyLogger log) {
            MyLogger = log;
            Config = config;
        }

        public int CheckMessageHeader(byte[] Stream) {
            if (Stream.Length < 8) {
                MyLogger.Log("The received Message is to short to be parsed!");
                return -1;
            }
            //TODO FRAME HEADER I MAYBE DELETED TOO MUCH
            if (FrameHeader.Version != Stream[0]) {
                MyLogger.Log("Version is different!");
                return -1;
            }
            if (FrameHeader.Cipher != Stream[1]) {
                MyLogger.Log("Cipher is different!");
                return -1;
            }
            var payloadsize = BytesToInt(Stream, 4, 4);
            return payloadsize;
        }

        protected virtual bool ProcessMessage(byte[] stream, int componentId, int messageType, int payloadSize) {
            throw new NotImplementedException();
        }

        public bool HandleMessage(byte[] stream) {
            if (CheckMessageHeader(stream) == -1)
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

        public static int BytesToInt(byte[] bytes, int start, int length) {

            var ret = 0;
            switch (length) {
                case 2:
                    ret += (bytes[start] << 8);
                    ret += bytes[start + 1];
                    break;
                case 4:
                    //MyLogger.Log("Bytes to int!");

                    ret += (bytes[start] << 24);
                    //MyLogger.Log("Current byte = " + bytes[start] + " and we calculated the sum = " + ret);
                    ret += (bytes[start + 1] << 16);
                    //MyLogger.Log("Current byte = " + bytes[start+1] + " and we calculated the sum = " + ret);
                    ret += (bytes[start + 2] << 8);
                    //MyLogger.Log("Current byte = " + bytes[start+2] + " and we calculated the sum = " + ret);
                    ret += (bytes[start + 3]);

                    //MyLogger.Log("Bytes to int!");
                    break;
                default:

                    break;
            }
            return ret;
        }

    }
}
