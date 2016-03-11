using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Models;
using Crytex.Model.Exceptions;

namespace Crytex.GameServers.Example
{
    internal interface IGameHost 
    {
        void Connect();
        void Disconnect();

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
        public bool Succes;
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

    

    internal class GameHostExample : IGameHost
    {

        GameHostExample(ConnectParam connectParam)
        {
            
        }


        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public GameResult Create(CreateParam param)
        {
            throw new NotImplementedException();
        }

        public GameResult ChangeStatus(ChangeStatusParam param)
        {
            throw new NotImplementedException();
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

}
