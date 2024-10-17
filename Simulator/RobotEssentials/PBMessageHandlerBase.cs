using System;
using System.Text;
using System.Threading;
using Google.Protobuf;
using LlsfMsgs;
using Org.BouncyCastle.Math.EC;
using Simulator.MPS;
using Simulator.Utility;
using Timer = Simulator.Utility.Timer;

namespace Simulator.RobotEssentials
{
    class PBMessageHandlerBase
    {
        public MyLogger MyLogger { get; private set; }
        public Configurations Config { get; private set; }
        public PBMessageHandlerBase(Configurations config, MyLogger log)
        {
            MyLogger = log;
            Config = config;
        }

        public int CheckMessageHeader(byte[] Stream)
        {
            if (Stream.Length < 4)
            {
                MyLogger.Log("The received Message is to short to be parsed!");
                return -1;
            }
            if (FrameHeader.Version != Stream[0])
            {
                MyLogger.Log("Version is different!");
                return -1;
            }
            if (FrameHeader.Cipher != Stream[1])
            {
                MyLogger.Log("Cipher is different!");
                return -1;
            }
            var payloadsize = BytesToInt(Stream, 4, 4);
            return payloadsize;
        }
        public virtual bool HandleMessage(byte[] Stream)
        {

            /*      Each row is 4 bytes
             * 1.   Protocol version, Cipher, Reserved byte1 , reserved byte2
             * 2.   Payload size 
             * 3.   component ID and Message type each 2 bytes. Used to detect the Protobuff message
             * 
             * */
            if (Stream.Length < 4)
            {
                MyLogger.Log("The received Message is to short to be parsed!");
                return false;
            }
            if (FrameHeader.Version != Stream[0])
            {
                MyLogger.Log("Version is different!");
                return false;
            }
            if (FrameHeader.Cipher != Stream[1])
            {
                MyLogger.Log("Cipher is different!");
                return false;
            }
            /*if (FrameHeader.Reserved != Stream[2] && FrameHeader.Reserved2 != Stream[3])
            {
                MyLogger.Log("Reserved is different!");
            }*/
            int payloadsize = BytesToInt(Stream, 4, 4);
            //MyLogger.Log("The payload has : " + payloadsize.ToString() + " bytes!");
            int cmpId = BytesToInt(Stream, 8, 2);
            int msgtype = BytesToInt(Stream, 10, 2);
            // string msg = "";
            //MyLogger.Log("The Recieved message is from component : " + cmpId.ToString() + " and the message type is = " + msgtype.ToString() + " and payloadsize = " + payloadsize);
            //MyLogger.Log("Length of the stream = " + Stream.Length);
            if (payloadsize == 0)
            {
                MyLogger.Log("The payload is " + payloadsize + " so we stop here!");
                return false;
            }
            
            return true;
        }
        public static int BytesToInt(byte[] bytes, int start, int length)
        {

            var ret = 0;
            switch (length)
            {
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
