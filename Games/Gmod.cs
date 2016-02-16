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
    public class Gmod : BaseGameHost
    {
        public Gmod(ConnectParam param) : base(param) { }

        public override void On(GameHostParam param)
        {
            var id = param.GameId;
            var userId = param.UserId;
            var port = param.GamePort;
            var slots = param.Slots;
            int cpu_gmod = param.MinCpu;
            var sv_password = param.GamePassword;
            var map = "gm_construct";
            var drop = $" +sv_password '{sv_password}'";
            drop += $" +rcon_password '{GeneratePassword(10)}'";
            drop += " -tickrate '66'";
            var tvport = port + 2000;
            drop += $" +tv_enable 0 +tv_maxclients 0  +tv_port {tvport}";
            /*
if($row['p9'] != ""){$dop = " +sv_password '".$pass2."'";}
if($row['p14'] != ""){$dop .= " +rcon_password '".$rcon."'";}
if($row['p15'] != ""){$dop .= " -tickrate '".$tick."'";}
$dop .= "  +tv_enable 0 +tv_maxclients 0  +tv_port ".$tv;
$run = 'cd /host/'.$user.'/'.$id.'/orangebox/;chmod 777 srcds_run;screen -dmS server_'.$id.' sudo -u s'.$id.' 
./srcds_run -console -game garrysmod -autoupdate +sv_lan 0 +servercfgfile server.cfg +map \''.$map.'\' +maxplayers '.$slots.' 
-ip '.$ip2.' -port '.$port2.' '.$dop.';';
$ssh->exec_cmd($run);
sleep('2');
$ssh->exec_cmd("ps -ef | grep srcds_linux  | grep -v grep | grep -v sh | grep ".$ip2." | grep ".$port2." | awk '{ print $2}';");
unset($data);
$data = $ssh->get_output();
$data = str_replace("\n","",$data);
$cpu = $slots*$conf['cpu_css'];
$ssh->exec_cmd('screen -dmS server_cpu_'.$id.' cpulimit -v -z -p '.$data.' -l '.$cpu.';');
*/
            var run = $"cd /host/{userId}/{id}/orangebox/;chmod 777 srcds_run;screen -dmS server_{id} sudo -u s{id}";
            run += $" ./srcds_run -console -game garrysmod -autoupdate +sv_lan 0 +servercfgfile server.cfg +map '{map}' +maxplayers {slots}";
            run += $" -ip {Ip} -port {port} {drop};";
            Client.RunCommand(run);
            Thread.Sleep(2000);
            run = $"ps -ef | grep srcds_linux | grep -v grep | grep -v sh | grep {Ip} | grep {port} | awk" + " '{ print $2}';";
            var result = Client.RunCommand(run);
            var data = result.Result.Replace('\n', ' ');
            var cpu = slots * cpu_gmod;
            Client.RunCommand($"screen -dmS server_cpu_{id} cpulimit -v -z -p {data} -l {cpu};");
        }

        public override void Off(GameHostParam param)
        {
            /*
    $ssh->exec_cmd("ps -ef | grep SCREEN | grep -v grep | grep server_".$id." | grep ".$ip2." | grep ".$port2." | awk '{ print $2}'");
	$data = $ssh->get_output();
	$data = str_replace("\n","",$data);
	$scan2 = "ps -ef | grep srcds_linux  | grep -v grep | grep -v sh | grep s".$id." | awk '{ print $2}'";
	$scan3 = "ps -ef | grep SCREEN | grep -v grep | grep server_cpu_".$id." | awk '{ print $2}'";
	$ssh->exec_cmd($scan2);
	$data2 = $ssh->get_output();
	$data2 = str_replace("\n","",$data2);
	$ssh->exec_cmd($scan3);
	$data3 = $ssh->get_output();
	$data3 = str_replace("\n","",$data3);
	$ssh->exec_cmd("kill ".$data2.";kill ".$data.";kill ".$data3.";screen -wipe");
            */
            var id = param.GameId;
            var port = param.GamePort;
            var run = $"ps -ef | grep SCREEN | grep -v grep | grep server_{id} | grep {Ip} | grep {port} | awk" + " '{ print $2}'";
            var result = Client.RunCommand(run);
            var data = result.Result.Replace('\n', ' ');
            var scan2 = $"ps -ef | grep srcds_linux | grep -v grep | grep -v sh | grep s{id} | awk '" + "{ print $2}'";
            var scan3 = $"ps -ef | grep SCREEN | grep -v grep | grep server_cpu_{id} | awk " + "{ print $2}'";
            result = Client.RunCommand(scan2);
            var data2 = result.Result.Replace('\n', ' ');
            result = Client.RunCommand(scan3);
            var data3 = result.Result.Replace('\n', ' ');
            Client.RunCommand($"kill {data2};kill {data};kill {data3};screen -wipe");
        }
    }
}
