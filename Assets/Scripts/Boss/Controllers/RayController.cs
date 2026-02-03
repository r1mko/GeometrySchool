using UnityEngine;

public class RayController : MonoBehaviour
{
    [SerializeField] private GameObject rayPrefab;
    [SerializeField] private Transform[] raySpawnPoint; // 0 1 2 from top

    private PlayerController player;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public void ShotRay(RayType rayType, int spawnPointIndex, bool fromTop)
    {
        if (spawnPointIndex > raySpawnPoint.Length || rayType == RayType.Dynamic)
        {
            spawnPointIndex = 1;
            Debug.Log("Dynamic type. Or index is out of bounds. Fallback to middle spawn point.");
        }

        GameObject projectile = Instantiate(rayPrefab, raySpawnPoint[spawnPointIndex]);
        if (projectile.TryGetComponent<RayBehaviour>(out var rayBehaviour))
        {
            rayBehaviour.Init(rayType, player.transform, fromTop);
        }
    }
}