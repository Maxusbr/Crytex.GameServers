using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class Cs : BaseGameServer
    {
        public override void Go(string userId, string id)
        {
            //TODO Add info to database
            base.Go(userId, id);
        }

        public override void On(string id, int slots)
        {
            var server = FindById(id);
            var userId = server.UserId;
            var ip = server.Ip;
            var port = server.Port;
            Client.Connect();
            var map = "de_dust2";
            int cpu_cs = server.MinCpu;
            var sv_password = server.Password;
            var drop = $" +sv_password '{sv_password}'";
            drop += $" +rcon_password '{GeneratePassword(10)}'";
            /*
            if($row['p9'] != ""){$dop = " +sv_password '".$pass2."'";}
            if($row['p14'] != ""){$dop .= " +rcon_password '".$rcon."'";}
            if($row['p15'] != ""){$dop .= " ";}
            $run = 'cd /host/'.$user.'/'.$id.'/;chmod 777 hlds_run;screen -dmS server_'.$id.' sudo -u s'.$id.' 
            ./hlds_run -game cstrike  +servercfgfile server.cfg +log off  +map \''.$map.'\' +maxplayers '.$slots.' +ip '.$ip2.' -port '.$port2.' +fps_max 1000  '.$dop.' +sv_lan 0  -pingboost 3;';
            $ssh->exec_cmd($run);
            sleep('2');
            $ssh->exec_cmd("ps -ef | grep hlds_i686 | grep -v grep | grep -v sh | grep ".$ip2." | grep ".$port2." | awk '{ print $2}';");
            unset($data);
            $data = $ssh->get_output();
            $data = str_replace("\n","",$data);
            $cpu = $slots*$conf['cpu_mc'];
            $ssh->exec_cmd('screen -dmS server_cpu_'.$id.' cpulimit -v -z -p '.$data.' -l '.$cpu.';');
            */
            var run = $"cd /host/{userId}/{id}/;chmod 777 hlds_run;screen -dmS server_{id} sudo -u s{id} " +
                      $"./hlds_run -game cstrike  +servercfgfile server.cfg +log off  +map '{map}' +maxplayers {slots} " +
                      $"+ip {ip} -port {port} +fps_max 1000  {drop} +sv_lan 0  -pingboost 3;";
            Client.RunCommand(run);
            Thread.Sleep(2000);
            run = $"ps -ef | grep hlds_i686 | grep -v grep | grep -v sh | grep {ip} | grep {port} | awk" + " '{ print $2}';";
            var result = Client.RunCommand(run);
            var data = result.Result.Replace('\n', ' ');
            var cpu = slots * cpu_cs;
            Client.RunCommand($"screen -dmS server_cpu_{id} cpulimit -v -z -p {data} -l {cpu};");
            Client.Disconnect();
        }

        public override void Off(string id)
        {
            /*
            $ssh->exec_cmd("ps -ef | grep SCREEN | grep -v grep | grep server_".$id." | grep ".$ip2." | grep ".$port2." | awk '{ print $2}'");
	        $data = $ssh->get_output();
	        $data = str_replace("\n","",$data);
	        $scan2 = "ps -ef | grep hlds_i686 | grep -v grep | grep -v sh | grep s".$id." | awk '{ print $2}'";
	        $scan3 = "ps -ef | grep SCREEN | grep -v grep | grep server_cpu_".$id." | awk '{ print $2}'";
	        $ssh->exec_cmd($scan2);
	        $data2 = $ssh->get_output();
	        $data2 = str_replace("\n","",$data2);
	        $ssh->exec_cmd($scan3);
	        $data3 = $ssh->get_output();
	        $data3 = str_replace("\n","",$data3);
	        $ssh->exec_cmd("kill ".$data2.";kill ".$data.";kill ".$data3.";screen -wipe");
	        $ssh->disconnect();
            */
            var server = FindById(id);
            var ip = server.Ip;
            var port = server.Port;
            Client.Connect();
            var run = $"ps -ef | grep SCREEN | grep -v grep | grep server_{id} | grep {ip} | grep {port} | awk" + " '{ print $2}'";
            var result = Client.RunCommand(run);
            var data = result.Result.Replace('\n', ' ');
            var scan2 = $"ps -ef | grep hlds_i686 | grep -v grep | grep -v sh | grep s{id} | awk '" + "{ print $2}'";
            var scan3 = $"ps -ef | grep SCREEN | grep -v grep | grep server_cpu_{id} | awk " + "{ print $2}'";
            result = Client.RunCommand(scan2);
            var data2 = result.Result.Replace('\n', ' ');
            result = Client.RunCommand(scan3);
            var data3 = result.Result.Replace('\n', ' ');
            Client.RunCommand($"kill {data2};kill {data};kill {data3};screen -wipe");

            Client.Disconnect();
        }
    }
}
