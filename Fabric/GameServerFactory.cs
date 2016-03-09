using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Games;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Crytex.Model.Enums;

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
                case GameFamily.Ark:
                    return new Ark(param);
                case GameFamily.Arma3:
                    return new Arma3(param);
                case GameFamily.Cs:
                    return new Cs(param);
                case GameFamily.Css:
                    return new Css(param);
                case GameFamily.CsGo:
                    return new CsGo(param);
                case GameFamily.Cure:
                    return new Cure(param);
                case GameFamily.Dods:
                    return new Dods(param);
                case GameFamily.GMod:
                    return new Gmod(param);
                case GameFamily.L4D:
                    return new L4D(param);
                case GameFamily.L4D2:
                    return new L4d2(param);
                case GameFamily.Minecraft:
                    return new Minecraft(param);
                case GameFamily.SaMp:
                    return new SAMP(param);
                case GameFamily.T2F:
                    return new TF2(param);
                case GameFamily.Bmdm:
                    return new Bmdm(param);
                case GameFamily.Cscz:
                    return new Cscz(param);
                case GameFamily.Insurgency:
                    return new Insurgency(param);
                case GameFamily.JustCause2:
                    return new JustCause2(param);
                default:
                    throw new ArgumentOutOfRangeException(nameof(param.FamilyGame), param.FamilyGame, null);
            }
        }

    }
}
