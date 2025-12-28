using UnityEngine;

public class BossTeacherController : MonoBehaviour
{
    // Common
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPointPosition;
    
    // Wave
    [SerializeField] private float waveAmplitudeStep = 4f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RayShot();
        }
    }

    private void ShotProjectile(BulletBehaviour.BulletType bulletType)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPointPosition);
        BulletBehaviour bulletBehaviour = projectile.GetComponent<BulletBehaviour>();
        Transform playerTransform = GameManager.Instance.Player.transform;
        bulletBehaviour.Init(bulletType, playerTransform, waveAmplitudeStep);
        waveAmplitudeStep = -waveAmplitudeStep;
    }

    //for subs
    private void StraigthShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Straight);
    }

    private void WaveShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Wave);
        //ShotProjectile(BulletBehaviour.BulletType.Wave);
    }

    private void RayShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Ray);
    }
}
