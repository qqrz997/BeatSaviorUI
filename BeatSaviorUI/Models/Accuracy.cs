using System;
using BeatSaviorUI.Utilities;

namespace BeatSaviorUI.Models;

internal struct Accuracy
{
    
    public float Before { get; }
    public float Center { get; }
    public float After { get; }

    public Accuracy(float before, float center, float after)
    {
        Before = before;
        Center = center;
        After = after;
    }
    
    public float Sum() => Before + Center + After;
	
    public static Accuracy operator +(Accuracy l, ValueTuple<float, float, float> r) =>
        new(l.Before + r.Item1,
            l.Center + r.Item2,
            l.After + r.Item3);
	
    public static Accuracy operator /(Accuracy l, float r) => 
        new(SafeMath.Divide(l.Before, r),
            SafeMath.Divide(l.Center, r),
            SafeMath.Divide(l.After, r));
}