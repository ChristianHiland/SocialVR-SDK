using UnityEngine;
using UnityEngine.XR;

public class VRRenderFixer : MonoBehaviour
{
    void Update()
    {
        // If you see the smearing, press a key or button to run this
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // 1. Kill any rogue cameras
            foreach (var cam in FindObjectsOfType<Camera>())
            {
                if (cam.gameObject != Camera.main.gameObject)
                {
                    cam.enabled = false;
                }
            }
            // 2. Refresh the XR display
            XRSettings.enabled = false;
            XRSettings.enabled = true;
            Debug.Log("Forced Renderer Refresh");
        }
    }
}