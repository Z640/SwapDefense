using UnityEngine;

public class HitBurstEffect : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private SpriteRenderer burstRenderer;

    private float elapsedTime;
    private float duration;
    private float startScale;
    private float endScale;
    private Color baseColor;

    public void SetupPrefabReferences(SpriteRenderer rendererComponent)
    {
        burstRenderer = rendererComponent;
    }

    public void Initialize(Vector3 worldPos, Color effectColor, float radius, float effectDuration)
    {
        transform.position = worldPos;
        baseColor = effectColor;
        duration = effectDuration;
        elapsedTime = 0f;
        startScale = radius * 1.2f;
        endScale = radius * 2.8f;

        transform.localScale = new Vector3(startScale, startScale, 1f);
        if (burstRenderer != null)
        {
            burstRenderer.color = baseColor;
        }
    }

    public bool TickEffect(float deltaTime)
    {
        elapsedTime += deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        float scale = Mathf.Lerp(startScale, endScale, t);
        transform.localScale = new Vector3(scale, scale, 1f);

        if (burstRenderer != null)
        {
            Color currentColor = baseColor;
            currentColor.a = 1f - t;
            burstRenderer.color = currentColor;
        }

        if (elapsedTime >= duration)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}
