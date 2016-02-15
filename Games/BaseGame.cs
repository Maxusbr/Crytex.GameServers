using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class BaseGame
    {
        public virtual void Go(string userId, string id)
        {
            var ip = "ip";
            var port = 61586;
            var rcon = GeneratePassword(10);
            using (var client = new SshClient(ip, port, "username", "password"))
            {
                client.Connect();
                /*
                $ssh->exec_cmd('cd /host/;mkdir '.$user.';');
	            $ssh->exec_cmd('cd /;groupadd -g '.$id.' s'.$id.';useradd -u '.$id.' -d /host/'.$user.'/'.$id.' -g s'.$id.' s'.$id.';');
	            $ssh->exec_cmd('cd /host/'.$user.'/;rm -R '.$id.';');
	            $ssh->exec_cmd('cd /host/;screen -dmS install_'.$id.' cp -rv '.decod($rate['p14']).' '.$user.'/'.$id.';');
                */

                client.RunCommand($"cd /host/;mkdir {userId};");
                client.RunCommand($"cd /;groupadd -g {id} s{id};useradd -u {id} -d /host/{userId}/{id} -g s{id} s{id};");
                client.RunCommand($"cd /host/{userId}/;rm -R {id};");
                client.RunCommand($"cd /host/;screen -dmS install_{id} cp -rv {rcon} {userId}/{id};");
                client.Disconnect();
            }
        }
        protected string GeneratePassword(int count)
        {
            var rand = new Random();
            var res = string.Empty;
            for (var i = 0; i < count; i++)
                res += rand.Next(9);

            return res;
        }
    }
}
