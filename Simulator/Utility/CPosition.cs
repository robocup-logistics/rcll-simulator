namespace Simulator.Utility {
    public struct CPosition {
        public float X { get; set; }
        public float Y { get; set; }
        public int Orientation { get; set; }

        public CPosition() {
            X = 0f;
            Y = 0f;
            Orientation = 0;
        }

        public CPosition(float x, float y, int orientation) {
            X = x;
            Y = y;
            Orientation = orientation;
        }
    }
}
