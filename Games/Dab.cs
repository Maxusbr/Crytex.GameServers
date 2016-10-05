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
    public class Dab : Cs
    {
        public Dab(ConnectParam param) : base(param, "dab") { GameName = "dab"; }

        public override GameResult Create(CreateParam param)
        {
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            var run = $"cd {Path}/{GameName}/serverfiles/{GameCode}/cfg;cp -r da-server.cfg {GameName}{GameServerId}.cfg";
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

    }
}
