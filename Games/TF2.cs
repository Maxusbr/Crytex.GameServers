﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Crytex.Model.Enums;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class TF2 : Cs
    {
        public TF2(ConnectParam param) : base(param, "tf") { GameName = "tf2"; }

        public override bool OpenConsole(UserGameParam param, string openCommand = "")
        {
            FoundConsoleEnd = new Regex(@"\[" + $"{GameName}{GameServerId}" + @"\].+");
            return base.OpenConsole(param);
        }

        protected override void FoundEndConsoleCommand(string command)
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

        protected AdvancedStateGameResult CreateTableData(AdvancedStateGameResult data)
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
    }
}
