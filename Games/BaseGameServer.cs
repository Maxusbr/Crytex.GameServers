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
        private SshClient _client;

        protected const int SshPort = 22;
        public string SshUserName { get; set; }
        public string SshPassword { get; set; }
        public string SshIp { get; set; }

        public SshClient Client => _client ?? (_client = new SshClient(SshIp, SshPort, SshUserName, SshPassword));

        public virtual void Go(string userId, string id)
        {
            Guid guid;
            if (!Guid.TryParse(id, out guid))
            {

            }
            var game = FindGameById(guid);
            var ip = game.Ip;
            var rcon = GeneratePassword(10);
            Client.Connect();
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
            Client.Disconnect();
            // TODO Save GameServer to database in implement
        }

        public virtual void On(string id, int slots) { }

        public virtual void Off(string id) { }
        public GameServerModel FindById(string id)
        {
            //TODO return _db.GameServer.FindById(id);
            var server = new GameServerModel();
            server.Game = FindGameById(server.GameId);
            return server;
        }


        private GameModel FindGameById(Guid id)
        {
            //TODO return _db.Games.FindById(id);
            var game = new GameModel();
            SshIp = game.Ip;
            SshUserName = game.SshUserName;
            SshPassword = game.SshPassword;
            return game;
        }

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
            Client?.Dispose();
        }
    }
}
