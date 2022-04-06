using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LlsfMsgs;
using YamlDotNet.RepresentationModel;
using MpsType = Simulator.MPS.Mps.MpsType;

namespace Simulator
{
    /// <summary>
    /// The configurations class is a singleton which stores all the different configuration values for the Refbox, Teamserver and machines
    /// It also stores the important values from the simulation.
    /// Further more it is intended to load the config from a .yaml file which is not currently support but should be usable in the near future
    /// </summary>
    public class Configurations
    {
        // general configurations are in this place
        public List<MpsConfig> MpsConfigs { get; set; }
        public List<RobotConfig> RobotConfigs { get; set; }
        public List<TeamConfig> Teams { get; set; }
        public RefboxConfig? Refbox { get; set; }
        public float TimeFactor { get; private set; } = 1f;

        // definitions for the web gui

        // all member variables concerning the simulation are here
        public bool MockUp { get; set; }
        public int RobotMoveTime = 5;

        public int FieldWidth = 14;
        public int FieldHeight = 8;

        public bool IgnoreTeamColor { get; private set; } = true;

        public int RobotActionDuration { get; private set; }
        public int RobotMoveZoneDuration { get; private set; }
        public int BeltActionDuration { get; private set; }

        public int CSTaskDuration { get; private set; }
        public int BSTaskDuration { get; private set; }
        public int DSTaskDuration { get; private set; }
        public int RSTaskDuration { get; private set; }

        //Constructor of my Singleton variable
        private Configurations()
        {
            MpsConfigs = new List<MpsConfig>();
            RobotConfigs = new List<RobotConfig>();
            Teams = new List<TeamConfig>();
            MockUp = true;
            RobotActionDuration = 10 * 1000;
            BeltActionDuration = 10 * 1000;
            CSTaskDuration = 10 * 1000;
            BSTaskDuration = 1100;
            DSTaskDuration = 1000;
            RSTaskDuration = 1000;
            RobotMoveZoneDuration = 5 * 1000;
        }

        //private member and getter for my singleton configurations class
        private static Configurations? Instance;
        /// <returns>
        /// Returns the instance of the Configurations Singleton
        /// </returns>
        /// 
        public static Configurations GetInstance()
        {
            return Instance ??= new Configurations();
        }

        public void LoadConfig(string path)
        {
            using var reader = new StreamReader(path);
            // Load the stream
            var yaml = new YamlStream();
            yaml.Load(reader);
            // the rest
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
            var stations = (YamlMappingNode)mapping.Children[new YamlScalarNode("stations")];
            var robots = (YamlMappingNode)mapping.Children[new YamlScalarNode("robots")];
            var teams = (YamlMappingNode)mapping.Children[new YamlScalarNode("teams")];
            var refbox = (YamlMappingNode)mapping.Children[new YamlScalarNode("refbox")];
            var general = (YamlMappingNode)mapping.Children[new YamlScalarNode("general")];
            foreach (var keyPair in stations.Children)
            {
                var config = CreateMachineConfig(keyPair);
                if (config != null)
                {
                    MpsConfigs.Add(config);
                }
            }
            foreach (var keyPair in robots.Children)
            {
                var config = CreateRobotConfig(keyPair);
                if (config != null)
                {
                    RobotConfigs.Add(config);
                }
            }
            foreach (var keyPair in teams.Children)
            {
                var config = CreateTeamConfig(keyPair);
                if (config != null)
                {
                    Teams.Add(config);
                }
            }
            Refbox = CreateRefboxConfig(refbox);
            foreach (var (key, value) in general.Children)
            {

                /*
                 *    belt-action-duration: 2.5 # time the belt takes to move a product from one place to another - taken from gazebo
                      bs-dispense-duration: 2.5 # time the base station takes to dispense a base - taken from gazebo
                      cs-buffer-duration: 3.5  # buffer cap - taken from gazebo
                      rs-mount-duration: 3.5  # mounting a ring 3.5 seconds - taken from gazebo
                      ds-deliver-duration: 3.5  # time it takes to deliver - taken from gazebo
                 */
                switch (key.ToString())
                {
                    case "timefactor":
                        TimeFactor = float.Parse(value.ToString(), CultureInfo.InvariantCulture);
                        break;
                    case "ignore-teamcolor":
                        IgnoreTeamColor = bool.Parse(value.ToString());
                        break;
                    case "mockup-connections":
                        MockUp = bool.Parse(value.ToString());
                        break;
                    case "robot-move-zone-duration":
                        RobotMoveZoneDuration = (int)(float.Parse(value.ToString(), CultureInfo.InvariantCulture) * 1000); // convert from seconds to milliseconds
                        break;
                    case "belt-action-duration":
                        BeltActionDuration = (int)(float.Parse(value.ToString(), CultureInfo.InvariantCulture) * 1000);
                        break;
                    case "cs-buffer-duration":
                        CSTaskDuration = (int)(float.Parse(value.ToString(), CultureInfo.InvariantCulture) * 1000);
                        break;
                    case "bs-dispense-duration":
                        BSTaskDuration = (int)(float.Parse(value.ToString(), CultureInfo.InvariantCulture) * 1000);
                        break;
                    case "rs-mount-duration":
                        RSTaskDuration = (int)(float.Parse(value.ToString(), CultureInfo.InvariantCulture) * 1000);
                        break;
                    case "ds-deliver-duration":
                        DSTaskDuration = (int)(float.Parse(value.ToString(), CultureInfo.InvariantCulture) * 1000);
                        break;

                }
            }
        }


