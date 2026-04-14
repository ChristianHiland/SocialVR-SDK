using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerNotification : MonoBehaviour {
    public TextMeshProUGUI notificationText;
    public CanvasGroup canvasGroup;

    public void ShowMessage() {
        StartCoroutine(FadeAndMove());
    }

    private IEnumerator FadeAndMove() {
        float duration = 2.0f;
        float elapsed = 0f;

        Vector3 startPos = new Vector3(0, -437.1f, 0);
        Vector3 endPos = new Vector3(0, -227.23f, 0);

        transform.localPosition = startPos;
        canvasGroup.alpha = 0;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            canvasGroup.alpha = Mathf.Sin(t * Mathf.PI);

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}