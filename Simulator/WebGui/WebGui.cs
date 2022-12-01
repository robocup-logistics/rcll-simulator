using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Simulator.MPS;
using Simulator.RobotEssentials;
using Simulator.Utility;
using System.Text.Json;
using LlsfMsgs;
using Robot = Simulator.RobotEssentials.Robot;

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
            //Console.WriteLine(path);
            listener = new HttpListener();
            Config = Configurations.GetInstance();
            Url = Config.WebguiPrefix + "://*:" + Config.WebguiPort + "/";
            listener.Prefixes.Add(Url);
            //listener.Prefixes.Add(url2);

            listener.Start();
            MyLogger = new MyLogger("Web", true);
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
                //Console.WriteLine("Request #: {0}", ++requestCount);
                MyLogger.Log(req.Url.ToString());
                //Console.WriteLine(req.Url.ToString());
                MyLogger.Log(req.HttpMethod);
                //Console.WriteLine(req.HttpMethod);
                //MyLogger.Log(req.UserHostName);
                //Console.WriteLine(req.UserHostName);
                //MyLogger.Log(req.UserAgent);
                //Console.WriteLine(req.UserAgent);

                MyLogger.Log(" ");

                switch (req.HttpMethod)
                {
                    // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                    case "POST" when (req.Url.AbsolutePath == "/shutdown"):
                        MyLogger.Log("Shutdown requested");
                        runServer = false;
                        break;
                    case "PUT":
                    {
                        Stream body = req.InputStream;
                        Encoding encoding = req.ContentEncoding;
                        StreamReader reader = new StreamReader(body, encoding);

                        string s = reader.ReadToEnd();
                        Console.WriteLine(s);

                        JsonTask? taskJson = JsonSerializer.Deserialize<JsonTask>(s);

                        var task = new AgentTask();
                        switch (taskJson.Task)
                        {
                            case "move":
                                var Move = new Move
                                {
                                    Waypoint = taskJson.Target,
                                    MachinePoint = taskJson.MachinePoint
                                };
                                task.Move = Move;
                                break;
                            case "buffer":
                                var buffer = new BufferStation
                                {
                                    MachineId = taskJson.Target,
                                    ShelfNumber = UInt32.Parse(taskJson.MachinePoint)
                                };
                                task.Buffer = buffer;
                                break;

                        }
                       

                        _robotManager.Robots[0].SetAgentTasks(task);
                        resp.StatusCode = (byte)HttpStatusCode.OK;
                        resp.ContentLength64 = 0;
                        resp.Close();
                        break;


                    }
                    case "OPTIONS":
                    {
                        //Console.WriteLine("Got options!");
                        resp.StatusCode = (byte)HttpStatusCode.OK;
                        resp.ContentType = "application/json";
                        resp.AddHeader("Access-Control-Allow-Headers",
                            "Content-Type, Accept, X-Requested-With, Authorization");
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
                        resp.AddHeader("Access-Control-Allow-Headers",
                            "Content-Type, Accept, X-Requested-With, Authorization");
                        resp.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                        resp.AddHeader("Access-Control-Max-Age", "1728000");

                        resp.AppendHeader("Access-Control-Allow-Origin", "*");
                        byte[] data;
                        MyLogger.Log("Switching with the segment: " + segment);

                        switch (segment)
                        {
                            case "zones":
                            {
                                resp.ContentType = "JSON";
                                var jsonString = JsonSerializer.Serialize(ZonesManager.GetInstance().ZoneList);
                                MyLogger.Log(jsonString);
                                data = Encoding.UTF8.GetBytes(jsonString);
                                break;
                            }
                            case "robots":
                            {
                                resp.ContentType = "JSON";
                                var jsonString = JsonSerializer.Serialize(_robotManager?.Robots);
                                MyLogger.Log(jsonString);
                                data = Encoding.UTF8.GetBytes(jsonString);
                                break;
                            }
                            case "machines":
                            {
                                resp.ContentType = "JSON";
                                var jsonString = JsonSerializer.Serialize(_mpsManager?.Machines);
                                MyLogger.Log(jsonString);
                                data = Encoding.UTF8.GetBytes(jsonString);
                                break;
                            }
                            case "products":
                            {
                                resp.ContentType = "JSON";
                                var product = new Products(BaseColor.BaseBlack);
                                product.AddPart(new RingElement(RingColor.RingBlue));
                                product.AddPart(new CapElement(CapColor.CapBlack));
                                var jsonString = JsonSerializer.Serialize(product);
                                data = Encoding.UTF8.GetBytes(jsonString);
                                break;
                            }
                            default:
                            {
                                resp.ContentType = "JSON";
                                data = Encoding.UTF8.GetBytes("{Connection:working}");
                                break;
                            }

                        }

                        resp.ContentLength64 = data.LongLength;
                        await resp.OutputStream.WriteAsync(data, 0, data.Length);
                        resp.Close();
                        break;
                    }
                }
            }

        }
        
    }

    internal class JsonTask
    {
        public string Task { get; set; }
        public string Target { get; set; }
        public string MachinePoint { get; set; }
    }
}
