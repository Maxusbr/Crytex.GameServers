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
    public class L4d2 : BaseGameHost
    {
        public L4d2(ConnectParam param) : base(param) { GameName = "l4d2"; }

        public override void Go(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName}/serverfiles/left4dead2/cfg;cp -r server.cfg l4d2{userId}.cfg";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }

        public override void On(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_l4d2{userId} " +
                      $"./{GameName} start -servicename l4d2{userId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }

        public override void Off(GameHostParam param)
        {
            var userId = param.UserId;
            var run = $"cd /host/{GameName};screen -dmS server_start_l4d2{userId} " +
                      $"./{GameName} stop -servicename l4d2{userId} -port {param.GamePort};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        }
    }
}
