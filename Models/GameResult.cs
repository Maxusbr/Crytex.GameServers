using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.Model.Exceptions;

namespace Crytex.GameServers.Models
{
    public class GameResult
    {
        public bool Succes = true;
        public GameHostTypeError? Error { get; set; }
        public string ErrorMessage { get; set; }
        public string Data { get; set; }
    }
}
