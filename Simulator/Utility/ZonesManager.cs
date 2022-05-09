using LlsfMsgs;
using Simulator.MPS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Opc.Ua;
using Robot = Simulator.RobotEssentials.Robot;

namespace Simulator.Utility
{
    public class ZonesManager
    {
        private List<Zones> ZoneList;
        private readonly Dictionary<Zone, Zones> Dictionary;
        private static ZonesManager? Instance;
        public Mutex ZoneManagerMutex;
        private MyLogger MyLogger;
        /// <returns>
        /// Returns the instance of the Configurations Singleton
        /// </returns>
        public static ZonesManager GetInstance()
        {
            return Instance ??= new ZonesManager();
        }

        private ZonesManager()
        {
            ZoneList = new List<Zones>();
            Dictionary = new Dictionary<Zone, Zones>();
            MyLogger = new MyLogger("Zones", true);
            ZoneManagerMutex = new Mutex();
            MyLogger.Log("Creating General Zones");
            foreach (Zone z in Enum.GetValues(typeof(Zone)))
            {
                int val = (int)z;
                var y = val % 10;
                val /= 10;
                var x = val % 10;
                val /= 10;
                Team color = Team.Cyan;
                if (val > 0)
                {
                    color = Team.Magenta;
                }

                Zones zone;
                zone = color == Team.Cyan ? new Zones(y, x, 0, color, z) : new Zones(-y+1, -x+1, 0, color, z);
                Dictionary.Add(z, zone);
                ZoneList.Add(zone);
            }

            //Todo add deployment zones for robots
            MyLogger.Log("Creating Deployment Zones");
            for (int i = 0; i < 3; i++)
            {
                var DeployZoneCyan = (Zone)((5 + i) * 10 + 1);
                var deploymentZoneCyan = new Zones(1, 5 + i, 0, Team.Cyan, DeployZoneCyan);
                var DeployZoneMagenta = (Zone)(1000 + (5 + i) * 10 + 1);
                var deploymentZoneMagenta = new Zones(1, 5 + i, 0, Team.Magenta, DeployZoneMagenta);
                ZoneList.Add(deploymentZoneMagenta);
                ZoneList.Add(deploymentZoneCyan);
                Dictionary.Add(DeployZoneCyan, deploymentZoneCyan);
                Dictionary.Add(DeployZoneMagenta, deploymentZoneMagenta);
            }
            MyLogger.Log("Starting to add Neighborhood");
            AddNeighborhood();

        }
        public Zones? GetZone(Zone zone)
        {
            return Dictionary.ContainsKey(zone) ? Dictionary[zone] : null;
        }

        public void ShowNeighbourhood(Zone z)
        {
            var zones = GetZone(z);
            foreach (var n in zones.GetNeighborhood())
            {
                n.GetsMovedTo = true;
            }
        }
        public Zone GetWaypoint(string target, string machinepoint = "")
        {
            MyLogger.Log("GetWayPoint with target ["+target+"]!");
            Zone result;
            try
            {
                //TODO update to a fancier handling of strings
                /*if(target.Contains("CS1") || target.Contains("CS2")||target.Contains("RS1") || target.Contains("RS2"))
                    result = (Zone)Enum.Parse(typeof(Zone), target.Replace("_", "").Substring(0, 5));
                else
                    result = (Zone)Enum.Parse(typeof(Zone), target.Replace("_", "").Substring(0, 4));*/
                result = (Zone)Enum.Parse(typeof(Zone), target[..5].Replace("_",""));
                MyLogger.Log("Is a Zone Waypoint!");
            }
            catch (Exception )
            {
                MyLogger.Log("Is not a Zone Waypoint!");
                return GetZoneNextToMachine(target,machinepoint); ;
            }
            return result;
        }
        public static Zone GetZoneFromString(string? zoneString)
        {
            if (zoneString == null)
            {
                return 0;
            }
            if (zoneString.Contains("_"))
            {
                zoneString = zoneString.Replace("_", "");
            }
            var result = (Zone)Enum.Parse(typeof(Zone), zoneString);
            return result;
        }

