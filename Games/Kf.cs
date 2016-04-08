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
    public class Kf : BaseGameHost
    {
        public Kf(ConnectParam param) : base(param, "System") { GameName = "kf"; }

        public override GameResult Create(CreateParam param)
        {
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            var run = $"cd {Path}/{GameName}/serverfiles/System;sed 's/Port=7707/Port={param.GamePort}/g' kf-server.ini > temp.ini";
            var res = Client.RunCommand(run);
            run = $"cd {Path}/{GameName}/serverfiles/System;" +
                  $"sed 's/OldQueryPortNumber=7717/OldQueryPortNumber={param.GamePort + 1}/g' temp.ini > {GameName}{GameServerId}.ini";
            res = Client.RunCommand(run);
            var result = new GameResult();
            return result;
        }

        public override bool CompleteInstal()
        {
            var run = $"cd {Path}/{GameName}/serverfiles/System;find {GameName}{GameServerId}.ini";
            var res = Client.RunCommand(run);
            return string.IsNullOrEmpty(res.Error);
        }
    }
}
