namespace BeatSaviorUI.Stats;

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