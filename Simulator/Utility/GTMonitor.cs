using LlsfMsgs;
using Simulator.MPS;

namespace Simulator.Utility;
public class GTMonitor {
    private bool Running = true;
    private Thread MonitorThread;
    private Queue<MachineInfo> MachineInfos = new Queue<MachineInfo>();
    private ZonesManager ZonesManager;
    private MpsManager MpsManager;
    private MyLogger logger;

    class Disagreement {
        public string key;
        public int value { get; set; }

        public Disagreement(string key) {
            this.key = key;
            this.value = 0;
        }
    }
    private List<Disagreement> KnownDisagreements = new List<Disagreement>();

    public GTMonitor() {
        logger = new MyLogger("GTMonitor");
        ZonesManager = ZonesManager.GetInstance();
        MpsManager = MpsManager.GetInstance();
        MonitorThread = new Thread(Monitor);
        MonitorThread.Name = "GTMonitorThread";
        MonitorThread.Start();
    }

    private string TypeToString(MpsType type) {
        switch(type) {
            case MpsType.BaseStation:
                return "BS";
            case MpsType.RingStation:
                return "RS";
            case MpsType.CapStation:
                return "CS";
            case MpsType.DeliveryStation:
                return "DS";
            case MpsType.StorageStation:
                return "SS";
        }
        return "";
    }

    private void SyncDisagreements(List<string> currentDisagreements) {
        foreach(var disagreement in currentDisagreements) {
            if(!KnownDisagreements.Any(kd => kd.key == disagreement)) {
                KnownDisagreements.Add(new Disagreement(disagreement));
            }
        }

        for (int i = 0; i < KnownDisagreements.Count; i++) {
            var dissagreement = KnownDisagreements[i];
            if (!currentDisagreements.Contains(dissagreement.key)) {
                if (dissagreement.value >= 2) {
                    logger.Warn("<= " + dissagreement.key);
                }
            }
        }

        KnownDisagreements.RemoveAll(d => !currentDisagreements.Contains(d.key));

        for (int i = 0; i < KnownDisagreements.Count; i++) {
            var dissagreement = KnownDisagreements[i];
            if (dissagreement.value++ == 2) {
                logger.Warn("=> " + dissagreement.key);
            }
        }
    }

    private void Monitor() {
        while (Running) {
            if(MachineInfos.Count == 0) {
                Thread.Sleep(500);
                continue;
            }

            MachineInfo info;
            lock (MachineInfos) {
                info = MachineInfos.Dequeue();
            }

            List<string> dissagreements = new List<string>();
            foreach (var machine in info.Machines) {
                CZones? zone = ZonesManager.GetZone(machine.Zone);
                Mps? mps = MpsManager.GetMachineByName(machine.Name);

                if(mps == null) {
                    string dissagreement = "Machine " + machine.Name + " only exists in Refbox";
                    dissagreements.Add(dissagreement);
                }

                if(zone == null) {
                    string dissagreement = "Zone " + machine.Zone.ToString() + " only exists in Refbox";
                    dissagreements.Add(dissagreement);
                }

                if(mps == null || zone == null) {
                    continue;
                }

                if(mps.Rotation != machine.Rotation) {
                    string dissagreement = "Machine rotation should be " + machine.Rotation + " but is acutally " + mps.Rotation;
                    dissagreements.Add(dissagreement);
                }

                if(mps.Zone != machine.Zone) {
                    string dissagreement = "Machine " + machine.Name + " zone should be " + machine.Zone + " but is acutally " + mps.Zone;
                    dissagreements.Add(dissagreement);
                }

                if(TypeToString(mps.Type) != machine.Type) {
                    string dissagreement = "Machine "+ machine.Name + " type should be " + machine.Type + " but is acutally " + TypeToString(mps.Type);
                    dissagreements.Add(dissagreement);
                }

                if(machine.Type == "CS") {
                    uint capCount = 0;
                    var cs = (MPS_CS)mps;
                    if(cs.StoredCap != null) {
                        capCount++;
                    }
                    if(machine.LoadedWith != capCount) {
                        string dissagreement = "Machine " + machine.Name + " should be loaded with " + machine.LoadedWith + " caps but is acutally loaded with " + capCount;
                        dissagreements.Add(dissagreement);

                    }
                }
                if(machine.Type == "RS") {
                    uint baseCount = 0;

                    if(mps.ProductAtIn != null) {
                        baseCount++;
                    }

                    if(mps.ProductOnBelt != null) {
                        baseCount++;
                    }

                    if(mps.ProductAtOut != null) {
                        baseCount++;
                    }

                    if(machine.LoadedWith != baseCount) {
                        string dissagreement = "Machine " + machine.Name + " should be loaded with " + machine.LoadedWith + " caps but is acutally loaded with " + baseCount;
                        dissagreements.Add(dissagreement);
                    }
                }
                if(machine.Type == "SS") {
                    var ss = (MPS_SS)mps;

                    foreach(var status in machine.StatusSs) {
                        var localProduct = ss.Storage[(int)status.Shelf][(int)status.Slot]?.MachineInfoDescription();
                        if((!status.IsFilled && localProduct != null)
                           || (status.IsFilled && localProduct != status.Description)) {
                            string dissagreement = "Machine " + machine.Name + " should have " + status.Description + " on shelf " + status.Shelf + " slot " + status.Slot + " but has " + ss.Storage[(int)status.Shelf][(int)status.Slot]?.MachineInfoDescription();
                            dissagreements.Add(dissagreement);
                        }
                    }
                }
            }

            SyncDisagreements(dissagreements);
        }
    }

    public void Append(MachineInfo machineInfo) {
        lock (MachineInfos) {
            MachineInfos.Enqueue(machineInfo);
        }
    }
    public void Stop() {
        Running = false;
    }
}
