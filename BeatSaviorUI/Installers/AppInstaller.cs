using JetBrains.Annotations;
using Zenject;

namespace BeatSaviorUI.Installers
{
    [UsedImplicitly]
    internal class AppInstaller : Installer
    {
        private PluginConfig PluginConfig { get; }
        
        public AppInstaller(PluginConfig pluginConfig) => PluginConfig = pluginConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(PluginConfig).AsSingle();
        }
    }
}