namespace Simulator.Utility {
    public class CPosition {
        public float X { get; set; }
        public float Y { get; set; }
        public float Orientation { get; set; }

        public CPosition() {
            X = 0f;
            Y = 0f;
            Orientation = 0;
        }

        public void SetPosition(float x, float y) {
            X = x;
            Y = y;
        }

        public void SetOrientation(float orientation) {
            Orientation = orientation;
        }

        public CPosition(float x, float y, float orientation) {
            X = x;
            Y = y;
            Orientation = orientation;
        }
    }
}
