using Simulator.RobotEssentials;

namespace Simulator.Utility;
public class RobotLock {
    private int? Robot;
    private readonly object _lock = new();
    private readonly ManualResetEventSlim _resourceAvailable = new(true);

    // Acquire the resource with timeout
    public bool Acquire(Robot me, int timeoutMilliseconds) {
        if (_resourceAvailable.Wait(timeoutMilliseconds)) {
            lock (_lock) {
                if (Robot == null) {
                    Robot = me.GetHashCode();
                    _resourceAvailable.Reset(); // Block other threads
                    return true;
                }
            }
        }
        return false;
    }

    // Release the resource
    public bool Release(Robot me) {
        lock (_lock) {
            if (Robot == Robot.GetHashCode()) {
                Robot = null;
                _resourceAvailable.Set(); // Signal that the resource is available
                return true;
            }
        }
        return false;
    }

    // Mother thread forcefully releases the resource
    public void ForceRelease() {
        lock (_lock) {
            Robot = null;
            _resourceAvailable.Set(); // Signal that the resource is available
        }
    }
}
