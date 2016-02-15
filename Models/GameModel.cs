using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crytex.GameServers.Models
{
    public class GameModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
    }
}
