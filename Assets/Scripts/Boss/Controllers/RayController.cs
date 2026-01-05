using UnityEngine;

public class RayController : MonoBehaviour
{
    [SerializeField] private GameObject rayPrefab;
    [SerializeField] private Transform raySpawnPoint;

    private PlayerController player;
    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public void ShotRay(RayType rayType)
    {
        GameObject projectile = Instantiate(rayPrefab, raySpawnPoint);
        if (projectile.TryGetComponent<RayBehaviour>(out var rayBehaviour))
        {
            rayBehaviour.Init(rayType, player.transform);
        }
    }
}