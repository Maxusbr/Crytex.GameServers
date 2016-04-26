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
    public class CsGo : Cs
    {
        public CsGo(ConnectParam param) : base(param) { GameName = "csgo"; }

        //https://developer.valvesoftware.com/wiki/Command_Line_Options#Source_Dedicated_Server
        protected override GameResult On(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} start -servicename {GameName}{GameServerId} -port {param.GamePort} " +
                      $"-clientport {param.GamePort + 1} -sourcetvport {param.GamePort + 2}";
            if (!string.IsNullOrEmpty(DefaultMap))
                run += $" -defaultmap {DefaultMap}";
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
    }
}
