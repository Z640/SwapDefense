using TMPro;
using UnityEngine;

public enum EnemyType
{
    Fast,
    Slow
}

public class Monster : MonoBehaviour
{
    [Header("Runtime Info")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int spawnId;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth = GameConstants.EnemyMaxHealth;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool isSelected;
    [SerializeField] private bool isDead;

    [Header("Hitbox & Selection Tuning")]
    [SerializeField] private float hitboxRadiusMultiplier = GameConstants.DefaultHitboxMultiplier;
    [SerializeField] private CircleCollider2D hitCollider;

    [Header("Prefab Visual References")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer outlineRenderer;
    [SerializeField] private SpriteRenderer healthBarBorderRenderer;
    [SerializeField] private SpriteRenderer healthBarBgRenderer;
    [SerializeField] private SpriteRenderer healthBarFillRenderer;
    [SerializeField] private TextMeshPro healthText;
    [SerializeField] private TextMeshPro dashDamageText;
    [SerializeField] private LineRenderer dashDirectionLine;
    [SerializeField] private TrailRenderer dashTrail;

    [Header("Health Bar Layout")]
    [SerializeField] private float healthBarMaxWidth = 42f;
    [SerializeField] private float healthBarHeight = 5f;
    [SerializeField] private Color baseBodyColor = Color.white;

    private float hitFlashTimer;
    private bool hasHigherHealthThanPlayer;
    private GameManager gameManager;
    private VFXManager vfxManager;
    private AbilitySwapSystem abilitySwapSystem;

    public EnemyType Type => enemyType;
    public int SpawnId => spawnId;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDashing => isDashing;
    public bool IsSelected => isSelected;
    public bool IsDead => isDead;
    public float HitboxRadiusMultiplier => hitboxRadiusMultiplier;

    public float BodyRadius => enemyType == EnemyType.Fast
        ? GameConstants.FastEnemyBodyRadius
        : GameConstants.SlowEnemyBodyRadius;

    public float HitRadius
    {
        get
        {
            float baseHitRadius = enemyType == EnemyType.Fast
                ? GameConstants.FastEnemyHitRadius
                : GameConstants.SlowEnemyHitRadius;
            float abilityScale = abilitySwapSystem != null
                ? abilitySwapSystem.GetMonsterHitboxScaleMultiplier()
                : 1f;
            return baseHitRadius * hitboxRadiusMultiplier * abilityScale;
        }
    }

    public float SelectRadius
    {
        get
        {
            float baseSelectRadius = enemyType == EnemyType.Fast
                ? GameConstants.FastEnemySelectRadius
                : GameConstants.SlowEnemySelectRadius;
            float abilityScale = abilitySwapSystem != null
                ? abilitySwapSystem.GetMonsterHitboxScaleMultiplier()
                : 1f;
            return baseSelectRadius * hitboxRadiusMultiplier * abilityScale;
        }
    }

    public float MoveSpeed
    {
        get
        {
            float speedMultiplier = abilitySwapSystem != null
                ? abilitySwapSystem.GetMonsterMoveSpeedMultiplier()
                : 1f;

            if (isDashing)
            {
                return GameConstants.EnemyDashSpeed * speedMultiplier;
            }
            float baseSpeed = enemyType == EnemyType.Fast
                ? GameConstants.FastEnemyMoveSpeed
                : GameConstants.SlowEnemyMoveSpeed;
            return baseSpeed * speedMultiplier;
        }
    }

    public string KoreanTypeName => enemyType == EnemyType.Fast ? "빠른 적" : "느린 적";

    public void SetupPrefabReferences(
        EnemyType presetType,
        CircleCollider2D colliderComponent,
        SpriteRenderer body,
        SpriteRenderer outline,
        SpriteRenderer barBg,
        SpriteRenderer barFill,
        TextMeshPro hpTmp,
        TextMeshPro dashTmp,
        LineRenderer dashLine,
        TrailRenderer trail,
        float barMaxWidth,
        float barHeight,
        Color defaultBodyColor,
        SpriteRenderer barBorder = null)
    {
        enemyType = presetType;
        hitCollider = colliderComponent;
        bodyRenderer = body;
        outlineRenderer = outline;
        healthBarBorderRenderer = barBorder;
        healthBarBgRenderer = barBg;
        healthBarFillRenderer = barFill;
        healthText = hpTmp;
        dashDamageText = dashTmp;
        dashDirectionLine = dashLine;
        dashTrail = trail;
        healthBarMaxWidth = barMaxWidth;
        healthBarHeight = barHeight;
        baseBodyColor = defaultBodyColor;
        hitboxRadiusMultiplier = GameConstants.DefaultHitboxMultiplier;
        SyncColliderRadius();
    }

    public void Initialize(
        EnemyType type,
        int id,
        Vector2 spawnPosition,
        GameManager manager,
        VFXManager vfx,
        AbilitySwapSystem swapSystem = null)
    {
        enemyType = type;
        spawnId = id;
        transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);
        gameManager = manager;
        vfxManager = vfx;
        abilitySwapSystem = swapSystem;

        float hpMultiplier = abilitySwapSystem != null
            ? abilitySwapSystem.GetMonsterHealthMultiplier()
            : 1f;

        maxHealth = GameConstants.EnemyMaxHealth * hpMultiplier;
        float baseStartHealth = enemyType == EnemyType.Fast
            ? GameConstants.FastEnemyStartHealth
            : GameConstants.SlowEnemyStartHealth;
        currentHealth = baseStartHealth * hpMultiplier;

        isDashing = false;
        isSelected = false;
        isDead = false;
        hitFlashTimer = 0f;
        hasHigherHealthThanPlayer = false;

        SyncColliderRadius();

        if (dashTrail != null)
        {
            dashTrail.Clear();
            dashTrail.emitting = false;
        }

        if (dashDirectionLine != null)
        {
            dashDirectionLine.enabled = false;
        }

        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = false;
        }

        if (healthBarBorderRenderer != null)
        {
            healthBarBorderRenderer.enabled = false;
        }

        UpdateVisualState();
    }

