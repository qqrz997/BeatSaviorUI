using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;

namespace BeatSaviorUI.UI
{
	[UsedImplicitly]
	internal class SettingsMenu
	{
		private PluginConfig Config { get; }
		
		public SettingsMenu(PluginConfig config) => Config = config;

		[UIValue("EnableUI")]
		private bool Enabled
		{
			get => Config.Enabled;
			set => Config.Enabled = value;
		}

		[UIValue("HidePauseCount")]
		private bool HidePauseCount
		{
			get => Config.HidePauseCount;
			set => Config.HidePauseCount = value;
		}

		[UIValue("DisableGraphPanel")]
		private bool DisableGraphPanel
		{
			get => Config.DisableGraphPanel;
			set => Config.DisableGraphPanel = value;
		}
	}
}
