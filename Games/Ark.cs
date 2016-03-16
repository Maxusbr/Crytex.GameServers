using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crytex.GameServers.Interface;
using Crytex.GameServers.Models;
using Renci.SshNet;

namespace Crytex.GameServers.Games
{
    public class Ark : BaseGameHost
    {
        public Ark(ConnectParam param) : base(param, "ShooterGame") {  }

        public override GameResult Create(CreateParam param)
        {
            UserId = param.UserId;
            var host = $"cd {Path}/{GameName}/serverfiles/{GameCode}/Saved/Config/LinuxServer;";
            Client.RunCommand(host + $"chmod 777 {GameName}{UserId}.ini;");
            Client.RunCommand(host + "echo  \"[ServerSettings]\nAllowFlyerCarryPvE=False\nAllowThirdPersonPlayer=False\n" +
                              "AlwaysNotifyPlayerLeft=False\nAutoSavePeriodMinutes=15.000000\nClampResourceHarvestDamage=False\n" +
                              "DifficultyOffset=0.200000\nDisableStructureDecayPvE=False\nDontAlwaysNotifyPlayerJoined=False\n" +
                              "EnablePvPGamma=False\nGlobalVoiceChat=False\nKickIdlePlayersPeriod=2400.000000\nNoTributeDownloads=False\n" +
                              "ProximityChat=False\nProximityVoiceChat=False\nPvEStructureDecayDestructionPeriod=0.000000\n" +
                              "RCONEnabled=True\nRCONPort=32330\nServerAdminPassword=adminpassword\nServerCrosshair=False\n" +
                              "ServerForceNoHUD=False\nServerHardcore=False\nServerPassword=\nServerPVE=False\nShowMapPlayerLocation=False\n" +
                              "TamedDinoDamageMultiplier=1.000000\nTamedDinoResistanceMultiplier=1.000000\n" +
                              "[/Script/ShooterGame.ShooterGameUserSettings]\nMasterAudioVolume=1.000000\nMusicAudioVolume=1.000000\n" +
                              "SFXAudioVolume=1.000000\nVoiceAudioVolume=1.000000\nCameraShakeScale=1.000000\nbFirstPersonRiding=False\n" +
                              "bThirdPersonPlayer=False\nbShowStatusNotificationMessages=True\nTrueSkyQuality=0.270000\n" +
                              "FOVMultiplier=1.000000\nGroundClutterDensity=1.000000\nbFilmGrain=False\nbMotionBlur=True\nbUseDFAO=True\n" +
                              "bUseSSAO=True\nbShowChatBox=True\nbCameraViewBob=True\nbInvertLookY=False\nbFloatingNames=True\n" +
                              "bChatBubbles=True\nbHideServerInfo=False\nbJoinNotifications=False\nbCraftablesShowAllItems=True\n" +
                              "LookLeftRightSensitivity=1.000000\nLookUpDownSensitivity=1.000000\nGraphicsQuality=2\n" +
                              "ActiveLingeringWorldTiles=10\nClientNetQuality=3\nLastServerSearchType=0\nLastServerSearchHideFull=False\n" +
                              "LastServerSearchProtected=False\nHideItemTextOverlay=False\nbDistanceFieldShadowing=True\nLODScalar=1.000000\n" +
                              "HighQualityMaterials=True\nHighQualitySurfaces=True\nbTemperatureF=False\nbDisableTorporEffect=False\n" +
                              "bChatShowSteamName=False\nbChatShowTribeName=True\nEmoteKeyBind1=0\nEmoteKeyBind2=0\nbUseVSync=False\n" +
                              "ResolutionSizeX=1280\nResolutionSizeY=720\nLastUserConfirmedResolutionSizeX=1280\n" +
                              "LastUserConfirmedResolutionSizeY=720\nWindowPosX=-1\nWindowPosY=-1\nbUseDesktopResolutionForFullscreen=False\n" +
                              "FullscreenMode=2\nLastConfirmedFullscreenMode=2\nVersion=5\n\n[ScalabilityGroups]\nsg.ResolutionQuality=100\n" +
                              "sg.ViewDistanceQuality=3\nsg.AntiAliasingQuality=3\nsg.ShadowQuality=3\nsg.PostProcessQuality=3\n" +
                              "sg.TextureQuality=3\nsg.EffectsQuality=3\nsg.TrueSkyQuality=3\nsg.GroundClutterQuality=3\nsg.IBLQuality=1\n" +
                              $"sg.HeightFieldShadowQuality=3\n\" > {GameName}{UserId}.ini;");
            Client.RunCommand(host + $"echo \"[SessionSettings]\nSessionName=arkserver\nQueryPort={param.GamePort+1}\nPort={param.GamePort}\n" +
                              $"[/Script/Engine.GameSession]\nMaxPlayers=127\n[MessageOfTheDay]\nMessage=Welcome to ARK Server\n" +
                              $"Duration=5\n\" >> {GameName}{UserId}.ini;");

            var result = new GameResult();
            return result;
        }
    }
}
