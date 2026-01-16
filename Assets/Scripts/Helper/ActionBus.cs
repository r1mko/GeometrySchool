using System;
using UnityEngine;

public static class ActionBus
{
    public static event Action<Vector3> SpawnedStaticTrap;
    public static event Action SpawnedColumnTrap;
    public static event Action SpawnedFallingTrap;

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
}
