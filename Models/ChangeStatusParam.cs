using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.Model.Enums;

namespace Crytex.GameServers.Models
{
    public class ChangeStatusParam : UserGameParam
    {
        public GameHostTypeStatus TypeStatus { get; set; }
    }
}
