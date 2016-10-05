using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Enums;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Crytex.Model.Enums;
using Crytex.Model.Exceptions;
using Microsoft.Practices.ObjectBuilder2;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class Cs : BaseGameHost
    {

        public Cs(ConnectParam param, string gameCode =  "cstrike") : base(param, gameCode) { GameName = "cs"; }

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
        protected override GameResult Restart(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} restart -servicename {GameName}{GameServerId} -port {param.GamePort} " +
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
        public override bool OpenConsole(UserGameParam param, string openCommand = "")
        {
            FoundConsoleEnd = new Regex(@"\[" + $"{GameName}{GameServerId}" + @"\].+");
            return base.OpenConsole(param);
        }

        protected virtual void FoundEndConsoleCommand(string command)
        {
            FoundConsoleEnd = command.Equals("status") ? new Regex(@"[\w]+\s*users") : null;
        }

        public override string SendConsoleCommand(string command, bool waitAll = false)
        {
            FoundEndConsoleCommand(command);
            CollectResiveString = string.Empty;
            return base.SendConsoleCommand(command, waitAll);
        }

        public override AdvancedStateGameResult GetAdvancedState(UserGameParam userGameParam)
        {
            var result = base.GetAdvancedState(userGameParam);
            SendConsoleCommand("status", true);
            CloseConsole(userGameParam);
            result = CreateTableData(result);
            result.Status = result.ServerStates.Any() ? GameHostTypeStatus.Enable : GameHostTypeStatus.Disable;
            return result;
        }

        protected virtual AdvancedStateGameResult CreateTableData(AdvancedStateGameResult data)
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
                if (i == 0) data.TableInfo.Headers.AddRange(listTable[i].Split(' ').Where(o => !o.Equals(string.Empty)));
                else data.TableInfo.Values.AddRange(listTable[i].Split(' ').Where(o => !o.Equals(string.Empty)));
            }
            return data;
        }

        protected override void CreateDefaultConfigParams()
        {
            ConfigParam.Add(new ConfigParam { Name = "hostname", Value = "Counter-Strike 1.6 Server", DefaultValue = "Counter-Strike 1.6 Server", Type = ConfigParamType.String});
            ConfigParam.Add(new ConfigParam { Name = "rcon_password", Value = "csrcon_password", DefaultValue = "csrcon_password", Type = ConfigParamType.String});
            ConfigParam.Add(new ConfigParam { Name = "sv_password", Value = "csserver_password", DefaultValue = "csserver_password", Type = ConfigParamType.String});
            ConfigParam.Add(new ConfigParam { Name = "mp_timelimit", Value = 20, DefaultValue = 20, Type = ConfigParamType.Int});
            //ConfigParam.Add(new ConfigParam { Name = "mapcyclefile", Value = 20, DefaultValue = 20, Type = ConfigParamType.Array});

        }

        public override GameResult SetConfigParams(ConfigParams configParams)
        {
            var result = base.SetConfigParams(configParams);

            var host = $"cd {Path}/{GameName}/serverfiles/{GameCode}/cfg;";
            Client.RunCommand(host + $"chmod 777 {GameName}{GameServerId}.cfg;");
            Client.RunCommand(host + $"echo  \"hostname \\\"{configParams.GetVal("hostname")}\\\"\" > {GameName}{GameServerId}.cfg;");
            Client.RunCommand(host + $"echo  \"rcon_password \\\"{configParams.GetVal("rcon_password")}\\\"\" >> {GameName}{GameServerId}.cfg;");
            Client.RunCommand(host + $"echo  \"sv_password \\\"{configParams.GetVal("sv_password")}\\\"\" >> {GameName}{GameServerId}.cfg;");
            Client.RunCommand(host + $"echo  \"log on\nsv_logbans 1\nsv_logecho 1\nsv_logfile 1\nsv_log_onefile 0\" >> {GameName}{GameServerId}.cfg;");
            Client.RunCommand(host + $"echo  \"sv_aim 0\npausable 0\nsv_maxspeed 320\" >> {GameName}{GameServerId}.cfg;");
            Client.RunCommand(host + $"echo  \"mp_timelimit {configParams.GetVal("mp_timelimit")}\" >> {GameName}{GameServerId}.cfg;");
            Client.RunCommand(host + $"echo  \"sv_cheats 0\nexec listip.cfg\nexec banned.cfg\" >> {GameName}{GameServerId}.cfg;");

            if (!CompleteInstal())
            {
                result.Error = GameHostTypeError.CantCreate;
                result.Succes = false;
                result.ErrorMessage = "Error сreate config file";
            }
            return result;
        }

        #region temporary

        private void OnServerOffReceived(DataReceivedModel data)
        {
            var rg = new Regex(@"\r\[\s*(?<value>\w+)\s*\]\s*Stopping[^:]+:\s*csserver");
            if (rg.IsMatch(data.Data))
            {
                data.ServerStates = rg.Matches(data.Data)
                    .Cast<Match>()
                    .Select(m => new ServerStateModel
                    {
                        ParameterName = "ServerOFF",
                        ParameterValue = m.Groups["value"].Value
                    }).ToList();
            }
        }

        #endregion
    }
}