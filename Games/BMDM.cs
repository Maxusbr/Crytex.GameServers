using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class Bmdm : Cs
    {
        public Bmdm(ConnectParam param) : base(param, "bms") {}

        protected override GameResult On(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} start -servicename {GameName}{UserId} -port {param.GamePort} " +
                      $"-clientport {param.GamePort + 1} -sourcetvport {param.GamePort + 2};";
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
