using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinishZone : MonoBehaviour
{
    [SerializeField] private float animationDuration = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(AnimateAndLoadMenu(collision.transform));
        }
    }

    private IEnumerator AnimateAndLoadMenu(Transform playerTransform)
    {
        var playerController = playerTransform.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.GameStarted = false;

        Vector3 startPos = playerTransform.position;
        Vector3 endPos = transform.position;

        float startAngle = playerTransform.eulerAngles.z;
        float endAngle = startAngle + 360f;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;

            playerTransform.position = Vector3.Lerp(startPos, endPos, t);

            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            playerTransform.rotation = Quaternion.Euler(0, 0, currentAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerTransform.position = endPos;
        playerTransform.rotation = Quaternion.Euler(0, 0, endAngle);

        if (SplatManager.Instance != null)
            SplatManager.Instance.DestroySelf();

        SaveData.MarkLevelAsCompleted(SaveData.CurrentLevelIndex);

        SceneManager.LoadScene("Menu");
    }
}