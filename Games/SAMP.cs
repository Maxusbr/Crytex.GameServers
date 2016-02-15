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
    public class SAMP : BaseGame, ISshCommand
    {
        public override void Go(string userId, string id)
        {
            //TODO Add info to database
            base.Go(userId, id);
        }

        public void On(string userId, string id, int slots)
        {
            var ip = "ip";
            var port = 61586;
            var url = "URL Server";
            using (var client = new SshClient(ip, port, "username", "password"))
            {
                client.Connect();
                int cpu_mc = 1;

                /*
                https://wiki.sa-mp.com/wiki/Server.cfg
    $run = 'cd /host/'.$user.'/'.$id.'/;chmod 777 samp03svr;screen -dmS server_'.$id.' sudo -u s'.$id.' ./samp03svr;';
	$ssh->exec_cmd('cd /host/;cp -rv samp/samp03svr '.$id_user.'/'.$sid.'/samp03svr;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;chmod 777 server.properties;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "lanmode 0" > server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "hostname '.decod($row['p21']).'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "gamemode0 '.decod($row['p22']).'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "filterscripts '.decod($row['p23']).'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "announce '.decod($row['p24']).'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "query 1" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "weburl '.decod($row['p25']).'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "maxnpc 0" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "onfoot_rate 40" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "incar_rate 40" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "weapon_rate 40" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "stream_distance 300.0" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "stream_rate 1000" >> server.cfg;');
	if(decod($row['p14']) == ""){$rcon = "1234567890";}else{$rcon = decod($row['p14']);}
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "rcon_password '.$rcon.'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "port '.decod($row['p2']).'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "maxplayers '.$slots.'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "plugins '.decod($row['p32']).'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "weburl '.decod($row['p33']).'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "map '.decod($row['p34']).'" >> server.cfg;');
	$ssh->exec_cmd('cd /host/'.$user.'/;chown -R s'.$id.':s'.$id.' '.$id.';chmod -R 777 '.$id.'');
	$ssh->exec_cmd($run);
	$ssh->exec_cmd("ps -ef | grep samp03svr | grep -v grep | grep -v sh | grep s".$id." | awk '{ print $2}';");
	$data = $ssh->get_output();
	$data = explode("\n",$data);
	$cpu = "45";
	$ssh->exec_cmd('screen -dmS server_cpu_'.$id.' cpulimit -v -z -p '.$data['2'].' -l '.$cpu.';');
    */
                var host = $"cd /host/{userId}/{id}/;";
                var run = host + $"chmod 777 samp03svr;screen -dmS server_{id} sudo -u s{id} ./samp03svr";
                client.RunCommand($"cd /host/;cp -rv samp/samp03svr {userId}/{id}/samp03svr;");
                client.RunCommand(host + "chmod 777 server.cfg;");
                client.RunCommand(host + "echo \"lanmode 0\" > server.cfg;");
                client.RunCommand(host + "echo \"hostname SA-MP 0.3 Server\" >> server.cfg;");
                client.RunCommand(host + "echo \"gamemode0 grandlarc 1\" >> server.cfg;");
                client.RunCommand(host + "echo \"filterscripts base gl_actions gl_property gl_realtime\" >> server.cfg;");
                client.RunCommand(host + "echo \"announce 0\" >> server.cfg;");
                client.RunCommand(host + "echo \"query 1\" >> server.cfg;");
                client.RunCommand(host + $"echo \"weburl {url}\" >> server.cfg;");
                client.RunCommand(host + "echo \"maxnpc 0\" >> server.cfg;");
                client.RunCommand(host + "echo \"onfoot_rate 40\" >> server.cfg;");
                client.RunCommand(host + "echo \"incar_rate 40\" >> server.cfg;");
                client.RunCommand(host + "echo \"weapon_rate 40\" >> server.cfg;");
                client.RunCommand(host + "echo \"stream_distance 300.0\" >> server.cfg;");
                client.RunCommand(host + "echo \"stream_rate 1000\" >> server.cfg;");
                client.RunCommand(host + $"echo \"rcon_password {GeneratePassword(10)}\" >> server.cfg;");  
                client.RunCommand(host + $"echo \"port {port}\" >> server.cfg;");  
                client.RunCommand(host + $"echo \"maxplayers {slots}\" >> server.cfg;");  
                //client.RunCommand(host + $"echo \"plugins {plugins}\" >> server.cfg;");  
                //client.RunCommand(host + $"echo \"map {map}\" >> server.cfg;");  

                run = $"ps -ef | grep samp03svr | grep -v grep | grep -v sh | grep s{id} | awk" + " '{ print $2}';";
                var result = client.RunCommand(run);
                var data = result.Result.Split('\n');
                var cpu = 45;
                client.RunCommand($"screen -dmS server_cpu_{id} cpulimit -v -z -p {data[2]} -l {cpu};");

                client.Disconnect();
            }
        }

        public void Off(string userId, string id)
        {
            /*
    $ssh->exec_cmd("ps -ef | grep SCREEN | grep -v grep | grep server_".$id." | awk '{ print $2}'");
	$data = $ssh->get_output();
	$data = str_replace("\n","",$data);
	$scan3 = "ps -ef | grep SCREEN | grep -v grep | grep server_cpu_".$id." | awk '{ print $2}'";
	$ssh->exec_cmd($scan3);
	$data3 = $ssh->get_output();
	$data3 = str_replace("\n","",$data3);
	$ssh->exec_cmd("kill ".$data.";kill ".$data3.";screen -wipe");
            */
            var ip = "ip";
            var port = 61586;
            using (var client = new SshClient(ip, port, "username", "password"))
            {
                client.Connect();
                var run = $"ps -ef | grep SCREEN | grep -v grep | grep server_{id} | awk" + " '{ print $2}'";
                var result = client.RunCommand(run);
                var data = result.Result.Replace('\n', ' ');
                var scan3 = $"ps -ef | grep SCREEN | grep -v grep | grep server_cpu_{id} | awk " + "{ print $2}'";
                result = client.RunCommand(scan3);
                var data3 = result.Result.Replace('\n', ' ');
                client.RunCommand($"kill {data};kill {data3};screen -wipe");

                client.Disconnect();
            }
        }
    }
}
