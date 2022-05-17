using System;
using LlsfMsgs;
using Simulator.MPS;
using Terminal.Gui;

namespace Simulator.TerminalGui
{
    class MpsGuiView
    {
        public Window MpsWindow { get; set; }

        private const string TeamColorString = "Team      [{0}]";
        private const string TypeString = "Type      [{0}]";
        private const string InDataString = "InData    [{0}][{1}]";
        private const string InActionString = "InAction     [{0}]";
        private const string BasicDataString = "BData     [{0}][{1}]";
        private const string BasicActionString = "BAction      [{0}]";
        //private const string ErrorString = "Error    [{0}]";
        private const string StatusString = "Busy     [{0}]\nReady    [{1}]\nError    [{2}]\nEnabled  [{3}]";
        private const string ZoneString = "Zone      [{0}]";
        private const string RotationString = "Rotation      [{0}]";
        private const string LightString = " ";
        private const string BeltString = "Belt:\n[{0}==={1}==={2}]\nInput            Output";
        private const string SlideCount = "SldCnt:     [{0}]";

        private readonly Label TeamColor;
        private readonly Label TypeLabel;
        private readonly Label InDataLabel;
        private readonly Label InActionLabel;
        private readonly Label BasicDataLabel;
        private readonly Label BasicActionLabel;
        //private readonly Label ErrorLabel;
        private readonly Label StatusLabel;
        private readonly Label ZoneLabel;
        private readonly Label RotationLabel;
        private readonly Label RedLabel;
        private readonly Label YellowLabel;
        private readonly Label GreenLabel;
        private readonly Label BeltLabel;
        private readonly Label SlideLabel;

        private readonly MPS.Mps Mps;
        private ColorScheme TeamColorScheme;

        private TerminalConfig Config;
        public MpsGuiView(MPS.Mps mps, int width)
        {
            Config = TerminalConfig.GetInstance();
            Mps = mps;
            var y = 0;
            TeamColorScheme = mps.Team == Team.Cyan
                ? TerminalConfig.GetInstance().Team1ColorScheme
                : TerminalConfig.GetInstance().Team2ColorScheme;
            var window = new Window(Mps.Name)
            {
                X = 0,
                Y = 0,
                Width = width,
                Height = Dim.Fill(),
                ColorScheme = TeamColorScheme
            };
            TeamColor = new Label(0, y++, String.Format(TeamColorString, Mps.Team))
            {
                AutoSize = true
            };
            TypeLabel = new Label(0, y++, String.Format(TypeString, Mps.Type))
            {
                AutoSize = true
            };
            InDataLabel = new Label(0, y++, String.Format(InDataString, 0, 0))
            {
                AutoSize = true
            };
            InActionLabel = new Label(0, y++, String.Format(InActionString, "no Action"))
            {
                AutoSize = true
            };
            BasicDataLabel = new Label(0, y++, String.Format(BasicDataString, 0, 0))
            {
                AutoSize = true
            };
            BasicActionLabel = new Label(0, y++, String.Format(BasicActionString, "no Action"))
            {
                AutoSize = true
            };
            ZoneLabel = new Label(0, y++, String.Format(ZoneString, "not Placed"))
            {
                AutoSize = true
            };
            RotationLabel = new Label(0, y++, String.Format(RotationString, "not Placed"))
            {
                AutoSize = true
            };
            StatusLabel = new Label(0, y++, String.Format(StatusString, "false", "false", "false", "false"))
            {
                AutoSize = true
            };
            y += 3;
            int x = 0;
            SlideLabel = new Label(0, y++, String.Format(SlideCount, "0"))
            {
                AutoSize = true
            };
            RedLabel = new Label(x++, y, LightString)
            {
                ColorScheme = Config.RedLightColorScheme
            };
            YellowLabel = new Label(x++, y, LightString)
            {
                ColorScheme = Config.YellowLightColorScheme
            };
            GreenLabel = new Label(x++, y, LightString)
            {
                ColorScheme = Config.GreenLightColorScheme
            };
            BeltLabel = new Label(0, y++, String.Format(BeltString, "Empty", "Empty", "Empty"))
            {
                AutoSize = true
            };
            window.Add(TeamColor, TypeLabel, InDataLabel, InActionLabel, BasicDataLabel, BasicActionLabel, ZoneLabel, RotationLabel, StatusLabel, RedLabel, SlideLabel, YellowLabel, GreenLabel, BeltLabel);
            MpsWindow = window;
        }

