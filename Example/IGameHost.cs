using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Models;
using Crytex.Model.Exceptions;
using Renci.SshNet;

namespace Crytex.GameServers.Example
{
    internal interface IGameHost 
    {
        GameResult Connect();
        GameResult Disconnect();

        GameResult Create(CreateParam param);//Go
        GameResult ChangeStatus(ChangeStatusParam param);//On/of

        StateGameResult GetState(UserGameParam userGameParam);



        //Пока оставляем но тоже будем рефакторить
        event EventHandler<DataReceivedModel> DataReceived;
        void OpenConsole(GameHostParam param);
        string CloseConsole(GameHostParam param);
        string SendConsoleCommand(string command, bool waitAll = false);
    }



    internal enum TypeStatus
    {
        Enable,
        Disable
    }



    internal class GameResult
    {
        public bool Succes=true;
        public TypeError? Error { get; set; }
    }
    internal enum TypeError
    {

    }

    internal class StateGameResult : GameResult
    {
        public TypeStatus Status { get; set; }
    }

    internal class UserGameParam
    {
        public int UserId { get; set; }
    }

    internal class ChangeStatusParam : UserGameParam
    {
        public TypeStatus TypeStatus { get; set; }
    }

    internal class CreateParam:UserGameParam
    {
        public int Slots { get; set; }
        public int MinCpu { get; set; }
    }

    

    internal class BaseGameHost : IGameHost
    {

        protected String GameCode { get; set; }
        protected ConnectParam ConnectParam { get; set; }
        protected  readonly SshClient Client { get; set; };
        public BaseGameHost(ConnectParam connectParam,String GameCode)
        {
            this.GameCode = GameCode;
            Client = new SshClient(connectParam.SshIp, connectParam.SshPort, connectParam.SshUserName, connectParam.SshPassword);
        }


        public GameResult Connect()
        {
            Client.Connect();
            return new GameResult();
        }

        public GameResult Disconnect()
        {
            Client.Disconnect();
            return new GameResult();
        }

        public GameResult Create(CreateParam param)
        {
            var resModel = base.Go(param);
            var run = $"cd /host/{GameName}/serverfiles/{GameCode}/cfg;cp -r {GameCode}-server.cfg cure{UserId}.cfg";
            var res = Client.RunCommand(run);
            resModel.Data = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
            return resModel;
        }

        public GameResult ChangeStatus(ChangeStatusParam param)
        {
            var resModel = base.On(param);
            var run = $"cd /host/{GameName};screen -dmS server_start_{GameCode}{UserId} " +
                      $"./{GameName} start -servicename {GameCode}{UserId} -port {param.GamePort} -clientport {param.GamePort + 1};";
            var res = Client.RunCommand(run);
            resModel.Data = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Result;
            return resModel;
        }

        public StateGameResult GetState(UserGameParam userGameParam)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<DataReceivedModel> DataReceived;
        public void OpenConsole(GameHostParam param)
        {
            throw new NotImplementedException();
        }

        public string CloseConsole(GameHostParam param)
        {
            throw new NotImplementedException();
        }

        public string SendConsoleCommand(string command, bool waitAll = false)
        {
            throw new NotImplementedException();
        }
    }

    internal class CureGame : BaseGameHost
    {
        public CureGame(ConnectParam param) :base(param, "cure")
        {
            
        }
    }


}
