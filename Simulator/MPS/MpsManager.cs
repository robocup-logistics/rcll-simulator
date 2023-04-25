using Simulator.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LlsfMsgs;
using Simulator.RobotEssentials;


namespace Simulator.MPS
{
    public class MpsManager
    {
        public bool AllMachineSet {get; private set;}
        public List<Mps> Machines { get; }
        private MyLogger myLogger;
        private TcpConnector Refbox;
        private Configurations Config;
        private Thread RefboxThread;
        public MpsManager(Configurations config)
        {
            myLogger = new MyLogger("MpsManager", true);
            Config = config;
            myLogger.Log("Started the Mps Manager!");
            Machines = new List<Mps>();
            AllMachineSet = false;
            CreateMachines();
            if(!Config.MockUp)
            {
                RefboxThread = new Thread(() => new TcpConnector(Config, Config.Refbox.IP, Config.Refbox.TcpPort, this, myLogger));
                RefboxThread.Start();
            }
        }
        private void CreateMachines()
        {
            foreach (var mps in Config.MpsConfigs)
            {
                Mps? currentMps;
                Thread? thread;
                switch (mps.Type)
                {
                    case Mps.MpsType.BaseStation:
                        var bs = new MPS_BS(Config, mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(bs.Run);
                        currentMps = bs;
                        break;
                    case Mps.MpsType.CapStation:
                        var cs = new MPS_CS(Config, mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(cs.Run);
                        currentMps = cs;
                        break;
                    case Mps.MpsType.DeliveryStation:
                        var ds = new MPS_DS(Config, mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(ds.Run);
                        currentMps = ds;
                        break;
                    case Mps.MpsType.RingStation:
                        var rs = new MPS_RS(Config, mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(rs.Run);
                        currentMps = rs;
                        break;
                    case Mps.MpsType.StorageStation:
                        var ss = new MPS_SS(Config, mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(ss.Run);
                        currentMps = ss;
                        break;
                    default:
                        Console.WriteLine("Unknown station type!");
                        thread = null;
                        currentMps = null;
                        break;
                }
                if(currentMps==null)
                {
                    continue;
                }
                if (thread != null)
                {
                    thread.Name = currentMps.Name + "_workingThread";
                }

                thread?.Start();
                Machines.Add(currentMps);
                //mps1.Run();
            }

        }
        public Mps? GetMachineViaId(string machineId)
        {
            foreach(var m in Machines)
            {
                if(machineId.Equals(m.Name))
                {
                    return m;
                }
            }
            
            return null;
        }
        public void PlaceMachines(MachineInfo Info)
        {
            myLogger.Log("Starting to PlaceMachines!");
            myLogger.Log("Received Information = " + Info.ToString());
            var list = new List<Zone>();
            foreach (var machine in Info.Machines)
            {
                list.Add(machine.Zone);
            }
            if(list.Distinct().Count() != Info.Machines.Count)
            {
                myLogger.Log("Duplicated zones for machines. Will skip this place machines!");
                return;
            }
            foreach (var machineInfo in Info.Machines)
            {
                foreach (var machine in Machines.Where(machine => machineInfo.Name.Equals(machine.Name)))
                {
                    if(machine.GotPlaced)
                    {
                        continue;
                    }
                    myLogger.Log("Placed " + machine.Name + "!");

                    machine.Zone = machineInfo.Zone;
                    machine.Rotation = machineInfo.Rotation;
                    ZonesManager.GetInstance().PlaceMachine(machineInfo.Zone, machine.Rotation, machine);
                    machine.GotPlaced = true;
                }
                /* TODO check if placement still works
                 foreach (var machine in Machines)
                {
                    if (machineinfo.Name.Equals(machine.Name))
                    {
                        machine.Zone = machineinfo.Zone;
                        machine.Rotation = machineinfo.Rotation;
                    }
                }*/
            }
            var notset = false;
            foreach(var machine in Machines)
            {
                if(machine.GotPlaced == false)
                {
                    notset = true;
                }
            }
            if (notset == false)
            {
                AllMachineSet = true;
            }
        }

        public void StopAllMachines()
        {
            foreach (var machine in Machines)
            {
                machine.StopMachine();
            }
        }
        public void StartRefboxConnection()
        {
            if (!Config.MockUp)
            {
                var rf = new UdpConnector(Config, Config.Refbox.IP, Config.Refbox.CyanRecvPort, this, myLogger);
                rf.Start();
            }
        }
        internal bool FindMachine(string machine, ref Zone Target)
        {
            foreach (var m in Machines)
            {
                if (m.Name.Equals(machine))
                {

                    Target = m.Zone;
                    return true;
                }
            }

            return false;
        }
    }
}
