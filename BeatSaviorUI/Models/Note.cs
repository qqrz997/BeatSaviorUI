using System;
using BeatSaviorUI.Stats;
using IPA.Utilities;
using UnityEngine;

namespace BeatSaviorUI.Models
{
	public class Note
	{
		private int Multiplier { get; }
		
		public NoteCutInfo NoteCutInfo { get; }
		public ColorType ColorType { get; }
		public int Index { get; }
		public float Time { get; }
		public CutType CutType { get; }
		public int[] Score { get; }
		public float TimeDependence { get; }
		public float Speed { get; private set; }
		public float PreSwing { get; private set; }
		public float PostSwing { get; private set; }

		public int TotalScore => (Score[0] + Score[1] + Score[2]) * Multiplier;
		public bool IsAMiss => TotalScore == 0;

		private Note(GoodCutScoringElement goodCut, CutType cut)
		{
			var noteData = goodCut.noteData;

			ColorType = noteData.colorType;
			CutType = cut;

			Score = [0, 0, 0];
			Index = noteData.lineIndex + 4 * (int)noteData.noteLineLayer;
			Time = noteData.time;
		}

		private Note(NoteController controller, CutType cut)
		{
			var noteData = controller.noteData;

			ColorType = noteData.colorType;
			CutType = cut;

			Score = [0, 0, 0];
			Index = noteData.lineIndex + 4 * (int)noteData.noteLineLayer;
			Time = noteData.time;
		}

		public Note(GoodCutScoringElement goodCut, CutType cut, NoteCutInfo noteCutInfo, int multiplier) : this(goodCut, cut)
		{
			Multiplier = multiplier;
			NoteCutInfo = noteCutInfo;

			TimeDependence = Math.Abs(noteCutInfo.cutNormal.z);
			Score = [0, goodCut.cutScoreBuffer.centerDistanceCutScore, 0];

			goodCut.cutScoreBuffer.RegisterDidFinishReceiver(new WaitForSwing(this));
		}
		
		// Bad Cut
		public Note(NoteController controller, CutType cut, NoteCutInfo noteCutInfo, int multiplier) : this(controller, cut)
		{
			Multiplier = multiplier;
			NoteCutInfo = noteCutInfo;
		}

		// Miss
		public Note(NoteController controller, CutType cut, int multiplier) : this(controller, cut)
		{
			Multiplier = multiplier;
		}

		private class WaitForSwing : ISaberSwingRatingCounterDidFinishReceiver, ICutScoreBufferDidFinishReceiver
		{
			private Note Note { get; }

			public WaitForSwing(Note note)
			{
				Note = note;
			}

            public void HandleCutScoreBufferDidFinish(CutScoreBuffer cutScoreBuffer)
            {
				var swingHolder = SwingTranspilerHandler.GetSwing(cutScoreBuffer.GetField<SaberSwingRatingCounter, CutScoreBuffer>("_saberSwingRatingCounter"));
				if (swingHolder != null)
				{
					Note.PreSwing = swingHolder.preswing;
					Note.PostSwing = swingHolder.postswing;
				}

				Note.Score[0] = cutScoreBuffer.beforeCutScore;
				Note.Score[2] = cutScoreBuffer.afterCutScore;
				Note.Speed = Note.NoteCutInfo.saberSpeed;

				cutScoreBuffer.UnregisterDidFinishReceiver(this);
			}

            public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter s)
			{
				var swingHolder = SwingTranspilerHandler.GetSwing(s as SaberSwingRatingCounter);
				if (swingHolder != null)
				{
					Note.PreSwing = swingHolder.preswing;
					Note.PostSwing = swingHolder.postswing;
				}

				Note.Score[0] = Mathf.RoundToInt(70f * s.beforeCutRating);
				Note.Score[2] = Mathf.RoundToInt(30f * s.afterCutRating);
				Note.Speed = Note.NoteCutInfo.saberSpeed;

				s.UnregisterDidFinishReceiver(this);
			}
		}
	}
}
