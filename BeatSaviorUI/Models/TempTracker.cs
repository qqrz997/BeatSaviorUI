using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaviorUI.Stats;
using BeatSaviorUI.Utilities;
using UnityEngine;

namespace BeatSaviorUI.Models;

public class TempTracker
{
    public int RawScore { get; }
    public int Score { get; }
    public float ModifiedRatio { get; }
    public float ModifiersMultiplier { get; }
    public List<string> Modifiers { get; } = [];

    public bool Won { get; }
    public string Rank { get; }
    public float EndTime { get; }
    public int PauseCount { get; }
    
    public Dictionary<float, float> Graph { get; } = [];
    
    public float AccRight { get; }
    public float AccLeft { get; }
    public float LeftSpeed { get; }
    public float RightSpeed { get; }
    public float LeftHighestSpeed { get; }
    public float RightHighestSpeed { get; }
    public float LeftPreSwing { get; }
    public float RightPreSwing { get; }
    public float LeftPostSwing { get; }
    public float RightPostSwing { get; }
    public float LeftTimeDependence { get; }
    public float RightTimeDependence { get; }
    public float[] LeftAverageCut { get; } = new float[3];
    public float[] RightAverageCut { get; } = new float[3];
    public float[] AverageCut { get; } = new float[3];
    public float[] GridAcc { get; } = new float[12];
    public int[] GridCut { get; } = new int[12];
    private int CutRight { get; }
    private int CutLeft { get; }

    public int LeftNoteHitCount { get; }
    public int RightNoteHitCount { get; }
    public int BombHitCount { get; }
    public int WallHitCount { get; }
    public int MaxCombo { get; }

    public int ComboBreaks { get; }
    public int Misses { get; }
    public int BadCuts { get; }
    public int LeftMisses { get; }
    public int LeftBadCuts { get; }
    public int RightMisses { get; }
    public int RightBadCuts { get; }

    public List<Note> Notes { get; }
    
    public SongInfo SongInfo { get; }
	
    public bool FullCombo => ComboBreaks == 0 && BombHitCount == 0 && WallHitCount == 0;
    
