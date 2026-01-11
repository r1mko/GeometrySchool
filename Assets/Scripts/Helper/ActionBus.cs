using System;
using UnityEngine;

public static class ActionBus
{
    public static event Action<Vector3> SpawnStaticTrap;
    public static void InvokeSpawnStaticTrap(Vector3 worldPosition)
    {
        SpawnStaticTrap?.Invoke(worldPosition);
    }
}
