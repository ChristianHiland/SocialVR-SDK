using UnityEngine;
using UnityEngine.SceneManagement;
using SocialSDK;
using TMPro;

public class SceneRefHandler : MonoBehaviour {
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Use unique names or tags to find your target
        GameObject target = GameObject.Find("SocialSDK");
        GameObject reasonObj = GameObject.Find("Reason");
        if (target != null) {
            API api = target.GetComponent<API>();
            TMP_Text reasonText = reasonObj.GetComponent<TMP_Text>();
            reasonText.text = api.errorReason;
        }
    }
}
