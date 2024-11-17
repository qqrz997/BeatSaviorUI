using System;
using System.Collections.Generic;
using BeatSaviorUI.Stats.Trackers;
using JetBrains.Annotations;
using Zenject;

namespace BeatSaviorUI.Stats;

[UsedImplicitly]
internal class SongDataManager : IInitializable, IDisposable
{
    private PluginConfig PluginConfig { get; }
    private StandardLevelScenesTransitionSetupDataSO StandardLevelScenesTransitionSetupData { get; }
    private BeatmapObjectManager BeatmapObjectManager { get; }
    private ScoreController ScoreController { get; }
    private PlayerHeadAndObstacleInteraction PlayerHeadAndObstacleInteraction { get; }
    private PauseController PauseController { get; }
    private PlayerDataModel PlayerDataModel { get; }

    public SongDataManager(PluginConfig pluginConfig, StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData, BeatmapObjectManager beatmapObjectManager, PlayerHeadAndObstacleInteraction playerHeadAndObstacleInteraction, ScoreController scoreController, PauseController pauseController, PlayerDataModel playerDataModel)
    {
        PluginConfig = pluginConfig;
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
    private int multiplierProgress = 0;
    
    private readonly List<Note> notes = [];
    private int maxCombo;
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

    private void OnLevelFinished(StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData, LevelCompletionResults levelCompletionResults)
    {
        Logger.log.Critical("SongDataManager OnLevelFinished");      
        Logger.log.Info($"combo: {combo}");
        Logger.log.Info($"multiplier: {multiplier}");
        Logger.log.Info($"multiplier progress: {multiplierProgress}");
        Logger.log.Info($"note count: {notes.Count}");
        Logger.log.Info($"bombs hit: {bombHitCount}");
        Logger.log.Info($"pauses: {pauseCount}");
        Logger.log.Info($"walls hit: {wallHitCount}");

        Tracker = new TempTracker(
	        levelCompletionResults,
	        notes,
	        maxCombo,
	        bombHitCount,
	        pauseCount,
	        wallHitCount,
	        standardLevelScenesTransitionSetupData.beatmapKey,
	        PlayerDataModel.playerData);
        
    }

    public void Dispose()
    {
	    StandardLevelScenesTransitionSetupData.didFinishEvent -= OnLevelFinished;
	    ScoreController.scoringForNoteStartedEvent -= OnScoringForNoteStarted;
	    BeatmapObjectManager.noteWasCutEvent -= OnNoteWasCut;
	    BeatmapObjectManager.noteWasMissedEvent -= OnNoteMissed;
	    PlayerHeadAndObstacleInteraction.headDidEnterObstaclesEvent -= OnHeadEnteredObstacle;
    }
    
    private void OnScoringForNoteStarted(ScoringElement scoringElement)
    {
	    if (scoringElement is not GoodCutScoringElement goodCut)
	    {
		    return;
	    }

	    var noteCutInfo = goodCut.cutScoreBuffer.noteCutInfo;

	    // (data.colorType != ColorType.None) checks if it is not a bomb
	    if (!noteCutInfo.allIsOK || goodCut.noteData.colorType == ColorType.None)
	    {
		    return;
	    }

	    combo++;
	    ComputeMultiplier(true);
	    notes.Add(new(goodCut, CutType.cut, noteCutInfo, multiplier));
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
    		notes.Add(new(noteController, CutType.badCut, noteCutInfo, multiplier));
    	} 
    	else if (noteCutInfo.noteData.colorType == ColorType.None)
    	{
    		bombHitCount++;
    	}
    }

    private void OnNoteMissed(NoteController controller)
    {
    	if (controller.noteData.colorType != ColorType.None)
    	{
    		ComputeMultiplier(false);
    		notes.Add(new Note(controller, CutType.miss, multiplier));
    	}
    }

    private void ComputeMultiplier(bool goodHit)
    {
    	if(!goodHit)
    	{
    		if (combo > maxCombo)
    			maxCombo = combo;
    		combo = 0;
    		if(multiplier > 1)
    			multiplier /= 2;
    		multiplierProgress = 0;
    	} else if (multiplier < 8)
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
    	// We only reset multiplier on walls hit because we already count miss, badcuts and bombs in other events
    	ComputeMultiplier(false);
    	wallHitCount++;
    }

    private void OnSongPaused() => 
	    pauseCount++;

    private static bool IsGoodCut(in NoteCutInfo noteCutInfo) =>
	    noteCutInfo.allIsOK && noteCutInfo.noteData.colorType != ColorType.None;
}