using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crytex.GameServers.Interface
{
    public interface ISshCommand
    {
        void Go(string userId, string id);
        void On(string userId, string id);
        void Off(string userId, string id);
    }
}
