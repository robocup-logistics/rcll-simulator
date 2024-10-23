using LlsfMsgs;
using Simulator.MPS;
using System.Text.Json.Serialization;

namespace Simulator.Utility {
    public class ZonesManager {
        public List<Zones> ZoneList { get; private set; }
        private readonly Dictionary<Zone, Zones> Dictionary;
        private static ZonesManager? Instance;
        public Mutex ZoneManagerMutex;
        private MyLogger MyLogger;
        /// <returns>
        /// Returns the instance of the Configurations Singleton
        /// </returns>
        public static ZonesManager GetInstance() {
            return Instance ??= new ZonesManager();
        }

        private ZonesManager() {
            ZoneList = new List<Zones>();
            Dictionary = new Dictionary<Zone, Zones>();
            MyLogger = new MyLogger("Zones", true);
            ZoneManagerMutex = new Mutex();
            MyLogger.Log("Creating General Zones");
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

                Zones zone;
                zone = color == Team.Cyan ? new Zones(x + 6, y, 0, color, z) : new Zones(7 - x, y, 0, color, z);
                Dictionary.Add(z, zone);
                ZoneList.Add(zone);
            }

            MyLogger.Log("Starting to add Neighborhood");
            AddNeighborhood();
        }

        public Zones? GetZone(Zone zone) {
            return Dictionary.ContainsKey(zone) ? Dictionary[zone] : null;
        }

        public Zone GetWaypoint(string target, string machinepoint = "") {
            MyLogger.Log("GetWayPoint with target [" + target + " and machinepoint = " + machinepoint + "]!");
            Zone result;
            try {
                if (target.Contains("C_Z") || target.Contains("M_Z")) {
                    target = target.Substring(0, 5);
                }
                //TODO ARE YOUR SURE WITH THE _ removal because the enum name has it
                result = (Zone)Enum.Parse(typeof(Zone), target.Replace("_", ""));
                MyLogger.Log("Is a Zone Waypoint!");
                return result;
            }
            catch (Exception) {
                MyLogger.Log("Is not a Zone Waypoint!");
                return GetZoneNextToMachine(target, machinepoint); ;
            }
        }

        public void PlaceMachine(Zone zone, uint orientation, Mps machine) {
            if (!Dictionary.ContainsKey(zone)) return;
            MyLogger.Log("Placed " + machine.Name + " at zone " + zone + " with the orientation " + orientation);
            Dictionary[zone].PlaceMachine(machine, orientation);
            machine.Zone = zone;
        }

        public Zones? GetMachineZone(string MachineName) {
            foreach (var (key, value) in Dictionary) {
                if (value.Machine != null && MachineName.Contains(value.Machine.Name)) {
                    return value;
                }
            }
            return null;
        }

        public Zone GetZoneNextToMachine(string MachineName, string machinepoint = "") {
            MyLogger.Log("Getting Zone next to machine!" + MachineName);
            foreach (var (key, value) in Dictionary) {
                if (value.Machine != null && MachineName.Contains(value.Machine.Name)) {
                    var orientation = value.Orientation;
                    var neighborhood = value.GetNeighborhood();
                    if (MachineName.Contains("output") || machinepoint.Equals("output")) {
                        orientation += 180;
                        orientation %= 360;
                    }

                    var waypoint = Zone.CZ11;

                    var radians = (Math.PI / 180) * orientation;
                    MyLogger.Log("Orientation = " + orientation + " and in radians " + radians);
                    var y = Convert.ToInt32(Math.Sin(radians));
                    var x = Convert.ToInt32(Math.Cos(radians));
                    MyLogger.Log("X offset = " + x + " and offset y = " + y);
                    waypoint = CheckNeighbours(neighborhood, value, x, y);
                    return waypoint;
                }
            }
            MyLogger.Log("Couldn't find the machine " + MachineName);
            return 0;
        }

