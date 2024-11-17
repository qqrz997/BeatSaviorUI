using System;
using BeatSaberMarkupLanguage.Settings;
using JetBrains.Annotations;
using Zenject;

namespace BeatSaviorUI.UI
{
    [UsedImplicitly]
    internal class SettingsMenuManager : IInitializable, IDisposable
    {
        private BSMLSettings BsmlSettings { get; }
        private SettingsMenu SettingsMenu { get; }
        
        public SettingsMenuManager(BSMLSettings bsmlSettings, SettingsMenu settingsMenu) =>
            (BsmlSettings, SettingsMenu) = (bsmlSettings, settingsMenu);

        private const string MenuName = nameof(BeatSaviorUI);
        private const string ResourcePath = nameof(BeatSaviorUI) + ".UI.Views.SettingsView.bsml";
        
        public void Initialize()
        {
            BsmlSettings.AddSettingsMenu(MenuName, ResourcePath, SettingsMenu);
        }

        public void Dispose()
        {
            BsmlSettings.RemoveSettingsMenu(SettingsMenu);
        }
    }
}