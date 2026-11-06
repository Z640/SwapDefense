using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Health & Cooldowns")]
    [SerializeField] private float currentHealth = GameConstants.PlayerStartHealth;
    [SerializeField] private float fireCooldownTimer;
    [SerializeField] private float swapCooldownTimer;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private VFXManager vfxManager;
    [SerializeField] private Transform aimReticleTransform;
    [SerializeField] private SpriteRenderer aimReticleRenderer;
    [SerializeField] private LineRenderer aimLineRenderer;

    private Vector2 currentAimWorldPos;

    public float CurrentHealth => currentHealth;
    public float SwapCooldownRemaining => Mathf.Max(0f, swapCooldownTimer);
    public bool IsSwapReady => swapCooldownTimer <= 0f;
    public Vector2 CurrentAimWorldPos => currentAimWorldPos;

    public void SetupReferences(Camera cam, CombatSystem combat, GameManager manager, VFXManager vfx, Transform reticleTransform, SpriteRenderer reticleRenderer, LineRenderer aimLine)
    {
        mainCamera = cam;
        combatSystem = combat;
        gameManager = manager;
        vfxManager = vfx;
        aimReticleTransform = reticleTransform;
        aimReticleRenderer = reticleRenderer;
        aimLineRenderer = aimLine;
    }

    public void ResetPlayer()
    {
        currentHealth = GameConstants.PlayerStartHealth;
        fireCooldownTimer = 0f;
        swapCooldownTimer = 0f;
        transform.position = new Vector3(GameConstants.PlayerPosX, GameConstants.PlayerPosY, 0f);
        currentAimWorldPos = new Vector2(0f, 0f);
        UpdateAimVisuals(false);
    }

    public void TickPlayer(float deltaTime)
    {
        if (currentHealth <= 0f)
        {
            return;
        }

        // 1. Continuous health decay (3.0 per second)
        currentHealth = Mathf.Clamp(currentHealth - (GameConstants.PlayerHealthDecayPerSecond * deltaTime), 0f, GameConstants.PlayerMaxHealth);
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            if (gameManager != null)
            {
                gameManager.TriggerGameOver();
            }
            return;
        }

        // 2. Cooldown timers
        if (fireCooldownTimer > 0f)
        {
            fireCooldownTimer -= deltaTime;
        }
        if (swapCooldownTimer > 0f)
        {
            swapCooldownTimer -= deltaTime;
        }

        // 3. Mouse world position tracking
        UpdateMouseWorldPosition();

        // 4. Update combat & target selection before processing F/Q input
        if (combatSystem != null)
        {
            combatSystem.TickCombat(deltaTime, currentAimWorldPos);
        }

        bool hasTarget = combatSystem != null && combatSystem.SelectedTarget != null;
        UpdateAimVisuals(hasTarget);

        // 5. Handle F (Fire) and Q (Swap) inputs
        HandleCombatInputs();
    }

    private void UpdateMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (Mouse.current != null && mainCamera != null)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCamera.transform.position.z));
            currentAimWorldPos = new Vector2(worldPos.x, worldPos.y);
        }
    }

    private void UpdateAimVisuals(bool hasTarget)
    {
        if (aimReticleTransform != null)
        {
            aimReticleTransform.position = new Vector3(currentAimWorldPos.x, currentAimWorldPos.y, 0f);
        }

        if (aimReticleRenderer != null)
        {
            aimReticleRenderer.color = hasTarget
                ? new Color(0.20f, 1.0f, 0.82f, 0.95f)
                : new Color(0.75f, 0.88f, 1.0f, 0.70f);
        }

        if (aimLineRenderer != null)
        {
            aimLineRenderer.SetPosition(0, transform.position);
            aimLineRenderer.SetPosition(1, new Vector3(currentAimWorldPos.x, currentAimWorldPos.y, 0f));
        }
    }

    private void HandleCombatInputs()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        // F key: Single shot or Hold continuous fire every 0.25s
        bool isFirePressed = Keyboard.current.fKey.isPressed;
        if (isFirePressed && fireCooldownTimer <= 0f)
        {
            FireBullet();
            fireCooldownTimer = GameConstants.PlayerFireInterval;
        }

        // Q key: Swap health with currently selected monster
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            TrySwapHealth();
        }
    }

    public void FireBullet()
    {
        if (combatSystem == null)
        {
            return;
        }

        Vector2 playerPos = transform.position;
        combatSystem.SpawnBullet(playerPos, currentAimWorldPos);
    }

    public bool TrySwapHealth()
    {
        if (swapCooldownTimer > 0f)
        {
            if (vfxManager != null)
            {
                vfxManager.ShowSystemNotice($"Q 쿨타임 중 ({swapCooldownTimer:F1}s)");
            }
            return false;
        }

        Monster target = combatSystem != null ? combatSystem.SelectedTarget : null;
        if (target == null || target.IsDead)
        {
            if (vfxManager != null)
            {
                vfxManager.ShowSystemNotice("교환 대상 없음 (조준점에 적을 맞추세요)");
            }
            return false;
        }

        float oldPlayerHp = currentHealth;
        float oldMonsterHp = target.CurrentHealth;

        currentHealth = Mathf.Clamp(oldMonsterHp, 0f, GameConstants.PlayerMaxHealth);
        target.ApplyHealthSwap(oldPlayerHp);

        swapCooldownTimer = GameConstants.PlayerSwapCooldown;

        if (vfxManager != null)
        {
            vfxManager.PlaySwapEffect(
                transform.position,
                target.transform,
                oldPlayerHp,
                currentHealth,
                oldMonsterHp,
                target.CurrentHealth
            );
            vfxManager.ShowSystemNotice($"체력 교환 완료! ({oldPlayerHp:F1} ↔ {oldMonsterHp:F1}) · 적이 빠르게 돌진합니다!");
        }

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            if (gameManager != null)
            {
                gameManager.TriggerGameOver();
            }
        }

        return true;
    }

    public void TakeDamage(float damageAmount)
    {
        if (currentHealth <= 0f || damageAmount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0f, GameConstants.PlayerMaxHealth);

        if (vfxManager != null)
        {
            vfxManager.PlayPlayerDamageEffect();
        }

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            if (gameManager != null)
            {
                gameManager.TriggerGameOver();
            }
        }
    }
}