        public Zone CheckNeighbours(List<Zones> Neighbours, Zones compareable, int x, int y) {
            MyLogger.Log("Checking " + compareable.X + "/" + compareable.Y);
            foreach (var n in Neighbours) {
                if (n.X == compareable.X + x && n.Y == compareable.Y + y) {
                    MyLogger.Log("The searched neighbour is " + n.ZoneId + " with " + n.X + "/" + n.Y);
                    return n.ZoneId;
                }
            }
            MyLogger.Log("No neighbour found!");
            return 0;
        }
        public List<Zones> GetPathToZone(Zone Start, Zone Target) {
            List<Zones> Path = new List<Zones>();
            var currentZone = Dictionary[Start];
            var targetZone = Dictionary[Target];
            MyLogger.Log("----------------------------------------");
            while (currentZone.ZoneId != Target) {
                var list = currentZone.GetNeighborhood();
                double shortest = 100;
                Zones? shortestZone = null;
                MyLogger.Log("Start search for " + targetZone.ZoneId.ToString() + " from zone " + currentZone.ZoneId.ToString());
                foreach (var e in list) {
                    if (e.Machine != null) {
                        continue;
                    }
                    MyLogger.Log("Checking " + e.ZoneId.ToString() + " with coordinates " + e.X + "/" + e.Y);
                    var dist = CalcDistance(e, targetZone);
                    MyLogger.Log("Distance = " + dist.ToString());
                    if (dist < shortest) {
                        shortest = dist;
                        shortestZone = e;
                    }
                }

                if (shortestZone == null) continue;
                Path.Add(shortestZone);
                currentZone = shortestZone;
            }

            return Path;
        }

        public List<Zones> Astar(Zones start, Zones end) {
            var comparer = Comparer<Zones>.Create(
                (k1, k2) => k1.ZoneId.CompareTo(k2.ZoneId));
            SortedDictionary<Zones, double> openList = new SortedDictionary<Zones, double>(comparer);
            Dictionary<Zone, int> closedList = new Dictionary<Zone, int>();
            Dictionary<Zones, Zones> cameFrom = new Dictionary<Zones, Zones>();
            openList.Add(start, 0);
            SortedDictionary<Zone, double> gScore = new SortedDictionary<Zone, double>();
            SortedDictionary<Zone, double> fScore = new SortedDictionary<Zone, double>();
            gScore.Add(start.ZoneId, 0);
            fScore.Add(start.ZoneId, CalcDistance(start, end));
            List<Zones> path = new List<Zones>();
            while (openList.Count != 0) {
                /*MyLogger.Log("###########################");
                MyLogger.Log("OpenList");
                foreach (var z in openList)
                {
                    MyLogger.Log("Zone: " + z.Key.GetZoneString() + "\tDist = " +z.Value);
                    
                }
                MyLogger.Log("###########################");*/

                var values = openList.Values.ToList();
                var index = values.IndexOf(values.Min());
                var current = openList.Keys.ElementAt(index);
                //MyLogger.Log("Current expanded zone = " + current.GetZoneString());
                //current.GetsMovedTo = true;
                if (current.ZoneId == end.ZoneId) {
                    MyLogger.Log("A Valid path has been found!");
                    return ReconstructPath(cameFrom, current);
                }

                openList.Remove(current);
                var neighborhood = current.GetNeighborhood();
                /*MyLogger.Log("----------------");
                MyLogger.Log("Neighborhood = " + neighborhood.Count);
                
                foreach(var n in neighborhood)
                    MyLogger.Log(n.GetZoneString());
                MyLogger.Log("----------------");*/
                foreach (var neighbor in neighborhood) {
                    if (neighbor.Machine != null) {
                        MyLogger.Log("Skipping field as there is a machine!");
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
                        //MyLogger.Log("Zone " + neighbor.GetZoneString() + " is " + dist + " from end");
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
            MyLogger.Log("No Valid Path found for target [" + end.GetZoneString() + "]!");
            return new List<Zones>();
        }

        private static List<Zones> ReconstructPath(Dictionary<Zones, Zones> cameFrom, Zones current) {
            var path = new List<Zones>
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

        public static double CalcDistance(Zones Start, Zones End) {
            if (Start == null || End == null) {
                ZonesManager.GetInstance().MyLogger.Log("CalcDistance has a null?");
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
                MyLogger.Log("Added " + z.GetNeighborhood().Count + " to the zone " + z.ZoneId);
            }

        }
    }

    public class Zones {

        private readonly Team ZoneColor;

        private readonly List<Zones> NeighborsList;

        [JsonIgnore]
        public Mps? Machine { get; private set; }

        [JsonIgnore]
        public uint Orientation { get; private set; }
        public Zone ZoneId { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public Zones(int x, int y, uint orientation, Team color, Zone zoneId) {
            X = x;
            Y = y;
            Orientation = orientation;
            ZoneColor = color;
            NeighborsList = new List<Zones>();
            ZoneId = zoneId;
            Machine = null;
        }

        public void AddNeighbor(Zones newNeighbor) {
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

        public List<Zones> GetNeighborhood() {
            return NeighborsList;
        }

    }




}
