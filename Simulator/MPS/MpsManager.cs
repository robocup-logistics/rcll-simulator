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
        //private member and getter for my singleton configurations class
        private static MpsManager Instance;
        /// <returns>
        /// Returns the instance of the Configurations Singleton
        /// </returns>
        public static MpsManager GetInstance()
        {
            return Instance ??= new MpsManager();
        }
        public bool AllMachineSet {get; private set;}
        public List<Mps> Machines { get; }
        private MyLogger myLogger;
        private TcpConnector Refbox;
        private Configurations Config;
        private MpsManager()
        {
            myLogger = new MyLogger("MpsManager", true);
            Config = Configurations.GetInstance();
            myLogger.Log("Started the Mps Manager!");
            Machines = new List<Mps>();
            CreateMachines();
            AllMachineSet = false;
            Instance = this;
            if(!Config.MockUp)
            {
                Refbox = new TcpConnector(Config.Refbox.IP, Config.Refbox.TcpPort, null, myLogger);
                Refbox.Start();
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
                        var bs = new MPS_BS(mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(bs.Run);
                        currentMps = bs;
                        break;
                    case Mps.MpsType.CapStation:
                        var cs = new MPS_CS(mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(cs.Run);
                        currentMps = cs;
                        break;
                    case Mps.MpsType.DeliveryStation:
                        var ds = new MPS_DS(mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(ds.Run);
                        currentMps = ds;
                        break;
                    case Mps.MpsType.RingStation:
                        var rs = new MPS_RS(mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(rs.Run);
                        currentMps = rs;
                        break;
                    case Mps.MpsType.StorageStation:
                        var ss = new MPS_SS(mps.Name, mps.Port, Machines.Count, mps.Team, mps.Debug);
                        thread = new Thread(ss.Run);
                        currentMps = ss;
                        break;
                    default:
                        Console.WriteLine("Unknown station type!");
                        thread = null;
                        currentMps = null;
                        break;
                }
                thread?.Start();
                if(currentMps==null)
                {
                    continue;
                }
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
        public void StartRefboxConnection()
        {
            if (!Config.MockUp)
            {
                var rf = new UdpConnector(Config.Refbox.IP, Config.Refbox.CyanRecvPort, myLogger);
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
