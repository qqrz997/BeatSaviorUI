using System.Collections.Generic;

namespace BeatSaviorData.Trackers
{
	class NoteTracker : ITracker
	{
		public List<Note> notes;

		public void EndOfSong(LevelCompletionResults results, SongData data) {
			notes = data.GetDataCollector().notes;
		}
	}
}
