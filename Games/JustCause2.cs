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
        public JustCause2(ConnectParam param) : base(param, "jc2") { }

        public override GameResult Create(CreateParam param)
        {
            GameServerId = param.GameServerId;
            var run = $"cd {Path}/jc2users/;find {GameServerId}";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error))
            {
                run = $"cd {Path}/;mkdir jc2users/{GameServerId};cp -r jc2 jc2users/{GameServerId}/{GameName}";
                Client.RunCommand(run);
            }
            Path = $"{Path}/jc2users/{GameServerId}";
            var host = $"cd {Path}/{GameName}/serverfiles;";
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


            var result = new GameResult();
            return result;
        }

    }
}
