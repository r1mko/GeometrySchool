using UnityEngine;

public enum TriggerType
{
    // Traps
    StaticTrap,
    FallingTrap,
    ColumnTrap,

    // Rays
    DynamicRay,
    StaticRay,
    TargetRay,

    // Bullets
    StraightBullet,
    WaveBullet,
    ZBullet
}

public class DynamicTrigger : MonoBehaviour
{
    [SerializeField] private TriggerType currentTriggerType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Игрок коснулся");
            switch (currentTriggerType)
            {
                // Traps
                case TriggerType.StaticTrap: ActionBus.InvokeTriggerStaticTrap(); break;
                case TriggerType.FallingTrap: ActionBus.InvokeTriggerFallingTrap(); break;
                case TriggerType.ColumnTrap: ActionBus.InvokeTriggerColumnTrap(); break;

                //// Rays
                //case TriggerType.DynamicRay: ActionBus.InvokeTriggerDynamicRay(); break;
                //case TriggerType.StaticRay: ActionBus.InvokeTriggerStaticRay(); break;
                //case TriggerType.TargetRay: ActionBus.InvokeTriggerTargetRay(); break;

                //// Bullets
                //case TriggerType.StraightBullet: ActionBus.InvokeTriggerStraightBullet(); break;
                //case TriggerType.WaveBullet: ActionBus.InvokeTriggerWaveBullet(); break;
                //case TriggerType.ZBullet: ActionBus.InvokeTriggerZBullet(); break;
            }
        }
    }
}