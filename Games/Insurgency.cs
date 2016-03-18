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
    public class Insurgency : Cs
    {
        public Insurgency(ConnectParam param) : base(param, "insurgency") { }

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

        protected override void FoundEndConsoleCommand(string command)
        {
            FoundConsoleEnd = command.Equals("status") ? new Regex("#end") : null;
        }

        protected override AdvancedStateGameResult CreateTableData(AdvancedStateGameResult data)
        {
            var rg = new Regex(@"(?<name>[\w\/]+)\s*:\s*(?<value>[^\n]+)");
            if (rg.IsMatch(CollectResiveString))
            {
                data.ServerStates = rg.Matches(CollectResiveString)
                .Cast<Match>()
                .Select(m => new ServerStateModel
                {
                    ParameterName = m.Groups["name"].Value,
                    ParameterValue = m.Groups["value"].Value
                }).ToList();
            }

            rg = new Regex(@"#(?<name>[^\n\r]+)");
            var listTable = rg.Matches(CollectResiveString)
                .Cast<Match>()
                .Select(m => m.Groups["name"].Value).ToList();

            data.TableInfo = new TableDataModel { Values = new List<string>(), Headers = new List<string> { "Number" } };
            if (!listTable.Any()) return data;
            for (var i = 0; i < listTable.Count; i++)
            {
                if(listTable[i].Equals("end")) continue;
                if (i == 0) data.TableInfo.Headers.AddRange(listTable[i].Split(' ').Where(o => !o.Equals(string.Empty)));
                else data.TableInfo.Values.AddRange(listTable[i].Split(' ').Where(o => !o.Equals(string.Empty)));
            }
            return data;
        }
    }
}
