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
    public class Arma3 : BaseGameHost
    {
        public Arma3(ConnectParam param) : base(param, "arma3") { }

        public override GameResult Create(CreateParam param)
        {
            UserId = param.UserId;
            var run = $"cd {Path}/{GameName}/serverfiles/cfg;cp -r arma3-server.server.cfg {GameName}{UserId}.server.cfg";
            var res = Client.RunCommand(run);
            var host = $"cd {Path}/{GameName}/serverfiles/cfg;";
            Client.RunCommand(host + $"chmod 777 {GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + $"echo  \"serverport={param.GamePort};\" > {GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + $"echo \"steamqueryport={param.GamePort+1};\" >> {GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + $"echo \"steamport={param.GamePort+2};\" >> {GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + $"echo \"passwordAdmin = \\\"{GameName}{UserId}adminpass\\\";\" >> {GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + $"echo \"maxPlayers = 32;\" >> {GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + $"echo \"logFile = \\\"{GameName}{UserId}server.log\\\";\" >> {GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + "echo \"motd[]={\\\"Welcome to My Arma 3 Server\\\"," +
                              "\\\"TS3 Server:     teamspeak.somewhere.com\\\",\\\"Web:            www.example.com\\\"};\" >> " +
                              $"{GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + "echo \"motdInterval = 30;\nvoteMissionPlayers = 1;\nvoteThreshold = 0.33;\n" +
                              "disableVoN = 0;\nvonCodecQuality = 3;\npersistent = 1;\ntimeStampFormat = \\\"short\\\";\n" +
                              "verifySignatures = 2;\nrequiredSecureId = 2;\nkickDuplicate = 1;\nBattlEye = 1;\" >> " +
                              $"{GameName}{UserId}.server.cfg;");

            Client.RunCommand(host + "echo \"allowedLoadFileExtensions[] = {\\\"hpp\\\",\\\"sqs\\\",\\\"sqf\\\",\\\"fsm\\\"," +
                              "\\\"cpp\\\",\\\"paa\\\",\\\"txt\\\",\\\"xml\\\",\\\"inc\\\",\\\"ext\\\",\\\"sqm\\\",\\\"ods\\\"," +
                              "\\\"fxy\\\",\\\"lip\\\",\\\"csv\\\",\\\"kb\\\",\\\"bik\\\",\\\"bikb\\\"," +
                              "\\\"html\\\",\\\"htm\\\",\\\"biedi\\\"};\" >> " +
                              $"{GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + "echo \"allowedPreprocessFileExtensions[] = {\\\"hpp\\\",\\\"sqs\\\",\\\"sqf\\\",\\\"fsm\\\"," +
                              "\\\"cpp\\\",\\\"paa\\\",\\\"txt\\\",\\\"xml\\\",\\\"inc\\\",\\\"ext\\\",\\\"sqm\\\",\\\"ods\\\"," +
                              "\\\"fxy\\\",\\\"lip\\\",\\\"csv\\\",\\\"kb\\\",\\\"bik\\\",\\\"bikb\\\",\\\"html\\\"," +
                              "\\\"htm\\\",\\\"biedi\\\"};\" >> " +
                              $"{GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + "echo \"allowedHTMLLoadExtensions[] = {\\\"htm\\\",\\\"html\\\",\\\"xml\\\",\\\"txt\\\"};\" >> " +
                              $"{GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + "echo \"onUserConnected = \\\"\\\";\nonUserDisconnected = \\\"\\\";\ndoubleIdDetected = \\\"\\\";\" >> " +
                              $"{GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + "echo \"onUnsignedData = \\\"kick (_this select 0)\\\";\nonHackedData = 	\\\"kick (_this select 0)\\\";\n" +
                              "onDifferentData = \\\"\\\";\" >> " +
                              $"{GameName}{UserId}.server.cfg;");
            Client.RunCommand(host + "echo \"headlessClients[]={\\\"127.0.0.1\\\"};\nlocalClient[]={\\\"127.0.0.1\\\"};\n" +
                              "battleyeLicense=1;\" >> " +
                              $"{GameName}{UserId}.server.cfg;");
            var result = new GameResult();

            run = $"cd {Path}/{GameName}/serverfiles/cfg;cp -r arma3-server.network.cfg {GameName}{UserId}.network.cfg";
            res = Client.RunCommand(run);
            
            if (!string.IsNullOrEmpty(res.Error))
            {
                ValidateError(res, result);
            }
            return result;
        }
    }
}
