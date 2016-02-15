using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crytex.GameServers.Models
{
    public class ConnectParam
    {
        public string UserId { get; set; }
        public string GameId { get; set; }
        public string ServerId { get; set; }
        public int Slots { get; set; }
    }
}
