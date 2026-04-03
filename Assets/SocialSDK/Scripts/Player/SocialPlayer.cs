using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SocialSDK {
    public class SocialPlayer : MonoBehaviour {
        [Header("Player Nametag")]
        public GameObject playerTag;

        [Header("Desktop Name Tag")]
        public GameObject desktopNameTagObj;
        public TMP_Text displayName;
        public TMP_Text rankText;
        
        [Header("Data & Info")]
        public UserData playerData;

        public void SaveUserRuntime(UserData userData) {
            playerData = userData;

            desktopNameTagObj.SetActive(true);
            displayName.text = playerData.DisplayName;
            if (playerData.Options.Rank == "Admin") {
                rankText.text = "Admin";
                rankText.color = new Color(153, 0, 255);
            } else {
                rankText.text = playerData.Options.Rank;
            }
        }
    }
}
