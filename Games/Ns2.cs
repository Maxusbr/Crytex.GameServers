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
    public class Ns2 : BaseGameHost
    {
        public Ns2(ConnectParam param) : base(param, "ns2") { GameName = "ns2"; }

        public override GameResult Create(CreateParam param)
        {
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            if (!CompleteInstal())
            {
                var run = $"cd {Path}/{GameName};cp -r ns2-server {GameName}{GameServerId}";
                Client.RunCommand(run);
            }
            var result = new GameResult();
            return result;
        }

        public override bool CompleteInstal()
        {
            var run = $"cd {Path}/{GameName};find {GameName}{GameServerId}";
            var res = Client.RunCommand(run);
            return string.IsNullOrEmpty(res.Error);
        }
    }
}
