using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Fabric;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Crytex.Model.Enums;

namespace Critex.GameConcole
{
    class Program
    {
        private const string LinuxSrvIp = "194.15.147.231";
        private const string Localhost = "192.168.1.131";

        private static ConnectParam _connectparam;
        private static CreateParam _gameparam;
        private static IGameHost _server;

        static void Main(string[] args)
        {
            ReadLine();
        }

        private static void RunServer()
        {
            Console.Write(" 1 - Ark \n 2 - Arma3\n 3 - CS\n 4 - CSGO\n 5 - CSS\n 6 - Dods\n 7 - Gmod\n 8 - L4D\n 9 - L4D2\n 10 - Minecraft\n" +
                          " 11 - TF2\n 12 - Bmdm\n 13 - Cscz\n 14 - Cure\n 15 - Insurgency\n 16 - JustCause2\n Выберите игру: ");
            var key = Console.ReadLine();
            _connectparam = GetLinuxConnect(key);
            if (_connectparam == null) return;
            _server = GameServerFactory.Instance.Get(_connectparam);
            var res = _server.Connect();
            _gameparam = new CreateParam
            {
                Slots = 2,
                GamePort = 27020,
                UserId = 1002,
                GamePassword = "",
                MinCpu = 1
            };
            res = _server.Create(_gameparam);
            if (!res.Succes)
                Console.WriteLine($"{res.ErrorMessage}");
            var param = GetChangeStatusParam();
            param.TypeStatus = GameHostTypeStatus.Enable;
            res = _server.ChangeStatus(param);
            if (!res.Succes)
                Console.WriteLine($"{res.ErrorMessage}");
            //_server.DataReceived += _server_DataReceived;
            //var res = _server.On(_gameparam);
            //if (res.ServerStates == null) return;
            //foreach (var param in res.ServerStates)
            //    Console.WriteLine($"{param.ParameterName}: {param.ParameterValue}");
        }

        private static ChangeStatusParam GetChangeStatusParam()
        {
            return new ChangeStatusParam
            {
                GamePort = _gameparam.GamePort,
                UserId = _gameparam.UserId,
                GamePassword = _gameparam.GamePassword
            };
        }

