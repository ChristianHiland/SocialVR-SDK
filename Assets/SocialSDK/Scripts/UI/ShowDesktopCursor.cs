using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ShowDesktopCursor : MonoBehaviour {
    void Update() {
        // Get what the cursor is hitting. Pointer ID -1 is Mouse/Desktop Pointer.
        PointerEventData pointerData = GetPointerData();

        // Check if the pointer is hitting a UI element.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) {
            Cursor.visible = true;
        } else {
            Cursor.visible = false;
        }
    }

    private PointerEventData GetPointerData() {
        var eventSystem = EventSystem.current;
        if (eventSystem == null) return null;

        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        return eventData;
    }
}
