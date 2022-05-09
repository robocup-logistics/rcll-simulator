using System;
using System.Collections.Generic;
using System.Diagnostics;
using LlsfMsgs;
using NStack;
using Simulator.MPS;
using Simulator.RobotEssentials;
using Simulator.Utility;
using Terminal.Gui;
using Application = Terminal.Gui.Application;
using Button = Terminal.Gui.Button;
using Label = Terminal.Gui.Label;
using MenuItem = Terminal.Gui.MenuItem;
using Robot = Simulator.RobotEssentials.Robot;
using View = Terminal.Gui.View;
using Timer = Simulator.Utility.Timer;

namespace Simulator.TerminalGui
{
    class TerminalGui
    {
        private readonly List<MpsGuiView> MpsGuiList = new List<MpsGuiView>();
        private readonly List<RobotGuiView> RobotGuiList = new List<RobotGuiView>();
        private readonly List<MapGuiView> MapGuiViews = new List<MapGuiView>();
        private readonly List<View> ViewList;
        private readonly int Console_Height;
        private readonly int Console_Width;
        private readonly MpsManager MpsManager;
        private readonly RobotManager RobotManager;
        private readonly ZonesManager ZonesManager;
        private readonly TabView MainView;

        public TerminalGui(RobotManager robotManager, MpsManager mpsManager, ZonesManager zonesManager)
        {
            /*Test(robotManager);
            return;*/
            MpsManager = mpsManager;
            RobotManager = robotManager;
            ZonesManager = zonesManager;
            ViewList = new List<View>();
            Application.Init();
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Quit", "", StopApplication)
                }),
                /*new MenuBarItem("_View", new MenuItem[]
                {
                    new MenuItem("_ViewMap", "Switch to the Map View" ,ViewMapView)
                }),*/
                new MenuBarItem( "_Debug", new MenuItem[] {
                    new MenuItem ("_SendCommand", "", SendCommandPrompt)
                })

            });

            Application.Top.Add(menu);

            MainView = new TabView
            {
                X = 0,
                Y = Pos.Bottom(menu),
                Width = Dim.Percent(100),
                Height = Dim.Percent(100),
                ColorScheme = TerminalConfig.GetInstance().DefaultColorScheme,
                Style = new TabView.TabStyle()
                {
                    ShowBorder = false
                }

            };
            Application.Top.GetCurrentHeight(out Console_Height);
            Application.Top.GetCurrentWidth(out Console_Width);


            var mpsView = CreateMpsView(mpsManager);
            var robotView = CreateRobotView(robotManager);
            var generalView = CreateGeneralView();
            ViewList.Add(generalView);
            ViewList.Add(mpsView);
            ViewList.Add(robotView);

            var first = true;

            foreach (var view in ViewList)
            {
                var TabView = new TabView.Tab
                {
                    Text = view.Text,
                    View = view
                };
                MainView.AddTab(TabView, first);
                first = false;
            }
            Application.Top.Add(MainView);
            Application.UseSystemConsole = true;
            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(200), UpdateViews);
            Application.Run(Application.Top);
        }

        private View CreateRobotView(RobotManager robotManager)
        {
            var robotView = new View("Robots")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(100),
                Height = Dim.Percent(100),
                ColorScheme = TerminalConfig.GetInstance().DefaultColorScheme
            };

            var offset = 0;
            foreach (var rob in robotManager.Robots)
            {
                var view = new RobotGuiView(rob, offset, TerminalConfig.GetInstance().ColumnWidthRobot, rob.TeamColor);
                robotView.Add(view.RobotWindow);
                RobotGuiList.Add(view);
                offset += TerminalConfig.GetInstance().ColumnWidthRobot - 1;
            }

            robotView.Text = "Robots";
            return robotView;
        }
        private View CreateMpsView(MpsManager mpsManager)
        {
            var mpsView = new View("Machines")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(100),
                Height = Dim.Percent(100),
                ColorScheme = TerminalConfig.GetInstance().DefaultColorScheme
            };
            var i = 0;
            foreach (var mps in mpsManager.Machines)
            {
                var view = new MpsGuiView(mps, TerminalConfig.GetInstance().ColumnWidthMPS) { MpsWindow = { X = i * (TerminalConfig.GetInstance().ColumnWidthMPS - (i != 0 ? 1 : 0)) } };
                mpsView.Add(view.MpsWindow);
                MpsGuiList.Add(view);
                i++;
            }

            mpsView.Text = "Machines";
            return mpsView;
        }

        private View CreateGeneralView()
        {
            var map = new MapGuiView(Console_Height, Console_Width)
            {
                GeneralWindow =
                {
                    Text = "General Information"
                }
            };
            MapGuiViews.Add(map);
            return map.GetWindow();
        }

        private bool UpdateViews(MainLoop arg)
        {

            foreach (var view in RobotGuiList)
            {
                view.Update();
            }

            foreach (var view in MpsGuiList)
            {
                view.Update();
            }

            foreach (var view in MapGuiViews)
            {
                view.Update();
            }
            return true;
        }
        private static void StopApplication()
        {
            Environment.Exit(0);
        }

        private void SendCommandPrompt()
        {
            var ok = new Button(3, 14, "Ok");


            var cancel = new Button(10, 14, "Cancel");
            cancel.Clicked += () => Application.RequestStop();
            var dialog = new Dialog("Send Command Prompt", 60, 18, ok, cancel);
            ustring[] taskOptions = { "Place ...", "Report", "MoveToWaypoint", "GetFromStation", "DeliverToStation", "BufferCapStation", "ExploreMachine" };
            var taskGroup = new RadioGroup(taskOptions)
            {
                X = 4,
                Y = 2,
                SelectedItem = 0,
            };
            var machineOptions = Array.Empty<ustring>();
            foreach (var machine in MpsManager.Machines)
            {
                Array.Resize<ustring>(ref machineOptions, machineOptions.Length + 1);
                machineOptions[^1] = machine.Name;
            }

            var machineGroup = new RadioGroup(machineOptions)
            {
                X = 25,
                Y = 2,
                SelectedItem = 0
            };

            var robotOptions = Array.Empty<ustring>();
            foreach (var robot in RobotManager.Robots)
            {
                Array.Resize<ustring>(ref robotOptions, robotOptions.Length + 1);
                robotOptions[^1] = robot.RobotName;
            }
            var robotGroup = new RadioGroup(robotOptions)
            {
                X = 40,
                Y = 2,
                SelectedItem = 0
            };
            var label = new Label(1, 1, "Zone: ");
            var entry = new TextField
            {
                X = 7,
                Y = 1,
                Width = Dim.Fill(),
                Height = 1
            };
            ok.Clicked += () =>
            {
                var newTask = new GripsMidlevelTasks
                {
                    CancelTask = false,
                    DeliverToStation = null,
                    Successful = false,
                    ExploreMachine = null,
                    GetFromStation = null,
                    MoveToWaypoint = null,
                    ReportAllSeenMachines = false,
                    RobotId = 1,
                    ReceiveMachineInfos = false
                };
                string? mpsName = machineOptions[machineGroup.SelectedItem].ToString();
                switch (taskGroup.SelectedItem)
                {
                    case 0:// Place CBS
                        {
                            if (entry.Text.IsEmpty)
                            {
                                ZonesManager.PlaceMachine(Zone.MZ28, 180, MpsManager.Machines[0]);
                                ZonesManager.PlaceMachine(Zone.CZ47, 0, MpsManager.Machines[1]);
                                ZonesManager.PlaceMachine(Zone.CZ76, 90, MpsManager.Machines[2]);
                                ZonesManager.PlaceMachine(Zone.MZ55, 225, MpsManager.Machines[3]);
                                ZonesManager.PlaceMachine(Zone.CZ14, 270, MpsManager.Machines[4]);
                                ZonesManager.PlaceMachine(Zone.CZ33, 225, MpsManager.Machines[5]);
                                ZonesManager.PlaceMachine(Zone.CZ72, 135, MpsManager.Machines[6]);
                            }
                            else
                            {
                                var z = ZonesManager.GetZoneFromString(entry.Text.ToString());
                                ZonesManager.PlaceMachine(z, 180, MpsManager.Machines[machineGroup.SelectedItem]);
                            }
                            break;
                        }
                    case 1: // Report
                        {
                            newTask.ReportAllSeenMachines = true;
                            break;
                        }
                    case 2: // MoveToWaypoint
                        {
                            if (entry.Text.IsEmpty)
                            {
                                newTask.MoveToWaypoint = new MoveToWaypoint
                                {
                                    Waypoint = mpsName
                                };
                            }
                            else
                            {
                                newTask.MoveToWaypoint = new MoveToWaypoint
                                {
                                    Waypoint = entry.Text.ToString()
                                };
                            }

                            break;
                        }
                    case 3: // GetFromStation
                        newTask.GetFromStation = new GetFromStation()
                        {
                            MachineId = mpsName,
                            MachinePoint = "shelf0"
                        };
                        RobotGuiList[0].Robot.SetGripsTasks(newTask);
                        //RobotGuiList[0].Robot.GetFromStation();
                        break;
                    case 4: // DeliverToStation
                        {
                            newTask.DeliverToStation = new DeliverToStation()
                            {
                                MachineId = mpsName,
                                MachinePoint = "shelf0"
                            };
                            break;
                        }
                    case 5:// BufferCapStation
                        {
                            newTask.BufferCapStation = new BufferCapStation()
                            {
                                MachineId = mpsName,
                                ShelfNumber = 1
                            };
                            break;
                        }
                    case 6: // ExploreMachine
                        {
                            newTask.ExploreMachine = new ExploreMachine()
                            {
                                MachineId = mpsName,
                                MachinePoint = "shelf0",
                                Waypoint = mpsName
                            };
                            break;
                        }
                    default:
                        break;
                }

                if (taskGroup.SelectedItem > 1)
                {
                    RobotManager.Robots[robotGroup.SelectedItem].SetGripsTasks(newTask);
                    //RobotManager.Robots[robotGroup.SelectedItem + 1].SetGripsTasks(newTask);
                }
                Application.RequestStop();
            };
            var timerButton = new Button(21, 14, "Timer Pause/Unpause");
            timerButton.Clicked += () =>
            {
                //ZonesManager.GetInstance().ShowNeighbourhood(ZonesManager.GetZoneFromString(entry.Text.ToString()));
                Timer.GetInstance().ContinueTicking();
            };
            dialog.Add(label, entry, taskGroup, machineGroup, robotGroup, timerButton);
            Application.Run(dialog);
        }
    }
}
