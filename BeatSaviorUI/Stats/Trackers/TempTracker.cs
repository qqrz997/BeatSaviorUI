using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeatSaviorUI.Stats.Trackers;

public class TempTracker
{
    public int RawScore { get; }
    public int Score { get; }
    public int PersonalBest { get; }
    public float RawRatio { get; }
    public float ModifiedRatio { get; }
    public float PersonalBestRawRatio { get; }
    public float PersonalBestModifiedRatio { get; }
    public float ModifiersMultiplier { get; }
    public List<string> Modifiers { get; } = [];

    public bool Won { get; }
    public string Rank { get; }
    public float EndTime { get; }
    public int PauseCount { get; }
    
    public Dictionary<float, float> Graph { get; } = [];
    
    public float AccRight { get; }
    public float AccLeft { get; }
    public float AverageAcc { get; }
    public float LeftSpeed { get; }
    public float RightSpeed { get; }
    public float AverageSpeed { get; }
    public float LeftHighestSpeed { get; }
    public float RightHighestSpeed { get; }
    public float LeftPreSwing { get; }
    public float RightPreSwing { get; }
    public float AveragePreSwing { get; }
    public float LeftPostSwing { get; }
    public float RightPostSwing { get; }
    public float AveragePostSwing { get; }
    public float LeftTimeDependence { get; }
    public float RightTimeDependence { get; }
    public float AverageTimeDependence { get; }
    public float[] LeftAverageCut { get; } = new float[3];
    public float[] RightAverageCut { get; } = new float[3];
    public float[] AverageCut { get; } = new float[3];
    public float[] GridAcc { get; } = new float[12];
    public int[] GridCut { get; } = new int[12];
    private int CutRight { get; }
    private int CutLeft { get; }

    public float RightSaber { get; }
    public float LeftSaber { get; }
    public float RightHand { get; }
    public float LeftHand { get; }

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
            if (note.IsAMiss())
            {
                ComboBreaks++;
                if (note.cutType == CutType.miss)
                {
                    Misses++;
                    if (note.noteType == BSDNoteType.left)
                        LeftMisses++;
                    else
                        RightMisses++;
                }
                else if (note.cutType == CutType.badCut)
                {
                    BadCuts++;
                    if (note.GetInfo().saberType == SaberType.SaberA)
                        LeftBadCuts++;
                    else
                        RightBadCuts++;
                }
            }
            else
            {
	            if (note.noteType == BSDNoteType.left) LeftNoteHitCount++;
	            else if (note.noteType == BSDNoteType.right) RightNoteHitCount++;
            }
        }
        
        Won = levelCompletionResults.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared;
        EndTime = levelCompletionResults.endSongTime;
        Rank = RankModel.GetRankName(levelCompletionResults.rank);
        
        var playerLevelStatsData = playerData.TryGetPlayerLevelStatsData(in beatmapKey);
        int maxRawScore = Utils.MaxRawScoreForNumberOfNotes(Notes.Count);
        int maxScore = Mathf.RoundToInt(maxRawScore * ModifiersMultiplier);

        ModifiersMultiplier = GetTotalMultiplier(playerData.gameplayModifiers, levelCompletionResults.energy);
        PersonalBestModifiedRatio = playerLevelStatsData.highScore / (float)maxScore;
        PersonalBestRawRatio = playerLevelStatsData.highScore / (float)maxRawScore;
        PersonalBest = playerLevelStatsData.highScore;

        RawScore = Notes.Select(note => note.GetTotalScore()).Sum();

        Score = Mathf.RoundToInt(RawScore * ModifiersMultiplier);

        ModifiedRatio = Score / (float)maxRawScore;
        RawRatio = RawScore / (float)maxRawScore;

        var rawGraph = new Dictionary<float, float>();
        var lastGraphNodes = new Queue<float>();
        int actualScore = 0, maxScore1 = 0, multiplier = 1, multiplierProgress = 0, lastBeat = 0;
        
        foreach(var note in Notes)
		{
			actualScore += note.GetTotalScore();

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

			if (Math.Truncate(note.time) > lastBeat)
			{
				rawGraph.Add((float) Math.Truncate(note.time), (float) actualScore / (float) maxScore1);
				lastBeat = (int) Math.Truncate(note.time);
			}

			if (note.IsAMiss())
			{
				continue;
			}
			 
			int acc = note.score[0] + note.score[1] + note.score[2];

			if (note.noteType == BSDNoteType.left)
			{
				CutLeft++;
				AccLeft += acc;
				LeftAverageCut[0] += note.score[0];
				LeftAverageCut[1] += note.score[1];
				LeftAverageCut[2] += note.score[2];
				LeftSpeed += note.speed;
				LeftPreSwing += note.preswing;
				LeftPostSwing += note.postswing;
				if (note.speed > LeftHighestSpeed)
					LeftHighestSpeed = note.speed;
				LeftTimeDependence += note.timeDependence;
			} 
			else
			{
				CutRight++;
				AccRight += acc;
				RightAverageCut[0] += note.score[0];
				RightAverageCut[1] += note.score[1];
				RightAverageCut[2] += note.score[2];
				RightSpeed += note.speed;
				RightPreSwing += note.preswing;
				RightPostSwing += note.postswing;
				if (note.speed > RightHighestSpeed)
					RightHighestSpeed = note.speed;
				RightTimeDependence += note.timeDependence;
			}
			
			if (note.index < 0 || note.index >= GridCut.Length) // it's noodle
			{
				continue;
			}

			GridCut[note.index] += 1;
			GridAcc[note.index] += acc;
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

		AverageAcc = SafeMath.Average(AccRight, CutRight, AccLeft, CutLeft);

		AverageSpeed = SafeMath.Divide(LeftSpeed + RightSpeed, CutRight + CutLeft);
		LeftSpeed = SafeMath.Divide(LeftSpeed, CutLeft);
		RightSpeed = SafeMath.Divide(RightSpeed, CutRight);

		AverageTimeDependence = SafeMath.Divide(LeftTimeDependence + RightTimeDependence, CutRight + CutLeft);
		LeftTimeDependence = SafeMath.Divide(LeftTimeDependence, CutLeft);
		RightTimeDependence = SafeMath.Divide(RightTimeDependence, CutRight);

		AveragePreSwing = SafeMath.Divide(LeftPreSwing + RightPreSwing, CutRight + CutLeft);
		LeftPreSwing = SafeMath.Divide(LeftPreSwing, CutLeft);
		RightPreSwing = SafeMath.Divide(RightPreSwing, CutRight);

		AveragePostSwing = SafeMath.Divide(LeftPostSwing + RightPostSwing, CutRight + CutLeft);
		LeftPostSwing = SafeMath.Divide(LeftPostSwing, CutLeft);
		RightPostSwing = SafeMath.Divide(RightPostSwing, CutRight);
		
		RightSaber = levelCompletionResults.rightSaberMovementDistance;
		RightHand = levelCompletionResults.rightHandMovementDistance;
		LeftSaber = levelCompletionResults.leftSaberMovementDistance;
		LeftHand = levelCompletionResults.leftHandMovementDistance;

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