        private static void ReadLine()
        {

            while (true)
            {
                if (_isWriteCommand) WriteCommand();
                var key = Console.ReadLine();

                try
                {
                    if (key != null)
                    {
                        switch (key.ToLower())
                        {
                            case "1":
                                RunServer();
                                break;
                            case "2":
                                _isWriteCommand = false;
                                StopServer();
                                break;
                            case "3":
                                GetStatus();
                                break;
                            case "4":
                                _isWriteCommand = false;
                                OpenConsole();
                                break;
                            case "5":
                                _isWriteCommand = false;
                                GetStatusServer();
                                break;
                            case "6":
                                _isWriteCommand = true;
                                CloseConsole();
                                _isConsoleOpen = false;
                                break;
                            case "0":

                                return;
                            case "exit":
                                return;
                            default:
                                _server?.SendConsoleCommand(key);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void GetStatusServer()
        {
            OpenConsole();
            if (_server == null) return;
            Console.WriteLine(_server.SendConsoleCommand("clear", true));
            Console.WriteLine(_server.SendConsoleCommand("status", true));
            WriteCommand();
        }

        static bool _isWriteCommand = true;
        static bool _isConsoleOpen;
        private static void WriteCommand()
        {
            Console.Write("\n");
            Console.Write(_connectparam == null ? " 1 - Старт сервер \n" : " 2 - Стоп сервер\n 3 - Состояние\n" +
                                                  " 4 - Открыть консоль\n 5 - Статус сервера\n 6 - Закрыть консоль\n 0 - Закрыть соединение\n");
            Console.Write(" >> ");
        }

        private static void CloseConsole()
        {
            if (_server == null) return;
            var res = _server.CloseConsole(_gameparam);
            Console.WriteLine($"Сервер {_connectparam.FamilyGame}: {res}");
        }

        private static void OpenConsole()
        {
            if (_server == null) return;
            _server.OpenConsole(_gameparam);
            Console.WriteLine($"Консоль {_connectparam.FamilyGame}: ");
        }

        private static void _server_DataReceived(object sender, DataReceivedModel data)
        {
            Console.Write(data.Data);
            MonitorStateServer(data);
        }

        private static void MonitorStateServer(DataReceivedModel data)
        {
            var state = data.ServerStates?.FirstOrDefault(o => o.ParameterName.Equals("ServerOFF"));
            if (state != null)
            {
                //_server.DataReceived -= _server_DataReceived;
                Console.WriteLine($"Сервер {_connectparam.FamilyGame} остановлен.");
                _connectparam = null;
                _server = null;
                _isWriteCommand = true;
                WriteCommand();
            }
            state = data.ServerStates?.FirstOrDefault(o => o.ParameterName.Equals("Console"));
            if (state != null && state.ParameterValue == "Ready")
                _isConsoleOpen = true;
            state = data.ServerStates?.FirstOrDefault(o => o.ParameterName.Equals("Status"));
            if (state != null && state.ParameterValue == "Ready")
            {
                //CloseConsole();
                _isConsoleOpen = false;
                foreach (var el in data.ServerStates)
                    Console.WriteLine($"{el.ParameterName} : {el.ParameterValue}");
                Console.Write("|");
                foreach (var head in data.TableInfo.Headers)
                    Console.Write($"{head}\t|");
                Console.Write('\n');
                if (data.TableInfo.Values.Any())
                    Console.Write("|");
                foreach (var value in data.TableInfo.Values)
                    Console.Write($"{value}\t|");
                Console.Write('\n');
                WriteCommand();
            }
        }

        private static void GetStatus()
        {
            var res = _server.GetState(_gameparam);
            if (!res.Succes)
                Console.WriteLine($"{res.ErrorMessage}");
            //if (_connectparam == null || _gameparam == null) return;
            //var res = _server.Monitor(_gameparam);
            //Console.Write($"Сервер {_connectparam.FamilyGame}:");
            //if (res.Data.Equals("OK"))
            //{
            //    Console.Write("\n");
            //    foreach (var param in res.ServerStates)
            //        Console.WriteLine($"{param.ParameterName}: {param.ParameterValue}");
            //}
            //else
            //    Console.WriteLine($" {res}");
        }

        private static void StopServer()
        {
            var param = GetChangeStatusParam();
            param.TypeStatus = GameHostTypeStatus.Disable;
            var res = _server.ChangeStatus(param);
            if (!res.Succes)
                Console.WriteLine($"{res.ErrorMessage}");
        }

        private static ConnectParam LinuxConnecton(GameFamily game)
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
                case GameFamily.Cs:
                    res.GameName = "cs";
                    break;
                    //case GameFamily.Ark:
                    //    res.GameName = "ark";
                    //    break;
                    //case GameFamily.Arma3:
                    //    res.GameName = "arma3";
                    //    break;

                    //case GameFamily.Css:
                    //    res.GameName = "css";
                    //    break;
                    //case GameFamily.CsGo:
                    //    res.GameName = "csgo";
                    //    break;
                    //case GameFamily.Cure:
                    //    res.GameName = "cure";
                    //    break;
                    //case GameFamily.Dods:
                    //    res.GameName = "dods";
                    //    break;
                    //case GameFamily.GMod:
                    //    res.GameName = "gmod";
                    //    break;
                    //case GameFamily.L4D:
                    //    res.GameName = "l4d";
                    //    break;
                    //case GameFamily.L4D2:
                    //    res.GameName = "l4d2";
                    //    break;
                    //case GameFamily.Minecraft:
                    //    break;
                    //case GameFamily.SaMp:
                    //    break;
                    //case GameFamily.T2F:
                    //    res.GameName = "t2f";
                    //    break;
                    //case GameFamily.Bmdm:
                    //    res.GameName = "bmdm";
                    //    break;
                    //case GameFamily.Cscz:
                    //    res.GameName = "cscz";
                    //    break;
                    //case GameFamily.Insurgency:
                    //    res.GameName = "ins";
                    //    break;
                    //case GameFamily.JustCause2:
                    //    res.GameName = "jc2";
                    //    break;
            }
            return res;
        }
        private static ConnectParam WindowsConnecton(GameFamily game)
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
                case "3":
                    connectparam = LinuxConnecton(GameFamily.Cs);
                    break;
                    //case "1":
                    //    connectparam = LinuxConnecton(GameFamily.Ark);
                    //    break;
                    //case "2":
                    //    connectparam = LinuxConnecton(GameFamily.Arma3);
                    //    break;

                    //case "4":
                    //    connectparam = LinuxConnecton(GameFamily.CsGo);
                    //    break;
                    //case "5":
                    //    connectparam = LinuxConnecton(GameFamily.Css);
                    //    break;
                    //case "6":
                    //    connectparam = LinuxConnecton(GameFamily.Dods);
                    //    break;
                    //case "7":
                    //    connectparam = LinuxConnecton(GameFamily.GMod);
                    //    break;
                    //case "8":
                    //    connectparam = LinuxConnecton(GameFamily.L4D);
                    //    break;
                    //case "9":
                    //    connectparam = LinuxConnecton(GameFamily.L4D2);
                    //    break;
                    //case "10":
                    //    connectparam = LinuxConnecton(GameFamily.Minecraft);
                    //    break;
                    //case "11":
                    //    connectparam = LinuxConnecton(GameFamily.T2F);
                    //    break;
                    //case "12":
                    //    connectparam = LinuxConnecton(GameFamily.Bmdm);
                    //    break;
                    //case "13":
                    //    connectparam = LinuxConnecton(GameFamily.Cscz);
                    //    break;
                    //case "14":
                    //    connectparam = LinuxConnecton(GameFamily.Cure);
                    //    break;
                    //case "15":
                    //    connectparam = LinuxConnecton(GameFamily.Insurgency);
                    //    break;
                    //case "16":
                    //    connectparam = LinuxConnecton(GameFamily.JustCause2);
                    //    break;
            }
            return connectparam;
        }
    }
}
