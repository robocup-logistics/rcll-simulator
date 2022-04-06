using LlsfMsgs;
using Terminal.Gui;
using Robot = Simulator.RobotEssentials.Robot;

namespace Simulator.TerminalGui
{
    class RobotGuiView
    {
        private readonly string TeamcolorString = "TeamColor    [{0}]";
        private readonly string JerseyString = "Jersey       [{0}]";
        private readonly string TaskString = "Task         [{0}]";
        private readonly string ConnectString = "Connected    [{0}]";
        private readonly string PositionString = "Position     [{0}]";
        private readonly string ProductString = "Product\n[{0}]";
        private readonly string ProgressString = "Progress:";
        public Window RobotWindow { get; set; }

        public Robot Robot { get; set; }
        private readonly Label TeamColor;
        private readonly Label JerseyLabel;
        private readonly Label TaskLabel;
        private readonly Label ConnectLabel;
        private readonly Label PositionLabel;
        private readonly Label ProductLabel;
        private readonly Label Progress;
        private ProgressBar ActionBar;
        private Window TaskSubWindow;
        private ColorScheme RobotTeamColorScheme;
        private Label DebugLog;
        public RobotGuiView(Robot rob,int x_offset, int width_, Team team)
        {
            Robot = rob;
            var width = width_;
            RobotTeamColorScheme = team == Team.Cyan
                ? TerminalConfig.GetInstance().Team1ColorScheme
                : TerminalConfig.GetInstance().Team2ColorScheme;
            var window = new Window(Robot.RobotName)
            {
                X = x_offset,
                Y = 0,
                Width = width,
                Height = Dim.Percent(100),
                ColorScheme = RobotTeamColorScheme
            };

            int pos = 0;
            TeamColor = new Label(0, pos++, string.Format(TeamcolorString, Robot.TeamColor))
            {
                AutoSize = true
            };
            JerseyLabel = new Label(0, pos++, string.Format(JerseyString, Robot.JerseyNumber))
            {
                AutoSize = true

            };

            ConnectLabel = new Label(0, pos++, string.Format(ConnectString, Robot.GetConnectionState()))
            {
                AutoSize = true
            };
            PositionLabel = new Label(0, pos++, string.Format(PositionString,Robot.GetZone().ToString()))
            {
                AutoSize = true
            };
            TaskLabel = new Label(0, pos++, string.Format(TaskString, Robot.GetTaskDescription()))
            {
                AutoSize = true
            };
            ProductLabel = new Label(0, pos++, string.Format(ProductString, Robot.GetHeldProductString()))
            {
                AutoSize = true
            };
            pos += 5;
            TaskSubWindow = new Window("Debug")
            {
                X = 0,
                Y = pos++,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = TerminalConfig.GetInstance().DefaultColorScheme
            };
            pos = 0;

            /*Progress = new Label(0, pos++, ProgressString)
            {
                AutoSize = true
            };
            ActionBar = new ProgressBar()
            {
                X = 0,
                Y = pos++,
                Width = Dim.Fill(),
                Height = 1,
                ColorScheme = Colors.Error,
                Fraction = 0.3f
            };*/
            DebugLog = new Label(0, pos++, "Debuglog")
            {
                AutoSize = true
            };
            window.Add(TeamColor, JerseyLabel, ConnectLabel, PositionLabel, TaskLabel, ProductLabel, TaskSubWindow);
            TaskSubWindow.Add( /*Progress, ActionBar,*/ DebugLog);
            RobotWindow = window;
        }
        private View TabWindow;
        public View GetView()
        {
            return TabWindow;
        }
        public void Update()
        {
            TeamColor.Text = string.Format(TeamcolorString, Robot.TeamColor);
            JerseyLabel.Text = string.Format(JerseyString, Robot.JerseyNumber);
            TaskLabel.Text = string.Format(TaskString, Robot.GetTaskDescription());
            ConnectLabel.Text = string.Format(ConnectString, Robot.GetConnectionState());
            PositionLabel.Text = string.Format(PositionString, Robot.GetZone().ZoneId.ToString());
            //ProductLabel.Text = string.Format(ProductString, Robot.GetHeldProductString());
            //ActionBar.Fraction = 0.5f;
            DebugLog.Text = Robot.GetDebugLog(50);
        }
    }
}
