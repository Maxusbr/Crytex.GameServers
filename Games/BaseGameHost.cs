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
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class BaseGameHost : IGameHost
    {
        protected StreamWriter Writer { get; set; }
        protected readonly SshClient Client;
        protected ShellStream Terminal;
        protected readonly string Ip;
        protected readonly string Password;
        protected string GameName;
        protected int UserId;
        public BaseGameHost(ConnectParam param)
        {
            GameName = param.GameName;
            Password = param.SshPassword;
            Ip = param.SshIp;
            Client = new SshClient(param.SshIp, param.SshPort, param.SshUserName, param.SshPassword);
            Client.Connect();
        }

        public virtual DataReceivedModel Go(GameHostParam param)
        {
            UserId = param.UserId;
            return new DataReceivedModel();
        }

        public virtual DataReceivedModel On(GameHostParam param)
        {
            if (UserId == 0) UserId = param.UserId;
            return new DataReceivedModel();
        }

        public virtual void Off(GameHostParam param)
        {
            if (UserId == 0) UserId = param.UserId;
            IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
            Terminal = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
            Terminal.DataReceived += Stream_DataReceived;
            Writer = new StreamWriter(Terminal) { AutoFlush = true };
        }

        public virtual DataReceivedModel Monitor(GameHostParam param) { if (UserId == 0) UserId = param.UserId; return new DataReceivedModel(); }
        public virtual void OpenConsole(GameHostParam param)
        {
            if(UserId == 0) UserId = param.UserId;
            IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
            Terminal = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
            Terminal.DataReceived += Stream_DataReceived;
            Writer = new StreamWriter(Terminal) { AutoFlush = true };
        }

        public virtual string CloseConsole(GameHostParam param)
        {
            var run = $"^b d";
            if (Terminal == null || Writer == StreamWriter.Null) return "";
            Writer?.WriteLine(run);
            Terminal.DataReceived -= Stream_DataReceived;
            Writer?.Close(); Writer?.Dispose(); Writer = StreamWriter.Null;
            Terminal?.Close(); Terminal?.Dispose();
            Terminal = null;
            return "";
            //return !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
        }

        public virtual string SendConsoleCommand(string command, bool waitAll = false)
        {
            Writer?.WriteLine(command);
            return "";
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

        protected void Stream_DataReceived(object sender, Renci.SshNet.Common.ShellDataEventArgs e)
        {
            var res = new DataReceivedModel {Data = EscapeUtf8(Encoding.UTF8.GetString(e.Data))};
            OnDataReceived(res);
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

        public void Dispose()
        {
            Writer?.Close(); Writer?.Dispose();
            if (Terminal != null)
                Terminal.DataReceived -= Stream_DataReceived;
            Terminal?.Close(); Terminal?.Dispose();
            Client?.Disconnect();
            //Client?.Dispose();
        }

    }
}
