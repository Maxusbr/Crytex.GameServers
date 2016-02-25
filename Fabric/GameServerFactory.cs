using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Enums;
using Crytex.GameServers.Games;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;

namespace Crytex.GameServers.Fabric
{
    public class GameServerFactory
    {
        private static GameServerFactory _instance;

        public static GameServerFactory Instance => _instance ?? (_instance = new GameServerFactory());

        public GameServerFactory()
        {
            _instance = this;
        }
        public IGameHost Get(ConnectParam param)
        {
            switch (param.FamilyGame)
            {
                case FamilyGame.Ark:
                    return new Ark(param);
                case FamilyGame.Arma3:
                    return new Arma3(param);
                case FamilyGame.Cs:
                    return new Cs(param);
                case FamilyGame.Css:
                    return new Css(param);
                case FamilyGame.CsGo:
                    return new CsGo(param);
                case FamilyGame.Dods:
                    return new Dods(param);
                case FamilyGame.GMod:
                    return new Gmod(param);
                case FamilyGame.L4D:
                    return new L4D(param);
                case FamilyGame.L4D2:
                    return new L4d2(param);
                case FamilyGame.Minecraft:
                    return new Minecraft(param);
                case FamilyGame.SaMp:
                    return new SAMP(param);
                case FamilyGame.T2F:
                    return new TF2(param);
                default:
                    throw new ArgumentOutOfRangeException(nameof(param.FamilyGame), param.FamilyGame, null);
            }
        }

    }
}
