using System;
using System.Collections.Generic;
using Simulator.Utility;
using Terminal.Gui;
using Label = Terminal.Gui.Label;
using View = Terminal.Gui.View;
using Timer = Simulator.Utility.Timer;
using Robot = Simulator.RobotEssentials.Robot;
using LlsfMsgs;
using Simulator.MPS;

namespace Simulator.TerminalGui
{
    class MapGuiView
    {
        public View Instance;

        public Window Map;
        public Window GeneralWindow;
        public Window GeneralInformation;
        private Label? TimeLabel;
        private Label? CyanTeamLabel;
        private Label? CyanTeamPointsLabel;
        private Label? MagentaTeamLabel;
        private Label? MagentaTeamPointsLabel;
        private const string TeamString = "{0}";
        private const string PointString = "{0,3}";
        private const string TimeString = "{0:D4}:{1:D2}:{2:D3}";
        private Timer time;
        private List<MapField> MapFieldList;
        private List<MpsInfoField> MpsInfoFieldList;
        private TerminalConfig Config;
        

        public MapGuiView(int height, int width)
        {
            var height_without_border = height - 2;
            var width_without_border = width - 2;
            var field_height = 8; //8;
            var field_width = 14;
            var pixel_height = height_without_border / field_height;
            var pixel_width = width_without_border / field_width;
            var height_percent = 100.0f / field_height;
            var widht_percent = 100.0f / field_width;
            Config = TerminalConfig.GetInstance();

            MapFieldList = new List<MapField>();
            MpsInfoFieldList = new List<MpsInfoField>();
            time = Timer.GetInstance();
            Instance = new View();
            GeneralWindow = new Window("General Information")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(100),
                Height = Dim.Percent(100),
                ColorScheme = Config.DefaultColorScheme
            };
            GeneralInformation = new Window("Information")
            {
                X = 0,
                Y = 0,
                Width = Config.ColumnWidthGeneralInfo,
                Height = Dim.Percent(100),
                ColorScheme = Config.DefaultColorScheme
            };
            Map = new Window("Map")
            {
                X = Pos.Right(GeneralInformation),
                Y = 0,//Pos.Left(GeneralInformation),
                Width = Dim.Fill(), // .Right(GeneralInformation), // Dim.Percent(70),
                Height = Dim.Percent(100),
                ColorScheme = Config.DefaultColorScheme
            };
            TimeLabel = null;
            AddGeneralInfo();
            View last_label = null;
            View last_label_row = null;
            Label newLabel = null;
            int[] Id = { 7, 6, 5, 4, 3, 2, 1, 1, 2, 3, 4, 5, 6, 7 };
            var widthDim = Dim.Percent(widht_percent);
            var heightDim = Dim.Percent(height_percent);
            for (var y = field_height; y > 0; y--)
            {
                for (var x = 0; x < field_width; x++)
                {
                    var PosX = x == 0 ? 0 : Pos.Right(last_label);
                    var PosY = last_label_row != null ? Pos.Bottom(last_label_row) : 0;
                    Zones? zones = null;
                    if (x < 7)
                    {
                        zones = ZonesManager.GetInstance().GetZone((Zone)(1000 + Id[x] * 10 + y));
                    }
                    else
                    {
                        zones = ZonesManager.GetInstance().GetZone((Zone)(Id[x] * 10 + y));
                    }
                    if (zones == null)
                    {
                        // TODO clean up the null zones
                        Console.WriteLine("A zone is NULL!");
                    }
                    var field = new MapField(PosX, PosY, zones, widthDim, heightDim, Config.MapColor1);

                    Console.WriteLine("Field Created with X=" + x + " Y" + y + " == Zone " + (zones != null ? zones.GetZoneString() : "no zone"));
                    newLabel = field.GetLabel();
                    Map.Add(newLabel);
                    MapFieldList.Add(field);
                    last_label = newLabel;
                }
                last_label_row = newLabel;
            }

