using System;
using BeatSaviorUI.Installers;
using BeatSaviorUI.Stats;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using JetBrains.Annotations;
using SiraUtil.Zenject;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaviorUI
{
	[UsedImplicitly]
	[Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
	public class Plugin
	{
		internal static string Name => nameof(BeatSaviorUI);
		internal static bool fish;

		private Harmony harmony;

		[Init]
		public void Init(IPALogger logger, IPAConfig ipaConfig, Zenjector zenjector) 
		{
			Logger.log = logger; 
			
			zenjector.Install<AppInstaller>(Location.App, ipaConfig.Generated<PluginConfig>());
			zenjector.Install<MenuInstaller>(Location.Menu);
			zenjector.Install<PlayerInstaller>(Location.Player);
			
			UserIDFix.GetUserID(); 
		}

		[OnStart]
		public void OnApplicationStart()
		{
			fish = DateTime.Now.Month == 4 && DateTime.Now.Day == 1;

			SceneManager.activeSceneChanged += OnActiveSceneChanged;

			harmony = new Harmony(Name);

			// Patch Classes
			harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
		}

        private void UploadStats(ScenesTransitionSetupDataSO obj)
		{ 
			new PlayerStats();	// Get and upload player related stats
		}

		[OnExit]
		public void OnApplicationExit()
		{
			harmony.UnpatchSelf();

			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

		private void UploadData(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results, bool isCampaign)
		{
			/*if (songData != null && !songData.IsAReplay() && results.levelEndAction == LevelCompletionResults.LevelEndAction.None)
			{
				FileManager.SaveSongStats(songData.GetDeepTrackersResults());
				FileManager.SavePBScoreGraph(
					((ScoreGraphTracker)songData.trackers["scoreGraphTracker"]).graph,
					((ScoreTracker)songData.trackers["scoreTracker"]).score, songData.songID);#1#
			}*/
		}

		private void UploadSoloData(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results) =>
			UploadData(data, results, false);

		private void UploadCampaignData(MissionLevelScenesTransitionSetupDataSO data, MissionCompletionResults results) =>
			UploadData(null, results.levelCompletionResults, true);

		public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
		{
			if (nextScene.name == "GameCore")
			{
				/*if (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer)
					return;*/
                    
				foreach(MissionLevelScenesTransitionSetupDataSO m in Resources.FindObjectsOfTypeAll<MissionLevelScenesTransitionSetupDataSO>())
				{
					m.didFinishEvent -= this.UploadCampaignData;
					m.didFinishEvent += this.UploadCampaignData;
				}
			}
		}
	}
}
