using RenderHeads.Media.AVProVideo;
using System.Collections.Generic;
using System.Collections;
using TMPro;

using UnityEngine;
namespace SocialSDK.Video {
    public class ScratchCaptionDisplay : MonoBehaviour {
        [SerializeField] MediaPlayer _mediaPlayer;
        [SerializeField] TMP_Text _subtitleText;

        public List<SubtitleEntry> captions = new List<SubtitleEntry>();

        void Update() {
            if (_mediaPlayer == null || captions.Count == 0) return;

            double currentTime = _mediaPlayer.Control.GetCurrentTime();
            string currentLine = "";

            foreach (var entry in captions) {
                if (currentTime >= entry.startTime && currentTime <= entry.endTime) {
                    currentLine = entry.text;
                    break;
                }
            }

            if (_subtitleText.text != currentLine) {
                _subtitleText.text = currentLine;
            }
        }

    }
}