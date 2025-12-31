using UnityEngine;

namespace SocialSDK {
    [CreateAssetMenu(fileName = "SettingsFile", menuName = "SocialSDK/Settings Asset")]
    public class Settings : ScriptableObject {
        public string serverURL = "http://127.0.0.1:8002/";
        public string userLoginFile = "User/player_login.info";
        public string worldPath = "Worlds/";
        public string worldThumbnailPath = "Worlds/thumbnails/";
        
        
        // User Info (Like Login, Default World, Avatar)
        public UserData userData = new UserData();
        public WorldInfoGet defaultWorld = new WorldInfoGet();
    }
}