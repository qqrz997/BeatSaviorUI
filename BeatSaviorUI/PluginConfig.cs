using System.Runtime.CompilerServices;
using BeatSaviorUI.Models;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
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
        
        [Ignore] public static PlayData? LastKnownPlayData { get; set; }
    }
}