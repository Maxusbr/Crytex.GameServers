﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Microsoft.Practices.ObjectBuilder2;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class Cs : BaseGameHost
    {

        public Cs(ConnectParam param) : base(param, "cstrike") { FoundConsoleEnd = new Regex(@"[\d]+\s*users"); }

        //public override DataReceivedModel Go(GameHostParam param)
        //{
        //    var resModel = base.Go(param);
        //    var run = $"cd /host/{GameName}/serverfiles/cstrike/cfg;cp -r cs-server.cfg s{UserId}.cfg";
        //    var res = Client.RunCommand(run);
        //    resModel.Data = !string.IsNullOrEmpty(res.Error) ? EscapeUtf8(res.Error) : EscapeUtf8(res.Result);
        //    return resModel;
        //}

        //public override DataReceivedModel On(GameHostParam param)
        //{
        //    var resModel = base.On(param);
        //    var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
        //              $"./{GameName} start -servicename cs{UserId} -port {param.GamePort} -clientport {param.GamePort + 1};";
        //    var res = Client.RunCommand(run);
        //    //Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? EscapeUtf8(res.Error) : EscapeUtf8(res.Result));
        //    if (!string.IsNullOrEmpty(res.Error)) resModel.Data = EscapeUtf8(res.Error);
        //    resModel.ServerStates = Regex.Matches(EscapeUtf8(res.Result),
        //        @"\r\[\s*(?<value>\w+)\s*\][^\r]*Starting[^\:\r]+:\s*(?<name>[\w\s]+)[^\r]*\n")
        //        .Cast<Match>()
        //        .Select(m => new ServerStateModel
        //        {
        //            ParameterName = m.Groups["name"].Value,
        //            ParameterValue = m.Groups["value"].Value
        //        }).ToList();
        //    return resModel;
        //}

        //public override void Off(GameHostParam param)
        //{
        //    base.Off(param);
        //    var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
        //              $"./{GameName} stop -servicename cs{UserId} -port {param.GamePort};";
        //    //var res = Client.RunCommand(run);
        //    //Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? EscapeUtf8(res.Error) : EscapeUtf8(res.Result));
        //    Writer.WriteLine(run);
        //}

        //public override DataReceivedModel Monitor(GameHostParam param)
        //{
        //    var resModel = base.Monitor(param);
        //    var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
        //                 $"./{GameName} monitor -servicename cs{UserId} -port {param.GamePort};";
        //    var res = Client.RunCommand(run);
        //    if (!string.IsNullOrEmpty(res.Error))
        //    {
        //        resModel.Data = EscapeUtf8(res.Error);
        //        return resModel;
        //    }
        //    resModel.ServerStates = Regex.Matches(EscapeUtf8(res.Result),
        //        @"\r\[\s*(?<value>\w+)\s*\][^\r]*Monitor[^\:\r]+:\s*(?<name>[\w\s]+)[^\r]*\n")
        //        .Cast<Match>()
        //        .Select(m => new ServerStateModel
        //        {
        //            ParameterName = m.Groups["name"].Value,
        //            ParameterValue = m.Groups["value"].Value
        //        }).ToList();
        //    resModel.Data = !string.IsNullOrEmpty(res.Error) ? EscapeUtf8(res.Error) : "OK";
        //    return resModel;
        //}

        public override void OpenConsole(UserGameParam param, string openCommand = "")
        {
            FoundConsoleEnd = new Regex(@"\[" + $"{GameName}{UserId}" + @"\].+");
            var run = $"cd /host/{GameName};./{GameName} console -servicename {GameName}{UserId} -port {param.GamePort};";
            base.OpenConsole(param);
        }

        public override string SendConsoleCommand(string command, bool waitAll = false)
        {
            FoundConsoleEnd = command.Equals("status") ? new Regex(@"[\w]+\s*users") : null;
            return base.SendConsoleCommand(command, waitAll);
        }

        public override AdvancedStateGameResult GetAdvancedState(UserGameParam userGameParam)
        {
            var result = base.GetAdvancedState(userGameParam);
            CollectResiveString = string.Empty;
            result.ErrorMessage = SendConsoleCommand("status", true);
            CloseConsole(userGameParam);
            CreateTableData(result);
            
            return result;
        }

        private void CreateTableData(AdvancedStateGameResult data)
        {
            //var data = new DataReceivedModel();// {Data = _collectResiveString};
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
            //if (data.ServerStates == null) data.ServerStates = new List<ServerStateModel>();
            //data.ServerStates.Add(new ServerStateModel
            //{
            //    ParameterName = "Status",
            //    ParameterValue = "Ready"
            //});

            rg = new Regex(@"#(?<name>[^\n\r]+)");
            var listTable = rg.Matches(CollectResiveString)
                .Cast<Match>()
                .Select(m => m.Groups["name"].Value).ToList();

            data.TableInfo = new TableDataModel { Values = new List<string>(), Headers = new List<string> { "Number" } };
            if (!listTable.Any()) return;
            for (var i = 0; i < listTable.Count; i++)
            {
                if (i == 0) data.TableInfo.Headers.AddRange(listTable[i].Split(' ').Where(o => !o.Equals(string.Empty)));
                else data.TableInfo.Values.AddRange(listTable[i].Split(' ').Where(o => !o.Equals(string.Empty)));
            }
        }

        protected override void OnDataReceived(DataReceivedModel data)
        {
            var rg = new Regex(@"\[" + $"{GameName}{UserId}" + @"\].+");
            if (rg.IsMatch(data.Data))
            {
                data.ServerStates = new List<ServerStateModel>{ new ServerStateModel
                {
                    ParameterValue = "Ready",
                    ParameterName = "Console"
                }};
                data.Data = rg.Replace(data.Data, "#>");
            }
            //data = EscapeUtf8(data);
            rg = new Regex(@"\r\[\s*(?<value>\w+)\s*\]\s*Stopping[^:]+:\s*csserver");
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

        


    }
}