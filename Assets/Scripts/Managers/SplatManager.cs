using UnityEngine;

public class SplatManager : MonoBehaviour
{
    public static SplatManager Instance { get; private set; }

    public GameObject bloodSplatPrefab;
    public Sprite[] bloodSprites;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnBloodSplat(Transform target)
    {
        if (bloodSplatPrefab == null || bloodSprites == null || bloodSprites.Length == 0)
        {
            Debug.LogWarning("Blood splat prefab or sprites not assigned in SplatManager.");
            return;
        }

        GameObject splat = Instantiate(bloodSplatPrefab, target.position, Quaternion.identity, transform);

        SpriteRenderer sr = splat.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = bloodSprites[Random.Range(0, bloodSprites.Length)];
        }

        float randomZRotation = Random.Range(0, 360);
        splat.transform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);
    }

    public void DestroySelf()
    {
        Instance = null;
        Destroy(gameObject);
    }
}