        public void PlaceMachine(Zone zone, uint orientation, Mps machine)
        {
            if (!Dictionary.ContainsKey(zone)) return;
            MyLogger.Log("Placed " + machine.Name + " at zone " + zone + " with the orientation " + orientation);
            Dictionary[zone].PlaceMachine(machine, orientation);
            machine.Zone = zone;
        }

        public bool PlaceRobot(Zone zone, uint orientation, Robot robot)
        {
            if (!Dictionary.ContainsKey(zone) || Dictionary[zone].Robot != null) return false;
            Dictionary[zone].PlaceRobot(robot, orientation);
            return true;

        }
        public bool RemoveRobot(Zone zone, uint orientation, Robot robot)
        {
            if (!Dictionary.ContainsKey(zone) || Dictionary[zone].Robot == null) return false;
            Dictionary[zone].RemoveRobot();
            return true;

        }

        public Zone GetZoneNextToMachine(string MachineName, string machinepoint = "")
        {
            MyLogger.Log("Getting Zone next to machine!" + MachineName);
            foreach (var (key, value) in Dictionary)
            {
                if (value.Machine != null && MachineName.Contains(value.Machine.Name))
                {
                    int offset = 0;
                    var orientation = value.Orientation;
                    if(MachineName.ElementAt(0).Equals("M"))
                    {
                        orientation += 180;
                        orientation %= 360;
                    }
                    if (MachineName.Contains("output") || machinepoint.Equals("output"))
                    {
                        orientation += 180;
                        orientation %= 360;
                    }
                    switch (orientation)
                    {
                        case 0:
                            offset += 10;
                            break;
                        case 45:
                            offset += 10;
                            offset += 1;
                            break;
                        case 90:
                            offset += 1;
                            break;
                        case 135:
                            offset -= 10;
                            offset += 1;
                            break;
                        case 180:
                            offset -= 10;
                            break;
                        case 225:
                            offset -= 10;
                            offset -= 1;
                            break;
                        case 270:
                            offset -= 1;
                            break;
                        case 315:
                            offset += 10;
                            offset -= 1;
                            break;
                        default:
                            offset = 0;
                            break;
                    }
                    if((((int)key+offset) % 100) < (int)Zone.CZ11)
                    {
                        MyLogger.Log("The key = " + key + " and the offset = " + offset);
                        MyLogger.Log("Below 10, need to switch sides!");
                        if((int)key > 500)
                        {
                            offset = -1000; 
                        }
                        else
                        {
                            offset = 1000;
                        }
                    }
                    MyLogger.Log("We got for " + MachineName + " the adjacent zone " + ((Zone) key + offset).ToString());
                    return key + offset;
                }
            }
            MyLogger.Log("Couldn't find the machine " + MachineName);
            return 0;
        }

