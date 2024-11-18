using BeatSaviorUI.UI;
using HMUI;
using JetBrains.Annotations;
using SiraUtil.Affinity;

namespace BeatSaviorUI.HarmonyPatches
{
	[UsedImplicitly]
	internal class SoloFreePlayFlowCoordinatorPatch : IAffinity
	{
		private PluginConfig PluginConfig { get; }
		private EndOfLevelViewController EndOfLevelViewController { get; }
		private ScoreGraphViewController ScoreGraphViewController { get; }

		public SoloFreePlayFlowCoordinatorPatch(EndOfLevelViewController endOfLevelViewController, ScoreGraphViewController scoreGraphViewController, PluginConfig pluginConfig)
		{
			EndOfLevelViewController = endOfLevelViewController;
			ScoreGraphViewController = scoreGraphViewController;
			PluginConfig = pluginConfig;
		}

		[AffinityPatch(typeof(SoloFreePlayFlowCoordinator), nameof(SoloFreePlayFlowCoordinator.ProcessLevelCompletionResultsAfterLevelDidFinish))]
		private void Postfix(ref SoloFreePlayFlowCoordinator __instance, LevelCompletionResults levelCompletionResults, BeatmapLevel beatmapLevel)
		{
			if (levelCompletionResults.levelEndAction != LevelCompletionResults.LevelEndAction.None)
			{
				return;
			}

			// Show end of song UI
			Plugin.Log.Debug($"Showing {nameof(ResultsViewController)}");
			
			__instance.SetLeftScreenViewController(EndOfLevelViewController, ViewController.AnimationType.None);
			EndOfLevelViewController.Refresh(PluginConfig.LastKnownPlayData, beatmapLevel);

			if (!PluginConfig.DisableGraphPanel)
			{
				__instance.SetRightScreenViewController(ScoreGraphViewController, ViewController.AnimationType.None);
				ScoreGraphViewController.Refresh(PluginConfig.LastKnownPlayData);
			}
		}
	}
}