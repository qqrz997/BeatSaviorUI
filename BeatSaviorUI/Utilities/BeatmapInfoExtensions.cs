using BeatSaviorUI.Models;
using UnityEngine;

namespace BeatSaviorUI.Utilities;

internal static class BeatmapInfoExtensions
{
    public static (string diffName, Color diffColor) GetDifficultyNameAndColor(this BeatmapInfo beatmapInfo) =>
        beatmapInfo.SongDifficulty switch
        {
            "easy" => ("Easy", new (0x3c, 0xb3, 0x71, 0xFF)),
            "normal" => ("Normal", new(0x59, 0xb0, 0xf4, 0xFF)),
            "hard" => ("Hard", new(0xFF, 0xa5, 0x00, 0xFF)),
            "expert" => ("Expert", new(0xbf, 0x2a, 0x42, 0xFF)),
            "expertplus" => ("Expert+", new(0x8f, 0x48, 0xdb, 0xFF)),
            _ => ("Unknown", new Color32(0xFF, 0xFF, 0xFF, 0xFF))
        };
}