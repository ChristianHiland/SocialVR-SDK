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
        public string platform;
    }

    [System.Serializable]
    public class GetWorldData {
        public List<WorldInfoGet> Worlds;
    }
    
    [System.Serializable]
    public class Room {
        public string worldName;
        public string worldPublisher;
        public string InstanceName;
        public string InstanceID;
        public string Owner;
    }

    [System.Serializable]
    public class InstanceList {
        public Room[] Rooms;
    }

    [System.Serializable]
    public enum PlatformType {
        VR,
        Desktop
    }

<<<<<<< Updated upstream
=======
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
    [System.Serializable]
    public class YouTubeResolverResponse {
        public string url;
    }

    [System.Serializable]
    public class SubtitleEntry {
        public double startTime;
        public double endTime;
        public string text;

        public SubtitleEntry(double start, double end, string txt) {
            startTime = start;
            endTime = end;
            text = txt;
        }
    }

<<<<<<< Updated upstream
=======
    [System.Serializable]
    public enum TargetPlatform {
        Windows,
        Linux,
        Android
    }

>>>>>>> Stashed changes
>>>>>>> Stashed changes
    public class Structs {
        
    }
}