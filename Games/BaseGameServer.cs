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
    public class BaseGameServer : IGameServer
    {
        protected readonly SshClient Client;

        public BaseGameServer(ConnectParam param)
        {
            Client = new SshClient(param.SshIp, param.SshPort, param.SshUserName, param.SshPassword);
            Client.Connect();
        }

        public virtual void Go(ConnectParam param)
        {
            var rcon = GeneratePassword(10);
            var userId = param.UserId;
            var id = param.GameId;
            /*
            $ssh->exec_cmd('cd /host/;mkdir '.$user.';');
            $ssh->exec_cmd('cd /;groupadd -g '.$id.' s'.$id.';useradd -u '.$id.' -d /host/'.$user.'/'.$id.' -g s'.$id.' s'.$id.';');
            $ssh->exec_cmd('cd /host/'.$user.'/;rm -R '.$id.';');
            $ssh->exec_cmd('cd /host/;screen -dmS install_'.$id.' cp -rv '.decod($rate['p14']).' '.$user.'/'.$id.';');
            */

            Client.RunCommand($"cd /host/;mkdir {userId};");
            Client.RunCommand($"cd /;groupadd -g {id} s{id};useradd -u {id} -d /host/{userId}/{id} -g s{id} s{id};");
            Client.RunCommand($"cd /host/{userId}/;rm -R {id};");
            Client.RunCommand($"cd /host/;screen -dmS install_{id} cp -rv {rcon} {userId}/{id};");
        }

        public virtual void On(ConnectParam param) { }

        public virtual void Off(ConnectParam param) { }

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
