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
    public class Cs : BaseGameHost
    {
        public Cs(ConnectParam param) : base(param) { GameName = "cs"; }

        public override void Go(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName}/serverfiles/cstrike/cfg;cp -r cs-server.cfg s{userId}.cfg";
            var res = Client.RunCommand(run);
        }

        public override void On(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_cs{userId} ./{GameName} start -servicename cs{userId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
        }

        public override void Off(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_cs{userId} ./{GameName} stop -servicename cs{userId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
        }

        
    }
}
