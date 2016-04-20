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
    public class Hw : BaseGameHost
    {
        public Hw(ConnectParam param) : base(param, "hw") { GameName = "hw"; }

        public override GameResult Create(CreateParam param)
        {
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            var result = new GameResult();
            return result;
        }

        protected override GameResult On(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} start -servicename {GameName}{GameServerId} -port {param.GamePort} -queryport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            result.Data = res.Result;
            return result;
        }

        public override bool CompleteInstal()
        {
            return true;
        }
    }
}
