using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Enums;
using Crytex.GameServers.Fabric;
using Crytex.GameServers.Models;

namespace Critex.GameConcole
{
    class Program
    {
        private const string LinuxSrvIp = "194.15.147.231";
        private const string Localhost = "192.168.1.131";
        static void Main(string[] args)
        {

            Console.Write(" 1 - Ark \n 2 - Arma3\n 3 - CS\n 4 - CSGO\n 5 - CSS\n 6 - Dods\n 7 - Gmod\n 8 - L4D\n 9 - L4D2\n 10 - Minecraft\n" +
                          " 11 - TF2\n 12 - Bmdm\n 13 - Cscz\n 14 - Cure\n 15 - Insurgency\n 16 - JustCause2\n Выберите игру: ");
            var key = Console.ReadLine();
            var connectparam = GetLinuxConnect(key);
            if (connectparam == null) return;
            using (var server = GameServerFactory.Instance.Get(connectparam))
            {
                var param = new GameHostParam
                {
                    GameId = 2000,
                    Slots = 2,
                    GamePort = 27020,
                    ServerId = Guid.NewGuid().ToString(),
                    UserId = 1002,
                    GamePassword = "",
                    MinCpu = 1
                };
                server.Go(param);
                server.On(param);
                Console.WriteLine($"Сервер {connectparam.FamilyGame} запущен.");
                Console.WriteLine("Для остановки сервера нажмите любую клавишу...");
                Console.ReadKey();
                server.Off(param);
                Console.WriteLine($"Сервер {connectparam.FamilyGame} остановлен.");
            }
            Console.Write("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private static ConnectParam LinuxConnecton(FamilyGame game)
        {
            var res = new ConnectParam
            {
                FamilyGame = game,
                SshIp = LinuxSrvIp,
                SshPort = 22,
                SshUserName = "max",
                SshPassword = "Qwerty#1"
            };
            switch (game)
            {
                case FamilyGame.Ark:
                    res.GameName = "ark";
                    break;
                case FamilyGame.Arma3:
                    res.GameName = "arma3";
                    break;
                case FamilyGame.Cs:
                    res.GameName = "cs";
                    break;
                case FamilyGame.Css:
                    res.GameName = "css";
                    break;
                case FamilyGame.CsGo:
                    res.GameName = "csgo";
                    break;
                case FamilyGame.Cure:
                    res.GameName = "cure";
                    break;
                case FamilyGame.Dods:
                    res.GameName = "dods";
                    break;
                case FamilyGame.GMod:
                    res.GameName = "gmod";
                    break;
                case FamilyGame.L4D:
                    res.GameName = "l4d";
                    break;
                case FamilyGame.L4D2:
                    res.GameName = "l4d2";
                    break;
                case FamilyGame.Minecraft:
                    break;
                case FamilyGame.SaMp:
                    break;
                case FamilyGame.T2F:
                    res.GameName = "t2f";
                    break;
                case FamilyGame.Bmdm:
                    res.GameName = "bmdm";
                    break;
                case FamilyGame.Cscz:
                    res.GameName = "cscz";
                    break;
                case FamilyGame.Insurgency:
                    res.GameName = "ins";
                    break;
                case FamilyGame.JustCause2:
                    res.GameName = "jc2";
                    break;
            }
            return res;
        }
        private static ConnectParam WindowsConnecton(FamilyGame game)
        {
            return new ConnectParam
            {
                FamilyGame = game,
                SshIp = Localhost,
                SshPort = 22,
                SshUserName = "Max",
                SshPassword = "qwerty"
            };
        }

        private static ConnectParam GetLinuxConnect(string key)
        {
            ConnectParam connectparam = null;
            switch (key)
            {
                case "1":
                    connectparam = LinuxConnecton(FamilyGame.Ark);
                    break;
                case "2":
                    connectparam = LinuxConnecton(FamilyGame.Arma3);
                    break;
                case "3":
                    connectparam = LinuxConnecton(FamilyGame.Cs);
                    break;
                case "4":
                    connectparam = LinuxConnecton(FamilyGame.CsGo);
                    break;
                case "5":
                    connectparam = LinuxConnecton(FamilyGame.Css);
                    break;
                case "6":
                    connectparam = LinuxConnecton(FamilyGame.Dods);
                    break;
                case "7":
                    connectparam = LinuxConnecton(FamilyGame.GMod);
                    break;
                case "8":
                    connectparam = LinuxConnecton(FamilyGame.L4D);
                    break;
                case "9":
                    connectparam = LinuxConnecton(FamilyGame.L4D2);
                    break;
                case "10":
                    connectparam = LinuxConnecton(FamilyGame.Minecraft);
                    break;
                case "11":
                    connectparam = LinuxConnecton(FamilyGame.T2F);
                    break;
                case "12":
                    connectparam = LinuxConnecton(FamilyGame.Bmdm);
                    break;
                case "13":
                    connectparam = LinuxConnecton(FamilyGame.Cscz);
                    break;
                case "14":
                    connectparam = LinuxConnecton(FamilyGame.Cure);
                    break;
                case "15":
                    connectparam = LinuxConnecton(FamilyGame.Insurgency);
                    break;
                case "16":
                    connectparam = LinuxConnecton(FamilyGame.JustCause2);
                    break;
            }
            return connectparam;
        }
    }
}
