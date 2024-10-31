using LlsfMsgs;
using Simulator.MPS;
using System.Text.Json.Serialization;

namespace Simulator.Utility;
public class ZonesManager {
    public List<CZones> ZoneList { get; private set; }
    private readonly Dictionary<Zone, CZones> Dictionary;
    private static ZonesManager? Instance;
    public Mutex ZoneManagerMutex;
    private MyLogger MyLogger;

    private static readonly object _lock = new object();
    /// <returns>
    /// Returns the instance of the Configurations Singleton
    /// </returns>
    public static ZonesManager GetInstance() {
        if (Instance == null) {
            lock (_lock) {
                if (Instance == null) {
                    Instance = new ZonesManager();
                }
            }
        }
        return Instance;
    }

    private ZonesManager() {
        ZoneList = new List<CZones>();
        Dictionary = new Dictionary<Zone, CZones>();
        MyLogger = new MyLogger("Zones");
        ZoneManagerMutex = new Mutex();
        MyLogger.Info("Creating General Zones");
        foreach (Zone z in Enum.GetValues(typeof(Zone))) {
            int val = (int)z;
            // Y value is the last digit
            var y = val % 10;
            val /= 10;
            // X value is the second last digit
            var x = val % 10;
            val /= 10;
            // Team side is determind by adding 1000 to Magenta
            Team color = Team.Cyan;
            if (val > 0) {
                color = Team.Magenta;
            }

            CZones zone;
            zone = color == Team.Cyan ? new CZones(x - 0.5f, y - 0.5f, 0, color, z) : new CZones(-x + 0.5f, y - 0.5f, 0, color, z);
            Dictionary.Add(z, zone);
            ZoneList.Add(zone);
        }

        MyLogger.Info("Starting to add Neighborhood");
        AddNeighborhood();
        SetInsertionZone();
    }

    //Removes the Insertionzones Connection
    public void SetInsertionZone() {
        Dictionary[Zone.MZ71].SetNeighborhood(Dictionary[Zone.MZ61]);
        Dictionary[Zone.MZ61].SetNeighborhood(Dictionary[Zone.MZ51]);
        Dictionary[Zone.MZ51].SetNeighborhood(Dictionary[Zone.MZ52]);
        Dictionary[Zone.CZ71].SetNeighborhood(Dictionary[Zone.CZ61]);
        Dictionary[Zone.CZ61].SetNeighborhood(Dictionary[Zone.CZ51]);
        Dictionary[Zone.CZ51].SetNeighborhood(Dictionary[Zone.CZ52]);
    }

    public CZones? GetZone(Zone zone) {
        return Dictionary.ContainsKey(zone) ? Dictionary[zone] : null;
    }

    public Zone GetWaypoint(string target, string machinepoint = "") {
        MyLogger.Info("GetWayPoint with target [" + target + " and machinepoint = " + machinepoint + "]!");
        Zone result;
        try {
            if (target.Contains("C_Z") || target.Contains("M_Z")) {
                target = target.Substring(0, 5);
            }
            result = (Zone)Enum.Parse(typeof(Zone), target.Replace("_", ""));
            MyLogger.Info("Is a Zone Waypoint!");
            return result;
        }
        catch (Exception) {
            return GetZoneNextToMachine(target, machinepoint); ;
        }
    }

    public void PlaceMachine(Zone zone, uint orientation, Mps machine) {
        if (!Dictionary.ContainsKey(zone)) return;
        MyLogger.Info("Placed " + machine.Name + " at zone " + zone + " with the orientation " + orientation);
        Dictionary[zone].PlaceMachine(machine, orientation);
        machine.Zone = zone;
    }

    public CZones? GetMachineZone(string MachineName) {
        foreach (var (key, value) in Dictionary) {
            if (value.Machine != null && MachineName.Contains(value.Machine.Name)) {
                return value;
            }
        }
        return null;
    }

