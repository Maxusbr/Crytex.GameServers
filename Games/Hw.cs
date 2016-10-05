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
    public class Hw : BaseGameHost
    {
        
        public Hw(ConnectParam param) : base(param, "hw") { GameName = "hw"; }

        public override GameResult Create(CreateParam param)
        {
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            var result = new GameResult();
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
                      $"./{GameName} start -servicename {GameName}{GameServerId} -port {param.GamePort} " +
                      $"-queryport {param.GamePort + 1}";
            if (!string.IsNullOrEmpty(ServerName))
                run += $" -servername {ServerName}";
            if (MaxPlayers > 0)
                run += $" -maxplayers {MaxPlayers}";
            run += ";";
            Command.Execute(run);
            if (!string.IsNullOrEmpty(Command.Error))
            {
                ValidateError(Command, result);
            }
            result.Data = Command.Result;
            return result;
        }
        protected override GameResult Restart(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} restart -servicename {GameName}{GameServerId} -port {param.GamePort} " +
                      $"-queryport {param.GamePort + 1}";
            if (!string.IsNullOrEmpty(ServerName))
                run += $" -servername {ServerName}";
            if (MaxPlayers > 0)
                run += $" -maxplayers {MaxPlayers}";
            run += ";";
            Command.Execute(run);
            if (!string.IsNullOrEmpty(Command.Error))
            {
                ValidateError(Command, result);
            }
            result.Data = Command.Result;
            return result;
        }
        public override bool CompleteInstal()
        {
            return true;
        }
    }
}