        private static MpsConfig? CreateMachineConfig(KeyValuePair<YamlNode, YamlNode> child)
        {

            var port = 0;
            var debug = false;
            var type = MpsType.BaseStation;
            var (yamlNode, yamlNode1) = child;
            var allNodes = ((YamlMappingNode)yamlNode1).Children;
            foreach (var (key, value) in allNodes)
            {
                switch (key.ToString())
                {
                    //Console.WriteLine(entry);
                    case "active" when value.ToString().Equals("false"):
                        //Console.WriteLine("This has to be skipped!");
                        return null;
                    case "debug":
                        switch (value.ToString())
                        {
                            case "true":
                                debug = true;
                                break;
                            case "false":
                                debug = false;
                                break;
                            default:
                                type = MpsType.CapStation;
                                break;
                        }

                        break;
                    case "type":
                        type = value.ToString() switch
                        {
                            "BS" => MpsType.BaseStation,
                            "CS" => MpsType.CapStation,
                            "DS" => MpsType.DeliveryStation,
                            "SS" => MpsType.StorageStation,
                            "RS" => MpsType.RingStation,
                            _ => MpsType.CapStation
                        };
                        break;
                    case "port":
                        port = int.Parse(value.ToString());
                        break;
                }
            }

            var color = yamlNode.ToString().Contains("M-") ? Team.Magenta : Team.Cyan;
            var config = new MpsConfig(yamlNode.ToString(), type, port, color, debug);
            return config;
        }

        private static RobotConfig? CreateRobotConfig(KeyValuePair<YamlNode, YamlNode> child)
        {
            var jersey = 0;
            var color = Team.Cyan;
            var (yamlNode, yamlNode1) = child;
            var allNodes = ((YamlMappingNode)yamlNode1).Children;
            foreach (var (key, value) in allNodes)
            {
                switch (key.ToString())
                {
                    //Console.WriteLine(entry);
                    case "active" when value.ToString().Equals("false"):
                        //Console.WriteLine("THis has to be skipped!");
                        return null;
                    case "jersey":
                        jersey = int.Parse(value.ToString());
                        break;
                    case "team" when yamlNode1.ToString().Contains("magenta"):
                        color = Team.Magenta;
                        break;
                    case "team":
                        color = Team.Cyan;
                        break;
                }
            }
            var config = new RobotConfig(yamlNode.ToString(), jersey, color);
            return config;
        }

        private static TeamConfig? CreateTeamConfig(KeyValuePair<YamlNode, YamlNode> child)
        {
            string? name = null;
            var port = 0;
            var ip = "";
            var color = Team.Cyan;

            var (yamlNode, yamlNode1) = child;
            if (!yamlNode.ToString().ToLower().Contains("magenta"))
            {
                if (yamlNode.ToString().ToLower().Contains("cyan"))
                {
                    color = Team.Cyan;
                }
                else
                {
                    Console.WriteLine("Not a known team color!");
                }
            }
            else
            {
                color = Team.Magenta;
            }

            var allNodes = ((YamlMappingNode)yamlNode1).Children;
            foreach (var (key, value) in allNodes)
            {
                switch (key.ToString())
                {
                    //Console.WriteLine(entry);
                    case "active" when value.ToString().Equals("false"):
                        //Console.WriteLine("THis has to be skipped!");
                        return null;
                    case "name":
                        name = value.ToString();
                        break;
                    case "host":
                        ip = value.ToString();
                        break;
                    case "port":
                        port = int.Parse(value.ToString());
                        break;
                }
            }
            if (name == null)
            {
                return null;
            }
            var config = new TeamConfig(name, color, ip, port);
            return config;
        }

