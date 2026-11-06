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
    [SerializeField] private bool isDashing;
    [SerializeField] private bool isSelected;
    [SerializeField] private bool isDead;

    [Header("Visual References")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer outlineRenderer;
    [SerializeField] private SpriteRenderer healthBarBgRenderer;
    [SerializeField] private SpriteRenderer healthBarFillRenderer;
    [SerializeField] private TextMeshPro healthText;
    [SerializeField] private TextMeshPro dashDamageText;
    [SerializeField] private LineRenderer dashDirectionLine;
    [SerializeField] private TrailRenderer dashTrail;

    private Color baseBodyColor;
    private float hitFlashTimer;
    private float healthBarMaxWidth = 42f;
    private float healthBarHeight = 5f;
    private GameManager gameManager;
    private VFXManager vfxManager;

    public EnemyType Type => enemyType;
    public int SpawnId => spawnId;
    public float CurrentHealth => currentHealth;
    public bool IsDashing => isDashing;
    public bool IsSelected => isSelected;
    public bool IsDead => isDead;

    public float BodyRadius => enemyType == EnemyType.Fast
        ? GameConstants.FastEnemyBodyRadius
        : GameConstants.SlowEnemyBodyRadius;

    public float HitRadius => enemyType == EnemyType.Fast
        ? GameConstants.FastEnemyHitRadius
        : GameConstants.SlowEnemyHitRadius;

    public float SelectRadius => enemyType == EnemyType.Fast
        ? GameConstants.FastEnemySelectRadius
        : GameConstants.SlowEnemySelectRadius;

    public float MoveSpeed
    {
        get
        {
            if (isDashing)
            {
                return GameConstants.EnemyDashSpeed;
            }
            return enemyType == EnemyType.Fast
                ? GameConstants.FastEnemyMoveSpeed
                : GameConstants.SlowEnemyMoveSpeed;
        }
    }

    public string KoreanTypeName => enemyType == EnemyType.Fast ? "빠른 적" : "느린 적";

    public void Initialize(EnemyType type, int id, Vector2 spawnPosition, TMP_FontAsset fontAsset, GameManager manager, VFXManager vfx)
    {
        enemyType = type;
        spawnId = id;
        transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);
        gameManager = manager;
        vfxManager = vfx;

        currentHealth = enemyType == EnemyType.Fast
            ? GameConstants.FastEnemyStartHealth
            : GameConstants.SlowEnemyStartHealth;

        isDashing = false;
        isSelected = false;
        isDead = false;
        hitFlashTimer = 0f;

        BuildVisualHierarchy(fontAsset);
        UpdateVisualState();
    }

    private void BuildVisualHierarchy(TMP_FontAsset fontAsset)
    {
        float diameter = BodyRadius * 2f;
        healthBarMaxWidth = enemyType == EnemyType.Fast ? 36f : 48f;

        // 1. Body Renderer
        GameObject bodyObj = new GameObject("Body");
        bodyObj.transform.SetParent(transform, false);
        bodyObj.transform.localScale = new Vector3(diameter, diameter, 1f);
        bodyRenderer = bodyObj.AddComponent<SpriteRenderer>();
        bodyRenderer.sortingOrder = 10;

        if (enemyType == EnemyType.Fast)
        {
            bodyRenderer.sprite = ShapeSpriteHelper.GetDiamondSprite(128, false);
            baseBodyColor = new Color(1.0f, 0.55f, 0.15f, 1.0f); // Orange diamond
        }
        else
        {
            bodyRenderer.sprite = ShapeSpriteHelper.GetHexagonSprite(128, false);
            baseBodyColor = new Color(0.65f, 0.32f, 0.92f, 1.0f); // Purple hexagon
        }
        bodyRenderer.color = baseBodyColor;

        // 2. Outline Renderer (Mint selection or Red dash)
        GameObject outlineObj = new GameObject("Outline");
        outlineObj.transform.SetParent(transform, false);
        float outlineDiameter = diameter + 10f;
        outlineObj.transform.localScale = new Vector3(outlineDiameter, outlineDiameter, 1f);
        outlineRenderer = outlineObj.AddComponent<SpriteRenderer>();
        outlineRenderer.sortingOrder = 11;
        outlineRenderer.sprite = enemyType == EnemyType.Fast
            ? ShapeSpriteHelper.GetDiamondSprite(128, true, 0.22f)
            : ShapeSpriteHelper.GetHexagonSprite(128, true, 0.20f);
        outlineRenderer.enabled = false;

        // 3. Health Bar Background
        float barOffsetY = BodyRadius + 11f;
        GameObject barBgObj = new GameObject("HealthBarBg");
        barBgObj.transform.SetParent(transform, false);
        barBgObj.transform.localPosition = new Vector3(0f, barOffsetY, 0f);
        barBgObj.transform.localScale = new Vector3(healthBarMaxWidth + 2f, healthBarHeight + 2f, 1f);
        healthBarBgRenderer = barBgObj.AddComponent<SpriteRenderer>();
        healthBarBgRenderer.sprite = ShapeSpriteHelper.GetSquareSprite();
        healthBarBgRenderer.color = new Color(0.08f, 0.10f, 0.14f, 0.90f);
        healthBarBgRenderer.sortingOrder = 14;

        // 4. Health Bar Fill
        GameObject barFillObj = new GameObject("HealthBarFill");
        barFillObj.transform.SetParent(transform, false);
        barFillObj.transform.localPosition = new Vector3(0f, barOffsetY, 0f);
        barFillObj.transform.localScale = new Vector3(healthBarMaxWidth, healthBarHeight, 1f);
        healthBarFillRenderer = barFillObj.AddComponent<SpriteRenderer>();
        healthBarFillRenderer.sprite = ShapeSpriteHelper.GetSquareSprite();
        healthBarFillRenderer.color = baseBodyColor;
        healthBarFillRenderer.sortingOrder = 15;

        // 5. Ceiling Integer Health Text above health bar
        GameObject hpTextObj = new GameObject("HealthText");
        hpTextObj.transform.SetParent(transform, false);
        hpTextObj.transform.localPosition = new Vector3(0f, barOffsetY + 13f, 0f);
        healthText = hpTextObj.AddComponent<TextMeshPro>();
        if (fontAsset != null)
        {
            healthText.font = fontAsset;
        }
        healthText.fontSize = 140f;
        healthText.alignment = TextAlignmentOptions.Center;
        healthText.color = new Color(0.95f, 0.97f, 1.0f, 1.0f);
        healthText.sortingOrder = 20;
        RectTransform hpRect = healthText.GetComponent<RectTransform>();
        hpRect.sizeDelta = new Vector2(100f, 24f);

        // 6. Dash Expected Arrival Damage Text
        GameObject dashTextObj = new GameObject("DashDamageText");
        dashTextObj.transform.SetParent(transform, false);
        dashTextObj.transform.localPosition = new Vector3(BodyRadius + 78f, 0f, 0f);
        dashDamageText = dashTextObj.AddComponent<TextMeshPro>();
        if (fontAsset != null)
        {
            dashDamageText.font = fontAsset;
        }
        dashDamageText.fontSize = 120f;
        dashDamageText.alignment = TextAlignmentOptions.Left;
        dashDamageText.color = new Color(1.0f, 0.32f, 0.36f, 1.0f);
        dashDamageText.sortingOrder = 20;
        RectTransform dashRect = dashDamageText.GetComponent<RectTransform>();
        dashRect.sizeDelta = new Vector2(140f, 26f);
        dashDamageText.gameObject.SetActive(false);

        // 7. Dash Direction Line toward Defense Line
        GameObject lineObj = new GameObject("DashDirectionLine");
        lineObj.transform.SetParent(transform, false);
        dashDirectionLine = lineObj.AddComponent<LineRenderer>();
        dashDirectionLine.material = ShapeSpriteHelper.GetLineMaterial();
        dashDirectionLine.positionCount = 2;
        dashDirectionLine.startWidth = 2.0f;
        dashDirectionLine.endWidth = 2.0f;
        dashDirectionLine.useWorldSpace = true;
        dashDirectionLine.sortingOrder = 5;
        dashDirectionLine.startColor = new Color(1.0f, 0.25f, 0.30f, 0.70f);
        dashDirectionLine.endColor = new Color(1.0f, 0.25f, 0.30f, 0.15f);
        dashDirectionLine.enabled = false;

        // 8. Dash Trail
        GameObject trailObj = new GameObject("DashTrail");
        trailObj.transform.SetParent(transform, false);
        dashTrail = trailObj.AddComponent<TrailRenderer>();
        dashTrail.material = ShapeSpriteHelper.GetLineMaterial();
        dashTrail.time = 0.28f;
        dashTrail.startWidth = diameter * 0.65f;
        dashTrail.endWidth = 2.0f;
        dashTrail.sortingOrder = 4;
        dashTrail.startColor = new Color(1.0f, 0.25f, 0.30f, 0.50f);
        dashTrail.endColor = new Color(1.0f, 0.25f, 0.30f, 0.0f);
        dashTrail.emitting = false;
    }

    public void TickMonster(float deltaTime, PlayerController playerController)
    {
        if (isDead)
        {
            return;
        }

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

        float arrivalDamage = isDashing
            ? Mathf.Max(0f, currentHealth)
            : (enemyType == EnemyType.Fast ? GameConstants.FastEnemyArrivalDamage : GameConstants.SlowEnemyArrivalDamage);

        if (playerController != null && arrivalDamage > 0f)
        {
            playerController.TakeDamage(arrivalDamage);
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

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0f, GameConstants.EnemyMaxHealth);
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
                gameManager.AddKillCount();
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

        currentHealth = Mathf.Clamp(newHealth, 0f, GameConstants.EnemyMaxHealth);
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

    private void UpdateVisualState()
    {
        if (isDead)
        {
            return;
        }

        if (hitFlashTimer <= 0f && bodyRenderer != null)
        {
            bodyRenderer.color = isDashing ? new Color(1.0f, 0.36f, 0.38f, 1f) : baseBodyColor;
        }

        float ratio = Mathf.Clamp01(currentHealth / GameConstants.EnemyMaxHealth);
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
                int expectedDamage = Mathf.CeilToInt(currentHealth);
                dashDamageText.text = $"돌진 피해 {expectedDamage}";
            }
        }

        UpdateOutlineState();
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
            outlineRenderer.color = new Color(0.20f, 1.0f, 0.82f, 1.0f); // Mint
        }
        else if (isDashing)
        {
            outlineRenderer.enabled = true;
            outlineRenderer.color = new Color(1.0f, 0.22f, 0.28f, 0.95f); // Red
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
