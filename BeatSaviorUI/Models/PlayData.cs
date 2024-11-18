using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaviorUI.Utilities;

namespace BeatSaviorUI.Models;

internal class PlayData
{
	public float ModifiersMultiplier { get; }
    public float ModifiedRatio { get; }

    public bool Won { get; }
    public string Rank { get; }

    public Dictionary<float, float> Graph { get; } = [];
    
    public float AccRight { get; }
    public float AccLeft { get; }
    public float LeftSpeed { get; }
    public float RightSpeed { get; }
    public float LeftPreSwing { get; }
    public float RightPreSwing { get; }
    public float LeftPostSwing { get; }
    public float RightPostSwing { get; }
    public float LeftTimeDependence { get; }
    public float RightTimeDependence { get; }
    public float[] LeftAverageCut { get; } = new float[3];
    public float[] RightAverageCut { get; } = new float[3];

    public int ComboBreaks { get; }
    
    public CompletionResultsExtraData CompletionResultsExtraData { get; }
    public BeatmapInfo BeatmapInfo { get; }
	
    public bool FullCombo { get; }
    
    public PlayData(LevelCompletionResults levelCompletionResults, CompletionResultsExtraData completionResultsExtraData, BeatmapInfo beatmapInfo)
    {
	    CompletionResultsExtraData = completionResultsExtraData;
	    BeatmapInfo = beatmapInfo;
        
        Won = levelCompletionResults.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared;
        FullCombo = levelCompletionResults.fullCombo;
		
        Rank = RankModel.GetRankName(levelCompletionResults.rank);
        ModifiersMultiplier = GetTotalMultiplier(levelCompletionResults.gameplayModifiers, levelCompletionResults.energy);
        ModifiedRatio = SafeMath.Divide(levelCompletionResults.modifiedScore, 
										Utils.MaxRawScoreForNumberOfNotes(CompletionResultsExtraData.Notes.Count));
        
        var rawGraph = new Dictionary<float, float>();
        var lastGraphNodes = new Queue<float>();
        int actualScore = 0;
        int maxScore = 0;
        int multiplier = 1;
        int multiplierProgress = 0;
        int lastBeat = 0;

        int leftNoteHitCount = 0;
        int rightNoteHitCount = 0;
        
        foreach(var note in CompletionResultsExtraData.Notes)
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

			maxScore += 115 * multiplier;

			float noteBeat = MathF.Truncate(note.Time);
			if (noteBeat > lastBeat)
			{
				rawGraph.Add(noteBeat, (float) actualScore / maxScore);
				lastBeat = (int) noteBeat;
			}

			if (note.IsAMiss)
			{
				ComboBreaks++;
				continue;
			}
			
			if (note.ColorType == ColorType.ColorA)
			{
				leftNoteHitCount++;
				AccLeft += note.Score[0] + note.Score[1] + note.Score[2];
				LeftAverageCut[0] += note.Score[0];
				LeftAverageCut[1] += note.Score[1];
				LeftAverageCut[2] += note.Score[2];
				LeftSpeed += note.Speed;
				LeftPreSwing += note.PreSwing;
				LeftPostSwing += note.PostSwing;
				LeftTimeDependence += note.TimeDependence;
			} 
			else
			{
				rightNoteHitCount++;
				AccRight += note.Score[0] + note.Score[1] + note.Score[2];
				RightAverageCut[0] += note.Score[0];
				RightAverageCut[1] += note.Score[1];
				RightAverageCut[2] += note.Score[2];
				RightSpeed += note.Speed;
				RightPreSwing += note.PreSwing;
				RightPostSwing += note.PostSwing;
				RightTimeDependence += note.TimeDependence;
			}
		}
        
		foreach(var point in rawGraph) {

			lastGraphNodes.Enqueue(point.Value);

			if (lastGraphNodes.Count > 5)
				lastGraphNodes.Dequeue();

			Graph.Add(point.Key, lastGraphNodes.Average());
		}

		AccRight = SafeMath.Divide(AccRight, rightNoteHitCount);
		AccLeft = SafeMath.Divide(AccLeft, leftNoteHitCount);

		for (int i = 0; i < 3; i++) {
			LeftAverageCut[i] = SafeMath.Divide(LeftAverageCut[i], leftNoteHitCount);
			RightAverageCut[i] = SafeMath.Divide(RightAverageCut[i], rightNoteHitCount);
		}

		LeftSpeed = SafeMath.Divide(LeftSpeed, leftNoteHitCount);
		RightSpeed = SafeMath.Divide(RightSpeed, rightNoteHitCount);

		LeftTimeDependence = SafeMath.Divide(LeftTimeDependence, leftNoteHitCount);
		RightTimeDependence = SafeMath.Divide(RightTimeDependence, rightNoteHitCount);

		LeftPreSwing = SafeMath.Divide(LeftPreSwing, leftNoteHitCount);
		RightPreSwing = SafeMath.Divide(RightPreSwing, rightNoteHitCount);

		LeftPostSwing = SafeMath.Divide(LeftPostSwing, leftNoteHitCount);
		RightPostSwing = SafeMath.Divide(RightPostSwing, rightNoteHitCount);
    }
    
    // Modifiers aren't used in UI, but they could be
    // public List<string> Modifiers { get; } = [];
    private float GetTotalMultiplier(GameplayModifiers gameplayModifiers, float energy)
    {
        float multiplier = 1f;

        if (gameplayModifiers.disappearingArrows)
        {
	        multiplier += 0.07f;
	        // Modifiers.Add("DA");
        }

	    /*string songSpeedModifier = gameplayModifiers.songSpeed switch
        {
	        GameplayModifiers.SongSpeed.SuperFast => "SF",
	        GameplayModifiers.SongSpeed.Faster => "FS",
	        GameplayModifiers.SongSpeed.Slower => "SS",
	        _ => null
        };
	    if (songSpeedModifier != null) Modifiers.Add(songSpeedModifier);*/
        multiplier += gameplayModifiers.songSpeedMul - 1f;

        if (gameplayModifiers.ghostNotes)
        {
	        multiplier += 0.11f;
	        // Modifiers.Add("GN");
        }

        if (gameplayModifiers.noArrows)
        {
	        multiplier -= 0.30f;
	        // Modifiers.Add("NA");
        }

        if (gameplayModifiers.noBombs)
        {
	        multiplier -= 0.10f;
	        // Modifiers.Add("NB");
        }

        if (gameplayModifiers.noFailOn0Energy && energy == 0)
        {
	        multiplier -= 0.50f;
	        // Modifiers.Add("NF");
        }

        if (gameplayModifiers.enabledObstacleType == GameplayModifiers.EnabledObstacleType.NoObstacles)
        {
	        multiplier -= 0.05f;
	        // Modifiers.Add("NO");
        }

        if (gameplayModifiers.zenMode)
        {
	        // Modifiers.Add("ZM");
	        return 0f;
        }

        return multiplier >= 0f ? multiplier : 0f;
    }
}