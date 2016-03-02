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
        StreamWriter Writer { get;set; }
        void Go(GameHostParam param);
        void On(GameHostParam param);
        void Off(GameHostParam param);
        string Monitor(GameHostParam param);
        string OpenConsole(GameHostParam param);
        string CloseConsole(GameHostParam param);
        string SendConsoleCommand(GameHostParam param);
    }
}
