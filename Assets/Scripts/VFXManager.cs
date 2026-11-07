using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private HitBurstEffect hitBurstPrefab;
    [SerializeField] private SwapConnectionEffect swapEffectPrefab;

    [Header("Scene References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private UnityEngine.UI.Image damageFlashOverlay;
    [SerializeField] private TMP_Text systemNoticeText;
    [SerializeField] private GameObject systemNoticeContainer;
    [SerializeField] private Transform vfxContainer;

    private readonly List<HitBurstEffect> activeBursts = new List<HitBurstEffect>();
    private readonly List<SwapConnectionEffect> activeSwapEffects = new List<SwapConnectionEffect>();

    private float playerDamageTimer;
    private float systemNoticeTimer;
    private Vector3 cameraDefaultPos = new Vector3(0f, 0f, -10f);

    public void SetupReferences(
        HitBurstEffect burstPrefab,
        SwapConnectionEffect swapPrefab,
        Camera cam,
        UnityEngine.UI.Image flashOverlay,
        TMP_Text noticeText,
        GameObject noticeContainer,
        Transform container)
    {
        hitBurstPrefab = burstPrefab;
        swapEffectPrefab = swapPrefab;
        mainCamera = cam;
        damageFlashOverlay = flashOverlay;
        systemNoticeText = noticeText;
        systemNoticeContainer = noticeContainer;
        vfxContainer = container;
        if (mainCamera != null)
        {
            cameraDefaultPos = mainCamera.transform.position;
        }
    }

    public void ClearAllEffects()
    {
        for (int i = activeBursts.Count - 1; i >= 0; i--)
        {
            if (activeBursts[i] != null)
            {
                Destroy(activeBursts[i].gameObject);
            }
        }
        activeBursts.Clear();

        for (int i = activeSwapEffects.Count - 1; i >= 0; i--)
        {
            if (activeSwapEffects[i] != null)
            {
                Destroy(activeSwapEffects[i].gameObject);
            }
        }
        activeSwapEffects.Clear();

        playerDamageTimer = 0f;
        systemNoticeTimer = 0f;

        if (mainCamera != null)
        {
            mainCamera.transform.position = cameraDefaultPos;
        }
        if (damageFlashOverlay != null)
        {
            damageFlashOverlay.color = new Color(1f, 0.15f, 0.2f, 0f);
        }
        if (systemNoticeContainer != null)
        {
            systemNoticeContainer.SetActive(false);
        }
    }

    public void TickVFX(float deltaTime)
    {
        UpdateHitBursts(deltaTime);
        UpdateSwapEffects(deltaTime);
        UpdatePlayerDamageEffect(deltaTime);
        UpdateSystemNotice(deltaTime);
    }

    public void PlayHitBurst(Vector3 position, Color burstColor, float radius)
    {
        if (hitBurstPrefab == null)
        {
            return;
        }

        HitBurstEffect burst = Instantiate(hitBurstPrefab, vfxContainer);
        burst.Initialize(position, burstColor, radius, GameConstants.HitFlashDuration);
        activeBursts.Add(burst);
    }

    public void PlayPlayerDamageEffect()
    {
        playerDamageTimer = GameConstants.PlayerDamageEffectDuration;
    }

    public void ShowSystemNotice(string message)
    {
        systemNoticeTimer = GameConstants.SystemNoticeDuration;
        if (systemNoticeText != null)
        {
            systemNoticeText.text = message;
        }
        if (systemNoticeContainer != null)
        {
            systemNoticeContainer.SetActive(true);
        }
    }

    public void PlaySwapEffect(Vector3 playerPos, Transform enemyTransform, float oldPlayerHp, float newPlayerHp, float oldEnemyHp, float newEnemyHp)
    {
        if (swapEffectPrefab == null)
        {
            return;
        }

        SwapConnectionEffect swapEffect = Instantiate(swapEffectPrefab, vfxContainer);
        swapEffect.Initialize(
            playerPos,
            enemyTransform,
            oldPlayerHp,
            newPlayerHp,
            oldEnemyHp,
            newEnemyHp,
            GameConstants.SwapEffectDuration
        );
        activeSwapEffects.Add(swapEffect);
    }

    private void UpdateHitBursts(float deltaTime)
    {
        for (int i = activeBursts.Count - 1; i >= 0; i--)
        {
            HitBurstEffect burst = activeBursts[i];
            if (burst == null || burst.TickEffect(deltaTime))
            {
                activeBursts.RemoveAt(i);
            }
        }
    }

    private void UpdateSwapEffects(float deltaTime)
    {
        for (int i = activeSwapEffects.Count - 1; i >= 0; i--)
        {
            SwapConnectionEffect effect = activeSwapEffects[i];
            if (effect == null || effect.TickEffect(deltaTime))
            {
                activeSwapEffects.RemoveAt(i);
            }
        }
    }

    private void UpdatePlayerDamageEffect(float deltaTime)
    {
        if (playerDamageTimer <= 0f)
        {
            return;
        }

        playerDamageTimer -= deltaTime;
        float t = Mathf.Clamp01(playerDamageTimer / GameConstants.PlayerDamageEffectDuration);

        if (damageFlashOverlay != null)
        {
            damageFlashOverlay.color = new Color(1.0f, 0.15f, 0.20f, t * 0.38f);
        }

        if (mainCamera != null)
        {
            if (playerDamageTimer > 0f)
            {
                float shakeStrength = 8.0f * t;
                Vector2 offset = Random.insideUnitCircle * shakeStrength;
                mainCamera.transform.position = cameraDefaultPos + new Vector3(offset.x, offset.y, 0f);
            }
            else
            {
                mainCamera.transform.position = cameraDefaultPos;
            }
        }
    }

    private void UpdateSystemNotice(float deltaTime)
    {
        if (systemNoticeTimer <= 0f)
        {
            return;
        }

        systemNoticeTimer -= deltaTime;
        if (systemNoticeTimer <= 0f && systemNoticeContainer != null)
        {
            systemNoticeContainer.SetActive(false);
        }
    }
}
