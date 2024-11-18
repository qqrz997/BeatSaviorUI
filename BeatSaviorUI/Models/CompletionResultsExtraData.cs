using System.Collections.Generic;

namespace BeatSaviorUI.Models;

internal class CompletionResultsExtraData(List<Note> notes, int maxCombo, int bombHitCount, int pauseCount, int wallHitCount)
{
    public List<Note> Notes { get; } = notes;
    public int MaxCombo { get; } = maxCombo;
    public int BombHitCount { get; } = bombHitCount;
    public int PauseCount { get; } = pauseCount;
    public int WallHitCount { get; } = wallHitCount;
}