        private static RefboxConfig? CreateRefboxConfig(YamlMappingNode refbox)
        {
            string? ip = null;
            int publicSendPort = 0, publicRecvPort = 0, cyanSendPort = 0, cyanRecvPort = 0, magentaSendPort = 0, magentaRecvPort = 0;
            var children = refbox.Children;
            // Step into the public information
            //Console.WriteLine(children[0].Key.ToString());
            var map = (YamlMappingNode)children[0].Value;
            foreach (var (key, value) in children)
            {
                switch (key.ToString())
                {
                    case "public":
                        var publicChild = ((YamlMappingNode)value).Children;
                        foreach (var (yamlNode, yamlNode1) in publicChild)
                        {
                            switch (yamlNode.ToString())
                            {
                                case "ip":
                                    ip = yamlNode1.ToString();
                                    break;
                                case "send":
                                    publicSendPort = int.Parse(yamlNode1.ToString());
                                    break;
                                case "recv":
                                    publicRecvPort = int.Parse(yamlNode1.ToString());
                                    break;
                                default:
                                    Console.WriteLine("Unknown key " + yamlNode.ToString());
                                    return null;
                            }
                        }

                        break;
                    case "cyan":
                        var cyanChild = ((YamlMappingNode)value).Children;
                        foreach (var (yamlNode, yamlNode1) in cyanChild)
                        {
                            switch (yamlNode.ToString())
                            {
                                case "send":
                                    cyanSendPort = int.Parse(yamlNode1.ToString());
                                    break;
                                case "recv":
                                    cyanRecvPort = int.Parse(yamlNode1.ToString());
                                    break;
                                default:
                                    Console.WriteLine("Unknown key " + yamlNode.ToString());
                                    return null;
                            }
                        }
                        break;
                    case "magenta":
                        var magentaChild = ((YamlMappingNode)value).Children;
                        foreach (var (yamlNode, yamlNode1) in magentaChild)
                        {
                            switch (yamlNode.ToString())
                            {
                                case "send":
                                    magentaSendPort = int.Parse(yamlNode1.ToString());
                                    break;
                                case "recv":
                                    magentaRecvPort = int.Parse(yamlNode1.ToString());
                                    break;
                                default:
                                    Console.WriteLine("Unknown key " + yamlNode.ToString());
                                    return null;
                            }
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown key " + key.ToString());
                        return null;
                }

                //Console.WriteLine(val);
            }
            if (ip == null)
            {
                return null;
            }
            var config = new RefboxConfig(ip, publicSendPort, publicRecvPort, cyanSendPort, cyanRecvPort,
                magentaSendPort, magentaRecvPort);
            return config;
        }
    }

    public class MpsConfig
    {
        public string Name { get; }
        public MPS.Mps.MpsType Type { get; }
        public int Port { get; }
        public Team Team { get; }
        public bool Debug { get; }
        public MpsConfig(string name, MPS.Mps.MpsType type, int port, Team team, bool debug)
        {
            Name = name;
            Type = type;
            Port = port;
            Team = team;
            Debug = debug;
        }

        public void PrintConfig()
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine("Name = [" + Name + "]");
            Console.WriteLine("Type = [" + Type + "]");
            Console.WriteLine("Port = [" + Port + "]");
            Console.WriteLine("Team = [" + Team + "]");
            Console.WriteLine("Debug = [" + Debug + "]");
        }
    }

    public class TeamConfig
    {
        public string Name { get; }
        public Team Color { get; }
        public string Ip { get; }
        public int Port { get; }
        public uint Points { get; set; }
        public TeamConfig(string name, Team color, string ip, int port)
        {
            Name = name;
            Color = color;
            Ip = ip;
            Port = port;
            Points = 0;
        }
    }

    public class RefboxConfig
    {
        public string IP { get; }
        public int PublicSendPort { get; }
        public int PublicRecvPort { get; }
        public int CyanSendPort { get; }
        public int CyanRecvPort { get; }
        public int MagentaSendPort { get; }
        public int MagentaRecvPort { get; }

        public RefboxConfig(string ip, int publicSend, int publicRecv, int cyanSend, int cyanRecv, int magentaSend, int magentaRecv)
        {
            IP = ip;
            PublicRecvPort = publicRecv;
            PublicSendPort = publicSend;
            CyanRecvPort = cyanRecv;
            CyanSendPort = cyanSend;
            MagentaRecvPort = magentaRecv;
            MagentaSendPort = magentaSend;
        }
        public void PrintConfig()
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine("Ip = [" + IP + "]");
            Console.WriteLine("PublicRecvPort = [" + PublicRecvPort + "]");
            Console.WriteLine("PublicSendPort = [" + PublicSendPort + "]");
            Console.WriteLine("CyanRecvPort = [" + CyanRecvPort + "]");
            Console.WriteLine("CyanSendPort = [" + CyanSendPort + "]");
            Console.WriteLine("MagentaRecvPort = [" + MagentaRecvPort + "]");
            Console.WriteLine("MagentaSendPort = [" + MagentaSendPort + "]");
        }
    }

    public class RobotConfig
    {
        public string Name;
        public int Jersey;
        public Team TeamColor;
        public RobotConfig(string name, int jersey, Team color)
        {
            Name = name;
            Jersey = jersey;
            TeamColor = color;
        }
        public void PrintConfig()
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine("Name = [" + Name + "]");
            Console.WriteLine("Jersey = [" + Jersey + "]");
            Console.WriteLine("Team = [" + TeamColor + "]");
        }
    }
}
