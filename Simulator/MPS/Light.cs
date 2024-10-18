using LlsfMsgs;

namespace Simulator.MPS {

    public class Light {
        public LightColor LightColor { get; }
        public LightState LightState;
        private bool _lightOn;
        public bool LightOn
        {
            get
            {
                switch (LightState) {
                    case LightState.Off:
                        LightOn = false;
                        break;
                    case LightState.On:
                        LightOn = true;
                        break;
                    case LightState.Blink:
                        // Blink every second based on the current time
                        if(DateTime.Now.Second % 2 == 0)
                            LightOn = true;
                        else
                            LightOn = false;
                        break;
                }
                return _lightOn;
            }
            private set
            {
                _lightOn = value;
            }
        }
        public Light(LightColor color) {
            LightColor = color;
            LightState = LightState.Off;
            LightOn = false;
        }

        public void SetLight(LightState update) {
            LightState = update;
        }
    }
}
