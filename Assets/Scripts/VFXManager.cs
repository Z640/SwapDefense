using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    private class ActiveBurst
    {
        public GameObject BurstObject;
        public SpriteRenderer Renderer;
        public float ElapsedTime;
        public float Duration;
        public float StartScale;
        public float EndScale;
        public Color BaseColor;
    }

    private class ActiveSwapEffect
    {
        public LineRenderer SwapLine;
        public TextMeshPro PlayerFloatingText;
        public TextMeshPro EnemyFloatingText;
        public Vector3 PlayerStartPos;
        public Transform EnemyTransform;
        public Vector3 LastEnemyPos;
        public float ElapsedTime;
        public float Duration;
    }

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private UnityEngine.UI.Image damageFlashOverlay;
    [SerializeField] private TMP_Text systemNoticeText;
    [SerializeField] private GameObject systemNoticeContainer;
    [SerializeField] private TMP_FontAsset worldFontAsset;
    [SerializeField] private Transform vfxContainer;

    private readonly List<ActiveBurst> activeBursts = new List<ActiveBurst>();
    private readonly List<ActiveSwapEffect> activeSwapEffects = new List<ActiveSwapEffect>();

    private float playerDamageTimer;
    private float systemNoticeTimer;
    private Vector3 cameraDefaultPos = new Vector3(0f, 0f, -10f);

    public void SetupReferences(Camera cam, UnityEngine.UI.Image flashOverlay, TMP_Text noticeText, GameObject noticeContainer, TMP_FontAsset font, Transform container)
    {
        mainCamera = cam;
        damageFlashOverlay = flashOverlay;
        systemNoticeText = noticeText;
        systemNoticeContainer = noticeContainer;
        worldFontAsset = font;
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
            if (activeBursts[i].BurstObject != null)
            {
                Destroy(activeBursts[i].BurstObject);
            }
        }
        activeBursts.Clear();

        for (int i = activeSwapEffects.Count - 1; i >= 0; i--)
        {
            DestroySwapEffectObjects(activeSwapEffects[i]);
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
        GameObject burstObj = new GameObject("HitBurst");
        if (vfxContainer != null)
        {
            burstObj.transform.SetParent(vfxContainer, false);
        }
        burstObj.transform.position = position;

        SpriteRenderer renderer = burstObj.AddComponent<SpriteRenderer>();
        renderer.sprite = ShapeSpriteHelper.GetRingSprite(64, 0.25f);
        renderer.color = burstColor;
        renderer.sortingOrder = 30;

        float startScale = radius * 1.2f;
        float endScale = radius * 2.8f;
        burstObj.transform.localScale = new Vector3(startScale, startScale, 1f);

        activeBursts.Add(new ActiveBurst
        {
            BurstObject = burstObj,
            Renderer = renderer,
            ElapsedTime = 0f,
            Duration = GameConstants.HitFlashDuration,
            StartScale = startScale,
            EndScale = endScale,
            BaseColor = burstColor
        });
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
        Vector3 enemyPos = enemyTransform != null ? enemyTransform.position : playerPos + Vector3.up * 100f;

        GameObject lineObj = new GameObject("SwapConnectionLine");
        if (vfxContainer != null)
        {
            lineObj.transform.SetParent(vfxContainer, false);
        }
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = ShapeSpriteHelper.GetLineMaterial();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 4.5f;
        lineRenderer.endWidth = 4.5f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingOrder = 28;
        lineRenderer.SetPosition(0, playerPos);
        lineRenderer.SetPosition(1, enemyPos);

        TextMeshPro playerFloat = CreateFloatingText(
            "SwapPlayerFloat",
            playerPos + new Vector3(0f, 36f, 0f),
            $"{oldPlayerHp:F1} → {newPlayerHp:F1}",
            new Color(0.20f, 1.0f, 0.82f, 1.0f)
        );

        TextMeshPro enemyFloat = CreateFloatingText(
            "SwapEnemyFloat",
            enemyPos + new Vector3(0f, 44f, 0f),
            $"{oldEnemyHp:F1} → {newEnemyHp:F1}",
            new Color(1.0f, 0.45f, 0.48f, 1.0f)
        );

        activeSwapEffects.Add(new ActiveSwapEffect
        {
            SwapLine = lineRenderer,
            PlayerFloatingText = playerFloat,
            EnemyFloatingText = enemyFloat,
            PlayerStartPos = playerPos,
            EnemyTransform = enemyTransform,
            LastEnemyPos = enemyPos,
            ElapsedTime = 0f,
            Duration = GameConstants.SwapEffectDuration
        });
    }

    private TextMeshPro CreateFloatingText(string objectName, Vector3 worldPos, string content, Color textColor)
    {
        GameObject textObj = new GameObject(objectName);
        if (vfxContainer != null)
        {
            textObj.transform.SetParent(vfxContainer, false);
        }
        textObj.transform.position = worldPos;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        if (worldFontAsset != null)
        {
            tmp.font = worldFontAsset;
        }
        tmp.text = content;
        tmp.fontSize = 150f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textColor;
        tmp.sortingOrder = 35;

        RectTransform rect = tmp.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(220f, 34f);
        return tmp;
    }

    private void UpdateHitBursts(float deltaTime)
    {
        for (int i = activeBursts.Count - 1; i >= 0; i--)
        {
            ActiveBurst burst = activeBursts[i];
            if (burst.BurstObject == null)
            {
                activeBursts.RemoveAt(i);
                continue;
            }

            burst.ElapsedTime += deltaTime;
            float t = Mathf.Clamp01(burst.ElapsedTime / burst.Duration);
            float scale = Mathf.Lerp(burst.StartScale, burst.EndScale, t);
            burst.BurstObject.transform.localScale = new Vector3(scale, scale, 1f);

            if (burst.Renderer != null)
            {
                Color c = burst.BaseColor;
                c.a = 1f - t;
                burst.Renderer.color = c;
            }

            if (burst.ElapsedTime >= burst.Duration)
            {
                Destroy(burst.BurstObject);
                activeBursts.RemoveAt(i);
            }
        }
    }

    private void UpdateSwapEffects(float deltaTime)
    {
        for (int i = activeSwapEffects.Count - 1; i >= 0; i--)
        {
            ActiveSwapEffect effect = activeSwapEffects[i];
            effect.ElapsedTime += deltaTime;
            float t = Mathf.Clamp01(effect.ElapsedTime / effect.Duration);
            float alpha = 1f - t;

            if (effect.EnemyTransform != null)
            {
                effect.LastEnemyPos = effect.EnemyTransform.position;
            }

            if (effect.SwapLine != null)
            {
                effect.SwapLine.SetPosition(0, effect.PlayerStartPos);
                effect.SwapLine.SetPosition(1, effect.LastEnemyPos);
                Color lineColor = new Color(0.20f, 1.0f, 0.82f, alpha * 0.9f);
                effect.SwapLine.startColor = lineColor;
                effect.SwapLine.endColor = lineColor;
                float width = Mathf.Lerp(5.0f, 1.0f, t);
                effect.SwapLine.startWidth = width;
                effect.SwapLine.endWidth = width;
            }

            if (effect.PlayerFloatingText != null)
            {
                effect.PlayerFloatingText.transform.position = effect.PlayerStartPos + new Vector3(0f, 36f + (t * 26f), 0f);
                Color c = effect.PlayerFloatingText.color;
                c.a = alpha;
                effect.PlayerFloatingText.color = c;
            }

            if (effect.EnemyFloatingText != null)
            {
                effect.EnemyFloatingText.transform.position = effect.LastEnemyPos + new Vector3(0f, 44f + (t * 26f), 0f);
                Color c = effect.EnemyFloatingText.color;
                c.a = alpha;
                effect.EnemyFloatingText.color = c;
            }

            if (effect.ElapsedTime >= effect.Duration)
            {
                DestroySwapEffectObjects(effect);
                activeSwapEffects.RemoveAt(i);
            }
        }
    }

    private void DestroySwapEffectObjects(ActiveSwapEffect effect)
    {
        if (effect.SwapLine != null)
        {
            Destroy(effect.SwapLine.gameObject);
        }
        if (effect.PlayerFloatingText != null)
        {
            Destroy(effect.PlayerFloatingText.gameObject);
        }
        if (effect.EnemyFloatingText != null)
        {
            Destroy(effect.EnemyFloatingText.gameObject);
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
