using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulator.Utility;
using Timer = System.Threading.Timer;

namespace Simulator.RobotEssentials
{
    internal class PBMessageFactoryRobot : PBMessageFactoryBase
    {
        private Robot Peer;
        
        public PBMessageFactoryRobot(Robot peer, MyLogger log) :base(log)
        {
            Peer = peer;
        }

    }
}
