using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LoadMenuScene();
        }
    }

    private void LoadMenuScene()
    {
        if (SplatManager.Instance != null)
            SplatManager.Instance.DestroySelf();

        SceneManager.LoadScene("Menu");
    }
}
