using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace SocialSDK {
    [System.Serializable]
    public class LoginProcessed : UnityEvent<UserData> { }
    
    [System.Serializable]
    public class SignupProcessed : UnityEvent { }
    
    [System.Serializable]
    public class DownloadProcessed : UnityEvent<string, string, string> { }
    
    [System.Serializable]
    public class WorldListProcessed : UnityEvent<GetWorldData> { }
    
    [System.Serializable]
    public class WorldThumbnailProcessed : UnityEvent<string> { }
}