            GeneralWindow.Add(GeneralInformation, Map);
            /*Map.Add(new Label("CS")
            {
                X = 0,
                Y = 2,
                Width = 2,
                Height = 1,
                ColorScheme = TerminalConfig.GetInstance().Team1ColorScheme
            });
            Map.Add(new Label("RS")
            {
                X = 4,
                Y = 3,
                Width = 2,
                Height = 1,
                ColorScheme = TerminalConfig.GetInstance().Team2ColorScheme
            });*/
        }
        public View GetWindow()
        {
            return GeneralWindow;
        }
        public void Update()
        {
            if(TimeLabel==null)
            {
                return;
            }
            TimeLabel.Text = string.Format(TimeString,time.Sec/60, time.Sec % 60, time.Nsec);
            CyanTeamPointsLabel.Text = string.Format(PointString,Configurations.GetInstance().Teams[0].Points);
            MagentaTeamPointsLabel.Text = string.Format(PointString,Configurations.GetInstance().Teams[1].Points);
            foreach (var field in MapFieldList)
            {
                field.UpdateLabel();
            }
            foreach (var field in MpsInfoFieldList)
            {
                field.UpdateLabel();
            }
        }

        private void AddGeneralInfo()
        {
            var timeTextLabel = new Label("Time: ")
            {
                X = Pos.Left(GeneralInformation),
                Y = Pos.Top(GeneralInformation),
                Width = Dim.Fill(),
                Height = 1,
                AutoSize = true
            };
            TimeLabel = new Label(string.Format(TimeString, time.Sec / 60, time.Sec % 60, time.Nsec))
            {
                X = Pos.Right(GeneralInformation) - 13,
                Y = Pos.Top(GeneralInformation),
                Width = Dim.Fill(),
                Height = 1,
                AutoSize = true
            };
            var headline = new Label("Points:")
            {
                X = Pos.Left(GeneralInformation),
                Y = Pos.Bottom(TimeLabel),
                Width = Dim.Fill(),
                Height = 1
            };

            CyanTeamLabel = new Label(String.Format(TeamString, Configurations.GetInstance().Teams[0].Name))
            {
                X = Pos.Left(GeneralInformation),
                Y = Pos.Bottom(headline),
                Width = Dim.Fill(),
                Height = 1,
                AutoSize = true,
                ColorScheme = Config.Team1ColorScheme
            };
            CyanTeamPointsLabel = new Label(String.Format(PointString, 0))
            {
                X = Pos.Right(GeneralInformation)-5,
                Y = Pos.Bottom(headline),
                Width = 3,
                Height = 1,
                AutoSize = true,
                ColorScheme = Config.Team1ColorScheme
            };
            MagentaTeamLabel = new Label(String.Format(TeamString, Configurations.GetInstance().Teams[1].Name))
            {
                X = Pos.Left(GeneralInformation),
                Y = Pos.Bottom(CyanTeamLabel),
                Width = Dim.Fill(),
                Height = 1,
                AutoSize = true,
                ColorScheme = Config.Team2ColorScheme
            };
            MagentaTeamPointsLabel = new Label(String.Format(PointString, 0))
            {
                X = Pos.Right(GeneralInformation)-5,
                Y = Pos.Bottom(CyanTeamLabel),
                Width = 3,
                Height = 1,
                AutoSize = true,
                ColorScheme = Config.Team2ColorScheme
            };
            var anchor = MagentaTeamLabel;
            foreach( var machine in MpsManager.GetInstance().Machines)
            {
                var MpsInfo = new MpsInfoField(machine, GeneralInformation, anchor); 
                MpsInfoFieldList.Add(MpsInfo);
                anchor = MpsInfo.GetAnchor();
            }

            GeneralInformation.Add(timeTextLabel, headline, TimeLabel, CyanTeamLabel, CyanTeamPointsLabel,MagentaTeamLabel,MagentaTeamPointsLabel);

        }
    }

    class MapField
    {
        private Zones Zone;
        private Label Label;
        private TerminalConfig Config;
        public MapField(Pos x, Pos y, Zones zone, Dim width, Dim height, ColorScheme color)
        {
            Zone = zone;
            Config = TerminalConfig.GetInstance();
            string text = "";
            if (Zone != null)
            {
                text = Zone.GetZoneString();
            }
            Label = new Label(text)
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,
                ColorScheme = color
            };

        }

        public Label GetLabel()
        {
            return Label;
        }

        public void UpdateLabel()
        {
            if (Zone == null)
            {
                Label.Text = "Start";
            }
            else
            {
                string text = Zone.GetZoneString();
                
                if (Zone.Machine != null)
                {
                    text += "\n " + Zone.Orientation + "°\n" + Zone.Machine.TaskDescription;
                    Label.ColorScheme = Zone.Machine.Team == Team.Cyan
                        ? Config.Team1ColorScheme
                        : Config.Team2ColorScheme;
                }

                if (Zone.Robot != null)
                {
                    text += "\n" + Zone.Robot.TaskDescription;
                    Label.ColorScheme = Zone.Robot.TeamColor == Team.Cyan
                        ? Config.Team1RobotColorScheme
                        : Config.Team2RobotColorScheme;
                }

                if (Zone.Robot == null && Zone.Machine == null)
                {
                    Label.ColorScheme = Zone.GetsMovedTo ? Config.HighlightColorScheme : Config.DefaultColorScheme;
                }
                Label.Text = text;
            }
        }
    }

    class MpsInfoField
    {
        private const string LightString = " ";
        private readonly Label NameLabel;
        private readonly Label RedLabel;
        private readonly Label YellowLabel;
        private readonly Label GreenLabel;
        private readonly Label? RingLabel1;
        private readonly Label? RingLabel2;
        private MPS.Mps Mps;
        private TerminalConfig Config;
        private Label Anchor;
        public MpsInfoField(MPS.Mps mps, View parent, Label anchor)
        {
            Anchor = anchor;
            Mps = mps;
            Config = TerminalConfig.GetInstance();
            NameLabel = new Label(String.Format("{0,-6}|",Mps.Name))
            {
                X = 0, 
                Y = Pos.Bottom(Anchor),
                Width = 1,
                Height = 1,
                AutoSize = true,
                ColorScheme = Config.Team1ColorScheme
            };
            RedLabel = new Label(LightString)
            {
                X = Pos.Right(NameLabel),
                Y = Pos.Bottom(Anchor),
                Width = 1,
                Height = 1,
                ColorScheme = Config.RedLightColorScheme
            };
            YellowLabel = new Label(LightString)
            {
                X = Pos.Right(RedLabel),
                Y = Pos.Bottom(Anchor),
                Width = 1,
                Height = 1,
                ColorScheme = Config.YellowLightColorScheme
            };
            GreenLabel = new Label(LightString)
            {
                X = Pos.Right(YellowLabel),
                Y = Pos.Bottom(Anchor),
                Width = 1,
                Height = 1,
                ColorScheme = Config.GreenLightColorScheme
            };

            parent.Add(NameLabel,RedLabel,YellowLabel,GreenLabel);
            if(mps.Type == Mps.MpsType.RingStation)
            {
                var divider = new Label("|")
                {
                    X = Pos.Right(GreenLabel),
                    Y = Pos.Bottom(Anchor),
                    Width = 1,
                    Height = 1,
                    ColorScheme = Config.Team1ColorScheme
                };
                RingLabel1 = new Label(LightString)
                {
                    X = Pos.Right(divider),
                    Y = Pos.Bottom(Anchor),
                    Width = 1,
                    Height = 1,
                    ColorScheme = Config.YellowLightColorScheme
                };
                RingLabel2 = new Label(LightString)
                {
                    X = Pos.Right(RingLabel1),
                    Y = Pos.Bottom(Anchor),
                    Width = 1,
                    Height = 1,
                    ColorScheme = Config.GreenLightColorScheme
                };
                parent.Add(divider,RingLabel1,RingLabel2);
            }
            Anchor = GreenLabel;
        }
        public Label GetAnchor()
        {
            return Anchor;
        }
        public void UpdateLabel()
        {            
            GreenLabel.ColorScheme = Mps.GreenLight.LightOn ? Config.GreenLightColorScheme : Config.LightOffColorScheme;
            YellowLabel.ColorScheme = Mps.YellowLight.LightOn ? Config.YellowLightColorScheme : Config.LightOffColorScheme;
            RedLabel.ColorScheme = Mps.RedLight.LightOn ? Config.RedLightColorScheme : Config.LightOffColorScheme;
        }
    }
}
