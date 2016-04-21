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
using Renci.SshNet.Common;

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
        protected string Path;
        protected string GameServerId;
        protected bool IsWaitAll;
        protected string CollectResiveString;
        protected Regex FoundConsoleEnd;
        protected SshCommand Command;

        public bool IsCompleteInstall => CompleteInstal();

        public BaseGameHost(ConnectParam param, string gameCode)
        {
            Path = param.Path;
            GameCode = gameCode;
            if (!string.IsNullOrEmpty(param.GameName)) GameName = param.GameName;
            Password = param.SshPassword;
            Ip = param.SshIp;
            Client = new SshClient(param.SshIp, param.SshPort, param.SshUserName, param.SshPassword);
        }

        public GameResult Connect()
        {
            Client.Connect();
            var result = new GameResult { Succes = Client.IsConnected };
            if(Client.IsConnected) Command = Client.CreateCommand("");
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
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            var result = new GameResult();
            var run = $"cd {Path}/{GameName}/serverfiles/{GameCode}/cfg;cp -r {GameName}-server.cfg {GameName}{GameServerId}.cfg";
            Command.Execute(run);
            
            if (!string.IsNullOrEmpty(Command.Error))
            {
                ValidateError(Command, result);
            }
            return result;
        }

        public virtual bool CompleteInstal()
        {
            var run = $"cd {Path}/{GameName}/serverfiles/{GameCode}/cfg;find {GameName}{GameServerId}.cfg";
            Command.Execute(run);
            return string.IsNullOrEmpty(Command.Error);
        }

        public GameResult ChangeStatus(ChangeStatusParam param)
        {
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            GameResult result = null;
            if (!CompleteInstal()) Create(new CreateParam {GamePort = param.GamePort});
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
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} start -servicename {GameName}{GameServerId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            Command.Execute(run);
            if (!string.IsNullOrEmpty(Command.Error))
            {
                ValidateError(Command, result);
            }
            result.Data = Command.Result;
            return result;
        }

        protected virtual GameResult Off(ChangeStatusParam param)
        {
            var result = new GameResult();
            //IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            //termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
            //Terminal = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
            //Writer = new StreamWriter(Terminal) { AutoFlush = true };
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} stop -servicename {GameName}{GameServerId} -port {param.GamePort};";
            Command.Execute(run);
            if (!string.IsNullOrEmpty(Command.Error))
            {
                ValidateError(Command, result);
            }
            //Writer.WriteLine(run);
            result.Data = Command.Result;
            return result;
        }

        public virtual StateGameResult GetState(UserGameParam userGameParam)
        {
            if (string.IsNullOrEmpty(GameServerId)) GameServerId = userGameParam.GameServerId;
            var result = new StateGameResult();
            var run = $"cd {Path}/{GameName};" +
                         $"./{GameName} monitor -servicename {GameName}{GameServerId} -port {userGameParam.GamePort};";
            Command.Execute(run);
            var states = Regex.Matches(EscapeUtf8(Command.Result),
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

            var result = new AdvancedStateGameResult { Succes = false };
            return result;
        }

        public virtual bool OpenConsole(UserGameParam param, string openCommand = "")
        {
            if (string.IsNullOrEmpty(GameServerId)) GameServerId = param.GameServerId;
            IDictionary<TerminalModes, uint> termkvp = new Dictionary<TerminalModes, uint>();
            termkvp.Add(TerminalModes.ECHO, 53);
            Terminal = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);

            var tsc = new TaskCompletionSource<bool>();
            EventHandler<ShellDataEventArgs> lambda = (obj, args) =>
            {
                CollectResiveString += EscapeUtf8(Encoding.UTF8.GetString(args.Data));
                if (FoundConsoleEnd?.IsMatch(CollectResiveString) ?? true)
                    tsc.SetResult(true);
            };
            Terminal.DataReceived += lambda;
            Terminal.DataReceived += Terminal_DataReceived;
            Writer = new StreamWriter(Terminal) { AutoFlush = true };
            if (string.IsNullOrEmpty(openCommand))
                openCommand = $"cd {Path}/{GameName};./{GameName} console -servicename {GameName}{GameServerId} -port {param.GamePort};";
            try
            {
                Writer.WriteLine(openCommand);
                var result = tsc.Task == Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(10)), tsc.Task).Result;
            }
            catch (Exception e)
            {
                Terminal.DataReceived -= lambda;
                return false;
            }

            Terminal.DataReceived -= lambda;
            return true;
        }

        private void Terminal_DataReceived(object sender, ShellDataEventArgs e)
        {
            ConsoleDataReceived?.Invoke(sender, EscapeUtf8(Encoding.UTF8.GetString(e.Data)));
        }

        public virtual string CloseConsole(UserGameParam param, string closeCommand = "")
        {
            var run = $"^b d";
            if (Terminal == null || Writer == StreamWriter.Null) return "";
            Writer?.WriteLine(run);
            FoundConsoleEnd = null;
            Terminal.DataReceived += Terminal_DataReceived;
            Writer?.Close(); Writer?.Dispose(); Writer = StreamWriter.Null;
            Terminal?.Close(); Terminal?.Dispose();
            return CollectResiveString;
        }

        public virtual string SendConsoleCommand(string command, bool waitAll = false)
        {
            IsWaitAll = waitAll;
            var tsc = new TaskCompletionSource<bool>();
            EventHandler<ShellDataEventArgs> lambda = (obj, args) =>
            {
                CollectResiveString += EscapeUtf8(Encoding.UTF8.GetString(args.Data));
                if (FoundConsoleEnd?.IsMatch(CollectResiveString) ?? true)
                    tsc.SetResult(true);
            };
            Terminal.DataReceived += lambda;
            Writer?.WriteLine(command);

            var result = tsc.Task != Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(10)), tsc.Task).Result ? "ErrorWait" : CollectResiveString;
            Terminal.DataReceived -= lambda;
            return result;
        }

        public event EventHandler<string> ConsoleDataReceived;

        protected void ValidateError(SshCommand res, GameResult result)
        {
            result.Succes = false;
            result.ErrorMessage = EscapeUtf8(res.Error);
        }

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
            Command.Dispose();
            FoundConsoleEnd = null;
            Writer?.Close(); Writer?.Dispose(); Writer = StreamWriter.Null;
            Terminal?.Close(); Terminal?.Dispose();
            Terminal = null;
        }

        protected string EscapeUtf8(string data)
        {
            var reg = new Regex(@"\u001b[\[\]\>\(]+(([\dA?;]+[JHcmrdhl])|[mcKBH]|[\d]*)");
            var res = reg.Replace(data, "");
            return res;
        }
    }
}
