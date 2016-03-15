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
    public class Ark : BaseGameHost
    {
        public Ark(ConnectParam param) : base(param, "Engine") {  }

        public override GameResult Create(CreateParam param)
        {
            UserId = param.UserId;
            var run = $"cd /host/{GameName}/serverfiles/ShooterGame/Saved/Config/LinuxServer;cp -r GameUserSettings.ini {GameName}{UserId}.ini";
            var res = Client.RunCommand(run);
            var result = new GameResult();
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            return result;
        }

        //public override DataReceivedModel Go(GameHostParam param)
        //{
        //    return base.Go(param);
        //    //var run = $"cd /host/{GameName}/serverfiles//Saved/Config/LinuxServer;cp -r GameUserSettings.ini s{userId}.ini";
        //    //var res = Client.RunCommand(run);
        //    //if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        //}

        //public override DataReceivedModel On(GameHostParam param)
        //{
        //    var resModel = base.On(param);
        //    var run = $"cd /host/{GameName};screen -dmS server_start_ark{UserId} " +
        //              $"./{GameName} start -servicename ark{UserId} -port {param.GamePort} -clientport {param.GamePort + 1};";
        //    var res = Client.RunCommand(run);
        //    resModel.Data = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
        //    return resModel;
        //}

        //public override void Off(GameHostParam param)
        //{
        //    base.Off(param);
        //    var run = $"cd /host/{GameName};screen -dmS server_stop_ark{UserId} " +
        //              $"./{GameName} stop -servicename ark{UserId} -port {param.GamePort};";
        //    var res = Client.RunCommand(run);
        //    Console.WriteLine(!string.IsNullOrEmpty(res.Error) ? res.Error : res.Result);
        //}
    }
}
