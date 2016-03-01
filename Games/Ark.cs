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
    public class Ark : BaseGameHost
    {
        public Ark(ConnectParam param) : base(param) { GameName = param.GameName; }

        public override void Go(GameHostParam param)
        {
            //var userId = param.UserId;
            //var run = $"cd /host/{GameName}/serverfiles//Saved/Config/LinuxServer;cp -r GameUserSettings.ini s{userId}.ini";
            //var res = Client.RunCommand(run);
            //if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }

        public override void On(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_ark{userId} " +
                      $"./{GameName} start -servicename ark{userId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
        }

        public override void Off(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_stop_ark{userId} " +
                      $"./{GameName} stop -servicename ark{userId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
        }
    }
}
