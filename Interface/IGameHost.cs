using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Models;

namespace Crytex.GameServers.Interface
{
    public interface IGameHost
    {
        bool IsCompleteInstall { get; }
        GameResult Connect();
        GameResult Disconnect();
        GameResult Create(CreateParam param);//Go
        GameResult ChangeStatus(ChangeStatusParam param);//On/of

        StateGameResult GetState(UserGameParam userGameParam);
        AdvancedStateGameResult GetAdvancedState(UserGameParam userGameParam);

        bool OpenConsole(UserGameParam param, string openCommand = "");
        string CloseConsole(UserGameParam param, string closeCommand = "");
        string SendConsoleCommand(string command, bool waitAll = false);

        event EventHandler<string> ConsoleDataReceived;
    }
}
