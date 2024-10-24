using System.Diagnostics;
using System.Text;
using System.Net;
using Simulator.MPS;
using Simulator.RobotEssentials;
using Simulator.Utility;
using System.Text.Json;
using LlsfMsgs;

namespace Simulator.WebGui {
    class WebGui {
        public HttpListener listener;

        public string Url;
        public int pageViews = 0;
        public int requestCount = 0;
        private MyLogger MyLogger;
        private RobotManager _robotManager;
        private MpsManager _mpsManager;
        private Configurations Config;
        Stopwatch timer = new Stopwatch();

        public WebGui(Configurations config, MpsManager mpsManager, RobotManager robotManager) {
            listener = new HttpListener();
            Config = config;
            Url = Config.WebguiPrefix + "://*:" + Config.WebguiPort + "/";
            listener.Prefixes.Add(Url);

            listener.Start();
            MyLogger = new MyLogger("Web", true);
            MyLogger.Log($"Listening for the WebGUI to connect to {Url}");
            Console.WriteLine($"Listening for the WebGUI to connect to {Url}");
            _robotManager = robotManager;
            _mpsManager = mpsManager;

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }

        public async Task HandleIncomingConnections() {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer) {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = listener.GetContext();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                //Console.WriteLine("Request #: {0}", ++requestCount);
                if (req.Url != null) {
                    MyLogger.Log(req.Url.ToString());
                }
                //Console.WriteLine(req.Url.ToString());
                MyLogger.Log(req.HttpMethod);
                //Console.WriteLine(req.HttpMethod);
                //MyLogger.Log(req.UserHostName);
                //Console.WriteLine(req.UserHostName);
                //MyLogger.Log(req.UserAgent);
                //Console.WriteLine(req.UserAgent);

                MyLogger.Log(" ");

                switch (req.HttpMethod) {
                    // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                    case "POST" when (req.Url?.AbsolutePath == "/shutdown"):
                        MyLogger.Log("Shutdown requested");
                        runServer = false;
                        break;
                    case "OPTIONS": {
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
                    case "GET": {
                            //timer.Start();
                            if (req.Url == null) {
                                return;
                            }
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

                            switch (segment) {
                                case "zones": {
                                        resp.ContentType = "JSON";
                                        var jsonString = JsonSerializer.Serialize(ZonesManager.GetInstance().ZoneList);
                                        MyLogger.Log(jsonString);
                                        data = Encoding.UTF8.GetBytes(jsonString);
                                        break;
                                    }
                                case "robots": {
                                        resp.ContentType = "JSON";
                                        var jsonString = JsonSerializer.Serialize(_robotManager?.Robots);
                                        MyLogger.Log(jsonString);
                                        data = Encoding.UTF8.GetBytes(jsonString);
                                        break;
                                    }
                                case "machines": {
                                        resp.ContentType = "JSON";
                                        var jsonString = JsonSerializer.Serialize(_mpsManager?.Machines);
                                        MyLogger.Log(jsonString);
                                        data = Encoding.UTF8.GetBytes(jsonString);
                                        //Console.WriteLine("Creating the Json took : {0}", timer.ElapsedMilliseconds.ToString());

                                        break;
                                    }
                                case "products": {
                                        resp.ContentType = "JSON";
                                        var product = new Products(BaseColor.BaseBlack);
                                        product.AddPart(new RingElement(RingColor.RingBlue));
                                        product.AddPart(new CapElement(CapColor.CapBlack));
                                        var jsonString = JsonSerializer.Serialize(product);
                                        data = Encoding.UTF8.GetBytes(jsonString);
                                        break;
                                    }
                                default: {
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

    internal class JsonTask {
        public string? Task { get; set; }
        public string? Target { get; set; }
        public string? MachinePoint { get; set; }
    }
}
