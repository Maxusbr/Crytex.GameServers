﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class L4D : BaseGameServer
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
            using (var client = new SshClient(ip, SshPort, "username", "password"))
            {
                client.Connect();
                var map = "l4d_farm01_hilltop";
                int cpu_l4d = server.MinCpu;
                var sv_password = server.Password;
                var drop = $" +sv_password '{sv_password}'";
                drop += $" +rcon_password '{GeneratePassword(10)}'";
                drop += " -tickrate '66'";
                var tvport = port + 2000;
                drop += $" +tv_enable 0 +tv_maxclients 0  +tv_port {tvport}";
                /*
    if($row['p9'] != ""){$dop = " +sv_password '".$pass2."'";}
	if($row['p14'] != ""){$dop .= " +rcon_password '".$rcon."'";}
	if($row['p15'] != ""){$dop .= " -tickrate '".$tick."'";}
	$dop .= " +sv_lan 0 +tv_enable 0 +tv_maxclients 0  +tv_port ".$tv;
	$run = 'cd /host/'.$user.'/'.$id.'/l4d/;chmod 777 srcds_run;screen -dmS server_'.$id.' sudo -u s'.$id.' 
    ./srcds_run -console -game left4dead +exec '.$slots.'.cfg  +servercfgfile server.cfg -autoupdate +map \''.$map.'\' 
    versus +maxplayers '.$slots.' +sv_removehumanlimit 1 +sv_maxslots '.$slots.' +l4d_maxplayers '.$slots.' 
    +sv_visiblemaxplayers '.$slots.' -ip '.$ip2.' -port '.$port2.' '.$dop.';';
	$ssh->exec_cmd($run);
	sleep('2');
	$ssh->exec_cmd("ps -ef | grep srcds_linux  | grep -v grep | grep -v sh | grep ".$ip2." | grep ".$port2." | awk '{ print $2}';");
	unset($data);
	$data = $ssh->get_output();
	$data = str_replace("\n","",$data);
	$cpu = $slots*$conf['cpu_css'];
	$ssh->exec_cmd('screen -dmS server_cpu_'.$id.' cpulimit -v -z -p '.$data.' -l '.$cpu.';');
    */
                var run = $"cd /host/{userId}/{id}/l4d/;chmod 777 srcds_run;screen -dmS server_{id} sudo -u s{id}";
                run += $" ./srcds_run -console -game left4dead +exec {slots}.cfg +servercfgfile server.cfg -autoupdate +map '{map}'";
                run += $" versus +maxplayers {slots} +sv_removehumanlimit 1 +sv_maxslots {slots} +l4d_maxplayers {slots}";
                run += $" +sv_visiblemaxplayers {slots} -ip {ip} -port {port} {drop};";
                client.RunCommand(run);
                Thread.Sleep(2000);
                run = $"ps -ef | grep srcds_linux | grep -v grep | grep -v sh | grep {ip} | grep {port} | awk" + " '{ print $2}';";
                var result = client.RunCommand(run);
                var data = result.Result.Replace('\n', ' ');
                var cpu = slots * cpu_l4d;
                client.RunCommand($"screen -dmS server_cpu_{id} cpulimit -v -z -p {data} -l {cpu};");
                client.Disconnect();
            }
        }

        public override void Off(string id)
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
            var server = FindById(id);
            var ip = server.Ip;
            var port = server.Port;
            using (var client = new SshClient(ip, port, "username", "password"))
            {
                client.Connect();
                var run = $"ps -ef | grep SCREEN | grep -v grep | grep server_{id} | grep {ip} | grep {port} | awk" + " '{ print $2}'";
                var result = client.RunCommand(run);
                var data = result.Result.Replace('\n', ' ');
                var scan2 = $"ps -ef | grep srcds_linux | grep -v grep | grep -v sh | grep s{id} | awk '" + "{ print $2}'";
                var scan3 = $"ps -ef | grep SCREEN | grep -v grep | grep server_cpu_{id} | awk " + "{ print $2}'";
                result = client.RunCommand(scan2);
                var data2 = result.Result.Replace('\n', ' ');
                result = client.RunCommand(scan3);
                var data3 = result.Result.Replace('\n', ' ');
                client.RunCommand($"kill {data2};kill {data};kill {data3};screen -wipe");

                client.Disconnect();
            }
        }
    }
}
