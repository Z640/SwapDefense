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

    [Header("Runtime State")]
    [SerializeField] private float spawnTimer;
    [SerializeField] private int nextSpawnId;
    [SerializeField] private int patternIndex;
    [SerializeField] private int currentPhaseIndex = -1;

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

    public void SetupReferences(Monster fastPrefab, Monster slowPrefab, Transform container, GameManager manager, VFXManager vfx)
    {
        fastMonsterPrefab = fastPrefab;
        slowMonsterPrefab = slowPrefab;
        monsterContainer = container;
        gameManager = manager;
        vfxManager = vfx;
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
        int targetPhase = GetPhaseIndex(survivalTime);
        if (targetPhase != currentPhaseIndex)
        {
            currentPhaseIndex = targetPhase;
            patternIndex = 0;
        }

        spawnTimer -= deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnNextPatternEnemy(survivalTime);
            spawnTimer += GetSpawnInterval(survivalTime);
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

    private int GetPhaseIndex(float survivalTime)
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

    private float GetSpawnInterval(float survivalTime)
    {
        if (survivalTime < GameConstants.Phase1EndTime)
        {
            return GameConstants.Phase1SpawnInterval;
        }
        if (survivalTime < GameConstants.Phase2EndTime)
        {
            return GameConstants.Phase2SpawnInterval;
        }
        return GameConstants.Phase3SpawnInterval;
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

    private void SpawnNextPatternEnemy(float survivalTime)
    {
        EnemyType[] pattern = GetCurrentPattern(survivalTime);
        EnemyType typeToSpawn = pattern[patternIndex % pattern.Length];
        patternIndex = (patternIndex + 1) % pattern.Length;

        float bodyRadius = typeToSpawn == EnemyType.Fast
            ? GameConstants.FastEnemyBodyRadius
            : GameConstants.SlowEnemyBodyRadius;

        float minX = GameConstants.SpawnMinX + bodyRadius;
        float maxX = GameConstants.SpawnMaxX - bodyRadius;
        float randomX = Random.Range(minX, maxX);
        Vector2 spawnPos = new Vector2(randomX, GameConstants.SpawnLineY);

        SpawnEnemy(typeToSpawn, spawnPos);
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
        monster.Initialize(enemyType, id, spawnPosition, gameManager, vfxManager);
        activeMonsters.Add(monster);
        return monster;
    }
}
