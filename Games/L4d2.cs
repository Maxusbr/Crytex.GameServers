using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Models;

namespace Crytex.GameServers.Games
{
    public class L4D2 : BaseGameHost
    {
        public L4D2(ConnectParam param) : base(param, "left4dead2") { GameName = "l4d2"; }
    }
}
