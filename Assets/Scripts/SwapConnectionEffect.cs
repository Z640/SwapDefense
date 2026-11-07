using TMPro;
using UnityEngine;

public class SwapConnectionEffect : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private LineRenderer swapLine;
    [SerializeField] private TextMeshPro playerFloatingText;
    [SerializeField] private TextMeshPro enemyFloatingText;

    private Vector3 playerStartPos;
    private Transform enemyTransform;
    private Vector3 lastEnemyPos;
    private float elapsedTime;
    private float duration;

    public void SetupPrefabReferences(LineRenderer lineComponent, TextMeshPro playerTextComponent, TextMeshPro enemyTextComponent)
    {
        swapLine = lineComponent;
        playerFloatingText = playerTextComponent;
        enemyFloatingText = enemyTextComponent;
    }

    public void Initialize(
        Vector3 playerPos,
        Transform targetEnemyTransform,
        float oldPlayerHp,
        float newPlayerHp,
        float oldEnemyHp,
        float newEnemyHp,
        float effectDuration)
    {
        playerStartPos = playerPos;
        enemyTransform = targetEnemyTransform;
        lastEnemyPos = targetEnemyTransform != null ? targetEnemyTransform.position : playerPos + Vector3.up * 100f;
        elapsedTime = 0f;
        duration = effectDuration;

        if (swapLine != null)
        {
            swapLine.SetPosition(0, playerStartPos);
            swapLine.SetPosition(1, lastEnemyPos);
        }

        if (playerFloatingText != null)
        {
            playerFloatingText.text = $"{oldPlayerHp:F1} → {newPlayerHp:F1}";
            playerFloatingText.transform.position = playerStartPos + new Vector3(0f, 36f, 0f);
        }

        if (enemyFloatingText != null)
        {
            enemyFloatingText.text = $"{oldEnemyHp:F1} → {newEnemyHp:F1}";
            enemyFloatingText.transform.position = lastEnemyPos + new Vector3(0f, 44f, 0f);
        }
    }

    public bool TickEffect(float deltaTime)
    {
        elapsedTime += deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        float alpha = 1f - t;

        if (enemyTransform != null)
        {
            lastEnemyPos = enemyTransform.position;
        }

        if (swapLine != null)
        {
            swapLine.SetPosition(0, playerStartPos);
            swapLine.SetPosition(1, lastEnemyPos);
            Color lineColor = new Color(0.20f, 1.0f, 0.82f, alpha * 0.9f);
            swapLine.startColor = lineColor;
            swapLine.endColor = lineColor;
            float width = Mathf.Lerp(5.0f, 1.0f, t);
            swapLine.startWidth = width;
            swapLine.endWidth = width;
        }

        if (playerFloatingText != null)
        {
            playerFloatingText.transform.position = playerStartPos + new Vector3(0f, 36f + (t * 26f), 0f);
            Color c = playerFloatingText.color;
            c.a = alpha;
            playerFloatingText.color = c;
        }

        if (enemyFloatingText != null)
        {
            enemyFloatingText.transform.position = lastEnemyPos + new Vector3(0f, 44f + (t * 26f), 0f);
            Color c = enemyFloatingText.color;
            c.a = alpha;
            enemyFloatingText.color = c;
        }

        if (elapsedTime >= duration)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}
