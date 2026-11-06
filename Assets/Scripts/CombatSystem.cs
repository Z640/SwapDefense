using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private class ActiveBullet
    {
        public GameObject BulletObject;
        public Vector2 Position;
        public Vector2 Direction;
    }

    [Header("References")]
    [SerializeField] private Transform bulletContainer;
    [SerializeField] private WaveSpawner waveSpawner;

    [Header("Targeting State")]
    [SerializeField] private Monster selectedTarget;

    private readonly List<ActiveBullet> activeBullets = new List<ActiveBullet>();

    public Monster SelectedTarget => selectedTarget;

    public void SetupReferences(Transform container, WaveSpawner spawner)
    {
        bulletContainer = container;
        waveSpawner = spawner;
    }

    public void ClearAllBullets()
    {
        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            if (activeBullets[i].BulletObject != null)
            {
                Destroy(activeBullets[i].BulletObject);
            }
        }
        activeBullets.Clear();
        selectedTarget = null;
    }

    public void SpawnBullet(Vector2 startPosition, Vector2 aimPosition)
    {
        Vector2 delta = aimPosition - startPosition;
        Vector2 direction = delta.sqrMagnitude > 0.0001f ? delta.normalized : Vector2.up;

        GameObject bulletObj = new GameObject("Bullet");
        if (bulletContainer != null)
        {
            bulletObj.transform.SetParent(bulletContainer, false);
        }
        bulletObj.transform.position = new Vector3(startPosition.x, startPosition.y, 0f);
        float diameter = GameConstants.BulletVisualRadius * 2f;
        bulletObj.transform.localScale = new Vector3(diameter, diameter, 1f);

        SpriteRenderer renderer = bulletObj.AddComponent<SpriteRenderer>();
        renderer.sprite = ShapeSpriteHelper.GetCircleSprite(64);
        renderer.color = new Color(1.0f, 0.92f, 0.35f, 1.0f);
        renderer.sortingOrder = 25;

        ActiveBullet bullet = new ActiveBullet
        {
            BulletObject = bulletObj,
            Position = startPosition,
            Direction = direction
        };
        activeBullets.Add(bullet);
    }

    public void TickCombat(float deltaTime, Vector2 aimWorldPos)
    {
        UpdateBullets(deltaTime);
        UpdateAimTargetSelection(aimWorldPos);
    }

    private void UpdateBullets(float deltaTime)
    {
        if (waveSpawner == null)
        {
            return;
        }

        IReadOnlyList<Monster> monsters = waveSpawner.ActiveMonsters;
        float halfW = (GameConstants.ScreenWidth * 0.5f) + GameConstants.BulletOutBoundsMargin;
        float halfH = (GameConstants.ScreenHeight * 0.5f) + GameConstants.BulletOutBoundsMargin;

        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            ActiveBullet bullet = activeBullets[i];
            if (bullet.BulletObject == null)
            {
                activeBullets.RemoveAt(i);
                continue;
            }

            Vector2 prevPos = bullet.Position;
            Vector2 currPos = prevPos + (bullet.Direction * (GameConstants.BulletSpeed * deltaTime));

            Monster firstHitMonster = FindFirstHitMonsterAlongSegment(prevPos, currPos, monsters);
            if (firstHitMonster != null)
            {
                firstHitMonster.TakeBulletDamage(GameConstants.PlayerAttackDamage);
                Destroy(bullet.BulletObject);
                activeBullets.RemoveAt(i);
                continue;
            }

            bullet.Position = currPos;
            bullet.BulletObject.transform.position = new Vector3(currPos.x, currPos.y, 0f);

            if (Mathf.Abs(currPos.x) > halfW || Mathf.Abs(currPos.y) > halfH)
            {
                Destroy(bullet.BulletObject);
                activeBullets.RemoveAt(i);
            }
        }
    }

    private Monster FindFirstHitMonsterAlongSegment(Vector2 prevPos, Vector2 currPos, IReadOnlyList<Monster> monsters)
    {
        Monster bestMonster = null;
        float bestHitFraction = float.MaxValue;
        float bestDistSqAtHit = float.MaxValue;

        for (int i = 0; i < monsters.Count; i++)
        {
            Monster monster = monsters[i];
            if (monster == null || monster.IsDead)
            {
                continue;
            }

            Vector2 enemyCenter = monster.transform.position;
            float hitRadius = monster.HitRadius;

            if (TryGetSegmentCircleFirstHit(prevPos, currPos, enemyCenter, hitRadius, out float hitFraction, out float distSq))
            {
                bool isBetter = false;
                if (hitFraction < bestHitFraction - 0.0001f)
                {
                    isBetter = true;
                }
                else if (Mathf.Abs(hitFraction - bestHitFraction) <= 0.0001f)
                {
                    if (distSq < bestDistSqAtHit - 0.0001f)
                    {
                        isBetter = true;
                    }
                    else if (Mathf.Abs(distSq - bestDistSqAtHit) <= 0.0001f && bestMonster != null && monster.SpawnId < bestMonster.SpawnId)
                    {
                        isBetter = true;
                    }
                }

                if (isBetter)
                {
                    bestMonster = monster;
                    bestHitFraction = hitFraction;
                    bestDistSqAtHit = distSq;
                }
            }
        }

        return bestMonster;
    }

    private bool TryGetSegmentCircleFirstHit(Vector2 p0, Vector2 p1, Vector2 center, float radius, out float hitFraction, out float distSq)
    {
        hitFraction = 1f;
        distSq = float.MaxValue;

        float radiusSq = radius * radius;
        Vector2 f = p0 - center;
        float startDistSq = f.sqrMagnitude;

        // Already inside hit radius at the start of the frame
        if (startDistSq <= radiusSq)
        {
            hitFraction = 0f;
            distSq = startDistSq;
            return true;
        }

        Vector2 d = p1 - p0;
        float a = d.sqrMagnitude;
        if (a <= 0.000001f)
        {
            return false;
        }

        // Exact ray-circle entry intersection along segment [0, 1]
        float b = 2f * Vector2.Dot(f, d);
        float c = startDistSq - radiusSq;
        float discriminant = (b * b) - (4f * a * c);

        if (discriminant < 0f)
        {
            return false;
        }

        float sqrtDisc = Mathf.Sqrt(discriminant);
        float t1 = (-b - sqrtDisc) / (2f * a);
        if (t1 >= 0f && t1 <= 1f)
        {
            hitFraction = t1;
            Vector2 hitPoint = p0 + d * t1;
            distSq = (hitPoint - center).sqrMagnitude;
            return true;
        }

        return false;
    }

    private void UpdateAimTargetSelection(Vector2 aimWorldPos)
    {
        if (waveSpawner == null)
        {
            selectedTarget = null;
            return;
        }

        IReadOnlyList<Monster> monsters = waveSpawner.ActiveMonsters;
        Monster bestCandidate = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < monsters.Count; i++)
        {
            Monster monster = monsters[i];
            if (monster == null || monster.IsDead)
            {
                continue;
            }

            Vector2 enemyPos = monster.transform.position;
            float distance = Vector2.Distance(aimWorldPos, enemyPos);

            if (distance <= monster.SelectRadius)
            {
                if (bestCandidate == null)
                {
                    bestCandidate = monster;
                    bestDist = distance;
                }
                else
                {
                    float diff = distance - bestDist;
                    if (diff < -0.0001f)
                    {
                        bestCandidate = monster;
                        bestDist = distance;
                    }
                    else if (Mathf.Abs(diff) <= 0.0001f && monster.SpawnId < bestCandidate.SpawnId)
                    {
                        // Tie-breaker: earlier SpawnId wins
                        bestCandidate = monster;
                        bestDist = distance;
                    }
                }
            }
        }

        selectedTarget = bestCandidate;

        for (int i = 0; i < monsters.Count; i++)
        {
            Monster monster = monsters[i];
            if (monster != null && !monster.IsDead)
            {
                monster.SetSelected(monster == selectedTarget);
            }
        }
    }
}
