using UnityEngine;

public class RayController : MonoBehaviour
{
    [SerializeField] private GameObject rayPrefab;
    [SerializeField] private Transform raySpawnPoint;

    public void ShotRay(RayType rayType)
    {
        GameObject projectile = Instantiate(rayPrefab, raySpawnPoint);
        if (projectile.TryGetComponent<RayBehaviour>(out var rayBehaviour))
        {
            rayBehaviour.Init(rayType);
        }
    }
}