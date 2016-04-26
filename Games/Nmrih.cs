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
    public class Nmrih : Cs
    {
        public Nmrih(ConnectParam param) : base(param, "nmrih") { GameName = "nmrih"; }

    }
}
