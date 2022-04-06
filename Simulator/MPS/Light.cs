using LlsfMsgs;
using System.Threading;


namespace Simulator.MPS
{

    public class Light
    {
        public LightColor LightColor { get; }
        public LightState LightState;
        public bool LightOn { get; private set; }
        private readonly ManualResetEvent Event;
        public Light(LightColor color, ManualResetEvent mre)
        {
            LightColor = color;
            LightState = LightState.Off;
            Event = mre;
            LightOn = false;
            var lightThread = new Thread(StateMachine);
            lightThread.Start();
        }

        public void SetLight(LightState update)
        {
            LightState = update;
            Event.Set();

        }

        private void StateMachine()
        {
            while (true)
            {
                switch (LightState)
                {
                    case LightState.Off:
                        if(LightOn)
                            LightOn = false;
                        break;
                    case LightState.On:
                        if(!LightOn)
                            LightOn = true;
                        break;
                    case LightState.Blink:
                        LightOn = !LightOn;
                        break;
                    default:
                        // nothing to do here
                        break;
                }
                //Console.WriteLine("Waiting on LightEvent");
                Event.WaitOne();
                Event.Reset();
                //Console.WriteLine("Got on LightEvent");
            }
        }

    }
}
