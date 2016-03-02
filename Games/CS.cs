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
    public class Cs : BaseGameHost
    {
        
        public Cs(ConnectParam param) : base(param) { }

        public override void Go(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName}/serverfiles/cstrike/cfg;cp -r cs-server.cfg s{userId}.cfg";
            var res = Client.RunCommand(run);
            Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
        }

        public override void On(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
                      $"./{GameName} start -servicename cs{userId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
        }

        public override void Off(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
                      $"./{GameName} stop -servicename cs{userId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
        }

        public override string Monitor(GameHostParam param)
        {
            var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
                         $"./{GameName} monitor -servicename cs{param.UserId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            return !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
        }

        public override string OpenConsole(GameHostParam param)
        {
            var run = $"cd /host/{GameName};" +//screen -dmS server_start_cs{userId} " +
                         $"./{GameName} console -servicename cs{param.UserId} -port {param.GamePort};";
            var res = "";
            run = $"tmux attach-session -t cs{param.UserId}";
            //run = $"bind -n C-k send-keys -R ; clear-history";
            IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

            Terminal = Client.CreateShellStream("xterm", 120, 24, 800, 600, 1024, termkvp);
            {
                var reader = new StreamReader(Terminal);
                Writer = new StreamWriter(Terminal) {AutoFlush = true};
                //var shell = Client.CreateShell(stream, stream, stream);
                //shell.Start();

                Terminal.DataReceived += Stream_DataReceived;
                Thread.Sleep(500);
                //res = reader.ReadToEnd();

                Writer.WriteLine(run);
                Thread.Sleep(1000);

                //res = reader.ReadToEnd();

                //if (res.Contains("FAIL")) return "fail open console";
                //Console.WriteLine(res);
                //WriteStream("status", writer, stream);

                //Thread.Sleep(1000);
                //res = ReadStream(reader);
            }
            //var res = sr.ReadToEnd();
            return res;
        }

        private void Stream_DataReceived(object sender, Renci.SshNet.Common.ShellDataEventArgs e)
        {
            var res = Encoding.UTF8.GetString(e.Data);

            Console.Write(res);
        }

        public override string CloseConsole(GameHostParam param)
        {
            var run = $"echo ^b d";
            var res = Client.RunCommand(run);
            return !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
        }

        public override string SendConsoleCommand(GameHostParam param) { return ""; }
    }
}
;