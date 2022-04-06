namespace Simulator.RobotEssentials
{
    interface IConnector
    {
        bool Connect();
        bool Close();
        bool Start();
        bool Stop();
        void SendThreadMethod(int port);
        void ReceiveThreadMethod(int port);

    }
}
