using UnityEngine;

public enum TriggerCategory
{
    Trap,
    Ray,
    Bullet
}

public class DynamicTrigger : MonoBehaviour
{
    [SerializeField] private TriggerCategory category;

    [SerializeField] private TrapType trapType;
    [SerializeField] private RayType rayType;
    [SerializeField] private BulletType bulletType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Игрок коснулся");
            switch (category)
            {
                case TriggerCategory.Trap:
                    switch (trapType)
                    {
                        case TrapType.Static: ActionBus.InvokeTriggerStaticTrap(); break;
                        case TrapType.Falling: ActionBus.InvokeTriggerFallingTrap(); break;
                        case TrapType.Column: ActionBus.InvokeTriggerColumnTrap(); break;

                    }
                    break;

                case TriggerCategory.Ray:
                    switch (rayType) { }
                    break;

                case TriggerCategory.Bullet:
                    switch (bulletType) { }
                    break;
            }
        }
    }
}