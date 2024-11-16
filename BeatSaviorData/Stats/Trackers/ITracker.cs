namespace BeatSaviorData
{
	public interface ITracker
	{
		void EndOfSong(LevelCompletionResults results, SongData data);
	}
}
