using UnityEngine;

public class BossTeacherController : MonoBehaviour
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
    }

    private void FixedUpdate()
    {
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
                rayController.ShotRay(RayType.Static);

            }
        }
    }
}