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
    public class CsGo : BaseGameHost
    {
        public CsGo(ConnectParam param) : base(param) { GameName = "csgo"; }

        public override void Go(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName}/serverfiles/csgo/cfg;cp -r csgo-server.cfg csgo{userId}.cfg";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }

        public override void On(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_csgo{userId} " +
                      $"./{GameName} start -servicename csgo{userId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }

        public override void Off(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_csgo{userId} " +
                      $"./{GameName} stop -servicename csgo{userId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }
    }
}
