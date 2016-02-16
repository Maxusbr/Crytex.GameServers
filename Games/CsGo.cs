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
    public class CsGo : BaseGameServer
    {
        public CsGo(ConnectParam param) : base(param) { }

        public override void On(ConnectParam param)
        {
            var id = param.GameId;
            var userId = param.UserId;
            var ip = param.SshIp;
            var port = param.GamePort;
            var slots = param.Slots;
            int cpu_csgo = param.MinCpu;
            var sv_password = param.GamePassword;
            var map = "de_dust2";
            var drop = $" +sv_password '{sv_password}'";
            drop += $" +rcon_password '{GeneratePassword(10)}'";
            drop += " -tickrate '66'";
            var tvport = port + 2000;
            drop += $" +sv_lan 0 +tv_enable 0 +tv_maxclients 0  +tv_port {tvport}";
            /*
            http://csgodev.com/csgo-server-command-line-options/
if($row['p9'] != ""){$dop = " +sv_password '".$pass2."'";}
if($row['p14'] != ""){$dop .= " +rcon_password '".$rcon."'";}
if($row['p15'] != ""){$dop .= " -tickrate '".$tick."'";}

    if(decod($row['p21']) == "1"){$mod = "+game_type 0 +game_mode 0 +mapgroup mg_bomb";}
    if(decod($row['p21']) == "2"){$mod = "+game_type 0 +game_mode 1 +mapgroup mg_bomb";}
    if(decod($row['p21']) == "3"){$mod = "+game_type 1 +game_mode 0 +mapgroup mg_armsrace";}
    if(decod($row['p21']) == "4"){$mod = "+game_type 1 +game_mode 1 +mapgroup mg_demolition";}

$dop .= " +sv_lan 0 +tv_enable 0 +tv_maxclients 0  +tv_port ".$tv;
$run = 'cd /host/'.$user.'/'.$id.'/;chmod 777 srcds_run;screen -dmS server_'.$id.' sudo -u s'.$id.' 
./srcds_run -game csgo -console -usercon -secure -autoupdate -nohltv -maxplayers_override '.$slots.' +sv_pure 0 
+hostport '.$port2.' +net_public_adr '.$ip2.' +map '.$map.' '.$mod.' '.$dop;
$ssh->exec_cmd($run);
sleep('2');
$ssh->exec_cmd("ps -ef | grep srcds_linux  | grep -v grep | grep -v sh | grep ".$ip2." | grep ".$port2." | awk '{ print $2}';");
unset($data);
$data = $ssh->get_output();
$data = str_replace("\n","",$data);
$cpu = $slots*$conf['cpu_csgo'];
$ssh->exec_cmd('screen -dmS server_cpu_'.$id.' cpulimit -v -z -p '.$data.' -l '.$cpu.';');
            */
            var run = $"cd /host/{userId}/{id}/;chmod 777 srcds_run;screen -dmS server_{id} sudo -u s{id}";
            run += $" ./srcds_run -game csgo -console -usercon -secure -autoupdate -nohltv -maxplayers_override {slots} +sv_pure 0";
            run += $" +hostport {port} +net_public_adr {ip} +map '{map}' {drop};";
            Client.RunCommand(run);
            Thread.Sleep(2000);
            run = $"ps -ef | grep srcds_linux | grep -v grep | grep -v sh | grep {ip} | grep {port} | awk" + " '{ print $2}';";
            var result = Client.RunCommand(run);
            var data = result.Result.Replace('\n', ' ');
            var cpu = slots * cpu_csgo;
            Client.RunCommand($"screen -dmS server_cpu_{id} cpulimit -v -z -p {data} -l {cpu};");
            Client.Disconnect();
        }

        public override void Off(ConnectParam param)
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
            var ip = param.SshIp;
            var port = param.GamePort;

            var run = $"ps -ef | grep SCREEN | grep -v grep | grep server_{id} | grep {ip} | grep {port} | awk" + " '{ print $2}'";
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
