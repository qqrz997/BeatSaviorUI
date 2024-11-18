namespace BeatSaviorUI.Models;

public record BeatmapInfo(
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