    public Zone GetZoneNextToMachine(string MachineName, string machinepoint = "") {
        MyLogger.Info("Getting Zone next to machine!" + MachineName);
        foreach (var (key, value) in Dictionary) {
            if (value.Machine != null && MachineName.Contains(value.Machine.Name)) {
                var orientation = value.Orientation;
                var neighborhood = value.GetNeighborhood();
                if (machinepoint.ToLower().Equals("output")) {
                    orientation += 180;
                    orientation %= 360;
                }

                var waypoint = Zone.CZ11;

                var radians = (Math.PI / 180) * orientation;
                MyLogger.Info("Orientation = " + orientation + " and in radians " + radians);
                var y = Convert.ToInt32(Math.Sin(radians));
                var x = Convert.ToInt32(Math.Cos(radians));
                MyLogger.Info("X offset = " + x + " and offset y = " + y);
                waypoint = CheckNeighbours(neighborhood, value, x, y);
                return waypoint;
            }
        }
        MyLogger.Error("Couldn't find the machine " + MachineName);
        return 0;
    }

    public Zone CheckNeighbours(List<CZones> Neighbours, CZones compareable, int x, int y) {
        MyLogger.Info("Checking " + compareable.X + "/" + compareable.Y);
        foreach (var n in Neighbours) {
            if (n.X == compareable.X + x && n.Y == compareable.Y + y) {
                MyLogger.Debug("The searched neighbour is " + n.ZoneId + " with " + n.X + "/" + n.Y);
                return n.ZoneId;
            }
        }
        MyLogger.Warn("No neighbour found!");
        return 0;
    }

    public List<CZones> Astar(CZones start, CZones end) {
        var comparer = Comparer<CZones>.Create(
            (k1, k2) => k1.ZoneId.CompareTo(k2.ZoneId));
        SortedDictionary<CZones, double> openList = new SortedDictionary<CZones, double>(comparer);
        Dictionary<Zone, int> closedList = new Dictionary<Zone, int>();
        Dictionary<CZones, CZones> cameFrom = new Dictionary<CZones, CZones>();
        openList.Add(start, 0);
        SortedDictionary<Zone, double> gScore = new SortedDictionary<Zone, double>();
        SortedDictionary<Zone, double> fScore = new SortedDictionary<Zone, double>();
        gScore.Add(start.ZoneId, 0);
        fScore.Add(start.ZoneId, CalcDistance(start, end));
        List<CZones> path = new List<CZones>();
        while (openList.Count != 0) {
            var values = openList.Values.ToList();
            var index = values.IndexOf(values.Min());
            var current = openList.Keys.ElementAt(index);

            if (current.ZoneId == end.ZoneId) {
                MyLogger.Info("A Valid path has been found!");
                return ReconstructPath(cameFrom, current);
            }

            openList.Remove(current);
            var neighborhood = current.GetNeighborhood();

            foreach (var neighbor in neighborhood) {
                if (neighbor.Machine != null) {
                    MyLogger.Debug("Skipping field as there is a machine!");
                    continue;
                }
                var tentative_gScore = gScore[current.ZoneId] + CalcDistance(neighbor, current);
                if (!gScore.ContainsKey(neighbor.ZoneId)) {
                    gScore.Add(neighbor.ZoneId, 1000);
                }
                if (tentative_gScore < gScore[neighbor.ZoneId]) {
                    // This path to neighbor is better than any previous one. Record it!
                    if (cameFrom.ContainsKey(neighbor)) {
                        cameFrom[neighbor] = current;
                    }
                    else {
                        cameFrom.Add(neighbor, current);
                    }
                    gScore[neighbor.ZoneId] = tentative_gScore;
                    var dist = CalcDistance(neighbor, end);
                    var comp = tentative_gScore + dist;

                    fScore[neighbor.ZoneId] = comp;

                    if (!openList.ContainsKey(neighbor)) {
                        openList.Add(neighbor, comp);
                    }
                    else {
                        openList[neighbor] = comp;
                    }
                }
            }
        }
        MyLogger.Warn("No Valid Path found for target [" + end.GetZoneString() + "]!");
        return new List<CZones>();
    }

    private static List<CZones> ReconstructPath(Dictionary<CZones, CZones> cameFrom, CZones current) {
        var path = new List<CZones>
        {
                current
            };
        bool finished = false;
        while (!finished) {
            try {
                var element = cameFrom[path[^1]];
                path.Add(element);
            }
            catch (Exception) {
                path.Reverse();
                finished = true;
            }
        }

        path.RemoveAt(0); // remove the starting position from the path!
        return path;
    }

