using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossTeacherController : MonoBehaviour
{
    // Common
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPointPosition;
    [SerializeField] private GameObject trapPrefab;

    // Trap Grid System
    [System.Serializable]
    public class SpawnCell
    {
        public Transform spawnPoint;
        public int row;
        public int column;
        public bool isBlocked;
    }

    [SerializeField] private List<SpawnCell> spawnCells = new();
    [SerializeField] private Transform spawnPointsContainer;
    [SerializeField] private float minPlayerDistance = 4f;
    [SerializeField] private float resetOffset = 1f;

    private Vector3 originalContainerLocalPosition;
    private bool isTrapSystemActive;
    private HashSet<SpawnCell> usedCells = new HashSet<SpawnCell>();
    private Dictionary<int, int> availableCellsPerRow = new Dictionary<int, int>();
    private float lastColumnWorldX;

    public PlayerController player;

    private void Start()
    {
        player = GameManager.Instance.Player;
        originalContainerLocalPosition = spawnPointsContainer.localPosition;
        CacheLastColumnPosition();
        ResetTrapSystemState();
    }

    private void Update()
    {
        HandleTrapSystemActivation();
        HandleTrapSpawning();
        CheckResetCondition();
    }

    private void FixedUpdate()
    {
        FollowPlayerWithBoss();
        UpdateTrapContainerPosition();
    }

    private void FollowPlayerWithBoss()
    {
        Vector3 bossPos = transform.position;
        bossPos.x = player.transform.position.x + 17f; // initialBossOffsetX
        transform.position = bossPos;
    }

    private void UpdateTrapContainerPosition()
    {
        if (!isTrapSystemActive)
        {
            spawnPointsContainer.localPosition = originalContainerLocalPosition;
        }
    }

    private void HandleTrapSystemActivation()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrapSystemActive)
        {
            ActivateTrapSystem();
        }
    }

    private void HandleTrapSpawning()
    {
        if (!isTrapSystemActive || !Input.GetKeyDown(KeyCode.Space)) return;

        SpawnRandomTrap();
    }

    private void CheckResetCondition()
    {
        if (isTrapSystemActive && player.transform.position.x > lastColumnWorldX + resetOffset)
        {
            ResetTrapSystem();
        }
    }

    private void ActivateTrapSystem()
    {
        isTrapSystemActive = true;
        usedCells.Clear();
        ResetBlockedStates();
        CacheAvailableCellsPerRow();
        UpdateLastColumnWorldPosition();
        DetachSpawnContainer();
    }

    private void ResetTrapSystem()
    {
        isTrapSystemActive = false;
        usedCells.Clear();
        ResetBlockedStates();
        AttachSpawnContainer();
    }

    private void ResetTrapSystemState()
    {
        isTrapSystemActive = false;
        usedCells.Clear();
        ResetBlockedStates();
    }

    private void ResetBlockedStates()
    {
        foreach (var cell in spawnCells)
        {
            cell.isBlocked = false;
        }
    }

    private void DetachSpawnContainer()
    {
        spawnPointsContainer.SetParent(null);
    }

    private void AttachSpawnContainer()
    {
        spawnPointsContainer.SetParent(transform);
        spawnPointsContainer.localPosition = originalContainerLocalPosition;
    }

    private void SpawnRandomTrap()
    {
        UpdateBlockedStates();
        UpdateAvailableCellsPerRow();

        var eligibleRows = availableCellsPerRow
            .Where(kvp => kvp.Value >= 2)
            .Select(kvp => kvp.Key)
            .ToList();

        if (eligibleRows.Count == 0) return;

        int randomRow = eligibleRows[Random.Range(0, eligibleRows.Count)];
        var availableCells = GetAvailableCellsInRow(randomRow);

        if (availableCells.Count == 0) return;

        SpawnCell selectedCell = availableCells[Random.Range(0, availableCells.Count)];
        usedCells.Add(selectedCell);
        selectedCell.isBlocked = true;

        GameObject trapObj = Instantiate(trapPrefab, selectedCell.spawnPoint.position, Quaternion.identity);
        if (trapObj.TryGetComponent<TrapBehaviour>(out var trap))
        {
            trap.Init(TrapBehaviour.TrapType.Middle, player.transform, this, selectedCell.spawnPoint.position);
        }

        availableCellsPerRow[randomRow]--;
    }

    private void UpdateBlockedStates()
    {
        float playerX = player.transform.position.x;

        foreach (var cell in spawnCells)
        {
            cell.isBlocked = usedCells.Contains(cell) ||
                             cell.spawnPoint.position.x < playerX + minPlayerDistance;
        }
    }

    private void UpdateAvailableCellsPerRow()
    {
        availableCellsPerRow.Clear();

        foreach (var cell in spawnCells)
        {
            if (cell.isBlocked) continue;

            if (!availableCellsPerRow.ContainsKey(cell.row))
                availableCellsPerRow[cell.row] = 0;

            availableCellsPerRow[cell.row]++;
        }
    }

    private void CacheAvailableCellsPerRow()
    {
        availableCellsPerRow = spawnCells
            .Where(cell => !cell.isBlocked)
            .GroupBy(cell => cell.row)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private List<SpawnCell> GetAvailableCellsInRow(int row)
    {
        return spawnCells
            .Where(cell => cell.row == row && !cell.isBlocked && !usedCells.Contains(cell))
            .ToList();
    }

    private void CacheLastColumnPosition()
    {
        int lastColumn = spawnCells.Max(cell => cell.column);
        lastColumnWorldX = spawnCells
            .First(cell => cell.column == lastColumn)
            .spawnPoint.position.x;
    }

    private void UpdateLastColumnWorldPosition()
    {
        int lastColumn = spawnCells.Max(cell => cell.column);
        lastColumnWorldX = spawnPointsContainer.TransformPoint(
            spawnCells.First(cell => cell.column == lastColumn).spawnPoint.localPosition
        ).x;
    }

    private void ShotProjectile(BulletBehaviour.BulletType bulletType)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPointPosition);
        if (projectile.TryGetComponent<BulletBehaviour>(out var bulletBehaviour))
        {
            Transform playerTransform = GameManager.Instance.Player.transform;
            bulletBehaviour.Init(bulletType, playerTransform, 4f); // waveAmplitudeStep
        }
    }

    // For subs
    private void StraigthShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Straight);
    }

    private void WaveShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Wave);
    }

    private void RayShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Ray);
    }

}