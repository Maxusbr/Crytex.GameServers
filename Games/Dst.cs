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
    public class Dst : BaseGameHost
    {
        public Dst(ConnectParam param) : base(param, "dst") { GameName = "dst"; }

        public override GameResult Create(CreateParam param)
        {
            if (!string.IsNullOrEmpty(param.GameServerId)) GameServerId = param.GameServerId;
            var run = $"cd {Path}/{GameName}/serverfiles/cfg;find {GameName}{GameServerId}";
            var res = Client.RunCommand(run);
            if (!string.IsNullOrEmpty(res.Error))
            {
                run = $"cd {Path}/{GameName}/serverfiles/cfg;cp -r DoNotStarveTogether {GameName}{GameServerId}";
                Client.RunCommand(run);
            }
            var host = $"cd {Path}/{GameName}/serverfiles/cfg/{GameName}{GameServerId};";
            Client.RunCommand(host + $"chmod 777 settings.ini;");
            Client.RunCommand(host + $"echo  \"[network]\ndefault_server_name = {GameName}{GameServerId}\n" +
                              $"default_server_description = Welcome to {GameName}{GameServerId} server\nserver_port = {param.GamePort}\n" +
                              $"server_password = {GameName}{GameServerId}\n\" > settings.ini;");
            Client.RunCommand(host + $"echo  \"max_players = 16\npvp = false\ngame_mode = endless\nenable_autosaver = true\ntick_rate = 30\n" +
                              $"connection_timeout = 8000\nserver_save_slot = 1\nenable_vote_kick = true\npause_when_empty = true\n\n" +
                              $"[account]\ndedicated_lan_server = false\n[STEAM]\nDISABLECLOUD = true\n[MISC]\nCONSOLE_ENABLED = true\n" +
                              $"autocompiler_enabled = true\n\" >> settings.ini;");

            var result = new GameResult();
            return result;
        }

        public override bool CompleteInstal()
        {
            var run = $"cd {Path}/{GameName}/serverfiles/cfg;find {GameName}{GameServerId}";
            var res = Client.RunCommand(run);
            return string.IsNullOrEmpty(res.Error);
        }
    }
}
