namespace BeatSaviorData.Trackers
{
	class DistanceTracker : ITracker
	{
		public float rightSaber, leftSaber, rightHand, leftHand;

		public void EndOfSong(LevelCompletionResults results, SongData data)
		{
			rightSaber = results.rightSaberMovementDistance;
			rightHand = results.rightHandMovementDistance;
			leftSaber = results.leftSaberMovementDistance;
			leftHand = results.leftHandMovementDistance;
		}

		public void RegisterTracker(SongData data) {}
	}
}
