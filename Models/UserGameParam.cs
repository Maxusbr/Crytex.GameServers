﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crytex.GameServers.Models
{
    public class UserGameParam
    {
        public string GameServerId { get; set; }
        public int GamePort { get; set; }
        public string GamePassword { get; set; }
    }
}
