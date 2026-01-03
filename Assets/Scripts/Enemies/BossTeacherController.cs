using UnityEngine;

public class BossTeacherController : MonoBehaviour
{
    // Common
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPointPosition;
    
    // Wave
    [SerializeField] private float waveAmplitudeStep = 4f;

    // Movement
    public PlayerController player;
    public float initialBossOffsetX = 17f;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RayShot();
        }
    }

    private void FixedUpdate()
    {
        Vector3 bossPos = transform.position;
        bossPos.x = player.transform.position.x + initialBossOffsetX;
        transform.position = new Vector3(bossPos.x, transform.position.y, transform.position.z);
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
