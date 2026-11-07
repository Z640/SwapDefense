using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private const string DefaultTargetGuideText = "빠른 적 30 HP · 느린 적 90 HP · 교환한 적은 남은 체력만큼 도착 피해를 줍니다. F로 맞히면 피해도 줄어듭니다.";

    [Header("Top HUD References")]
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private RectTransform playerHealthBarFill;
    [SerializeField] private UnityEngine.UI.Image playerHealthBarImage;
    [SerializeField] private float playerHealthBarMaxWidth = 340f;

    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text killCountText;

    [SerializeField] private TMP_Text swapCooldownText;
    [SerializeField] private RectTransform swapCooldownBarFill;
    [SerializeField] private UnityEngine.UI.Image swapCooldownBarImage;
    [SerializeField] private float swapCooldownBarMaxWidth = 220f;

    [Header("Bottom HUD References")]
    [SerializeField] private TMP_Text targetInfoText;

    [Header("Panel References")]
    [SerializeField] private GameObject hudRootPanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game Over References")]
    [SerializeField] private TMP_Text finalSurvivalTimeText;
    [SerializeField] private TMP_Text finalKillCountText;

    [Header("Button References")]
    [SerializeField] private UnityEngine.UI.Button startButton;
    [SerializeField] private UnityEngine.UI.Button resumeButton;
    [SerializeField] private UnityEngine.UI.Button restartButton;
    [SerializeField] private UnityEngine.UI.Button pauseMainMenuButton;
    [SerializeField] private UnityEngine.UI.Button gameOverMainMenuButton;

    public void SetupReferences(
        TMP_Text hpText,
        RectTransform hpFill,
        UnityEngine.UI.Image hpImg,
        float hpMaxWidth,
        TMP_Text timeText,
        TMP_Text killsText,
        TMP_Text qText,
        RectTransform qFill,
        UnityEngine.UI.Image qImg,
        float qMaxWidth,
        TMP_Text targetText,
        GameObject hudRoot,
        GameObject title,
        GameObject pause,
        GameObject gameOver,
        TMP_Text finalTime,
        TMP_Text finalKills,
        UnityEngine.UI.Button startBtn,
        UnityEngine.UI.Button resumeBtn,
        UnityEngine.UI.Button restartBtn,
        UnityEngine.UI.Button pauseMenuBtn = null,
        UnityEngine.UI.Button gameOverMenuBtn = null)
    {
        playerHealthText = hpText;
        playerHealthBarFill = hpFill;
        playerHealthBarImage = hpImg;
        playerHealthBarMaxWidth = hpMaxWidth;
        survivalTimeText = timeText;
        killCountText = killsText;
        swapCooldownText = qText;
        swapCooldownBarFill = qFill;
        swapCooldownBarImage = qImg;
        swapCooldownBarMaxWidth = qMaxWidth;
        targetInfoText = targetText;
        hudRootPanel = hudRoot;
        titlePanel = title;
        pausePanel = pause;
        gameOverPanel = gameOver;
        finalSurvivalTimeText = finalTime;
        finalKillCountText = finalKills;
        startButton = startBtn;
        resumeButton = resumeBtn;
        restartButton = restartBtn;
        pauseMainMenuButton = pauseMenuBtn;
        gameOverMainMenuButton = gameOverMenuBtn;
    }

    public void BindButtons(GameManager gameManager)
    {
        if (gameManager == null)
        {
            return;
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(gameManager.StartGame);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(gameManager.ResumeGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(gameManager.RestartGame);
        }

        if (pauseMainMenuButton != null)
        {
            pauseMainMenuButton.onClick.RemoveAllListeners();
            pauseMainMenuButton.onClick.AddListener(gameManager.ReturnToMainMenu);
        }

        if (gameOverMainMenuButton != null)
        {
            gameOverMainMenuButton.onClick.RemoveAllListeners();
            gameOverMainMenuButton.onClick.AddListener(gameManager.ReturnToMainMenu);
        }
    }

    public void UpdatePanelVisibility(GameState state)
    {
        if (titlePanel != null)
        {
            titlePanel.SetActive(state == GameState.Title);
        }
        if (pausePanel != null)
        {
            pausePanel.SetActive(state == GameState.Paused);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(state == GameState.GameOver);
        }
        if (hudRootPanel != null)
        {
            hudRootPanel.SetActive(true);
        }
    }

    public void UpdateHUD(float playerHp, float survivalTime, int killCount, float swapCooldownRemaining, Monster selectedTarget)
    {
        bool isLowHealth = playerHp <= GameConstants.LowHealthWarningThreshold;
        Color normalHpColor = new Color(0.22f, 0.92f, 0.72f, 1.0f);
        Color warningHpColor = new Color(1.0f, 0.25f, 0.30f, 1.0f);
        Color activeHpColor = isLowHealth ? warningHpColor : normalHpColor;

        if (playerHealthText != null)
        {
            playerHealthText.text = $"{playerHp:F1} / 100";
            playerHealthText.color = isLowHealth ? warningHpColor : new Color(0.95f, 0.98f, 1.0f, 1.0f);
        }

        if (playerHealthBarFill != null)
        {
            float hpRatio = Mathf.Clamp01(playerHp / GameConstants.PlayerMaxHealth);
            playerHealthBarFill.sizeDelta = new Vector2(playerHealthBarMaxWidth * hpRatio, playerHealthBarFill.sizeDelta.y);
        }

        if (playerHealthBarImage != null)
        {
            playerHealthBarImage.color = activeHpColor;
        }

        if (survivalTimeText != null)
        {
            survivalTimeText.text = $"{survivalTime:F1} s";
        }

        if (killCountText != null)
        {
            killCountText.text = killCount.ToString("D2");
        }

        if (swapCooldownText != null)
        {
            if (swapCooldownRemaining <= 0f)
            {
                swapCooldownText.text = "사용 가능 ↔";
                swapCooldownText.color = new Color(0.20f, 1.0f, 0.82f, 1.0f);
            }
            else
            {
                swapCooldownText.text = $"{swapCooldownRemaining:F1} s";
                swapCooldownText.color = new Color(0.95f, 0.75f, 0.30f, 1.0f);
            }
        }

        if (swapCooldownBarFill != null)
        {
            float qRatio = swapCooldownRemaining <= 0f
                ? 1.0f
                : Mathf.Clamp01(1.0f - (swapCooldownRemaining / GameConstants.PlayerSwapCooldown));
            swapCooldownBarFill.sizeDelta = new Vector2(swapCooldownBarMaxWidth * qRatio, swapCooldownBarFill.sizeDelta.y);
        }

        if (swapCooldownBarImage != null)
        {
            swapCooldownBarImage.color = swapCooldownRemaining <= 0f
                ? new Color(0.20f, 1.0f, 0.82f, 1.0f)
                : new Color(0.85f, 0.62f, 0.22f, 1.0f);
        }

        if (targetInfoText != null)
        {
            if (selectedTarget != null && !selectedTarget.IsDead)
            {
                string dashState = selectedTarget.IsDashing ? " [돌진 중]" : "";
                int arrivalDmg = selectedTarget.IsDashing
                    ? Mathf.CeilToInt(selectedTarget.CurrentHealth)
                    : (int)(selectedTarget.Type == EnemyType.Fast ? GameConstants.FastEnemyArrivalDamage : GameConstants.SlowEnemyArrivalDamage);
                targetInfoText.text = $"{selectedTarget.KoreanTypeName}{dashState} · 현재 체력 {selectedTarget.CurrentHealth:F1} HP · 예상 도착 피해 {arrivalDmg}";
                targetInfoText.color = new Color(0.20f, 1.0f, 0.82f, 1.0f);
            }
            else
            {
                targetInfoText.text = DefaultTargetGuideText;
                targetInfoText.color = new Color(0.76f, 0.82f, 0.90f, 1.0f);
            }
        }
    }

    public void ShowGameOverSummary(float survivalTime, int killCount)
    {
        if (finalSurvivalTimeText != null)
        {
            finalSurvivalTimeText.text = $"생존 시간 : {survivalTime:F1} s";
        }
        if (finalKillCountText != null)
        {
            finalKillCountText.text = $"처치 수 : {killCount:D2}";
        }
    }
}
