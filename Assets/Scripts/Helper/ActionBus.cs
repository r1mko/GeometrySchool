using System;
using UnityEngine;

public static class ActionBus
{
    //spawned
    public static event Action<Vector3> SpawnedStaticTrap;
    public static event Action SpawnedColumnTrap;
    public static event Action SpawnedFallingTrap;

    //triggered
    public static event Action TriggeredStaticTrap;
    public static event Action TriggeredColumnTrap;
    public static event Action TriggeredFallingTrap;

    //spawned
    public static void InvokeSpawnStaticTrap(Vector3 worldPosition)
    {
        SpawnedStaticTrap?.Invoke(worldPosition);
    }

    public static void InvokeSpawnColumn()
    {
        SpawnedColumnTrap?.Invoke();
    }

    public static void InvokeSpawnFallingTrap()
    {
        SpawnedFallingTrap?.Invoke();
    }

    //triggered
    public static void InvokeTriggerStaticTrap()
    {
        TriggeredStaticTrap?.Invoke();
    }

    public static void InvokeTriggerColumnTrap()
    {
        TriggeredColumnTrap?.Invoke();
    }

    public static void InvokeTriggerFallingTrap()
    {
        TriggeredFallingTrap?.Invoke();
    }
}
