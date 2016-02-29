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
    public class Insurgency : BaseGameHost
    {
        public Insurgency(ConnectParam param) : base(param) { GameName = "ins"; }

        public override void Go(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName}/serverfiles/insurgency/cfg;cp -r ins-server.cfg ins{userId}.cfg";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }

        public override void On(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_ins{userId} " +
                      $"./{GameName} start -servicename ins{userId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }

        public override void Off(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_ins{userId} " +
                      $"./{GameName} stop -servicename ins{userId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }
    }
}