    public void SyncColliderRadius()
    {
        if (hitCollider != null)
        {
            hitCollider.isTrigger = true;
            hitCollider.radius = HitRadius;
        }
    }

    public float GetExpectedArrivalDamage()
    {
        float rawDamage = isDashing
            ? Mathf.Max(0f, currentHealth)
            : (enemyType == EnemyType.Fast ? GameConstants.FastEnemyArrivalDamage : GameConstants.SlowEnemyArrivalDamage);

        float damageMultiplier = abilitySwapSystem != null
            ? abilitySwapSystem.GetMonsterArrivalDamageMultiplier()
            : 1f;

        return rawDamage * damageMultiplier;
    }

    public void TickMonster(float deltaTime, PlayerController playerController)
    {
        if (isDead)
        {
            return;
        }

        SyncColliderRadius();
        UpdateHigherHealthBorder(playerController);

        if (hitFlashTimer > 0f)
        {
            hitFlashTimer -= deltaTime;
            if (hitFlashTimer <= 0f && bodyRenderer != null)
            {
                bodyRenderer.color = isDashing ? new Color(1.0f, 0.36f, 0.38f, 1f) : baseBodyColor;
            }
        }

        Vector3 pos = transform.position;
        pos.y -= MoveSpeed * deltaTime;
        transform.position = pos;

        if (isDashing && dashDirectionLine != null && dashDirectionLine.enabled)
        {
            UpdateDashLinePositions();
        }

        if (pos.y <= GameConstants.DefenseLineY)
        {
            HandleDefenseLineArrival(playerController);
        }
    }

    private void HandleDefenseLineArrival(PlayerController playerController)
    {
        if (isDead)
        {
            return;
        }

        float arrivalDamage = GetExpectedArrivalDamage();

        if (playerController != null)
        {
            if (abilitySwapSystem != null && abilitySwapSystem.DoesMonsterArrivalDoublePlayerDecay())
            {
                playerController.ActivateBreakthroughDecayPenalty();
            }

            if (arrivalDamage > 0f)
            {
                playerController.TakeDamage(arrivalDamage);
            }
        }

        if (vfxManager != null)
        {
            vfxManager.PlayHitBurst(transform.position, new Color(1.0f, 0.28f, 0.32f, 1.0f), BodyRadius * 1.4f);
        }

        DestroyMonster();
    }

    public bool TakeBulletDamage(float damageAmount)
    {
        if (isDead)
        {
            return false;
        }

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0f, maxHealth);
        hitFlashTimer = GameConstants.HitFlashDuration;

