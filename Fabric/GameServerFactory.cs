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

        public IGameServer CreateGameServer(ConnectParam param)
        {
            // TODO var game = _db.Games.FindById(param.GameId);
            // FamilyGame family = game.Family;
            var family = FamilyGame.Cs;
            var server = Get(family);
            server?.Go(param.UserId, param.GameId);
            // TODO Save bought server to Database

            return server;
        }

        public void GameServerOn(ConnectParam param)
        {
            // TODO var boughtserver = _db.Servers.FindById(param.ServerId);
            // var family = boughtserver.Game;
            var family = FamilyGame.Minecraft;
            var server = Get(family);

            server?.On(param.ServerId, param.Slots);
        }

        public void GameServerOff(ConnectParam param)
        {
            // TODO var boughtserver = _db.Servers.FindById(param.ServerId);
            // var family = boughtserver.Game;
            var family = FamilyGame.Minecraft;
            var server = Get(family);

            server?.Off(param.ServerId);
        }
        private IGameServer Get(FamilyGame family)
        {
            switch (family)
            {
                case FamilyGame.CssOld:
                    return new CssOld();
                case FamilyGame.Cs:
                    return new Cs();
                case FamilyGame.Css:
                    return new Css();
                case FamilyGame.CsGo:
                    return new CsGo();
                case FamilyGame.Dods:
                    return new Dods();
                case FamilyGame.GMod:
                    return new Gmod();
                case FamilyGame.L4D:
                    return new L4D();
                case FamilyGame.Minecraft:
                    return new Minecraft();
                case FamilyGame.SaMp:
                    return new SAMP();
                case FamilyGame.T2F:
                    return new TF2();
                default:
                    throw new ArgumentOutOfRangeException(nameof(family), family, null);
            }
        }

    }
}
