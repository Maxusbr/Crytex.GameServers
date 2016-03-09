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
    public class Arma3 : BaseGameHost
    {
        public Arma3(ConnectParam param) : base(param) { }

        public override void Go(GameHostParam param)
        {
            base.Go(param);
            var run = $"cd /host/{GameName}/serverfiles/cfg;cp -r arma3-server.server.cfg arm{UserId}.server.cfg";
            var res = Client.RunCommand(run);
            Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
            run = $"cd /host/{GameName}/serverfiles/cfg;cp -r arma3-server.network.cfg arm{UserId}.network.cfg";
            res = Client.RunCommand(run);
            Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
        }

        public override DataReceivedModel On(GameHostParam param)
        {
            var resModel = base.On(param);
            var run = $"cd /host/{GameName};screen -dmS server_start_arm{UserId} " +
                      $"./{GameName} start -servicename arm{UserId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            resModel.Data = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
            return resModel;
        }

        public override void Off(GameHostParam param)
        {
            base.Off(param);
            var run = $"cd /host/{GameName};" + //screen -dmS server_stop_arm{userId} " +
                      $"./{GameName} stop -servicename arm{UserId};";
            var res = Client.RunCommand(run);
            Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
        }
    }
}
