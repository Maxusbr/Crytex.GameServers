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
    public class Rust : BaseGameHost
    {
        public Rust(ConnectParam param) : base(param, "rust") { GameName = "rust"; }

        public override GameResult Create(CreateParam param)
        {
            GameServerId = param.GameServerId;
            var run = $"cd {Path}/rust/serverfiles/server/;find {GameName}{GameServerId}";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error))
            {
                run = $"cd {Path}/rust/serverfiles/server/;mkdir {GameName}{GameServerId};" +
                      $"cd rust-server; cp -r * {Path}/rust/serverfiles/server/{GameName}{GameServerId}";
                Client.RunCommand(run);
            }
            var result = new GameResult();
            return result;
        }

        protected override GameResult On(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} start -servicename {GameName}{GameServerId} -port {param.GamePort} " +
                      $"-rconport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            result.Data = res.Result;
            return result;
        }
    }
}
