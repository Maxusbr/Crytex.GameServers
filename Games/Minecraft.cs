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
    public class Minecraft : BaseGameServer
    {
        public Minecraft(ConnectParam param) : base(param) { }

        public override void On(ConnectParam param)
        {
            var id = param.GameId;
            var userId = param.UserId;
            var ip = param.SshIp;
            var port = param.GamePort;
            var slots = param.Slots;
            int cpu_mc = param.MinCpu;
            var mem = slots * 70;

            /*
            http://mmo-db.com/minecraft/info/sozdanie_i_nastroyka_servera
$mem = $slots*70;
$run = 'cd /host/'.$user.'/'.$id.'/;chmod 777 craftbukkit.jar;screen -dmS server_'.$id.' sudo -u s'.$id.' 
java -Xincgc -Xmx'.$mem.'m -Xms100m -jar craftbukkit.jar nogui;';
if(decod($row['p16']) == ""){$slots = decod($row['p5']);}else{$slots = decod($row['p16']);}
//$ssh->exec_cmd('cd /host/;cp -rv mc/craftbukkit.jar '.$id_user.'/'.$sid.'/craftbukkit.jar;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;chmod 777 server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "" > server.log;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "allow-nether='.decod($row['p25']).'" > server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "level-name='.decod($row['p21']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "enable-query=true" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "allow-flight='.decod($row['p35']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "server-port='.decod($row['p2']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "level-type='.decod($row['p23']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "enable-rcon='.decod($row['p37']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "level-seed='.decod($row['p22']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "server-ip='.decod($row['p1']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "spawn-npcs='.decod($row['p31']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "white-list='.decod($row['p28']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "spawn-animals='.decod($row['p30']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "online-mode='.decod($row['p27']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "pvp='.decod($row['p34']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "difficulty='.decod($row['p22']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "gamemode='.decod($row['p33']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "max-players='.$slots.'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "spawn-monsters='.decod($row['p29']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "generate-structures='.decod($row['p24']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "view-distance=10" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "motd='.decod($row['p36']).'" >> server.properties;chmod 555 server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "rcon.password='.decod($row['p14']).'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "debug=false" >> server.properties;');
$rcon_port = decod($row['p2'])+5000;
$query_port = decod($row['p2'])+3000;
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "rcon.port='.$rcon_port.'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/'.$id.'/;echo "query.port='.$query_port.'" >> server.properties;');
$ssh->exec_cmd('cd /host/'.$user.'/;chown -R s'.$id.':s'.$id.' '.$id.';chmod -R 777 '.$id.'');
$ssh->exec_cmd($run);
sleep('2');
$ssh->exec_cmd("ps -ef | grep java  | grep -v grep | grep -v sh | grep s".$id." | awk '{ print $2}';");
$data = $ssh->get_output();
$data = explode("\n",$data);
$cpu = $slots*$conf['cpu_mc'];$id2 = cod($id);
*/
            var host = $"cd /host/{userId}/{id}/;";
            var run = host + $"chmod 777 craftbukkit.jar;screen -dmS server_{id} sudo -u s{id}";
            run += $" java -Xincgc -Xmx{mem}m -Xms100m -jar craftbukkit.jar nogui;";
            Client.RunCommand(host + "chmod 777 server.properties;");
            Client.RunCommand(host + "echo \"\" > server.log;");
            Client.RunCommand(host + "echo \"allow-nether=true\" > server.properties;");
            Client.RunCommand(host + "echo \"level-name=world\" >> server.properties;");
            Client.RunCommand(host + "echo \"enable-query=true\" >> server.properties;");
            Client.RunCommand(host + "echo \"allow-flight=false\" >> server.properties;");
            Client.RunCommand(host + $"echo \"server-port={port}\" >> server.properties;");
            Client.RunCommand(host + "echo \"level-type=DEFAULT\" >> server.properties;");
            Client.RunCommand(host + $"echo \"server-ip={ip}\" >> server.properties;");
            Client.RunCommand(host + "echo \"spawn-npcs=true\" >> server.properties;");
            Client.RunCommand(host + "echo \"white-list=false\" >> server.properties;");
            Client.RunCommand(host + "echo \"spawn-animals=true\" >> server.properties;");
            Client.RunCommand(host + "echo \"online-mode=true\" >> server.properties;");
            Client.RunCommand(host + "echo \"pvp=true\" >> server.properties;");
            Client.RunCommand(host + "echo \"gamemode=1\" >> server.properties;");
            Client.RunCommand(host + $"echo \"max-players={slots}\" >> server.properties;");
            Client.RunCommand(host + "echo \"spawn-monsters=true\" >> server.properties;");
            Client.RunCommand(host + "echo \"generate-structures=true\" >> server.properties;");
            Client.RunCommand(host + "echo \"view-distance=10\" >> server.properties;");
            Client.RunCommand(host + "echo \"vmotd=Minecraft\" >> server.properties;chmod 555 server.properties;");
            Client.RunCommand(host + $"echo \"rcon.password={GeneratePassword(10)}\" >> server.properties;");
            Client.RunCommand(host + "echo \"debug=false\" >> server.properties;");
            var rconPort = port + 5000;
            var queryPort = port + 2000;
            Client.RunCommand(host + $"echo \"rcon.port={rconPort}\" >> server.properties;");
            Client.RunCommand(host + $"echo \"query.port={queryPort}\" >> server.properties;");
            Client.RunCommand(host + $"chown -R s{id}:s{id} {id};chmod -R 777 {id}");
            Client.RunCommand(run);

            run = $"ps -ef | grep java | grep -v grep | grep -v sh | grep s{id} | awk" + " '{ print $2}';";
            var result = Client.RunCommand(run);
            var data = result.Result.Replace('\n', ' ');
            var cpu = slots * cpu_mc;

            Client.Disconnect();
        }

        public override void Off(ConnectParam param)
        {
            /*
    $ssh->exec_cmd("ps -ef | grep SCREEN | grep -v grep | grep server_".$id." | awk '{ print $2}'");
	$data = $ssh->get_output();
	$data = str_replace("\n","",$data);
	$scan2 = "ps -ef | grep java  | grep -v grep | grep -v sh | grep s".$id." | awk '{ print $2}'";
	$scan3 = "ps -ef | grep SCREEN | grep -v grep | grep server_cpu_".$id." | awk '{ print $2}'";
	$ssh->exec_cmd($scan2);
	$data2 = $ssh->get_output();
	$data5 = explode("\n",$data2);
	$data2 = $data5['1'];
	$ssh->exec_cmd($scan3);
	$data3 = $ssh->get_output();
	$data3 = str_replace("\n","",$data3);
	$ssh->exec_cmd("kill -9 ".$data2.";kill -9 ".$data.";kill -9 ".$data3.";screen -wipe");
            */
            var id = param.GameId;

            var run = $"ps -ef | grep SCREEN | grep -v grep | grep server_{id} | awk" + " '{ print $2}'";
            var result = Client.RunCommand(run);
            var data = result.Result.Replace('\n', ' ');
            var scan2 = $"ps -ef | grep java | grep -v grep | grep -v sh | grep s{id} | awk '" + "{ print $2}'";
            var scan3 = $"ps -ef | grep SCREEN | grep -v grep | grep server_cpu_{id} | awk " + "{ print $2}'";
            result = Client.RunCommand(scan2);
            var data2 = result.Result.Split('\n');
            result = Client.RunCommand(scan3);
            var data3 = result.Result.Replace('\n', ' ');
            Client.RunCommand($"kill -9 {data2[1]};kill -9 {data};kill -9 {data3};screen -wipe");
        }
    }
}