    public TempTracker(LevelCompletionResults levelCompletionResults, List<Note> notes, int maxCombo, int bombHitCount, int pauseCount, int wallHitCount, BeatmapKey beatmapKey, PlayerData playerData)
    {
	    Notes = notes;
        MaxCombo = maxCombo;
        BombHitCount = bombHitCount;
        PauseCount = pauseCount;
        WallHitCount = wallHitCount;
        
        foreach(var note in Notes)
        {
            if (note.IsAMiss)
            {
                ComboBreaks++;
                if (note.CutType == CutType.Miss)
                {
                    Misses++;
                    if (note.ColorType == ColorType.ColorA)
                        LeftMisses++;
                    else
                        RightMisses++;
                }
                else if (note.CutType == CutType.BadCut)
                {
                    BadCuts++;
                    if (note.NoteCutInfo.saberType == SaberType.SaberA)
                        LeftBadCuts++;
                    else
                        RightBadCuts++;
                }
            }
            else
            {
	            if (note.ColorType == ColorType.ColorA) LeftNoteHitCount++;
	            else if (note.ColorType == ColorType.ColorB) RightNoteHitCount++;
            }
        }
        
        Won = levelCompletionResults.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared;
        EndTime = levelCompletionResults.endSongTime;
        Rank = RankModel.GetRankName(levelCompletionResults.rank);

        ModifiersMultiplier = GetTotalMultiplier(playerData.gameplayModifiers, levelCompletionResults.energy);
        RawScore = Notes.Select(note => note.TotalScore).Sum();
        Score = Mathf.RoundToInt(RawScore * ModifiersMultiplier);
        ModifiedRatio = Score / (float)Utils.MaxRawScoreForNumberOfNotes(Notes.Count);

        var rawGraph = new Dictionary<float, float>();
        var lastGraphNodes = new Queue<float>();
        int actualScore = 0, maxScore1 = 0, multiplier = 1, multiplierProgress = 0, lastBeat = 0;
        
        foreach(var note in Notes)
		{
			actualScore += note.TotalScore;

			if (multiplier < 8)
			{
				multiplierProgress++;
				if (multiplierProgress == multiplier * 2)
				{
					multiplier *= 2;
					multiplierProgress = 0;
				}
			}

			maxScore1 += 115 * multiplier;

			float noteBeat = MathF.Truncate(note.Time);
			if (noteBeat > lastBeat)
			{
				rawGraph.Add(noteBeat, (float) actualScore / maxScore1);
				lastBeat = (int) noteBeat;
			}

			if (note.IsAMiss)
			{
				continue;
			}
			 
			int acc = note.Score[0] + note.Score[1] + note.Score[2];

			if (note.ColorType == ColorType.ColorA)
			{
				CutLeft++;
				AccLeft += acc;
				LeftAverageCut[0] += note.Score[0];
				LeftAverageCut[1] += note.Score[1];
				LeftAverageCut[2] += note.Score[2];
				LeftSpeed += note.Speed;
				LeftPreSwing += note.PreSwing;
				LeftPostSwing += note.PostSwing;
				if (note.Speed > LeftHighestSpeed)
					LeftHighestSpeed = note.Speed;
				LeftTimeDependence += note.TimeDependence;
			} 
			else
			{
				CutRight++;
				AccRight += acc;
				RightAverageCut[0] += note.Score[0];
				RightAverageCut[1] += note.Score[1];
				RightAverageCut[2] += note.Score[2];
				RightSpeed += note.Speed;
				RightPreSwing += note.PreSwing;
				RightPostSwing += note.PostSwing;
				if (note.Speed > RightHighestSpeed)
					RightHighestSpeed = note.Speed;
				RightTimeDependence += note.TimeDependence;
			}
			
			if (note.Index < 0 || note.Index >= GridCut.Length) // it's noodle
			{
				continue;
			}

			GridCut[note.Index] += 1;
			GridAcc[note.Index] += acc;
		}
        
		foreach(var point in rawGraph) {

			lastGraphNodes.Enqueue(point.Value);

			if (lastGraphNodes.Count > 5)
				lastGraphNodes.Dequeue();

			Graph.Add(point.Key, lastGraphNodes.Average());
		}

		AccRight = SafeMath.Divide(AccRight, CutRight);
		AccLeft = SafeMath.Divide(AccLeft, CutLeft);

		for (int i = 0; i < 12; i++)
			GridAcc[i] = SafeMath.Divide(GridAcc[i], GridCut[i]);

		for (int i = 0; i < 3; i++) {
			LeftAverageCut[i] = SafeMath.Divide(LeftAverageCut[i], CutLeft);
			RightAverageCut[i] = SafeMath.Divide(RightAverageCut[i], CutRight);
			AverageCut[i] = SafeMath.Average(RightAverageCut[i], CutRight, LeftAverageCut[i], CutLeft);
		}

		SafeMath.Average(AccRight, CutRight, AccLeft, CutLeft);

		SafeMath.Divide(LeftSpeed + RightSpeed, CutRight + CutLeft);
		LeftSpeed = SafeMath.Divide(LeftSpeed, CutLeft);
		RightSpeed = SafeMath.Divide(RightSpeed, CutRight);

		SafeMath.Divide(LeftTimeDependence + RightTimeDependence, CutRight + CutLeft);
		LeftTimeDependence = SafeMath.Divide(LeftTimeDependence, CutLeft);
		RightTimeDependence = SafeMath.Divide(RightTimeDependence, CutRight);

		SafeMath.Divide(LeftPreSwing + RightPreSwing, CutRight + CutLeft);
		LeftPreSwing = SafeMath.Divide(LeftPreSwing, CutLeft);
		RightPreSwing = SafeMath.Divide(RightPreSwing, CutRight);

		SafeMath.Divide(LeftPostSwing + RightPostSwing, CutRight + CutLeft);
		LeftPostSwing = SafeMath.Divide(LeftPostSwing, CutLeft);
		RightPostSwing = SafeMath.Divide(RightPostSwing, CutRight);

		SongInfo = SongData.SongInfo;
    }
    
    private float GetTotalMultiplier(GameplayModifiers gameplayModifiers, float energy)
    {
        float multiplier = 1;
        
        if (gameplayModifiers.disappearingArrows) { multiplier += 0.02f; Modifiers.Add("DA"); }
        if (gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Faster) { multiplier += 0.08f; Modifiers.Add("FS"); }
        if (gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Slower) { multiplier -= 0.3f; Modifiers.Add("SS"); }
        if (gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.SuperFast) { multiplier += 0.1f; Modifiers.Add("SF"); }
        if (gameplayModifiers.ghostNotes) { multiplier += 0.04f; Modifiers.Add("GN"); }
        if (gameplayModifiers.noArrows) { multiplier -= 0.3f; Modifiers.Add("NA"); }
        if (gameplayModifiers.noBombs) { multiplier -= 0.1f; Modifiers.Add("NB"); }
        if (gameplayModifiers.noFailOn0Energy && energy == 0) { multiplier -= 0.5f; Modifiers.Add("NF"); }
        if (gameplayModifiers.enabledObstacleType == GameplayModifiers.EnabledObstacleType.NoObstacles) { multiplier -= 0.05f; Modifiers.Add("NO"); }
        if (gameplayModifiers.zenMode) { multiplier -= 1f; Modifiers.Add("ZM"); }

        return multiplier;
    }
}