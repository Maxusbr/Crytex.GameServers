using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crytex.GameServers.Models
{
    public class CreateParam : UserGameParam
    {
        public int Slots { get; set; }
        public int MinCpu { get; set; }
    }
}
