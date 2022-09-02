using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Simulator.MPS;
using Simulator.RobotEssentials;
using Simulator.Utility;
using System.Text.Json;

namespace Simulator.WebGui
{
    class WebGui
    {
        public HttpListener listener;
        //public string url = "http://localhost:8000/";
        //public string Url = "https://localhost:8000/";
        public string Url;
        public int pageViews = 0;
        public int requestCount = 0;
        private MyLogger MyLogger;
        private RobotManager _robotManager;
        private MpsManager _mpsManager;
        private string Path;
        private Configurations Config;
        public WebGui(RobotManager robotManager = null, MpsManager machine = null)
        {
            var path = Directory.GetCurrentDirectory();
            Console.WriteLine(path);
            listener = new HttpListener();
            Config = Configurations.GetInstance();
            Url = Config.WebguiPrefix + "://localhost:" + Config.WebguiPort + "/";
            listener.Prefixes.Add(Url);
            //listener.Prefixes.Add(url2);

            listener.Start();
            MyLogger = new MyLogger("Web", true);

            Path = "C:\\Users\\lumpi\\source\\repos\\rcll-simulation\\Simulator\\WebGui\\page\\";
            MyLogger.Log($"Listening for connections on {Url}");
            Console.WriteLine($"Listening for connections on {Url}");
            _robotManager = robotManager;
            _mpsManager = machine;
            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();
            // Close the listener
            listener.Close();

        }

        public async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = listener.GetContext();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                MyLogger.Log(req.Url.ToString());
                Console.WriteLine(req.Url.ToString());
                MyLogger.Log(req.HttpMethod);
                Console.WriteLine(req.HttpMethod);
                MyLogger.Log(req.UserHostName);
                Console.WriteLine(req.UserHostName);
                MyLogger.Log(req.UserAgent);
                Console.WriteLine(req.UserAgent);

                MyLogger.Log(" ");

