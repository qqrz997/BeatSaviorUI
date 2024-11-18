using System;
using System.Collections.Generic;
using BeatSaviorUI.Models;
using JetBrains.Annotations;
using Zenject;

namespace BeatSaviorUI.Stats;

[UsedImplicitly]
internal class NoteDataCollector : IInitializable, IDisposable
{
    private StandardLevelScenesTransitionSetupDataSO StandardLevelScenesTransitionSetupData { get; }
    private BeatmapObjectManager BeatmapObjectManager { get; }
    private ScoreController ScoreController { get; }
    private PlayerHeadAndObstacleInteraction PlayerHeadAndObstacleInteraction { get; }
    private PauseController PauseController { get; }
    private PlayerDataModel PlayerDataModel { get; }

    public NoteDataCollector(StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData, BeatmapObjectManager beatmapObjectManager, PlayerHeadAndObstacleInteraction playerHeadAndObstacleInteraction, ScoreController scoreController, PauseController pauseController, PlayerDataModel playerDataModel)
    {
        StandardLevelScenesTransitionSetupData = standardLevelScenesTransitionSetupData;
        BeatmapObjectManager = beatmapObjectManager;
        PlayerHeadAndObstacleInteraction = playerHeadAndObstacleInteraction;
        ScoreController = scoreController;
        PauseController = pauseController;
        PlayerDataModel = playerDataModel;
    }
    
    public static TempTracker Tracker { get; private set; }
    
    private int combo;
    private int multiplier = 1;
    private int multiplierProgress;
    
    private readonly List<Note> notes = [];
    private int maxCombo = 69;
    private int bombHitCount;
    private int pauseCount;
    private int wallHitCount;

    public void Initialize()
    {
        StandardLevelScenesTransitionSetupData.didFinishEvent += OnLevelFinished;
        ScoreController.scoringForNoteStartedEvent += OnScoringForNoteStarted;
        BeatmapObjectManager.noteWasCutEvent += OnNoteWasCut;
        BeatmapObjectManager.noteWasMissedEvent += OnNoteMissed;
        PlayerHeadAndObstacleInteraction.headDidEnterObstaclesEvent += OnHeadEnteredObstacle;
        PauseController.didPauseEvent += OnSongPaused;
    }

    public void Dispose()
    {
	    StandardLevelScenesTransitionSetupData.didFinishEvent -= OnLevelFinished;
	    ScoreController.scoringForNoteStartedEvent -= OnScoringForNoteStarted;
	    BeatmapObjectManager.noteWasCutEvent -= OnNoteWasCut;
	    BeatmapObjectManager.noteWasMissedEvent -= OnNoteMissed;
	    PlayerHeadAndObstacleInteraction.headDidEnterObstaclesEvent -= OnHeadEnteredObstacle;
	    PauseController.didPauseEvent -= OnSongPaused;
    }

    private void OnLevelFinished(StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData, LevelCompletionResults levelCompletionResults)
    {
	    if (combo > maxCombo)
	    {
		    maxCombo = combo;
	    }

	    Tracker = new(
	        levelCompletionResults,
	        notes,
	        maxCombo,
	        bombHitCount,
	        pauseCount,
	        wallHitCount,
	        standardLevelScenesTransitionSetupData.beatmapKey,
	        PlayerDataModel.playerData);
    }
    
    private void OnScoringForNoteStarted(ScoringElement scoringElement)
    {
	    if (scoringElement is not GoodCutScoringElement goodCut)
	    {
		    return;
	    }

	    var noteCutInfo = goodCut.cutScoreBuffer.noteCutInfo;

	    if (!IsGoodCut(in noteCutInfo))
	    {
		    return;
	    }

	    combo++;
	    ComputeMultiplier(true);
	    notes.Add(new(goodCut, CutType.Cut, noteCutInfo, multiplier));
    }
    
    private void OnNoteWasCut(NoteController noteController, in NoteCutInfo noteCutInfo)
    {
	    if (IsGoodCut(in noteCutInfo))
	    {
		    return;
	    }
	    ComputeMultiplier(false);
    	
	    if (noteCutInfo.noteData.colorType != ColorType.None)
    	{
    		notes.Add(new(noteController, CutType.BadCut, noteCutInfo, multiplier));
    	} 
    	else if (noteCutInfo.noteData.colorType == ColorType.None)
    	{
    		bombHitCount++;
    	}
    }

    private void OnNoteMissed(NoteController controller)
    {
	    if (controller.noteData.colorType == ColorType.None)
	    {
		    return;
	    }

	    notes.Add(new Note(controller, CutType.Miss, multiplier));
	    ComputeMultiplier(false);
    }

    private void ComputeMultiplier(bool goodHit)
    {
	    Plugin.Log.Info("Calculating multiplier");
    	if(!goodHit)
    	{
    		if (combo > maxCombo)
		    {
			    maxCombo = combo;
		    }

		    combo = 0;
    		if(multiplier > 1)
		    {
			    multiplier /= 2;
		    }

		    multiplierProgress = 0;
    	} 
	    else if (multiplier < 8)
    	{
    		multiplierProgress++;
    		if(multiplierProgress == multiplier * 2)
    		{
    			multiplierProgress = 0;
    			multiplier *= 2;
    		}
    	}
    }

    private void OnHeadEnteredObstacle()
    {
    	wallHitCount++;
    	ComputeMultiplier(false);
    }

    private void OnSongPaused() => 
	    pauseCount++;

    private static bool IsGoodCut(in NoteCutInfo noteCutInfo) =>
	    noteCutInfo.allIsOK && noteCutInfo.noteData.colorType != ColorType.None;
}