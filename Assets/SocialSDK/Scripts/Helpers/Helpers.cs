using SocialSDK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class Helpers {
    public static Settings settings;

    public static void DeleteAllChildren(GameObject parent) {
        for (int i = parent.transform.childCount - 1; i >= 0; i--) {
            Object.Destroy(parent.transform.GetChild(i).gameObject);
        }
    }

    public static Texture2D GetImageFromPath(string path) {
        if (!File.Exists(path)) return null;

        try {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            return texture;
        } catch (IOException e) {
            Debug.LogError($"File Locked: {e.Message}");
        }
        return null;
    }

    public static bool IsFileLocked(string path) {
        if (!File.Exists(path)) return false;
        try {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None)) {
                stream.Close();
            }
        } catch (IOException) {
            return true;
        }
        return false;
    }
}
