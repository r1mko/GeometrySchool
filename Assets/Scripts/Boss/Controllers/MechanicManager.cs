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
        ActionBus.TriggeredColumnTrap += SpawnColumn;
    }

    private void OnDestroy()
    {
        ActionBus.TriggeredColumnTrap -= SpawnColumn;
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
                //rayController.ShotRay(RayType.Target);
                //rayController.ShotRay(RayType.Static);
                //rayController.ShotRay(RayType.Dynamic);
                //trapController.TriggerTrapPlacement(TrapType.Falling);
                //trapController.TriggerTrapPlacement(TrapType.Static);
            }
        }
    }

    private void SpawnColumn()
    {
        trapController.TriggerTrapPlacement(TrapType.Column);
    }
}