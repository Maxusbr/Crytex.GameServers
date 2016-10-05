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
    public class Ql : Cs
    {
        public Ql(ConnectParam param) : base(param, "baseq3") { GameName = "ql"; }

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

        protected override GameResult On(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} start -servicename {GameName}{GameServerId} -gameport {param.GamePort} " +
                      $"-rconport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            result.Data = res.Result;
            return result;
        }
        protected override GameResult Restart(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} restart -servicename {GameName}{GameServerId} -gameport {param.GamePort} " +
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
