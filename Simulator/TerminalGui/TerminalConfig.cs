using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace Simulator.TerminalGui
{
    class TerminalConfig
    {
        //private member and getter for my singleton configurations class
        private static TerminalConfig Instance;
        /// <returns>
        /// Returns the instance of the Configurations Singleton
        /// </returns>
        public static TerminalConfig GetInstance()
        {
            return Instance ??= new TerminalConfig();
        }

        private Color Background;
        private Color Background1;
        private Color Background2;
        private Color DefaultColor;
        private Color Team1;
        private Color Team2;
        private Color Team1Robot;
        private Color Team2Robot;
        private Color RedLight;
        private Color YellowLight;
        private Color GreenLight;
        private Color LightOff;
        private Color HighlightColor;
        public ColorScheme DefaultColorScheme { get; set; }
        public ColorScheme Team1ColorScheme { get; set; }
        public ColorScheme Team2ColorScheme { get; set; }
        public ColorScheme Team1RobotColorScheme { get; set; }
        public ColorScheme Team2RobotColorScheme { get; set; }
        public ColorScheme MapColor1 { get; set; }
        public ColorScheme MapColor2 { get; set; }
        public ColorScheme RedLightColorScheme { get; set; }
        public ColorScheme YellowLightColorScheme { get; set; }
        public ColorScheme GreenLightColorScheme { get; set; }
        public ColorScheme LightOffColorScheme { get; set; }
        public ColorScheme HighlightColorScheme {get; set;}
        private Attribute DefaultAttribute;
        private Attribute Team1Attribute;
        private Attribute Team2Attribute;
        private Attribute Team1RobotAttribute;
        private Attribute Team2RobotAttribute;
        public Attribute MapColor1Attribute;
        public Attribute MapColor2Attribute;
        private Attribute RedLightAttribute;
        private Attribute YellowLightAttribute;
        private Attribute GreenLightAttribute;
        private Attribute LightOffAttribute;
        private Attribute HighlightAttribute;
        public int ColumnWidthMPS;
        public int ColumnWidthRobot;
        public int ColumnWidthGeneralInfo;
        private TerminalConfig()
        {
            ColumnWidthMPS = 25;
            ColumnWidthRobot = 60;
            ColumnWidthGeneralInfo = 25;

            Background = Color.Black;
            Background1 = Color.DarkGray;
            Background2 = Color.Gray;

            DefaultColor = Color.Gray;
            Team1 = Color.BrightCyan;
            Team2 = Color.BrightMagenta;
            Team1Robot = Color.BrightBlue;
            Team2Robot = Color.BrightRed;


            RedLight = Color.BrightRed;
            YellowLight = Color.BrightYellow;
            GreenLight = Color.BrightGreen;
            LightOff = Color.DarkGray;

            HighlightColor = Color.BrightYellow;

            DefaultAttribute = new Attribute(DefaultColor, Background);
            Team1Attribute = new Attribute(Team1, Background);
            Team2Attribute = new Attribute(Team2, Background);
            Team1RobotAttribute = new Attribute(Team1Robot, Background);
            Team2RobotAttribute = new Attribute(Team2Robot, Background);
            MapColor1Attribute = new Attribute(DefaultColor, Background);
            MapColor2Attribute = new Attribute(DefaultColor, Background2);
            RedLightAttribute = new Attribute(RedLight, RedLight);
            YellowLightAttribute = new Attribute(YellowLight, YellowLight);
            GreenLightAttribute = new Attribute(GreenLight, GreenLight);
            LightOffAttribute = new Attribute(LightOff, LightOff);
            HighlightAttribute = new Attribute(HighlightColor, Background);

            DefaultColorScheme = new ColorScheme()
            {
                Focus = DefaultAttribute,
                HotFocus = DefaultAttribute,
                Disabled = DefaultAttribute,
                Normal = DefaultAttribute
            };

            Team1ColorScheme = new ColorScheme()
            {
                Focus = Team1Attribute,
                HotFocus = Team1Attribute,
                Disabled = Team1Attribute,
                Normal = Team1Attribute
            };

            Team2ColorScheme = new ColorScheme()
            {
                Focus = Team2Attribute,
                HotFocus = Team2Attribute,
                Disabled = Team2Attribute,
                Normal = Team2Attribute
            };
            Team1RobotColorScheme = new ColorScheme()
            {
                Focus = Team1RobotAttribute,
                HotFocus = Team1RobotAttribute,
                Disabled = Team1RobotAttribute,
                Normal = Team1RobotAttribute
            };

            Team2RobotColorScheme = new ColorScheme()
            {
                Focus = Team2RobotAttribute,
                HotFocus = Team2RobotAttribute,
                Disabled = Team2RobotAttribute,
                Normal = Team2RobotAttribute
            };
            MapColor1 = new ColorScheme()
            {
                Focus = MapColor1Attribute,
                HotFocus = MapColor1Attribute,
                Disabled = MapColor1Attribute,
                Normal = MapColor1Attribute
            };
            MapColor2 = new ColorScheme()
            {
                Focus = MapColor2Attribute,
                HotFocus = MapColor2Attribute,
                Disabled = MapColor2Attribute,
                Normal = MapColor2Attribute
            };

            RedLightColorScheme = new ColorScheme()
            {
                Focus = RedLightAttribute,
                HotFocus = RedLightAttribute,
                Disabled = RedLightAttribute,
                Normal = RedLightAttribute
            };

            YellowLightColorScheme = new ColorScheme()
            {
                Focus = YellowLightAttribute,
                HotFocus = YellowLightAttribute,
                Disabled = YellowLightAttribute,
                Normal = YellowLightAttribute
            };

            GreenLightColorScheme = new ColorScheme()
            {
                Focus = GreenLightAttribute,
                HotFocus = GreenLightAttribute,
                Disabled = GreenLightAttribute,
                Normal = GreenLightAttribute
            };

            LightOffColorScheme = new ColorScheme()
            {
                Focus = LightOffAttribute,
                HotFocus = LightOffAttribute,
                Disabled = LightOffAttribute,
                Normal = LightOffAttribute
            };
            HighlightColorScheme = new ColorScheme()
            {
                Focus = HighlightAttribute,
                HotFocus = HighlightAttribute,
                Disabled = HighlightAttribute,
                Normal = HighlightAttribute
            };
            







            Instance = this;
        }


    }
}
