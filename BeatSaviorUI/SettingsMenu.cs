using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Util;
using BS_Utils.Utilities;

namespace BeatSaviorUI
{
	class SettingsMenu : PersistentSingleton<SettingsMenu>
	{
		private static readonly Config config = new Config(Plugin.Name);

		[UIValue("HideNbOfPauses")]
		public bool HideNbOfPauses
		{
			get => config.GetBool(Plugin.Name, "HideNbOfPauses", false, true);
			set => config.SetBool(Plugin.Name, "HideNbOfPauses", value);
		}

		[UIValue("EnableUI")]
		public bool EnableUI
		{
			get => config.GetBool(Plugin.Name, "EnableUI", true, true);
			set => config.SetBool(Plugin.Name, "EnableUI", value);
		}

		[UIValue("DisableGraphPanel")]
		public bool DisableGraphPanel
		{
			get => config.GetBool(Plugin.Name, "DisableGraphPanel", false, true);
			set => config.SetBool(Plugin.Name, "DisableGraphPanel", value);
		}

		public bool EnableCustomUrlUpload
		{
			get => config.GetBool(Plugin.Name, "EnableCustomUrlUpload", false, true);
			set => config.SetBool(Plugin.Name, "EnableCustomUrlUpload", value);
		}

		public string CustomUploadUrl
		{
			get => config.GetString(Plugin.Name, "CustomUploadUrl", "", true);
			set => config.GetString(Plugin.Name, "CustomUploadUrl", value);
		}
	}
}