        if (bodyRenderer != null)
        {
            bodyRenderer.color = Color.white;
        }

        if (vfxManager != null)
        {
            vfxManager.PlayHitBurst(transform.position, new Color(1.0f, 0.95f, 0.65f, 1.0f), BodyRadius);
        }

        if (currentHealth <= 0f)
        {
            if (gameManager != null)
            {
                gameManager.NotifyMonsterKilledByPlayer();
            }
            DestroyMonster();
            return true;
        }

        UpdateVisualState();
        return false;
    }

    public void ApplyHealthSwap(float newHealth)
    {
        if (isDead)
        {
            return;
        }

        currentHealth = Mathf.Clamp(newHealth, 0f, maxHealth);
        isDashing = true;

        if (dashTrail != null)
        {
            dashTrail.Clear();
            dashTrail.emitting = true;
        }

        if (dashDirectionLine != null)
        {
            dashDirectionLine.enabled = true;
            UpdateDashLinePositions();
        }

        if (currentHealth <= 0f)
        {
            DestroyMonster();
            return;
        }

        UpdateVisualState();
    }

    public void SetSelected(bool selected)
    {
        if (isSelected == selected)
        {
            return;
        }

        isSelected = selected;
        UpdateOutlineState();
    }

    public void UpdateVisualState()
    {
        if (isDead)
        {
            return;
        }

        if (hitFlashTimer <= 0f && bodyRenderer != null)
        {
            bodyRenderer.color = isDashing ? new Color(1.0f, 0.36f, 0.38f, 1f) : baseBodyColor;
        }

        float denominator = Mathf.Max(1f, maxHealth);
        float ratio = Mathf.Clamp01(currentHealth / denominator);
        if (healthBarFillRenderer != null)
        {
            float fillWidth = healthBarMaxWidth * ratio;
            float offsetX = -(healthBarMaxWidth - fillWidth) * 0.5f;
            float barOffsetY = BodyRadius + 11f;
            healthBarFillRenderer.transform.localPosition = new Vector3(offsetX, barOffsetY, 0f);
            healthBarFillRenderer.transform.localScale = new Vector3(fillWidth, healthBarHeight, 1f);
            healthBarFillRenderer.color = isDashing
                ? new Color(1.0f, 0.30f, 0.35f, 1.0f)
                : baseBodyColor;
        }

        if (healthText != null)
        {
            int ceilHp = Mathf.CeilToInt(currentHealth);
            healthText.text = ceilHp.ToString();
        }

        if (dashDamageText != null)
        {
            dashDamageText.gameObject.SetActive(isDashing);
            if (isDashing)
            {
                int expectedDamage = Mathf.CeilToInt(GetExpectedArrivalDamage());
                dashDamageText.text = $"돌진 피해 {expectedDamage}";
            }
        }

        UpdateOutlineState();
    }

    public void UpdateHigherHealthBorder(PlayerController playerController)
    {
        bool isHigher = playerController != null && currentHealth > playerController.CurrentHealth;
        hasHigherHealthThanPlayer = isHigher;

        if (healthBarBorderRenderer != null)
        {
            healthBarBorderRenderer.enabled = hasHigherHealthThanPlayer;
        }
    }

    private void UpdateOutlineState()
    {
        if (outlineRenderer == null)
        {
            return;
        }

        // Mint selection border takes priority over Red dash border
        if (isSelected)
        {
            outlineRenderer.enabled = true;
            outlineRenderer.color = new Color(0.20f, 1.0f, 0.82f, 1.0f);
        }
        else if (isDashing)
        {
            outlineRenderer.enabled = true;
            outlineRenderer.color = new Color(1.0f, 0.22f, 0.28f, 0.95f);
        }
        else
        {
            outlineRenderer.enabled = false;
        }
    }

    private void UpdateDashLinePositions()
    {
        if (dashDirectionLine == null)
        {
            return;
        }

        Vector3 startPos = new Vector3(transform.position.x, transform.position.y - BodyRadius, 0f);
        Vector3 endPos = new Vector3(transform.position.x, GameConstants.DefenseLineY, 0f);
        dashDirectionLine.SetPosition(0, startPos);
        dashDirectionLine.SetPosition(1, endPos);
    }

    public void DestroyMonster()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Destroy(gameObject);
    }
}
