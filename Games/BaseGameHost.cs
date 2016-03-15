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
using Crytex.Model.Enums;
using Crytex.Model.Exceptions;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class BaseGameHost : IGameHost
    {
        protected readonly string GameCode;
        protected StreamWriter Writer;
        protected StreamReader Reader;
        protected readonly SshClient Client;
        protected ShellStream Terminal;
        protected readonly string Ip;
        protected readonly string Password;
        protected string GameName;
        protected int UserId;
        protected bool IsWaitAll;
        protected string CollectResiveString;
        protected Regex FoundConsoleEnd;
        private bool _isReadyConsoleCommand;

        public BaseGameHost(ConnectParam param, string gameCode)
        {
            GameCode = gameCode;
            GameName = param.GameName;
            Password = param.SshPassword;
            Ip = param.SshIp;
            Client = new SshClient(param.SshIp, param.SshPort, param.SshUserName, param.SshPassword);
        }

        public GameResult Connect()
        {
            Client.Connect();
            var result = new GameResult { Succes = Client.IsConnected };
            return result;
        }

        public GameResult Disconnect()
        {
            Dispose();
            Client?.Disconnect();
            var result = new GameResult { Succes = !Client?.IsConnected ?? true };
            return result;
        }

        public virtual GameResult Create(CreateParam param)
        {
            UserId = param.UserId;
            var run = $"cd /host/{GameName}/serverfiles/{GameCode}/cfg;cp -r {GameName}-server.cfg {GameName}{UserId}.cfg";
            var res = Client.RunCommand(run);
            var result = new GameResult();
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            return result;
        }


        public GameResult ChangeStatus(ChangeStatusParam param)
        {
            GameResult result = null;
            switch (param.TypeStatus)
            {
                case GameHostTypeStatus.Enable:
                    result = On(param);
                    break;
                case GameHostTypeStatus.Disable:
                    result = Off(param);
                    break;
            }
            return result;
        }

        protected virtual GameResult On(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd /host/{GameName};" +
                      $"./{GameName} start -servicename {GameName}{UserId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            return result;
        }

        protected virtual GameResult Off(ChangeStatusParam param)
        {
            var result = new GameResult();
            //IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            //termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
            //Terminal = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
            //Terminal.DataReceived += Stream_DataReceived;
            //Writer = new StreamWriter(Terminal) { AutoFlush = true };
            var run = $"cd /host/{GameName};" +
                      $"./{GameName} stop -servicename cs{UserId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            //Writer.WriteLine(run);
            return result;
        }

        public virtual StateGameResult GetState(UserGameParam userGameParam)
        {
            var result = new StateGameResult();
            var run = $"cd /host/{GameName};" +
                         $"./{GameName} monitor -servicename {GameName}{UserId} -port {userGameParam.GamePort};";
            var res = Client.RunCommand(run);
            var states = Regex.Matches(EscapeUtf8(res.Result),
                @"\r\[\s*(?<value>\w+)\s*\][^\r]*Monitor[^\:\r]+:\s*(?<name>[\w\s]+)[^\r]*\n")
                .Cast<Match>()
                .Select(m => new ServerStateModel
                {
                    ParameterName = m.Groups["name"].Value,
                    ParameterValue = m.Groups["value"].Value
                }).ToList();
            if (!states.All(o => o.ParameterValue.Equals("OK")))
            {
                foreach (var st in states)
                    result.ErrorMessage += $"{st.ParameterName}: {st.ParameterValue}\n";
                result.Status = GameHostTypeStatus.Disable;
                return result;
            }
            result.Status = GameHostTypeStatus.Enable;
            return result;
        }

        public virtual AdvancedStateGameResult GetAdvancedState(UserGameParam userGameParam)
        {
            OpenConsole(userGameParam);
            CollectResiveString = string.Empty;
            
            var result = new AdvancedStateGameResult {Succes = false};
            return result;
        }

        public virtual void OpenConsole(UserGameParam param, string openCommand = "")
        {
            IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
            Terminal = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
            Terminal.DataReceived += Stream_DataReceived;
            Writer = new StreamWriter(Terminal) { AutoFlush = true };
            if(string.IsNullOrEmpty(openCommand))
                openCommand = $"cd /host/{GameName};./{GameName} console -servicename {GameName}{UserId} -port {param.GamePort};";
            Writer.WriteLine(openCommand);
            IsWaitAll = true; _isReadyConsoleCommand = false;
            CollectResiveString = string.Empty;
            ReadyConsoleAnswer();
            
        }

        private bool ReadyConsoleAnswer()
        {
            var count = 0;
            while (IsWaitAll && !_isReadyConsoleCommand)
            {
                Thread.Sleep(500);
                if (count++ > 5) return false;
            }
            return true;
        }

        public virtual string CloseConsole(UserGameParam param, string closeCommand = "")
        {
            var run = $"^b d";
            if (Terminal == null || Writer == StreamWriter.Null) return "";
            Writer?.WriteLine(run);
            Dispose();
            return CollectResiveString;
        }

        public virtual string SendConsoleCommand(string command, bool waitAll = false)
        {
            IsWaitAll = waitAll;
            _isReadyConsoleCommand = false;
            Writer?.WriteLine(command);

            var result = new GameResult();
            if (!ReadyConsoleAnswer())
            {
                result.Error = GameHostTypeError.WaitTimeOut;
                return "ErrorWait";
            }

            return CollectResiveString;
        }

        protected void Stream_DataReceived(object sender, Renci.SshNet.Common.ShellDataEventArgs e)
        {
            CollectResiveString += EscapeUtf8(Encoding.UTF8.GetString(e.Data));
            if (FoundConsoleEnd?.IsMatch(CollectResiveString) ?? true)
                _isReadyConsoleCommand = true;
        }

        protected void ValidateError(SshCommand res, GameResult result)
        {
            result.Succes = false;
            result.ErrorMessage = EscapeUtf8(res.Error);
        }

        public event EventHandler<DataReceivedModel> DataReceived;

        protected string GeneratePassword(int count)
        {
            var rand = new Random();
            var res = string.Empty;
            for (var i = 0; i < count; i++)
                res += rand.Next(9);

            return res;
        }

        private void Dispose()
        {
            FoundConsoleEnd = null;
            Terminal.DataReceived -= Stream_DataReceived;
            Writer?.Close(); Writer?.Dispose(); Writer = StreamWriter.Null;
            Terminal?.Close(); Terminal?.Dispose();
            Terminal = null;
        }

        protected virtual void OnDataReceived(DataReceivedModel data)
        {
            DataReceived?.Invoke(this, data);
        }

        protected string EscapeUtf8(string data)
        {
            var reg = new Regex(@"\u001b[\[\]\>\(]+(([\dA?;]+[JHcmrdhl])|[mcKBH]|[\d]*)");
            var res = reg.Replace(data, "");
            return res;
        }
    }
}
