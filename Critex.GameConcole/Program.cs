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
        //private const string LinuxSrvIp = "149.202.66.34";
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
            Console.Write(" 1 - Ark \t 2 - Arma3\n 3 - CS\t 4 - CSGO\n 5 - CSS\t 6 - Dod\n 7 - Gmod\t 8 - L4D\n 9 - L4D2\t 10 - Minecraft\n" +
                          " 11 - TF2\t 12 - Bmdm\n 13 - Cscz\t 14 - Cure\n 15 - Insurgency\t 16 - JustCause2\n 17 - Rust\t 18 - Dods\n" +
                          " 19 - Dst\t 20 - DoubleAction\n 21 - FistfulofFrags\t 22 - Hurtworld\n 23 - KillingFloor\t 24 - NS2\n" +
                          " Выберите игру: ");
            var key = Console.ReadLine();
            _connectparam = GetLinuxConnect(key);
            if (_connectparam == null) return;
            _server = GameServerFactory.Instance.Get(_connectparam);
            var res = _server.Connect();
            _gameparam = new CreateParam
            {
                Slots = 2,
                GamePort = 27020,
                GameServerId = 1002,
                GamePassword = "",
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
                GameServerId = _gameparam.GameServerId,
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
                                CloseConnect();
                                _isWriteCommand = true;
                                break;
                            case "exit":
                                return;
                            default:
                                Console.WriteLine(_server?.SendConsoleCommand(key));
                                WriteCommand();
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

        private static void CloseConnect()
        {
            var res = _server.Disconnect();
            Console.WriteLine($"Соединение закрыто: {res.Succes}");
            _connectparam = null;
            _gameparam = null;
        }

        private static void GetStatusServer()
        {
            //OpenConsole();
            //if (_server == null) return;
            //Console.WriteLine(_server.SendConsoleCommand("status", true));
            //CloseConsole();
            var data = _server.GetAdvancedState(_gameparam);
            Console.WriteLine($"Сервер {_connectparam.FamilyGame}: {data.Status}");
            foreach (var st in data.ServerStates)
                Console.WriteLine($"{st.ParameterName}\t: {st.ParameterValue}");

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
            Console.Write($"Консоль {_connectparam.FamilyGame}#> ");
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
            Console.Write($"Сервер {_connectparam.FamilyGame}: ");
            var res = _server.GetState(_gameparam);
            if (!res.Succes)
                Console.Write($"{res.ErrorMessage}\n");
            Console.Write($"{res.Status}\n");
            WriteCommand();
        }

        private static void StopServer()
        {
            var param = GetChangeStatusParam();
            param.TypeStatus = GameHostTypeStatus.Disable;
            var res = _server.ChangeStatus(param);
            if (!res.Succes)
                Console.WriteLine($"{res.ErrorMessage}");
            WriteCommand();
        }

        private static ConnectParam LinuxConnecton(GameFamily game)
        {
            var res = new ConnectParam
            {
                FamilyGame = game,
                SshIp = LinuxSrvIp,
                SshPort = 22,
                SshUserName = "max",
                SshPassword = "Qwerty#1",
                Path = "/host"
                //SshPort = 20002,
                //SshUserName = "vncuser",
                //SshPassword = "QwerT@12",
                //Path = "/home/vncuser/host"
            };
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
                SshPassword = "qwerty",
                Path = "/host"
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
                case "1":
                    connectparam = LinuxConnecton(GameFamily.Ark);
                    break;
                case "2":
                    connectparam = LinuxConnecton(GameFamily.Arma3);
                    break;
                case "4":
                    connectparam = LinuxConnecton(GameFamily.CsGo);
                    break;
                case "5":
                    connectparam = LinuxConnecton(GameFamily.Css);
                    break;
                case "6":
                    connectparam = LinuxConnecton(GameFamily.Dod);
                    break;
                case "7":
                    connectparam = LinuxConnecton(GameFamily.GMod);
                    break;
                case "8":
                    connectparam = LinuxConnecton(GameFamily.L4D);
                    break;
                case "9":
                    connectparam = LinuxConnecton(GameFamily.L4D2);
                    break;
                case "10":
                    connectparam = LinuxConnecton(GameFamily.Minecraft);
                    break;
                case "11":
                    connectparam = LinuxConnecton(GameFamily.T2F);
                    break;
                case "12":
                    connectparam = LinuxConnecton(GameFamily.Bmdm);
                    break;
                case "13":
                    connectparam = LinuxConnecton(GameFamily.Cscz);
                    break;
                case "14":
                    connectparam = LinuxConnecton(GameFamily.Cure);
                    break;
                case "15":
                    connectparam = LinuxConnecton(GameFamily.Insurgency);
                    break;
                case "16":
                    connectparam = LinuxConnecton(GameFamily.JustCause2);
                    break;
                //case "17":
                //    connectparam = LinuxConnecton(GameFamily.Rust);
                //    break;
                case "18":
                    connectparam = LinuxConnecton(GameFamily.Dods);
                    break;
                case "19":
                    connectparam = LinuxConnecton(GameFamily.DontStarveTogether);
                    break;
                case "20":
                    connectparam = LinuxConnecton(GameFamily.DoubleAction);
                    break;
                case "21":
                    connectparam = LinuxConnecton(GameFamily.FistfulofFrags);
                    break;
                case "22":
                    connectparam = LinuxConnecton(GameFamily.Hurtworld);
                    break;
                //case "23":
                //    connectparam = LinuxConnecton(GameFamily.KillingFloor);
                //    break;
                case "24":
                    connectparam = LinuxConnecton(GameFamily.Ns2);
                    break;
            }
            return connectparam;
        }
    }
}
