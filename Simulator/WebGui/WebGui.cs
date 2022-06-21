using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Simulator.MPS;
using Simulator.RobotEssentials;
using Simulator.Utility;

namespace Simulator.WebGui
{
    class WebGui
    {
        public HttpListener listener;
        public string url = "http://localhost:8000/";
        public int pageViews = 0;
        public int requestCount = 0;
        private MyLogger MyLogger;
        private RobotManager _robotManager;
        private MpsManager _mpsManager;
        public WebGui(RobotManager robotManager = null, MpsManager machine = null)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            MyLogger = new MyLogger("Web", true);
            MyLogger.Log($"Listening for connections on {url}");
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
                MyLogger.Log(req.HttpMethod);
                MyLogger.Log(req.UserHostName);
                MyLogger.Log(req.UserAgent);
                MyLogger.Log(" ");

                switch (req.HttpMethod)
                {
                    // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                    case "POST" when (req.Url.AbsolutePath == "/shutdown"):
                        MyLogger.Log("Shutdown requested");
                        runServer = false;
                        break;
                    case "GET":
                        {
                            var pageData = "";
                            var segment = req.Url.Segments[req.Url.Segments.Length - 1];
                            MyLogger.Log("The query = " + req.Url.Query.ToString());

                            var disableSubmit = !runServer ? "disabled" : "";
                            resp.ContentType = "text/html";
                            resp.ContentEncoding = Encoding.UTF8;
                            byte[] data;
                            MyLogger.Log("Switching with the segment: " + segment);

                            switch (segment)
                            {
                                case "styles.css":
                                    MyLogger.Log("Requested the CSS File!");
                                    resp.ContentType = "text/css";
                                    pageData = File.ReadAllText("../../webgui/page/" + req.Url.Segments[1]);
                                    data = Encoding.UTF8.GetBytes(pageData);
                                    MyLogger.Log("The css = " + data);
                                    break;
                                case "map.html":
                                    pageData = File.ReadAllText("../../webgui/page/" + req.Url.Segments[1]);
                                    data = Encoding.UTF8.GetBytes(String.Format(pageData));
                                    break;
                                case "robot.html":
                                    {
                                        int id = Int32.Parse(req.Url.ToString().Split('=')[1]);
                                        pageData = File.ReadAllText("../../webgui/page/" + req.Url.Segments[1]);
                                        if (_robotManager != null)
                                        {
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
                                        pageData = File.ReadAllText("../../webgui/page/" + req.Url.Segments[1]);
                                        if (_mpsManager != null)
                                        {
                                            MPS.Mps mp = _mpsManager.Machines[id];
                                            MyLogger.Log("Mps Nr" + mp.InternalId + " is of type " + mp.Type+ " with a machine state of : " + mp.MachineState);
                                            var text = String.Format(pageData, mp.Name, mp.Type.ToString(), mp.MachineState.ToString(),"4","5","6","7","8");
                                            data = Encoding.UTF8.GetBytes(text);
                                        }
                                        else
                                        {
                                            data = Encoding.UTF8.GetBytes(String.Format(pageData));
                                        }
                                    }
                                    break;
                                case "debug.html":
                                    pageData = File.ReadAllText("../../webgui/page/debug.html");
                                    data = Encoding.UTF8.GetBytes(String.Format(pageData));
                                    break;
                                case "favicon.ico":
                                    return;
                                default:
                                    pageData = File.ReadAllText("../../webgui/page/home.html");
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
}
