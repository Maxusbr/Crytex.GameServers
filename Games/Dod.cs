using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Crytex.Model.Exceptions;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class Dod : Cs
    {
        public Dod(ConnectParam param) : base(param, "dod") { GameName = "dod"; }

        public override GameResult Create(CreateParam param)
        {
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            var run = $"cd {Path}/{GameName}/serverfiles/{GameCode};cp -r {GameName}-server.cfg {GameName}{GameServerId}.cfg";
            var res = Client.RunCommand(run);
            var result = new GameResult();
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            if (!CompleteInstal())
            {
                result.Error = GameHostTypeError.CantCreate;
                result.Succes = false;
                result.ErrorMessage = "Error Create";
            }
            return result;
        }

        public override bool CompleteInstal()
        {
            var run = $"cd {Path}/{GameName}/serverfiles/{GameCode};find {GameName}{GameServerId}.cfg";
            var res = Client.RunCommand(run);
            return string.IsNullOrEmpty(res.Error);
        }
    }
}
