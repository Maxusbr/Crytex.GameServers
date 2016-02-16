using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Enums;

namespace Crytex.GameServers.Models
{
    public class ConnectParam
    {
        public string SshIp { get; set; }
        public int SshPort { get; set; }
        public string SshUserName { get; set; }
        public string SshPassword { get; set; }
        public string UserId { get; set; }
        public FamilyGame FamilyGame { get; set; }
        public string ServerId { get; set; }
        public string GameId { get; set; }
        public int GamePort { get; set; }
        public string GamePassword { get; set; }
        public int Slots { get; set; }
        public int MinCpu { get; set; }
        public string Url { get; set; }
    }
}