    public static double CalcDistance(CZones Start, CZones End) {
        if (Start == null || End == null) {
            ZonesManager.GetInstance().MyLogger.Warn("CalcDistance has a null?");
            return 1000;
        }

        return Math.Sqrt(Math.Pow((Start.X - End.X), 2) + Math.Pow((Start.Y - End.Y), 2));
    }
    private void AddNeighborhood() {
        foreach (var z in ZoneList) {
            // add all adjacent fields
            if (Dictionary.ContainsKey(z.ZoneId + 1)) {
                z.AddNeighbor(Dictionary[z.ZoneId + 1]);
            }
            if (Dictionary.ContainsKey(z.ZoneId - 1)) {
                z.AddNeighbor(Dictionary[z.ZoneId - 1]);
            }
            if (Dictionary.ContainsKey(z.ZoneId + 10)) {
                z.AddNeighbor(Dictionary[z.ZoneId + 10]);
            }
            if (Dictionary.ContainsKey(z.ZoneId - 10)) {
                z.AddNeighbor(Dictionary[z.ZoneId - 10]);
            }
            // Add diagonal fields
            if (Dictionary.ContainsKey(z.ZoneId - 9)) {
                z.AddNeighbor(Dictionary[z.ZoneId - 9]);
            }
            if (Dictionary.ContainsKey(z.ZoneId - 11)) {
                z.AddNeighbor(Dictionary[z.ZoneId - 11]);
            }
            if (Dictionary.ContainsKey(z.ZoneId + 9)) {
                z.AddNeighbor(Dictionary[z.ZoneId + 9]);
            }
            if (Dictionary.ContainsKey(z.ZoneId + 11)) {
                z.AddNeighbor(Dictionary[z.ZoneId + 11]);
            }

            //Added to connect the two halves of the mapping
            if (((int)z.ZoneId) % 100 - 10 < 10) {
                if ((int)z.ZoneId > 100) {
                    if (Dictionary.ContainsKey(z.ZoneId - 1000)) {
                        z.AddNeighbor(Dictionary[z.ZoneId - 1000]);
                    }
                    if (Dictionary.ContainsKey(z.ZoneId - 1001)) {
                        z.AddNeighbor(Dictionary[z.ZoneId - 1001]);
                    }
                    if (Dictionary.ContainsKey(z.ZoneId - 999)) {
                        z.AddNeighbor(Dictionary[z.ZoneId - 999]);
                    }
                }
                else {
                    if (Dictionary.ContainsKey(z.ZoneId + 1000)) {
                        z.AddNeighbor(Dictionary[z.ZoneId + 1000]);
                    }
                    if (Dictionary.ContainsKey(z.ZoneId + 1001)) {
                        z.AddNeighbor(Dictionary[z.ZoneId + 1001]);
                    }
                    if (Dictionary.ContainsKey(z.ZoneId + 999)) {
                        z.AddNeighbor(Dictionary[z.ZoneId + 999]);
                    }
                }
            }
            MyLogger.Debug("Added " + z.GetNeighborhood().Count + " to the zone " + z.ZoneId);
        }

    }
}

public class CZones {

    private readonly Team ZoneColor;

    private readonly List<CZones> NeighborsList;

    [JsonIgnore]
    public Mps? Machine { get; private set; }

    [JsonIgnore]
    public uint Orientation { get; private set; }
    public Zone ZoneId { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }

    public CZones(float x, float y, uint orientation, Team color, Zone zoneId) {
        X = x;
        Y = y;
        Orientation = orientation;
        ZoneColor = color;
        NeighborsList = new List<CZones>();
        ZoneId = zoneId;
        Machine = null;
    }

    public void AddNeighbor(CZones newNeighbor) {
        NeighborsList.Add(newNeighbor);
    }
    public bool Free() {
        if (Machine == null) {
            return true;
        }
        return false;
    }

    public void PlaceMachine(MPS.Mps machine, uint orientation) {
        Machine = machine;
        Orientation = orientation;
    }

    public string GetZoneString() {
        if (Machine != null) {
            return Machine.Name;
        }

        return ZoneId.ToString();
    }

    public List<CZones> GetNeighborhood() {
        return NeighborsList;
    }

    public void SetNeighborhood(CZones zones) {
        NeighborsList.Clear();
        NeighborsList.Add(zones);
    }

}
