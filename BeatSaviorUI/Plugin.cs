using System;
using System.Reflection;
using BeatSaviorUI.Installers;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using JetBrains.Annotations;
using SiraUtil.Zenject;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaviorUI
{
	[Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable, UsedImplicitly]
	internal class Plugin
	{
		public static Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();
		public static PluginMetadata Metadata { get; private set; }
		public static IPALogger Log { get; private set; }
		public static bool Fish { get; private set; }

		[Init]
		public Plugin(IPALogger logger, IPAConfig ipaConfig, Zenjector zenjector, PluginMetadata pluginMetadata) 
		{
			Log = logger; 
			Metadata = pluginMetadata;
			
			zenjector.Install<AppInstaller>(Location.App, ipaConfig.Generated<PluginConfig>());
			zenjector.Install<MenuInstaller>(Location.Menu);
			zenjector.Install<PlayerInstaller>(Location.Player);
			
			var dateTime = IPA.Utilities.Utils.CanUseDateTimeNowSafely ? DateTime.Now : DateTime.UtcNow;
			Fish = dateTime is { Day: 1, Month: 4 };
		}
	}
}
