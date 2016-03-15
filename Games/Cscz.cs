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
    public class Cscz : Cs
    {
        public Cscz(ConnectParam param) : base(param) { }

        //public override DataReceivedModel Go(GameHostParam param)
        //{
        //    var resModel = base.Go(param);
        //    var run = $"cd /host/{GameName}/serverfiles/czero;cp -r cscz-server.cfg cscz{UserId}.cfg";
        //    var res = Client.RunCommand(run);
        //    resModel.Data = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
        //    return resModel;
        //}

        //public override DataReceivedModel On(GameHostParam param)
        //{
        //    var resModel = base.On(param); ;
        //    var run = $"cd /host/{GameName};screen -dmS server_cscz{UserId} " +
        //              $"./{GameName} start -servicename cscz{UserId} -port {param.GamePort} -clientport {param.GamePort + 1};";
        //    var res = Client.RunCommand(run);
        //    resModel.Data = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
        //    return resModel;
        //}

        //public override void Off(GameHostParam param)
        //{
        //    base.Off(param);
        //    var run = $"cd /host/{GameName};screen -dmS server_cscz{UserId} " +
        //              $"./{GameName} stop -servicename cscz{UserId} -port {param.GamePort};";
        //    var res = Client.RunCommand(run);
        //    if (!string.IsNullOrEmpty(res.Error)) Console.WriteLine(res.Error);
        //}

        
    }
}
