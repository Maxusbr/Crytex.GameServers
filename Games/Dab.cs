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
    public class Dab : Cs
    {
        public Dab(ConnectParam param) : base(param, "dab") { GameName = "dab"; }

        public override GameResult Create(CreateParam param)
        {
            GameServerId = param.GameServerId;
            var run = $"cd {Path}/{GameName}/serverfiles/{GameCode}/cfg;cp -r da-server.cfg {GameName}{GameServerId}.cfg";
            var res = Client.RunCommand(run);
            var result = new GameResult();
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            return result;
        }

        protected override GameResult On(ChangeStatusParam param)
        {
            var result = new GameResult();
            var run = $"cd {Path}/{GameName};" +
                      $"./{GameName} start -servicename {GameName}{GameServerId} -port {param.GamePort} " +
                      $"-clientport {param.GamePort + 1} -sourcetvport {param.GamePort + 2};";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            result.Data = res.Result;
            return result;
        }
    }
}
