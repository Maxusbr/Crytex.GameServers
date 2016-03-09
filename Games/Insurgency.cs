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
        public Insurgency(ConnectParam param) : base(param) { }

        public override void Go(GameHostParam param)
        {
            base.Go(param);
            UserId = param.UserId;
            var run = $"cd /host/{GameName}/serverfiles/insurgency/cfg;cp -r ins-server.cfg ins{UserId}.cfg";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }

        public override DataReceivedModel On(GameHostParam param)
        {
            var resModel = base.On(param);
            var run = $"cd /host/{GameName};screen -dmS server_start_ins{UserId} " +
                      $"./{GameName} start -servicename ins{UserId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            resModel.Data = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
            return resModel;
        }

        public override void Off(GameHostParam param)
        {
            base.Off(param);
            var run = $"cd /host/{GameName};screen -dmS server_start_ins{UserId} " +
                      $"./{GameName} stop -servicename ins{UserId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }
    }
}