                switch (req.HttpMethod)
                {
                    // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                    case "POST" when (req.Url.AbsolutePath == "/shutdown"):
                        MyLogger.Log("Shutdown requested");
                        runServer = false;
                        break;
                    case "OPTIONS":
                        {
                            Console.WriteLine("Got options!");
                            resp.StatusCode = (byte)HttpStatusCode.OK;
                            resp.ContentType = "application/json";
                            resp.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With, Authorization");
                            //resp.AddHeader("Access-Control-Request-Method", "GET");
                            //resp.AddHeader("Access-Control-Request-Headers", "authorization");
                            resp.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                            resp.AddHeader("Access-Control-Max-Age", "1728000");
                            resp.AppendHeader("Access-Control-Allow-Origin", "*");
                            /*
                                response.setHeader("Access-Control-Allow-Origin", "*");
                                response.setHeader("Access-Control-Allow-Credentials", "true");
                                response.setHeader("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
                                response.setHeader("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");
                             */
                            resp.ContentLength64 = 0;
                            resp.Close();
                            break;
                        }
                    case "GET":
                        {
                            var pageData = "";
                            var segment = req.Url.Segments[req.Url.Segments.Length - 1];
                            MyLogger.Log("The query = " + req.Url.Query.ToString());

                            var disableSubmit = !runServer ? "disabled" : "";
                            resp.ContentType = "text/html";
                            resp.ContentEncoding = Encoding.UTF8;
                            var cookie = new Cookie("connection", "1");
                            resp.AppendCookie(cookie);
                            resp.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With, Authorization");
                            resp.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                            resp.AddHeader("Access-Control-Max-Age", "1728000");

                            resp.AppendHeader("Access-Control-Allow-Origin", "*");
                            byte[] data;
                            MyLogger.Log("Switching with the segment: " + segment);

                            switch (segment)
                            {
                                case "styles.css":
                                    MyLogger.Log("Requested the CSS File!");
                                    resp.ContentType = "text/css";
                                    pageData = File.ReadAllText(Path + req.Url.Segments[1]);
                                    data = Encoding.UTF8.GetBytes(pageData);
                                    MyLogger.Log("The css = " + data);
                                    break;
                                case "map.html":
                                    pageData = File.ReadAllText(Path + req.Url.Segments[1]);
                                    data = Encoding.UTF8.GetBytes(String.Format(pageData));
                                    break;
                                case "zones":
                                    {
                                        resp.ContentType = "JSON";
                                        var jsonString = JsonSerializer.Serialize(ZonesManager.GetInstance().ZoneList);
                                        //Console.WriteLine(jsonString);
                                        data = Encoding.UTF8.GetBytes(jsonString);
                                        break;
                                    }
                                case "robots":
                                {
                                    resp.ContentType = "JSON";
                                    Console.WriteLine("Creating the json of robots!");
                                    //var text = _robotManager.Robots[0].SerializeRobotToJson();
                                    var jsonString = JsonSerializer.Serialize(_robotManager?.Robots);
                                    Console.WriteLine(jsonString);
                                    data = Encoding.UTF8.GetBytes(jsonString);
                                    break;
                                }
                                case "machines":
                                {
                                    resp.ContentType = "JSON";
                                    Console.WriteLine("Creating the json of machines!");

                                        var jsonString = JsonSerializer.Serialize(_mpsManager?.Machines);
                                    Console.WriteLine(jsonString);
                                    data = Encoding.UTF8.GetBytes(jsonString);
                                    break;

                                }
                                case "robot.html":
                                    {
                                        int id = Int32.Parse(req.Url.ToString().Split('=')[1]);
                                        pageData = File.ReadAllText("/simulator/WebGui/page/" + req.Url.Segments[1]);
                                        if (_robotManager != null)
                                        {
                                            id -= 1;
                                            MyLogger.Log("The Robot we get is : " + id);
                                            Robot rob = _robotManager.Robots[id];
                                            var text = String.Format(pageData, rob.RobotName, rob.JerseyNumber, rob.TeamColor.ToString());
                                            data = Encoding.UTF8.GetBytes(text);
                                        }
                                        else
                                        {
                                            data = Encoding.UTF8.GetBytes(String.Format(pageData));
                                        }
                                    }

                                    break;
                                case "mps.html":
                                    {
                                        int id = Int32.Parse(req.Url.ToString().Split('=')[1]);

                                        MyLogger.Log("Loading HMTL file : " + req.Url.Segments[1]);
                                        pageData = File.ReadAllText(Path + req.Url.Segments[1]);
                                        if (_mpsManager != null)
                                        {
                                            MPS.Mps mp = _mpsManager.Machines[id];
                                            MyLogger.Log("Mps Nr" + mp.InternalId + " is of type " + mp.Type + " with a machine state of : " + mp.MachineState);
                                            var text = String.Format(pageData, mp.Name, mp.Type.ToString(), mp.MachineState.ToString(), "4", "5", "6", "7", "8");
                                            data = Encoding.UTF8.GetBytes(text);
                                        }
                                        else
                                        {
                                            data = Encoding.UTF8.GetBytes(String.Format(pageData));
                                        }
                                    }
                                    break;
                                case "debug.html":
                                    pageData = File.ReadAllText(Path + "debug.html");
                                    data = Encoding.UTF8.GetBytes(String.Format(pageData));
                                    break;
                                case "favicon.ico":
                                    continue;
                                case "json.html":
                                    {
                                        resp.ContentType = "JSON";
                                        var test = new TestJson("Test1", "Val2");
                                        string jsonString = JsonSerializer.Serialize(test);
                                        Console.WriteLine(jsonString);
                                        data = Encoding.UTF8.GetBytes(jsonString);
                                        Console.WriteLine(JsonSerializer.Serialize(MpsManager.GetInstance().Machines));
                                        break;
                                    }
                                default:
                                    pageData = File.ReadAllText(Path + "home.html");
                                    var MapFields = CreateMapFields();
                                    var RobotFields = CreateRobotFields();
                                    var MpsField = CreateMpsFields();
                                    MyLogger.Log("In front of the string.format");
                                    data = Encoding.UTF8.GetBytes(String.Format(pageData, MapFields, RobotFields, MpsField));
                                    MyLogger.Log("after string.format");
                                    break;
                            }

                            resp.ContentLength64 = data.LongLength;

                            await resp.OutputStream.WriteAsync(data, 0, data.Length);
                            resp.Close();
                            break;
                        }
                }
            }

        }

        private string CreateRobotFields()
        {
            var RobotFields = "";
            MyLogger.Log("CreateRobotFields got called!");
            foreach (var rob in _robotManager.Robots)
            {
                RobotFields +=
                    " <iframe src=\"/robot.html?id=" + rob.JerseyNumber + "\" title=\"Robot test\"></iframe>\n";
                //" <iframe src=\"/robot.html?id=" + rob.JerseyNumber + "\" title=\"Robot test\" width = \"300\" height=\"300\" style=\"border: 1px solid black;\" ></iframe>\n";
            }
            MyLogger.Log(RobotFields);
            return RobotFields;
        }
        private string CreateMpsFields()
        {
            var MpsFields = "";
            MyLogger.Log("CreateMpsFields got called!");

            foreach (var mps in _mpsManager.Machines)
            {
                MpsFields +=
                    " <iframe src=\"/mps.html?id=" + mps.InternalId + "\" title=\"Mps test\" ></iframe>\n";
            }
            MyLogger.Log(MpsFields);
            return MpsFields;
        }

        private string CreateMapFields()
        {
            MyLogger.Log("CreateMapFields got called!");
            return " <iframe src=\"/map.html\" title=\"Mps test\" ></iframe>\n";
        }
    }

    internal class TestJson
    {
        public string Test { get; set; }
        public string Value1 { get; set; }

        public TestJson(string val1, string val2)
        {
            Test = val1;
            Value1 = val2;
        }
    }
}
