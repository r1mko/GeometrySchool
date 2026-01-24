using UnityEngine;

public class MechanicManager : MonoBehaviour
{
    // Movement
    public PlayerController player;
    public float initialBossOffsetX = 17f;

    // Trap
    [SerializeField] private TrapController trapController;
    // Ray
    [SerializeField] private RayController rayController;
    // Wave
    [SerializeField] private BulletController bulletController;

    private void Start()
    {
        player = GameManager.Instance.Player;
        trapController = GetComponent<TrapController>();
        rayController = GetComponent<RayController>();
        bulletController = GetComponent<BulletController>();

        //subs
        ActionBus.TriggeredTrap += SpawnTrap;
        ActionBus.TriggerSpawnRay += SpawnRay;
    }

    private void OnDestroy()
    {
        ActionBus.TriggeredTrap -= SpawnTrap;
        ActionBus.TriggerSpawnRay -= SpawnRay;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        Vector3 bossPos = transform.position;
        bossPos.x = player.transform.position.x + initialBossOffsetX;
        transform.position = bossPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 1; i++)
            {
                //bulletController.ShotProjectile(BulletType.Straight);
                //bulletController.ShotProjectile(BulletType.Wave);
                //bulletController.ShotProjectile(BulletType.ZBullet);
            }
        }
    }

    private void SpawnTrap(TrapType trapType)
    {
        trapController.TriggerTrapPlacement(trapType);
    }

    private void SpawnRay(RayType rayType, int spawnIndex, bool fromTop)
    {
        rayController.ShotRay(rayType, spawnIndex, fromTop);
    }
}