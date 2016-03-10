﻿using System;
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
        public JustCause2(ConnectParam param) : base(param) { }

        public override DataReceivedModel Go(GameHostParam param)
        {
            var resModel = base.Go(param);
            var run = $"cd /host/;cp -r jc2 jc2{UserId}";
            var res = Client.RunCommand(run);
            var host = $"cd /host/jc2{UserId}/serverfiles;";
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

            resModel.Data = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
            return resModel;
        }

        public override DataReceivedModel On(GameHostParam param)
        {
            var resModel = base.On(param);
            var run = $"cd /host/jc2{UserId};" + //screen -dmS server_start_jc2{userId} " +
                      $"./{GameName} start -servicename jc2{UserId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            resModel.Data = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
            return resModel;
        }

        public override void Off(GameHostParam param)
        {
            base.Off(param);
            var run = $"cd /host/{GameName};screen -dmS server_start_jc2{UserId} " +
                      $"./{GameName} stop -servicename jc2{UserId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }
    }
}