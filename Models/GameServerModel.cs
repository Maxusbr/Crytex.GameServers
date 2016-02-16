using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crytex.GameServers.Models
{
    public class GameServerModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public int MinCpu { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public Guid GameId { get; set; }

        [ForeignKey("GameId")]
        public virtual GameModel Game { get; set; }
    }
}
