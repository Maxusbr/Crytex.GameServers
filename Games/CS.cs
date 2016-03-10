using System;
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

        public Cs(ConnectParam param) : base(param) { }

        public override DataReceivedModel Go(GameHostParam param)
        {
            var resModel = base.Go(param);
            var run = $"cd /host/{GameName}/serverfiles/cstrike/cfg;cp -r cs-server.cfg s{UserId}.cfg";
            var res = Client.RunCommand(run);
            resModel.Data = !string.IsNullOrEmpty(res.Error) ? EscapeUtf8(res.Error) : EscapeUtf8(res.Result);
            return resModel;
        }

        public override DataReceivedModel On(GameHostParam param)
        {
            var resModel = base.On(param);
            var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
                      $"./{GameName} start -servicename cs{UserId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            //Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? EscapeUtf8(res.Error) : EscapeUtf8(res.Result));
            if (!string.IsNullOrEmpty(res.Error)) resModel.Data = EscapeUtf8(res.Error);
            resModel.ServerStates = Regex.Matches(EscapeUtf8(res.Result),
                @"\r\[\s*(?<value>\w+)\s*\][^\r]*Starting[^\:\r]+:\s*(?<name>[\w\s]+)[^\r]*\n")
                .Cast<Match>()
                .Select(m => new ServerStateModel
                {
                    ParameterName = m.Groups["name"].Value,
                    ParameterValue = m.Groups["value"].Value
                }).ToList();
            return resModel;
        }

        public override void Off(GameHostParam param)
        {
            base.Off(param);
            var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
                      $"./{GameName} stop -servicename cs{UserId} -port {param.GamePort};";
            //var res = Client.RunCommand(run);
            //Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? EscapeUtf8(res.Error) : EscapeUtf8(res.Result));
            Writer.WriteLine(run);
        }

        public override DataReceivedModel Monitor(GameHostParam param)
        {
            var resModel = base.Monitor(param);
            var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
                         $"./{GameName} monitor -servicename cs{UserId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error))
            {
                resModel.Data = EscapeUtf8(res.Error);
                return resModel;
            }
            resModel.ServerStates = Regex.Matches(EscapeUtf8(res.Result),
                @"\r\[\s*(?<value>\w+)\s*\][^\r]*Monitor[^\:\r]+:\s*(?<name>[\w\s]+)[^\r]*\n")
                .Cast<Match>()
                .Select(m => new ServerStateModel
                {
                    ParameterName = m.Groups["name"].Value,
                    ParameterValue = m.Groups["value"].Value
                }).ToList();
            resModel.Data = !string.IsNullOrEmpty(res.Error) ? EscapeUtf8(res.Error) : "OK";
            return resModel;
        }

        public override void OpenConsole(GameHostParam param)
        {
            base.OpenConsole(param);
            var run = $"cd /host/{GameName};./{GameName} console -servicename cs{UserId} -port {param.GamePort};";
            var res = "";
            //run = $"tmux attach-session -t cs{param.UserId};echo cancelread";
            Writer.WriteLine(run);
            //res = Terminal.Expect(new Regex(@"[\[" + $"cs{param.UserId}" + @"\]]"), new TimeSpan(0, 0, 3));
        }

        public override string SendConsoleCommand(string command, bool waitAll = false)
        {
            _isWaitAll = waitAll;
            if (_isWaitAll)
            {
                _collectResiveString = string.Empty;
                _foundEnd = new Regex(@"[\d]+\s*users");
            }
            return base.SendConsoleCommand(command);
        }

        private bool _isWaitAll;
        private string _collectResiveString;
        private Regex _foundEnd;

        protected override void OnDataReceived(DataReceivedModel data)
        {
            var rg = new Regex(@"\[" + $"cs{UserId}" + @"\].+");
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
            if (_isWaitAll)
            {
                _collectResiveString += data.Data;
                if (_foundEnd.IsMatch(_collectResiveString)) CreateTableData();
            }
            else
                base.OnDataReceived(data);
        }

        private void CreateTableData()
        {
            var data = new DataReceivedModel();// {Data = _collectResiveString};
            var rg = new Regex(@"(?<name>[\w\/]+)\s*:\s*(?<value>[^\n]+)");
            if (rg.IsMatch(_collectResiveString))
            {
                data.ServerStates = rg.Matches(_collectResiveString)
                .Cast<Match>()
                .Select(m => new ServerStateModel
                {
                    ParameterName = m.Groups["name"].Value,
                    ParameterValue = m.Groups["value"].Value
                }).ToList();
            }
            if (data.ServerStates == null) data.ServerStates = new List<ServerStateModel>();
            data.ServerStates.Add(new ServerStateModel
            {
                ParameterName = "Status",
                ParameterValue = "Ready"
            });
            rg = new Regex(@"#(?<name>[^\n\r]+)");
            var listTable = rg.Matches(_collectResiveString)
                .Cast<Match>()
                .Select(m => m.Groups["name"].Value).ToList();

            data.TableInfo = new TableDataModel {Values = new List<string>(), Headers = new List<string>{"Number"} };
            if (listTable.Any())
                for (var i = 0; i < listTable.Count; i++)
                {
                    if (i == 0) data.TableInfo.Headers.AddRange(listTable[i].Split(' ').Where(o => !o.Equals(string.Empty)));
                    else data.TableInfo.Values.AddRange(listTable[i].Split(' ').Where(o => !o.Equals(string.Empty)));
                }

            CloseConsole(null);
            _collectResiveString = string.Empty;
            _isWaitAll = false;
            base.OnDataReceived(data);
        }
    }
}