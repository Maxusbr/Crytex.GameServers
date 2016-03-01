using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Models;

namespace Crytex.GameServers.Interface
{
    public interface IGameHost : IDisposable
    {
        void Go(GameHostParam param);
        void On(GameHostParam param);
        void Off(GameHostParam param);
    }
}
