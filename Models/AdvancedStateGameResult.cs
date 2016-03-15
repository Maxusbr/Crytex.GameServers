using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crytex.GameServers.Models
{
    public class AdvancedStateGameResult : StateGameResult
    {
        public string Data { get; set; }
        public List<ServerStateModel> ServerStates { get; set; }
        public TableDataModel TableInfo { get; set; }
    }
}
