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
        public JustCause2(ConnectParam param) : base(param) { GameName = param.GameName; }

        public override void Go(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/;cp -r jc2 jc2{userId}";
            var res = Client.RunCommand(run);
            var host = $"cd /host/jc2{userId}/serverfiles;";
            Client.RunCommand(host + "chmod 777 config.lua;");
            Client.RunCommand(host + "echo \"Server={\" > config.lua;");
            Client.RunCommand(host + "echo \"MaxPlayers=5000,\" >> config.lua;");
            Client.RunCommand(host + "echo \"BindIP= \\\"\\\",\" >> config.lua;");
            Client.RunCommand(host +$"echo \"BindPort={param.GamePort},\" >> config.lua;");
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


            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }

        public override void On(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/jc2{userId};" + //screen -dmS server_start_jc2{userId} " +
                      $"./{GameName} start -servicename jc2{userId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
        }

        public override void Off(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_jc2{userId} " +
                      $"./{GameName} stop -servicename jc2{userId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }
    }
}
