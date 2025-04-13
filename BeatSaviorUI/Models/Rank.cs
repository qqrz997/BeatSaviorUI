using UnityEngine;

namespace BeatSaviorUI.Models;

internal class Rank
{
    public string Name { get; }
    public Color Color { get; }

    public Rank(string rankName)
    {
        Name = rankName;
        Color = rankName switch
        {
            "SSS" or "SS" => new Color32(0x00, 0xF0, 0xFF, 0xFF),
            "A" => new(0x00, 0xFF, 0x00, 0xFF),
            "B" => new(0xFF, 0xFF, 0x00, 0xFF),
            "C" => new(0xFF, 0xA7, 0x00, 0xFF),
            "D" or "E" => new(0xFF, 0x00, 0x00, 0xFF),
            _ => new(0xFF, 0xFF, 0xFF, 0xFF)
        };
    }
}