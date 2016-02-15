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

        public IGameServer CreateGameServer(FamilyGame family, ConnectParam param)
        {
            // TODO var game = _db.Games.FindById(param.GameId);
            IGameServer server = null;
            switch (family)
            {
                case FamilyGame.CssOld:
                    server = new CssOld();
                    break;
                case FamilyGame.Cs:
                    server = new Cs();
                    break;
                case FamilyGame.Css:
                    server = new Css();
                    break;
                case FamilyGame.CsGo:
                    server = new CsGo();
                    break;
                case FamilyGame.Dods:
                    server = new Dods();
                    break;
                case FamilyGame.GMod:
                    server = new Gmod();
                    break;
                case FamilyGame.L4D:
                    server = new L4D();
                    break;
                case FamilyGame.Minecraft:
                    server = new Minecraft();
                    break;
                case FamilyGame.SaMp:
                    server = new SAMP();
                    break;
                case FamilyGame.T2F:
                    server = new TF2();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(family), family, null);
            }
            server?.Go(param.UserId, param.GameId);
            // TODO Save bought server to Database

            return server;
        }

        public void GameServerOn(ConnectParam param)
        {
            // TODO var server = _db.Servers.FindById(param.ServerId);
            IGameServer server = null;

            server?.On(param.ServerId, param.Slots);
        }

    }
}
