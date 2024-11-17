using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Zenject;

namespace BeatSaviorUI.Stats
{
	public enum SongDataType
	{
		none,
		pass,
		fail,
		practice,
		replay,
		campaign
	}

	public record SongInfo(
		SongDataType SongDataType,
		string SongID,
		string SongDifficulty,
		string SongName,
		string SongArtist,
		string SongMapper,
		string GameMode,
		int SongDifficultyRank,
		float SongSpeed,
		float SongDuration,
		float SongJumpDistance);
	
	[UsedImplicitly]
	internal class SongData : IInitializable
	{
		private GameplayCoreSceneSetupData GameplayCoreSceneSetupData { get; }
		private AudioTimeSyncController AudioTimeSyncController { get; }
		
		public SongData(GameplayCoreSceneSetupData gameplayCoreSceneSetupData, AudioTimeSyncController audioTimeSyncController)
		{
			GameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
			AudioTimeSyncController = audioTimeSyncController;
		}
		
		public static SongInfo SongInfo { get; private set; }
		
		private static bool ScoreSaberPlaybackEnabled =>
			AccessTools.Method("ScoreSaber.Core.ReplaySystem.HarmonyPatches.PatchHandleHMDUnmounted:Prefix") != null;	

		public void Initialize()
		{
			string songID = GameplayCoreSceneSetupData.beatmapKey.levelId.Replace("custom_level_","").Split('_')[0];
			string songDifficulty = GameplayCoreSceneSetupData.beatmapKey.difficulty.ToString().ToLower();
			string gameMode = GameplayCoreSceneSetupData.beatmapKey.beatmapCharacteristic.serializedName;

			int songDifficultyRank = GameplayCoreSceneSetupData.beatmapKey.beatmapCharacteristic.sortingOrder;
			string songName = GameplayCoreSceneSetupData.beatmapLevel.songName;
			string songArtist = GameplayCoreSceneSetupData.beatmapLevel.songAuthorName;
			string songMapper = GameplayCoreSceneSetupData.beatmapLevel.allMappers.FirstOrDefault() ?? "Unknown";

			float songDuration = AudioTimeSyncController.songLength;

			SongDataType songDataType;
			float songSpeed = 1;
			float songStartTime = 0;
			if (GameplayCoreSceneSetupData.practiceSettings != null) {
				songDataType = SongDataType.practice;
				songSpeed = GameplayCoreSceneSetupData.practiceSettings.songSpeedMul;
				songStartTime = GameplayCoreSceneSetupData.practiceSettings.startSongTime;
			}
			else
			{
				songDataType = ScoreSaberPlaybackEnabled ? SongDataType.replay : SongDataType.none;
			}

			float songJumpDistance = GameplayCoreSceneSetupData.beatmapBasicData.noteJumpStartBeatOffset;

			SongInfo = new(songDataType, songID, songDifficulty, songName, songArtist, songMapper, gameMode, songDifficultyRank, songSpeed, songDuration, songJumpDistance);
		}
	}
}
