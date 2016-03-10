
using Crytex.Model.Enums;

namespace Crytex.GameServers.Models
{
    public class ConnectParam
    {
        public string SshIp { get; set; }
        public int SshPort { get; set; }
        public string SshUserName { get; set; }
        public string SshPassword { get; set; }
        public GameFamily FamilyGame { get; set; }
        public string GameName { get; set; }
    }
}
