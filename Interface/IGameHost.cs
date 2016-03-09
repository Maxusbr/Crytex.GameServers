using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Models;

namespace Crytex.GameServers.Interface
{
    public interface IGameHost : IDisposable
    {
        event EventHandler<DataReceivedModel> DataReceived;
        void Go(GameHostParam param);
        DataReceivedModel On(GameHostParam param);
        void Off(GameHostParam param);
        DataReceivedModel Monitor(GameHostParam param);
        void OpenConsole(GameHostParam param);
        string CloseConsole(GameHostParam param);
        string SendConsoleCommand(string command);
    }
}
