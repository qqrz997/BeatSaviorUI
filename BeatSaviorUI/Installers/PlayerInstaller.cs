using BeatSaviorUI.Stats;
using JetBrains.Annotations;
using Zenject;

namespace BeatSaviorUI.Installers;

[UsedImplicitly]
internal class PlayerInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<SongData>().AsSingle().NonLazy();
        Container.BindInterfacesTo<SongDataManager>().AsSingle();
    }
}