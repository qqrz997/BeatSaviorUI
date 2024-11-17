using BeatSaviorUI.Stats;
using BeatSaviorUI.UI;
using HMUI;
using JetBrains.Annotations;
using SiraUtil.Affinity;

namespace BeatSaviorUI.HarmonyPatches
{
	[UsedImplicitly]
	internal class SoloFreePlayFlowCoordinatorPatch : IAffinity
	{
		private EndOfLevelViewController EndOfLevelViewController { get; }
		private ScoreGraphViewController ScoreGraphViewController { get; }

		public SoloFreePlayFlowCoordinatorPatch(EndOfLevelViewController endOfLevelViewController, ScoreGraphViewController scoreGraphViewController)
		{
			EndOfLevelViewController = endOfLevelViewController;
			ScoreGraphViewController = scoreGraphViewController;
		}

		[AffinityPatch(typeof(SoloFreePlayFlowCoordinator), nameof(SoloFreePlayFlowCoordinator.ProcessLevelCompletionResultsAfterLevelDidFinish))]
		private void Postfix(ref SoloFreePlayFlowCoordinator __instance, LevelCompletionResults levelCompletionResults, BeatmapLevel beatmapLevel)
		{
			if (levelCompletionResults.levelEndAction != LevelCompletionResults.LevelEndAction.None)
			{
				return;
			}

			// Show end of song UI
			Logger.log.Info("Showing level results view controller");
			
			__instance.SetLeftScreenViewController(EndOfLevelViewController, ViewController.AnimationType.None);
			__instance.SetRightScreenViewController(ScoreGraphViewController, ViewController.AnimationType.None);
			
			EndOfLevelViewController.Refresh(SongDataManager.Tracker, beatmapLevel);
			ScoreGraphViewController.Refresh(SongDataManager.Tracker);
		}
	}
}