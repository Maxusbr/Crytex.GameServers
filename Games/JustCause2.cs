using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class JustCause2 : BaseGameHost
    {
        public JustCause2(ConnectParam param) : base(param, "jc2") { GameName = "jc2"; _path = param.Path; }
        private readonly string _path;

        public override GameResult Create(CreateParam param)
        {
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            var result = new GameResult();
            var run = $"cd {_path}/jc2users;mkdir {GameServerId};cp -r jc2 {GameServerId}/{GameName}";
            Client.RunCommand(run);
            var host = $"cd {_path}/jc2users/{GameServerId}/{GameName}/serverfiles;";
            Client.RunCommand(host + "chmod 777 config.lua;");
            Client.RunCommand(host + "echo \"Server={\" > config.lua;");
            Client.RunCommand(host + "echo \"MaxPlayers=5000,\" >> config.lua;");
            Client.RunCommand(host + "echo \"BindIP= \\\"\\\",\" >> config.lua;");
            Client.RunCommand(host + $"echo \"BindPort={param.GamePort},\" >> config.lua;");
            Client.RunCommand(host + "echo \"Timeout=10000,\" >> config.lua;");
            Client.RunCommand(host + "echo \"Name= \\\"JC2 - MP Server\\\",\" >> config.lua;");
            Client.RunCommand(host + "echo \"Description=\\\"\\\",\" >> config.lua;");
            Client.RunCommand(host + "echo \"Password=\\\"\\\",\" >> config.lua;");

            Client.RunCommand(host + "echo \"Announce=true,\" >> config.lua;");
            Client.RunCommand(host + "echo \"SyncUpdate==180,\" >> config.lua;");
            Client.RunCommand(host + "echo \"IKnowWhatImDoing=false}\" >> config.lua;");

            Client.RunCommand(host + "echo \"SyncRates ={Vehicle = 75,OnFoot = 120,Passenger = 1000,MountedGun = 250,StuntPosition = 350}\" >> config.lua;");
            Client.RunCommand(host + "echo \"Streamer ={StreamDistance= 500}\" >> config.lua;");
            Client.RunCommand(host + "echo \"Vehicle ={DeathRespawnTime = 10,DeathRemove = false,UnoccupiedRespawnTime = 45,UnoccupiedRemove = false,}\" >> config.lua;");
            Client.RunCommand(host + "echo \"Player ={SpawnPosition = Vector3(-6550, 209, -3290)}\" >> config.lua;");
            Client.RunCommand(host + "echo \"Module ={MaxErrorCount = 5,ErrorDecrementTime = 500,SendAutorunWhenEmpty = false}\" >> config.lua;");
            Client.RunCommand(host + "echo \"World ={Time = 0.0,TimeStep = 1,WeatherSeverity = 0}\" >> config.lua;");

            Path = $"{_path}/jc2users/{GameServerId}/";
            return result;
        }

        public override bool CompleteInstal()
        {
            var run = $"cd {Path}/{GameName}/serverfiles;find config.lua";
            var res = Client.RunCommand(run);
            return string.IsNullOrEmpty(res.Error);
        }
    }
}
