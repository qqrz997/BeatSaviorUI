using BeatSaviorUI.HarmonyPatches;
using BeatSaviorUI.UI;
using JetBrains.Annotations;
using Zenject;

namespace BeatSaviorUI.Installers
{
    [UsedImplicitly]
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<SettingsMenu>().AsSingle();
            Container.BindInterfacesTo<SettingsMenuManager>().AsSingle();
            
            Container.Bind<EndOfLevelViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ScoreGraphViewController>().FromNewComponentAsViewController().AsSingle();
            
            Container.BindInterfacesTo<SoloFreePlayFlowCoordinatorPatch>().AsSingle();
        }
    }
}