        public void Update()
        {
            if(Mps.Team == Team.Cyan)
                MpsWindow.ColorScheme = Mps.GotConnection ? Config.Team1ColorScheme : Config.DefaultColorScheme;
            else
            {
                MpsWindow.ColorScheme = Mps.GotConnection ? Config.Team2ColorScheme : Config.DefaultColorScheme;
            }
            TeamColor.Text = String.Format(TeamColorString, Mps.Team);
            TypeLabel.Text = String.Format(TypeString, GetTypeString(Mps.Type));
            if (!Configurations.GetInstance().MockUp)
            {
                InDataLabel.Text = String.Format(InDataString, Mps.InNodes.Data0.Value.ToString(), Mps.InNodes.Data1.Value.ToString());
                InActionLabel.Text = String.Format(InActionString, Mps.InNodes.ActionId.Value.ToString());
                BasicDataLabel.Text = String.Format(BasicDataString, Mps.BasicNodes.Data0.Value.ToString(), Mps.BasicNodes.Data1.Value.ToString());
                BasicActionLabel.Text = String.Format(BasicActionString, Mps.BasicNodes.ActionId.Value.ToString());
                StatusLabel.Text = String.Format(StatusString, Mps.BasicNodes.StatusNodes.busy.Value.ToString(),
                    Mps.BasicNodes.StatusNodes.ready.Value.ToString(), Mps.BasicNodes.StatusNodes.error.Value.ToString(), Mps.BasicNodes.StatusNodes.enable.Value.ToString());
            }
            ZoneLabel.Text = String.Format(ZoneString, Mps.Zone);
            RotationLabel.Text = String.Format(RotationString, Mps.Rotation);
            GreenLabel.ColorScheme = Mps.GreenLight.LightOn ? Config.GreenLightColorScheme : Config.LightOffColorScheme;
            YellowLabel.ColorScheme = Mps.YellowLight.LightOn ? Config.YellowLightColorScheme : Config.LightOffColorScheme;
            RedLabel.ColorScheme = Mps.RedLight.LightOn ? Config.RedLightColorScheme : Config.LightOffColorScheme;
            BeltLabel.Text = String.Format(BeltString,
                                            Mps.ProductAtIn != null ? Mps.ProductAtIn.ProductDescription() : "Empty",
                                            Mps.ProductOnBelt != null ? Mps.ProductOnBelt.ProductDescription() : "Empty",
                                             Mps.ProductAtOut != null ? Mps.ProductAtOut.ProductDescription() : "Empty");
            SlideLabel.Text = String.Format(SlideCount, Mps.InNodes.SlideCnt.Value);
            //Belt.Fraction = 1f;
            /*switch (Mps.Belt.Direction)
            {
                case Direction.FromInToOut:
                    Belt.Fraction = 0;
                    break;
                case Direction.FromOutToIn:
                    break;
            }*/

        }

        private string GetTypeString(Mps.MpsType type)
        {
            switch (type)
            {
                case Mps.MpsType.BaseStation:
                    return "BS";
                case Mps.MpsType.CapStation:
                    return "CS"; ;
                case Mps.MpsType.DeliveryStation:
                    return "DS"; ;
                case Mps.MpsType.RingStation:
                    return "RS"; ;
                case Mps.MpsType.StorageStation:
                    return "SS"; ;
            }

            return "";
        }
    }
}
