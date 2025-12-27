using UnityEngine;

public class BossTeacherController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPointPosition;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShotProjectile();
        }
    }

    private void ShotProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPointPosition);
        BulletBehaviour bulletBehaviour = projectile.GetComponent<BulletBehaviour>();
        var bulletType = BulletBehaviour.BulletType.Straight;
        Transform playerTransform = GameManager.Instance.Player.transform;
        bulletBehaviour.Init(bulletType, playerTransform);
    }
}
