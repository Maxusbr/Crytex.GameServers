using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class BaseGameHost : IGameHost
    {
        public StreamWriter Writer { get; set; }
        protected readonly SshClient Client;
        protected ShellStream Terminal;
        protected readonly string Ip;
        protected readonly string Password;
        protected string GameName;
        public BaseGameHost(ConnectParam param)
        {
            GameName = param.GameName;
            Password = param.SshPassword;
            Ip = param.SshIp;
            Client = new SshClient(param.SshIp, param.SshPort, param.SshUserName, param.SshPassword);
            Client.Connect();
        }

        public virtual void Go(GameHostParam param)
        {
            var userId = param.UserId;
            /*
            $ssh->exec_cmd('cd /host/;mkdir '.$user.';');
            $ssh->exec_cmd('cd /;groupadd -g '.$id.' s'.$id.';useradd -u '.$id.' -d /host/'.$user.'/'.$id.' -g s'.$id.' s'.$id.';');
            $ssh->exec_cmd('cd /host/'.$user.'/;rm -R '.$id.';');
            $ssh->exec_cmd('cd /host/;screen -dmS install_'.$id.' cp -rv '.decod($rate['p14']).' '.$user.'/'.$id.';');
            */
            var run = $"cd /host/;mkdir s{userId};";
            var res = Client.RunCommand(run);
            run = $"echo {Password} | sudo -S groupadd -g {userId} s{userId};";
            res = Client.RunCommand(run);
            run = $"echo {Password} | sudo -S useradd -u {userId} -d /host/s{userId} -g s{userId} -p s{userId} s{userId};";
            res = Client.RunCommand(run);
            run = $"cd /host/s{userId}/;rm -R {GameName};";
            res = Client.RunCommand(run);
            run = $"cd /host/;screen -dmS install_{GameName} cp -r {GameName} s{userId}/{GameName};";
            res = Client.RunCommand(run);
        }

        public virtual void On(GameHostParam param) { }

        public virtual void Off(GameHostParam param) { }

        public virtual string Monitor(GameHostParam param) { return ""; }
        public virtual string OpenConsole(GameHostParam param) { return ""; }
        public virtual string CloseConsole(GameHostParam param) { return ""; }

        public virtual string SendConsoleCommand(GameHostParam param) { return ""; }

        protected string GeneratePassword(int count)
        {
            var rand = new Random();
            var res = string.Empty;
            for (var i = 0; i < count; i++)
                res += rand.Next(9);

            return res;
        }

        protected string ReadStream(StreamReader reader)
        {
            var res = "";
            var line = reader.ReadLine();
            while (line != null)
            {
                res += line + "\n";
                line = reader.ReadLine();
            }
            return res;
        }

        protected void WriteStream(string cmd, StreamWriter writer, ShellStream stream)
        {
            writer.WriteLine(cmd);
            while (stream.Length == 0)
            {
                Thread.Sleep(500);
            }
        }

        public void Dispose()
        {
            Terminal?.Dispose();
            Client?.Disconnect();
            Client?.Dispose();
        }
    }
}
