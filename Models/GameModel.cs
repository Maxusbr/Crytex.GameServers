﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.Model.Enums;

namespace Crytex.GameServers.Models
{
    public class GameModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public GameFamily Family { get; set; }
        public string SshUserName { get; set; }
        public string SshPassword { get; set; }
    }
}
