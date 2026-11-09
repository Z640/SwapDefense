using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Prefab & System References")]
    [SerializeField] private Monster fastMonsterPrefab;
    [SerializeField] private Monster slowMonsterPrefab;
    [SerializeField] private Transform monsterContainer;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private VFXManager vfxManager;
    [SerializeField] private AbilitySwapSystem abilitySwapSystem;

    [Header("Phase Spawn Tuning")]
    [SerializeField] private float phase1SpawnInterval = GameConstants.Phase1SpawnInterval;
    [SerializeField] private float phase2SpawnInterval = GameConstants.Phase2SpawnInterval;
    [SerializeField] private float phase3SpawnInterval = GameConstants.Phase3SpawnInterval;

    [SerializeField] private int phase1SpawnBatchCount = GameConstants.Phase1SpawnBatchCount;
    [SerializeField] private int phase2SpawnBatchCount = GameConstants.Phase2SpawnBatchCount;
    [SerializeField] private int phase3SpawnBatchCount = GameConstants.Phase3SpawnBatchCount;

    [SerializeField] private int phase1MaxMonsters = GameConstants.Phase1MaxMonsterCount;
    [SerializeField] private int phase2MaxMonsters = GameConstants.Phase2MaxMonsterCount;
    [SerializeField] private int phase3MaxMonsters = GameConstants.Phase3MaxMonsterCount;

    [Header("Runtime State")]
    [SerializeField] private float spawnTimer;
    [SerializeField] private int nextSpawnId;
    [SerializeField] private int patternIndex;
    [SerializeField] private int currentPhaseIndex;

    private readonly List<Monster> activeMonsters = new List<Monster>();

    private static readonly EnemyType[] phase1Pattern = new EnemyType[]
    {
        EnemyType.Fast,
        EnemyType.Fast,
        EnemyType.Slow
    };

    private static readonly EnemyType[] phase2Pattern = new EnemyType[]
    {
        EnemyType.Fast,
        EnemyType.Slow
    };

    private static readonly EnemyType[] phase3Pattern = new EnemyType[]
    {
        EnemyType.Fast,
        EnemyType.Fast,
        EnemyType.Slow
    };

    public IReadOnlyList<Monster> ActiveMonsters => activeMonsters;
    public int CurrentPhaseNumber => currentPhaseIndex + 1;

    public void SetupReferences(
        Monster fastPrefab,
        Monster slowPrefab,
        Transform container,
        GameManager manager,
        VFXManager vfx,
        AbilitySwapSystem swapSystem = null)
    {
        fastMonsterPrefab = fastPrefab;
        slowMonsterPrefab = slowPrefab;
        monsterContainer = container;
        gameManager = manager;
        vfxManager = vfx;
        abilitySwapSystem = swapSystem;

        phase1SpawnInterval = GameConstants.Phase1SpawnInterval;
        phase2SpawnInterval = GameConstants.Phase2SpawnInterval;
        phase3SpawnInterval = GameConstants.Phase3SpawnInterval;

        phase1SpawnBatchCount = GameConstants.Phase1SpawnBatchCount;
        phase2SpawnBatchCount = GameConstants.Phase2SpawnBatchCount;
        phase3SpawnBatchCount = GameConstants.Phase3SpawnBatchCount;

        phase1MaxMonsters = GameConstants.Phase1MaxMonsterCount;
        phase2MaxMonsters = GameConstants.Phase2MaxMonsterCount;
        phase3MaxMonsters = GameConstants.Phase3MaxMonsterCount;
    }

    public void ResetSpawner()
    {
        ClearAllMonsters();
        spawnTimer = GameConstants.InitialSpawnDelay;
        nextSpawnId = 0;
        patternIndex = 0;
        currentPhaseIndex = 0;
    }

    public void ClearAllMonsters()
    {
        for (int i = activeMonsters.Count - 1; i >= 0; i--)
        {
            if (activeMonsters[i] != null)
            {
                Destroy(activeMonsters[i].gameObject);
            }
        }
        activeMonsters.Clear();
    }

    public void TickSpawner(float deltaTime, float survivalTime, PlayerController playerController)
    {
        PruneDeadMonsters();

        int targetPhase = GetPhaseIndex(survivalTime);
        if (targetPhase != currentPhaseIndex)
        {
            currentPhaseIndex = targetPhase;
            patternIndex = 0;
        }

        spawnTimer -= deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnBatchForCurrentPhase(survivalTime);
            spawnTimer += GetEffectiveSpawnInterval(survivalTime);
        }

        for (int i = activeMonsters.Count - 1; i >= 0; i--)
        {
            Monster monster = activeMonsters[i];
            if (monster == null || monster.IsDead)
            {
                activeMonsters.RemoveAt(i);
                continue;
            }

            monster.TickMonster(deltaTime, playerController);

            if (monster == null || monster.IsDead)
            {
                activeMonsters.RemoveAt(i);
            }
        }
    }

    public void RefreshActiveMonstersForAbilityChanges()
    {
        for (int i = 0; i < activeMonsters.Count; i++)
        {
            Monster monster = activeMonsters[i];
            if (monster != null && !monster.IsDead)
            {
                monster.SyncColliderRadius();
                monster.UpdateVisualState();
            }
        }
    }

    private void PruneDeadMonsters()
    {
        for (int i = activeMonsters.Count - 1; i >= 0; i--)
        {
            if (activeMonsters[i] == null || activeMonsters[i].IsDead)
            {
                activeMonsters.RemoveAt(i);
            }
        }
    }

    public int GetPhaseIndex(float survivalTime)
    {
        if (survivalTime < GameConstants.Phase1EndTime)
        {
            return 0;
        }
        if (survivalTime < GameConstants.Phase2EndTime)
        {
            return 1;
        }
        return 2;
    }

    public float GetEffectiveSpawnInterval(float survivalTime)
    {
        float baseInterval = phase3SpawnInterval;
        if (survivalTime < GameConstants.Phase1EndTime)
        {
            baseInterval = phase1SpawnInterval;
        }
        else if (survivalTime < GameConstants.Phase2EndTime)
        {
            baseInterval = phase2SpawnInterval;
        }

        float multiplier = abilitySwapSystem != null
            ? abilitySwapSystem.GetMonsterSpawnIntervalMultiplier()
            : 1f;

        return Mathf.Max(0.15f, baseInterval * multiplier);
    }

    public int GetSpawnBatchCount(float survivalTime)
    {
        if (survivalTime < GameConstants.Phase1EndTime)
        {
            return phase1SpawnBatchCount;
        }
        if (survivalTime < GameConstants.Phase2EndTime)
        {
            return phase2SpawnBatchCount;
        }
        return phase3SpawnBatchCount;
    }

    public int GetMaxConcurrentMonsters(float survivalTime)
    {
        if (survivalTime < GameConstants.Phase1EndTime)
        {
            return phase1MaxMonsters;
        }
        if (survivalTime < GameConstants.Phase2EndTime)
        {
            return phase2MaxMonsters;
        }
        return phase3MaxMonsters;
    }

    private EnemyType[] GetCurrentPattern(float survivalTime)
    {
        if (survivalTime < GameConstants.Phase1EndTime)
        {
            return phase1Pattern;
        }
        if (survivalTime < GameConstants.Phase2EndTime)
        {
            return phase2Pattern;
        }
        return phase3Pattern;
    }

    private void SpawnBatchForCurrentPhase(float survivalTime)
    {
        int maxMonsters = GetMaxConcurrentMonsters(survivalTime);
        int availableSlots = maxMonsters - activeMonsters.Count;
        if (availableSlots <= 0)
        {
            return;
        }

        int batchCount = Mathf.Min(GetSpawnBatchCount(survivalTime), availableSlots);
        float laneMinX = GameConstants.SpawnMinX;
        float laneMaxX = GameConstants.SpawnMaxX;
        float laneWidth = laneMaxX - laneMinX;

        for (int i = 0; i < batchCount; i++)
        {
            EnemyType[] pattern = GetCurrentPattern(survivalTime);
            EnemyType typeToSpawn = pattern[patternIndex % pattern.Length];
            patternIndex = (patternIndex + 1) % pattern.Length;

            float bodyRadius = typeToSpawn == EnemyType.Fast
                ? GameConstants.FastEnemyBodyRadius
                : GameConstants.SlowEnemyBodyRadius;

            float segmentMinX = laneMinX + (laneWidth * i / batchCount) + bodyRadius;
            float segmentMaxX = laneMinX + (laneWidth * (i + 1) / batchCount) - bodyRadius;
            if (segmentMinX > segmentMaxX)
            {
                segmentMinX = laneMinX + bodyRadius;
                segmentMaxX = laneMaxX - bodyRadius;
            }

            float randomX = Random.Range(segmentMinX, segmentMaxX);
            Vector2 spawnPos = new Vector2(randomX, GameConstants.SpawnLineY);
            SpawnEnemy(typeToSpawn, spawnPos);
        }
    }

    public Monster SpawnEnemy(EnemyType enemyType, Vector2 spawnPosition)
    {
        Monster prefabToUse = enemyType == EnemyType.Fast ? fastMonsterPrefab : slowMonsterPrefab;
        if (prefabToUse == null)
        {
            return null;
        }

        int id = nextSpawnId;
        nextSpawnId++;

        Monster monster = Instantiate(prefabToUse, monsterContainer);
        monster.name = $"Monster_{enemyType}_{id}";
        monster.Initialize(enemyType, id, spawnPosition, gameManager, vfxManager, abilitySwapSystem);
        activeMonsters.Add(monster);
        return monster;
    }
}
