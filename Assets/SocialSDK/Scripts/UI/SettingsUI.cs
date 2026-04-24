using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;
using SocialSDK.Audio;
using UnityEngine.UI;
using UnityEngine;


namespace SocialSDK.UI {
    public class SettingsUI : MonoBehaviour {

        [Header("Volume")]
        [SerializeField] Slider worldVolume;
        [SerializeField] Slider videoVolume;

        void Start() {
            worldVolume.onValueChanged.AddListener(HandleWorldVolChange);
            videoVolume.onValueChanged.AddListener(HandleVideoVolChange);
        }

        public void HandleWorldVolChange(float value) {
            SocialWorldVolume volumeManager = GameObject.Find("World Audio Mixer").GetComponent<SocialWorldVolume>();

            float volume = Mathf.Clamp(value, 0.0001f, 1.5f);

            float dB = Mathf.Log10(volume) * 20;

            volumeManager.mainMixer.SetFloat("WorldVolume", dB);
        }

        public void HandleVideoVolChange(float value) {
            SocialWorldVolume volumeManager = GameObject.Find("World Audio Mixer").GetComponent<SocialWorldVolume>();

            float volume = Mathf.Clamp(value, 0.0001f, 1.5f);

            float dB = Mathf.Log10(volume) * 20;

            volumeManager.mainMixer.SetFloat("VideoVolume", dB);
        }

    }
}