using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class L4D : BaseGameHost
    {
        public L4D(ConnectParam param) : base(param, "left4dead") { GameName = "l4d"; }

    }
}
