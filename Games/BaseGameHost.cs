using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class BaseGameHost : IGameHost
    {
        protected readonly SshClient Client;
        protected readonly string Ip;
        protected readonly string Password;
        protected string GameName;
        public BaseGameHost(ConnectParam param)
        {
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

        protected string GeneratePassword(int count)
        {
            var rand = new Random();
            var res = string.Empty;
            for (var i = 0; i < count; i++)
                res += rand.Next(9);

            return res;
        }

        public void Dispose()
        {
            Client?.Disconnect();
            Client?.Dispose();
        }
    }
}
