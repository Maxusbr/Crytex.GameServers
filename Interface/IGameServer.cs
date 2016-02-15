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
        void Go(string userId, string id);
        void On(string id, int slots);
        void Off(string id);
        GameServerModel FindById(string id);
    }
}