        public List<Zones> GetPathToZone(Zone Start, Zone Target)
        {
            List<Zones> Path = new List<Zones>();
            var currentZone = Dictionary[Start];
            var targetZone = Dictionary[Target];
            MyLogger.Log("----------------------------------------");
            while (currentZone.ZoneId != Target)
            {
                var list = currentZone.GetNeighborhood();
                double shortest = 100;
                Zones shortestZone = null;
                MyLogger.Log("Start search for " + targetZone.ZoneId.ToString() + " from zone " + currentZone.ZoneId.ToString());
                foreach (var e in list)
                {
                    if (e.Machine != null)
                    {
                        continue;
                    }
                    MyLogger.Log("Checking " + e.ZoneId.ToString() + " with coordinates " + e.X + "/" + e.Y);
                    var dist = CalcDistance(e, targetZone);
                    MyLogger.Log("Distance = " + dist.ToString());
                    if (dist < shortest)
                    {
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
        
        public List<Zones> Astar(Zones start, Zones end)
        {
            var comparer = Comparer<Zones>.Create(
                (k1, k2) => k1.ZoneId.CompareTo(k2.ZoneId));
            SortedDictionary<Zones, double> openList = new SortedDictionary<Zones, double>(comparer);
            Dictionary<Zone, int> closedList = new Dictionary<Zone, int>();
            Dictionary<Zones, Zones> cameFrom = new Dictionary<Zones, Zones>();
            openList.Add(start,0);
            SortedDictionary<Zone, double> gScore = new SortedDictionary<Zone, double>();
            SortedDictionary<Zone, double> fScore = new SortedDictionary<Zone, double>();
            gScore.Add(start.ZoneId, 0);
            fScore.Add(start.ZoneId, CalcDistance(start,end));
            List<Zones> path = new List<Zones>();
            while (openList.Count != 0)
            {
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
                if (current.ZoneId == end.ZoneId)
                {
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
                foreach (var neighbor in neighborhood)
                {
                    if (neighbor.Machine != null)
                    {
                        MyLogger.Log("Skipping field as there is a machine!");
                        continue;
                    }
                    var tentative_gScore = gScore[current.ZoneId] + CalcDistance(neighbor, current);
                    if (!gScore.ContainsKey(neighbor.ZoneId))
                    {
                        gScore.Add(neighbor.ZoneId,1000);
                    }
                    if (tentative_gScore < gScore[neighbor.ZoneId])
                    {
                        // This path to neighbor is better than any previous one. Record it!
                        if (cameFrom.ContainsKey(neighbor))
                        {
                            cameFrom[neighbor] = current;
                        }
                        else
                        {
                            cameFrom.Add(neighbor, current);
                        }
                        gScore[neighbor.ZoneId] = tentative_gScore;
                        var dist = CalcDistance(neighbor, end);
                        var comp = tentative_gScore + dist;
                        //MyLogger.Log("Zone " + neighbor.GetZoneString() + " is " + dist + " from end");
                        fScore[neighbor.ZoneId] = comp;

                        if (!openList.ContainsKey(neighbor))
                        {
                            openList.Add(neighbor, comp);
                        }
                        else
                        {
                            openList[neighbor] = comp;
                        }
                            
                    }

                }

            }
            MyLogger.Log("No Valid Path found for target [" + end.GetZoneString() +"]!");
            return new List<Zones>();
        }

        private static List<Zones> ReconstructPath(Dictionary<Zones, Zones> cameFrom, Zones current)
        {
            var path = new List<Zones>
            {
                current
            };
            bool finished = false;
            while (!finished)
            {
                try
                {
                    var element = cameFrom[path[^1]];
                    path.Add(element);
                }
                catch (Exception)
                {
                    path.Reverse();
                    finished = true;
                }
            }

            path.RemoveAt(0); // remove the starting position from the path!
            return path;
        }

        public static double CalcDistance(Zones Start, Zones End)
        {
            return Math.Sqrt(Math.Pow((Start.X - End.X), 2) + Math.Pow((Start.Y - End.Y), 2));
        }
        private void AddNeighborhood()
        {
            foreach (var z in ZoneList)
            {
                // add all adjacent fields
                if (Dictionary.ContainsKey(z.ZoneId + 1))
                {
                    z.AddNeighbor(Dictionary[z.ZoneId + 1]);
                }
                if (Dictionary.ContainsKey(z.ZoneId - 1))
                {
                    z.AddNeighbor(Dictionary[z.ZoneId - 1]);
                }
                if (Dictionary.ContainsKey(z.ZoneId + 10))
                {
                    z.AddNeighbor(Dictionary[z.ZoneId + 10]);
                }
                if (Dictionary.ContainsKey(z.ZoneId - 10))
                {
                    z.AddNeighbor(Dictionary[z.ZoneId - 10]);
                }
                // Add diagonal fields
                if (Dictionary.ContainsKey(z.ZoneId - 9))
                {
                    z.AddNeighbor(Dictionary[z.ZoneId - 9]);
                }
                if (Dictionary.ContainsKey(z.ZoneId - 11))
                {
                    z.AddNeighbor(Dictionary[z.ZoneId - 11]);
                }
                if (Dictionary.ContainsKey(z.ZoneId + 9))
                {
                    z.AddNeighbor(Dictionary[z.ZoneId + 9]);
                }
                if (Dictionary.ContainsKey(z.ZoneId + 11))
                {
                    z.AddNeighbor(Dictionary[z.ZoneId + 11]);
                }

                //Added to connect the two halves of the mapping
                if (((int)z.ZoneId) % 100 - 10 < 10)
                {
                    if ((int)z.ZoneId > 100)
                    {
                        if (Dictionary.ContainsKey(z.ZoneId - 1000))
                        {
                            z.AddNeighbor(Dictionary[z.ZoneId - 1000]);
                        }
                        if (Dictionary.ContainsKey(z.ZoneId - 1001))
                        {
                            z.AddNeighbor(Dictionary[z.ZoneId - 1001]);
                        }
                        if (Dictionary.ContainsKey(z.ZoneId - 999))
                        {
                            z.AddNeighbor(Dictionary[z.ZoneId - 999]);
                        }
                    }
                    else
                    {
                        if (Dictionary.ContainsKey(z.ZoneId + 1000))
                        {
                            z.AddNeighbor(Dictionary[z.ZoneId + 1000]);
                        }
                        if (Dictionary.ContainsKey(z.ZoneId + 1001))
                        {
                            z.AddNeighbor(Dictionary[z.ZoneId + 1001]);
                        }
                        if (Dictionary.ContainsKey(z.ZoneId + 999))
                        {
                            z.AddNeighbor(Dictionary[z.ZoneId + 999]);
                        }
                    }
                }
                MyLogger.Log("Added " + z.GetNeighborhood().Count + " to the zone " + z.ZoneId);
            }

        }
    }

    public class Zones
    {
        public readonly int X;
        public readonly int Y;
        private readonly Team ZoneColor;

        private readonly List<Zones> NeighborsList;
        public Mps? Machine { get; private set; }
        public Robot? Robot { get; private set; }
        public uint Orientation { get; private set; }
        public Zone ZoneId { get; private set; }
        public bool GetsMovedTo { get; set; }
        public Mutex ZoneMutex {get; private set;}
        public Zones(int x, int y, uint orientation, Team color, Zone zoneId)
        {
            X = x;
            Y = y;
            Orientation = orientation;
            ZoneColor = color;
            GetsMovedTo = false;
            NeighborsList = new List<Zones>();
            ZoneId = zoneId;
            Machine = null;
            Robot = null;
            ZoneMutex = new Mutex();
        }

        public void AddNeighbor(Zones newNeighbor)
        {
            NeighborsList.Add(newNeighbor);
        }
        public bool Free()
        {
            if (Machine == null && Robot == null)
            {
                return true;
            }
            return false;
        }

        public void PlaceMachine(MPS.Mps machine, uint orientation)
        {
            Machine = machine;
            Orientation = orientation;
        }

        public void PlaceRobot(Robot robot, uint orientation)
        {
            Robot = robot;
            Orientation = orientation;
        }
        public void RemoveRobot()
        {
            Robot = null;
            Orientation = 0;
        }
        public string GetZoneString()
        {
            if (Machine != null)
            {
                return Machine.Name;
            }

            if (Robot != null)
            {
                return Robot.JerseyNumber + " " +Robot.RobotName;
            }

            return ZoneId.ToString();
        }

        public List<Zones> GetNeighborhood()
        {
            return NeighborsList;
        }

    }




}
