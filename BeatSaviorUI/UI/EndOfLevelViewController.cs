using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaviorUI.Models;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatSaviorUI.UI
{
	internal class EndOfLevelViewController : BSMLResourceViewController
    {
        [Inject] private readonly PluginConfig config = null!;
        [Inject] private readonly PlayerDataModel playerDataModel = null!;
        
        [UIObject("titleCard")] private readonly GameObject titleCard = null!;
        [UIObject("songCover")] private readonly GameObject songCover = null!;
        [UIComponent("title")] private readonly TextMeshProUGUI title = null!;
        [UIComponent("artist")] private readonly TextMeshProUGUI artist = null!;
        [UIComponent("mapper")] private readonly TextMeshProUGUI mapper = null!;
        [UIComponent("difficulty")] private readonly TextMeshProUGUI difficulty = null!;

        [UIObject("upperBand")] private readonly GameObject upperBand = null!;
        [UIObject("middleBand")] private readonly GameObject middleBand = null!;
        [UIObject("lowerBand")] private readonly GameObject lowerBand = null!;
        [UIComponent("rankLabel")] private readonly TextMeshProUGUI rankLabel = null!;
        [UIComponent("rank")] private readonly TextMeshProUGUI rank = null!;
        [UIComponent("percentLabel")] private readonly TextMeshProUGUI percentLabel = null!;
        [UIComponent("percent")] private readonly TextMeshProUGUI percent = null!;
        [UIComponent("comboLabel")] private readonly TextMeshProUGUI comboLabel = null!;
        [UIComponent("combo")] private readonly TextMeshProUGUI combo = null!;
        [UIComponent("missLabel")] private readonly TextMeshProUGUI missLabel = null!;
        [UIComponent("miss")] private readonly TextMeshProUGUI miss = null!;
        [UIComponent("pausesLabel")] private readonly TextMeshProUGUI pausesLabel = null!;
        [UIComponent("pauses")] private readonly TextMeshProUGUI pauses = null!;

        [UIObject("leftCircle")] private readonly GameObject leftCircle = null!;
        [UIObject("rightCircle")] private readonly GameObject rightCircle = null!;
        [UIComponent("leftAverage")] private readonly TextMeshProUGUI leftAverage = null!;
        [UIComponent("rightAverage")] private readonly TextMeshProUGUI rightAverage = null!;
        [UIComponent("leftBeforeCut")] private readonly TextMeshProUGUI leftBeforeCut = null!;
        [UIComponent("rightBeforeCut")] private readonly TextMeshProUGUI rightBeforeCut = null!;
        [UIComponent("leftAccuracy")] private readonly TextMeshProUGUI leftAccuracy = null!;
        [UIComponent("rightAccuracy")] private readonly TextMeshProUGUI rightAccuracy = null!;
        [UIComponent("leftAfterCut")] private readonly TextMeshProUGUI leftAfterCut = null!;
        [UIComponent("rightAfterCut")] private readonly TextMeshProUGUI rightAfterCut = null!;
        [UIComponent("leftTD")] private readonly TextMeshProUGUI leftTd = null!;
        [UIComponent("rightTD")] private readonly TextMeshProUGUI rightTd = null!;
        [UIComponent("leftSpeed")] private readonly TextMeshProUGUI leftSpeed = null!;
        [UIComponent("rightSpeed")] private readonly TextMeshProUGUI rightSpeed = null!;
        [UIComponent("leftBeforeSwing")] private readonly TextMeshProUGUI leftBeforeSwing = null!;
        [UIComponent("rightBeforeSwing")] private readonly TextMeshProUGUI rightBeforeSwing = null!;
        [UIComponent("leftAfterSwing")] private readonly TextMeshProUGUI leftAfterSwing = null!;
        [UIComponent("rightAfterSwing")] private readonly TextMeshProUGUI rightAfterSwing = null!;


        private readonly List<(float, float)> curve =
        [
            (0, 0),
            (40, 8),
            (50, 15),
            (69, 25),
            (75, 42.5f), 
            (82, 56), 
            (84.5f, 63),
            (86, 72),
            (88, 76.6f),
            (90, 81.5f),
            (91, 85),
            (92, 88.5f),
            (93, 92),
            (94, 97.4f), 
            (95, 103.6f),
            (100, 110)
        ];

        public override string ResourceName => $"{Plugin.Name}.UI.Views.EndOfLevelView.bsml";
        private bool postParseDone;
        private PlayData playData;
        private BeatmapLevel tempBeatmapLevel;
        private ImageView songCoverImg, upperBandImg, lowerBandImg, leftCircleImg, rightCircleImg;

        private List<string> Lyrics { get; } =
        [
            "Never", "gonna", "give", "you", "up", "Never", "gonna", "let", "you", "down"
        ];

        private readonly Color32 goldColor = new(237, 201, 103, 255);

        [UIAction("#post-parse")]
        public void PostParse()
        {
            var playerData = playerDataModel.playerData;
            var colorScheme = playerData.colorSchemesSettings.GetSelectedColorScheme();
            var circleSprite = Resources.FindObjectsOfTypeAll<Sprite>().First(x => x.name == "Circle");

            titleCard.GetComponent<ContentSizeFitter>().enabled = false;
            titleCard.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            songCoverImg = songCover.AddComponent<ImageView>();
            songCoverImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;

            title.enableAutoSizing = true;
            title.fontSizeMax = 15;
            title.fontSizeMin = 1;

            artist.enableAutoSizing = true;
            artist.fontSizeMax = 15;
            artist.fontSizeMin = 1;

            mapper.enableAutoSizing = true;
            mapper.fontSizeMax = 15;
            mapper.fontSizeMin = 1;

            difficulty.enableAutoSizing = true;
            difficulty.fontSizeMax = 15;
            difficulty.fontSizeMin = 1;

            upperBandImg = upperBand.AddComponent<ImageView>();
            upperBandImg.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
            upperBandImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;

            lowerBandImg = lowerBand.AddComponent<ImageView>();
            lowerBandImg.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
            lowerBandImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;

            leftCircleImg = leftCircle.AddComponent<ImageView>();
            leftCircleImg.sprite = circleSprite;
            leftCircleImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            leftCircleImg.color = colorScheme.saberAColor;
            leftCircleImg.preserveAspect = true;
            leftCircleImg.transform.localScale *= 1.55f;
            leftCircleImg.type = Image.Type.Filled;
            leftCircleImg.fillOrigin = (int)Image.Origin360.Top;
            leftCircleImg.fillAmount = 0;

            leftAverage.color = colorScheme.saberAColor;
            leftAverage.enableAutoSizing = true;
            leftAverage.fontSizeMin = 1;
            leftAverage.fontSizeMax = 12;

            leftBeforeCut.color = colorScheme.saberAColor;
            leftBeforeCut.overflowMode = TextOverflowModes.Overflow;

            leftAccuracy.color = colorScheme.saberAColor;
            leftAccuracy.overflowMode = TextOverflowModes.Overflow;

            leftAfterCut.color = colorScheme.saberAColor;
            leftAfterCut.overflowMode = TextOverflowModes.Overflow;

            leftTd.color = colorScheme.saberAColor;
            leftTd.overflowMode = TextOverflowModes.Overflow;

            leftSpeed.color = colorScheme.saberAColor;
            leftSpeed.overflowMode = TextOverflowModes.Overflow;

            leftBeforeSwing.color = colorScheme.saberAColor;
            leftBeforeSwing.overflowMode = TextOverflowModes.Overflow;

            leftAfterSwing.color = colorScheme.saberAColor;
            leftAfterSwing.overflowMode = TextOverflowModes.Overflow;

            var img = middleBand.AddComponent<ImageView>();
            img.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
            img.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            img.color = new Color32(255, 255, 255, 100);

            rightCircleImg = rightCircle.AddComponent<ImageView>();
            rightCircleImg.sprite = circleSprite;
            rightCircleImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            rightCircleImg.color = colorScheme.saberBColor;
            rightCircleImg.preserveAspect = true;
            rightCircleImg.transform.localScale *= 1.55f;
            rightCircleImg.type = Image.Type.Filled;
            rightCircleImg.fillOrigin = (int)Image.Origin360.Top;
            rightCircleImg.fillAmount = 0;

            rightAverage.color = colorScheme.saberBColor;
            rightAverage.enableAutoSizing = true;
            rightAverage.fontSizeMin = 1;
            rightAverage.fontSizeMax = 12;

            rightBeforeCut.color = colorScheme.saberBColor;
            rightBeforeCut.overflowMode = TextOverflowModes.Overflow;

            rightAccuracy.color = colorScheme.saberBColor;
            rightAccuracy.overflowMode = TextOverflowModes.Overflow;

            rightAfterCut.color = colorScheme.saberBColor;
            rightAfterCut.overflowMode = TextOverflowModes.Overflow;

            rightTd.color = colorScheme.saberBColor;
            rightTd.overflowMode = TextOverflowModes.Overflow;

            rightSpeed.color = colorScheme.saberBColor;
            rightSpeed.overflowMode = TextOverflowModes.Overflow;

            rightBeforeSwing.color = colorScheme.saberBColor;
            rightBeforeSwing.overflowMode = TextOverflowModes.Overflow;

            rightAfterSwing.color = colorScheme.saberBColor;
            rightAfterSwing.overflowMode = TextOverflowModes.Overflow;

            postParseDone = true;
            if(playData != null && tempBeatmapLevel != null)
                Refresh(playData, tempBeatmapLevel);
        }

        public void Refresh(PlayData tracker, BeatmapLevel beatmapLevel)
        {
            if (!postParseDone) {
                playData = tracker;
                tempBeatmapLevel = beatmapLevel;
                return;
            }

            songCoverImg.sprite = beatmapLevel.previewMediaData.GetCoverSpriteAsync().Result;
            
            title.text = tracker.BeatmapInfo.SongName;
            artist.text = tracker.BeatmapInfo.SongArtist;
            mapper.text = tracker.BeatmapInfo.SongMapper;

            difficulty.text = FormatSongDifficulty(tracker.BeatmapInfo.SongDifficulty);
            difficulty.color = SetColorBasedOnDifficulty(tracker.BeatmapInfo.SongDifficulty);

            // ReSharper disable RedundantLogicalConditionalExpressionOperand
            // ReSharper disable once RedundantBoolCompare
            if(!Plugin.Fish && true != false && (true || !false) && 1+3 != 5 || 42 == 69)
            {
                rank.text = tracker.Rank;
                percent.text = (tracker.ModifiedRatio * 100).ToString("F") + " %";
                combo.text = tracker.CompletionResultsExtraData.MaxCombo.ToString();
                miss.text = tracker.FullCombo ? "FC" : tracker.ComboBreaks.ToString();
                pauses.text = config.HidePauseCount ? "-" : tracker.CompletionResultsExtraData.PauseCount.ToString();
            }
            else
            {
                rankLabel.text = Lyrics[0];
                rank.text = Lyrics[5];
                percentLabel.text = Lyrics[1];
                percent.text = Lyrics[6];
                comboLabel.text = Lyrics[2];
                combo.text = Lyrics[7];
                missLabel.text = Lyrics[3];
                miss.text = Lyrics[8];
                pausesLabel.text = Lyrics[4];
                pauses.text = Lyrics[9];
            }
            rank.color = SetColorBasedOnRank(rank.text);
            
            var color = tracker.FullCombo ? (Color)goldColor : Color.white;
            miss.color = color;
            missLabel.color = color;
            lowerBandImg.color = color;
            upperBandImg.color = color;

            StartCoroutine(AnimateCircle(leftCircleImg, GetCircleFillRatio(tracker.AccLeft), 1.5f));
            StartCoroutine(AnimateCircle(rightCircleImg, GetCircleFillRatio(tracker.AccRight), 1.5f));

            leftAverage.text = tracker.AccLeft.ToString("0.##");
            rightAverage.text = tracker.AccRight.ToString("0.##");
            
            leftBeforeCut.text = tracker.LeftAverageCut[0].ToString("0.#");
            rightBeforeCut.text = tracker.RightAverageCut[0].ToString("0.#");
            
            leftAccuracy.text = tracker.LeftAverageCut[1].ToString("0.#");
            rightAccuracy.text = tracker.RightAverageCut[1].ToString("0.#");
            
            leftAfterCut.text = tracker.LeftAverageCut[2].ToString("0.#");
            rightAfterCut.text = tracker.RightAverageCut[2].ToString("0.#");
            
            leftTd.text = tracker.LeftTimeDependence.ToString("0.###");
            rightTd.text = tracker.RightTimeDependence.ToString("0.###");
            
            leftSpeed.text = (tracker.LeftSpeed * 3.6f).ToString("0.##") + " Km/h";
            rightSpeed.text = (tracker.RightSpeed * 3.6f).ToString("0.##") + " Km/h";
            
            leftBeforeSwing.text = (tracker.LeftPreSwing * 100).ToString("0.##") + " %";
            rightBeforeSwing.text = (tracker.RightPreSwing * 100).ToString("0.##") + " %";
            
            leftAfterSwing.text = (tracker.LeftPostSwing * 100).ToString("0.##") + " %";
            rightAfterSwing.text = (tracker.RightPostSwing * 100).ToString("0.##") + " %";
        }

        private static Color32 SetColorBasedOnRank(string rank) => rank switch
        {
            "SSS" or "SS" => new(0x00, 0xF0, 0xFF, 0xFF),
            "A" => new(0x00, 0xFF, 0x00, 0xFF),
            "B" => new(0xFF, 0xFF, 0x00, 0xFF),
            "C" => new(0xFF, 0xA7, 0x00, 0xFF),
            "D" or "E" => new(0xFF, 0x00, 0x00, 0xFF),
            _ => new(0xFF, 0xFF, 0xFF, 0xFF)
        };

        private static Color32 SetColorBasedOnDifficulty(string diffName) => diffName switch
        {
            "easy" => new(0x3c, 0xb3, 0x71, 0xFF),
            "normal" => new(0x59, 0xb0, 0xf4, 0xFF),
            "hard" => new(0xFF, 0xa5, 0x00, 0xFF),
            "expert" => new(0xbf, 0x2a, 0x42, 0xFF),
            "expertplus" => new(0x8f, 0x48, 0xdb, 0xFF),
            _ => new(0xFF, 0xFF, 0xFF, 0xFF)
        };

        private static string FormatSongDifficulty(string diffName) => diffName switch
        {
            "easy" => "Easy",
            "normal" => "Normal",
            "hard" => "Hard",
            "expert" => "Expert",
            "expertplus" => "Expert+",
            _ => "Unknown"
        };

        private float GetCircleFillRatio(float accuracy)
        {
            const float maxPpPercent = 110; // The max number of pp (in %) (yes it's not 100%, look at the curve)
            float accRatio = (accuracy / 115) * 100;

            for(int i = 0; i < curve.Count; i++)
            {
                if (!(curve[i].Item1 >= accRatio))
                {
                    continue;
                }

                float max = curve[i].Item1; 
                float maxValue = curve[i].Item2;
                float min = curve[i - 1].Item1; 
                float minValue = curve[i - 1].Item2;
                float curveRatio = (accRatio - min) / (max - min);

                return (minValue + (maxValue - minValue) * curveRatio) / maxPpPercent;
            }

            return 1;
        }

        private static IEnumerator AnimateCircle(ImageView img, float final, float totalTime)
        {
            float timeLeft = totalTime;
            img.fillAmount = 0;

            yield return new WaitForSeconds(1.5f);

            while(timeLeft > Time.deltaTime)
            {
                timeLeft -= Time.deltaTime;
                img.fillAmount = Mathf.SmoothStep(0, final, 1 - timeLeft / totalTime);
                yield return null;
            }

            img.fillAmount = final;
        }
    }
}
