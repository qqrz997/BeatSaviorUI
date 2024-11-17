using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatSaviorUI
{
    [UsedImplicitly]
    internal class PluginConfig
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual bool HidePauseCount { get; set; } = false;
        public virtual bool DisableGraphPanel { get; set; } = false;
    }
}