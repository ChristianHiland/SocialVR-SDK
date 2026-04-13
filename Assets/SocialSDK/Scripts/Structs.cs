using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;

namespace SocialSDK {

    [System.Serializable]
    public class UserAssets {
        public string pfp;
        public string[] Stickers;
    }

    [System.Serializable]
    public class UserOptions {
        public string Rank;
    }

    [System.Serializable]
    public class UserData {
        public string DisplayName;
        public UserAssets Assets;
        public UserOptions Options;
    }

    [System.Serializable]
    public class WorldData {
        public string name;
        public string publisher;
        public string scene;
        public string bundlepath;
        public string bundledata;
        public string worldthumbnail;
    }

    public class UserLogin {
        public string username;
        public string password;
    }
    
    public class UserSignup {
        public string name;
        public string username;
        public string password;
    }

    [System.Serializable]
    public class WorldInfoGet {
        public string name;
        public string publisher;
    }

    [System.Serializable]
    public class GetWorldData {
        public List<WorldInfoGet> Worlds;
    }

    public struct InstanceCreationData {
        public RoomOptions roomOptions;
        public ExitGames.Client.Photon.Hashtable roomProps;
        public ExitGames.Client.Photon.Hashtable playerProps;
    }
    
    public class Structs {
        
    }
}