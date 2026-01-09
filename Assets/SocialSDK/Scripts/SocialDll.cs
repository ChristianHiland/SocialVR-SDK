using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SocialDll : MonoBehaviour {
    [DllImport("SocialVR_SDK")]
    public static extern IntPtr getServerWorldSize(string url, string world_name, string publisher);
    
    [DllImport("SocialVR_SDK")]
    public static extern IntPtr login(string url, string username, string password);
    
    [DllImport("SocialVR_SDK")]
    public static extern IntPtr signup(string name, string username, string password);

    [DllImport("SocialVR_SDK")]
    public static extern IntPtr checkForPresentLogin(string local_file);

    [DllImport("SocialVR_SDK")]
    public static extern void free_string(IntPtr ptr);
    
    public string HandleRustString(IntPtr ptr) {
        if (ptr == IntPtr.Zero) return "Error: Null Pointer returned";

        try {
            // Convert pointer to managed C# string
            string result = Marshal.PtrToStringAnsi(ptr);
            return result;
        } finally {
            // IMPORTANT: We must tell Rust to free the memory it allocated
            free_string(ptr);
        }
    }
}
