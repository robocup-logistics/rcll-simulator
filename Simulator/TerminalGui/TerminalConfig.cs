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
        private Color Ring1;
        private Color Ring2;
        private Color Ring3;
        private Color Ring4;
        private Color Base1;
        private Color Base2;
        private Color Base3;
        private Color Base0;
        private Color Cap1;
        private Color Cap2;

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
        public ColorScheme HighlightColorScheme { get; set; }


        public ColorScheme ProductColorSchemeRing1 { get; set; }
        public ColorScheme ProductColorSchemeRing2 { get; set; }
        public ColorScheme ProductColorSchemeRing3 { get; set; }
        public ColorScheme ProductColorSchemeRing4 { get; set; }
        public ColorScheme ProductColorSchemeBase1 { get; set; }
        public ColorScheme ProductColorSchemeBase2 { get; set; }
        public ColorScheme ProductColorSchemeBase3 { get; set; }
        public ColorScheme ProductColorSchemeBase0 { get; set; }
        public ColorScheme ProductColorSchemeCap1 { get; set; }
        public ColorScheme ProductColorSchemeCap2 { get; set; }

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

        public Attribute AttributeRing1 { get; set; }
        public Attribute AttributeRing2 { get; set; }
        public Attribute AttributeRing3 { get; set; }
        public Attribute AttributeRing4 { get; set; }
        public Attribute AttributeBase1 { get; set; }
        public Attribute AttributeBase2 { get; set; }
        public Attribute AttributeBase3 { get; set; }
        public Attribute AttributeBase0 { get; set; }
        public Attribute AttributeCap1 { get; set; }
        public Attribute AttributeCap2 { get; set; }

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

            Ring1 = Color.Blue;
            Ring2 = Color.Green;
            Ring3 = Color.BrightRed;
            Ring4 = Color.BrightYellow;
            Base0 = Color.Gray;
            Base1 = Color.Red;
            Base2 = Color.Black;
            Base3 = Color.DarkGray;

            Cap1 = Color.Black;
            Cap2 = Color.Gray;


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
            AttributeRing1 = new Attribute(Color.Black, Ring1);
            AttributeRing2 = new Attribute(Color.Black, Ring2);
            AttributeRing3 = new Attribute(Color.Black, Ring3);
            AttributeRing4 = new Attribute(Color.Black, Ring4);
            AttributeBase1 = new Attribute(Color.White, Base1);
            AttributeBase2 = new Attribute(Color.White, Base2);
            AttributeBase3 = new Attribute(Color.White, Base3);
            AttributeBase3 = new Attribute(Color.White, Base0);
            AttributeCap1 = new Attribute(Color.White, Cap1);
            AttributeCap2 = new Attribute(Color.White, Cap2);

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
            //------------------------------------------------
            // PRODUCT COLOR SCHEMES
            ProductColorSchemeRing1 = new ColorScheme()
            {
                Focus = AttributeRing1,
                HotFocus = AttributeRing1,
                Disabled = AttributeRing1,
                Normal = AttributeRing1
            };
            ProductColorSchemeRing2 = new ColorScheme()
            {
                Focus = AttributeRing2,
                HotFocus = AttributeRing2,
                Disabled = AttributeRing2,
                Normal = AttributeRing2
            };
            ProductColorSchemeRing3 = new ColorScheme()
            {
                Focus = AttributeRing3,
                HotFocus = AttributeRing3,
                Disabled = AttributeRing3,
                Normal = AttributeRing3
            };
            ProductColorSchemeRing4 = new ColorScheme()
            {
                Focus = AttributeRing4,
                HotFocus = AttributeRing4,
                Disabled = AttributeRing4,
                Normal = AttributeRing4
            };
            ProductColorSchemeBase1 = new ColorScheme()
            {
                Focus = AttributeBase1,
                HotFocus = AttributeBase1,
                Disabled = AttributeBase1,
                Normal = AttributeBase1
            };
            ProductColorSchemeBase2 = new ColorScheme()
            {
                Focus = AttributeBase2,
                HotFocus = AttributeBase2,
                Disabled = AttributeBase2,
                Normal = AttributeBase2
            };
            ProductColorSchemeBase3 = new ColorScheme()
            {
                Focus = AttributeBase3,
                HotFocus = AttributeBase3,
                Disabled = AttributeBase3,
                Normal = AttributeBase3
            };
            ProductColorSchemeBase0 = new ColorScheme()
            {
                Focus = AttributeBase0,
                HotFocus = AttributeBase0,
                Disabled = AttributeBase0,
                Normal = AttributeBase0
            };
            ProductColorSchemeCap1 = new ColorScheme()
            {
                Focus = AttributeCap1,
                HotFocus = AttributeCap1,
                Disabled = AttributeCap1,
                Normal = AttributeCap1
            };
            ProductColorSchemeCap2 = new ColorScheme()
            {
                Focus = AttributeCap2,
                HotFocus = AttributeCap2,
                Disabled = AttributeCap2,
                Normal = AttributeCap2
            };

            Instance = this;
        }


    }
}
