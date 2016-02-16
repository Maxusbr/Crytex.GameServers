using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Models;

namespace Crytex.GameServers.Interface
{
    public interface IGameServer : IDisposable
    {
        void Go(ConnectParam param);
        void On(ConnectParam param);
        void Off(ConnectParam param);
    }
}
