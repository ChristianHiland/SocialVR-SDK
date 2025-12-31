using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour {
    [Header("Settings")]
    public Color highlightColor = Color.cyan;
    private Color originalColor;
    private Renderer myRenderer;

    [Header("Events")]
    // This is where you drag-and-drop what happens (e.g., Play Animation, Add Score)
    public UnityEvent onInteract;

    public GameObject heldObj;
    public Rigidbody heldObjRb;
    public Transform holdPoint;

    void Start() {
        myRenderer = GetComponent<Renderer>();
        originalColor = myRenderer.material.color;
        holdPoint = GameObject.Find("HoldPoint").transform;
    }

    // Called when the player looks at the object
    public void OnLook() {
        myRenderer.material.color = highlightColor;
    }

    // Called when the player looks away
    public void OnLookAway() {
        myRenderer.material.color = originalColor;
    }

    public void PickupObject() {
        GameObject pickObj = gameObject;
        if (pickObj.GetComponent<Rigidbody>()) {
            heldObj = pickObj;
            heldObjRb = pickObj.GetComponent<Rigidbody>();

            // 3. Disable physics so it doesn't fall
            heldObjRb.useGravity = false;
            heldObjRb.drag = 10; // Adds "air resistance" to stop it from swinging wildly
            heldObjRb.isKinematic = true; // Hard lock: physics won't move it, only code will

            // 4. Parent it so it moves with player? 
            // NOTE: Parenting can cause scale issues. It's safer to just move it in Update.
            heldObj.transform.parent = holdPoint;

            // Reset rotation to match player (optional)
            heldObj.transform.localPosition = Vector3.zero;
            heldObj.transform.localRotation = Quaternion.identity;
        }
    }

    public void DropObject() {
        heldObjRb.useGravity = true;
        heldObjRb.drag = 1;
        heldObjRb.isKinematic = false;
        
        heldObj.transform.parent = null;
        heldObj = null;
    }

    // Called when the player presses the interact button
    public void Interact() {
        onInteract.Invoke();
    }
}