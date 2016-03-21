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
    public class Dod : Cs
    {
        public Dod(ConnectParam param) : base(param, "dod") { GameName = "dod"; }

        public override GameResult Create(CreateParam param)
        {
            GameServerId = param.GameServerId;
            var run = $"cd {Path}/{GameName}/serverfiles/{GameCode};cp -r {GameName}-server.cfg {GameName}{GameServerId}.cfg";
            var res = Client.RunCommand(run);
            var result = new GameResult();
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            return result;
        }
    }
}
