using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private const string DefaultTargetGuideText = "빠른 적 30 HP · 느린 적 90 HP · 매 30초마다 상대와 능력 카드를 1:1 교환합니다. [Esc]로 보유 효과 확인 가능";

    [Header("Top HUD References")]
    [SerializeField] private TMP_Text playerHealthLabelText;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private RectTransform playerHealthBarFill;
    [SerializeField] private UnityEngine.UI.Image playerHealthBarImage;
    [SerializeField] private float playerHealthBarMaxWidth = 320f;

    [SerializeField] private TMP_Text survivalTimeLabelText;
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text killCountText;

    [SerializeField] private TMP_Text swapCooldownText;
    [SerializeField] private RectTransform swapCooldownBarFill;
    [SerializeField] private UnityEngine.UI.Image swapCooldownBarImage;
    [SerializeField] private float swapCooldownBarMaxWidth = 234f;

    [Header("Bottom HUD References")]
    [SerializeField] private TMP_Text targetInfoText;

    [Header("Panel References")]
    [SerializeField] private GameObject hudRootPanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject abilitySwapPanel;
    [SerializeField] private GameObject resumeCountdownPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Pause Ability Overview References")]
    [SerializeField] private TMP_Text pausePlayerAbilitiesText;
    [SerializeField] private TMP_Text pauseMonsterAbilitiesText;

    [Header("Ability Swap Modal References")]
    [SerializeField] private TMP_Text swapModalHeaderText;
    [SerializeField] private TMP_Text swapModalStepGuideText;
    [SerializeField] private TMP_Text swapInspectedCardDetailText;
    [SerializeField] private UnityEngine.UI.Button[] monsterSlotButtons;
    [SerializeField] private UnityEngine.UI.Image[] monsterSlotCardImages;
    [SerializeField] private TMP_Text[] monsterSlotTitleTexts;
    [SerializeField] private TMP_Text[] monsterSlotDescTexts;
    [SerializeField] private TMP_Text[] monsterSlotBadgeTexts;

    [SerializeField] private UnityEngine.UI.Button[] playerSlotButtons;
    [SerializeField] private UnityEngine.UI.Image[] playerSlotCardImages;
    [SerializeField] private TMP_Text[] playerSlotTitleTexts;
    [SerializeField] private TMP_Text[] playerSlotDescTexts;
    [SerializeField] private TMP_Text[] playerSlotBadgeTexts;

    [SerializeField] private UnityEngine.UI.Button confirmAbilitySwapButton;
    [SerializeField] private UnityEngine.UI.Image confirmAbilitySwapButtonImage;
    [SerializeField] private TMP_Text confirmAbilitySwapButtonText;

    [Header("Resume Countdown References")]
    [SerializeField] private TMP_Text countdownResultSummaryText;
    [SerializeField] private TMP_Text countdownNumberText;

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
        UnityEngine.UI.Button gameOverMenuBtn = null,
        TMP_Text hpLabel = null,
        TMP_Text timeLabel = null)
    {
        playerHealthLabelText = hpLabel;
        playerHealthText = hpText;
        playerHealthBarFill = hpFill;
        playerHealthBarImage = hpImg;
        playerHealthBarMaxWidth = hpMaxWidth;
        survivalTimeLabelText = timeLabel;
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

    public void SetupAbilityUIRefs(
        TMP_Text pausePlayerText,
        TMP_Text pauseMonsterText,
        GameObject swapModal,
        TMP_Text swapHeader,
        TMP_Text swapStepGuide,
        TMP_Text swapInspectedDetail,
        UnityEngine.UI.Button[] mSlotButtons,
        UnityEngine.UI.Image[] mSlotImages,
        TMP_Text[] mSlotTitles,
        TMP_Text[] mSlotDescs,
        TMP_Text[] mSlotBadges,
        UnityEngine.UI.Button[] pSlotButtons,
        UnityEngine.UI.Image[] pSlotImages,
        TMP_Text[] pSlotTitles,
        TMP_Text[] pSlotDescs,
        TMP_Text[] pSlotBadges,
        UnityEngine.UI.Button confirmSwapBtn,
        UnityEngine.UI.Image confirmSwapBtnImg,
        TMP_Text confirmSwapBtnTxt,
        GameObject countdownModal,
        TMP_Text countdownSummaryTxt,
        TMP_Text countdownNumTxt)
    {
        pausePlayerAbilitiesText = pausePlayerText;
        pauseMonsterAbilitiesText = pauseMonsterText;
        abilitySwapPanel = swapModal;
        swapModalHeaderText = swapHeader;
        swapModalStepGuideText = swapStepGuide;
        swapInspectedCardDetailText = swapInspectedDetail;
        monsterSlotButtons = mSlotButtons;
        monsterSlotCardImages = mSlotImages;
        monsterSlotTitleTexts = mSlotTitles;
        monsterSlotDescTexts = mSlotDescs;
        monsterSlotBadgeTexts = mSlotBadges;
        playerSlotButtons = pSlotButtons;
        playerSlotCardImages = pSlotImages;
        playerSlotTitleTexts = pSlotTitles;
        playerSlotDescTexts = pSlotDescs;
        playerSlotBadgeTexts = pSlotBadges;
        confirmAbilitySwapButton = confirmSwapBtn;
        confirmAbilitySwapButtonImage = confirmSwapBtnImg;
        confirmAbilitySwapButtonText = confirmSwapBtnTxt;
        resumeCountdownPanel = countdownModal;
        countdownResultSummaryText = countdownSummaryTxt;
        countdownNumberText = countdownNumTxt;
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

        if (monsterSlotButtons != null)
        {
            for (int i = 0; i < monsterSlotButtons.Length; i++)
            {
                int slotIdx = i;
                if (monsterSlotButtons[slotIdx] != null)
                {
                    monsterSlotButtons[slotIdx].onClick.RemoveAllListeners();
                    monsterSlotButtons[slotIdx].onClick.AddListener(() =>
                    {
                        gameManager.SelectMonsterAbilitySlot(slotIdx);
                        gameManager.InspectAbilityCardInSwapModal(true, slotIdx);
                    });
                }
            }
        }

        if (playerSlotButtons != null)
        {
            for (int i = 0; i < playerSlotButtons.Length; i++)
            {
                int slotIdx = i;
                if (playerSlotButtons[slotIdx] != null)
                {
                    playerSlotButtons[slotIdx].onClick.RemoveAllListeners();
                    playerSlotButtons[slotIdx].onClick.AddListener(() =>
                    {
                        gameManager.InspectAbilityCardInSwapModal(false, slotIdx);
                    });
                }
            }
        }

        if (confirmAbilitySwapButton != null)
        {
            confirmAbilitySwapButton.onClick.RemoveAllListeners();
            confirmAbilitySwapButton.onClick.AddListener(gameManager.ConfirmAbilitySwapAndStartCountdown);
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
        if (abilitySwapPanel != null)
        {
            abilitySwapPanel.SetActive(state == GameState.AbilitySwap);
        }
        if (resumeCountdownPanel != null)
        {
            resumeCountdownPanel.SetActive(state == GameState.ResumeCountdown);
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

    public void UpdateHUD(
        float playerHp,
        float currentDecayRate,
        float survivalTime,
        float nextSwapTime,
        int phaseNumber,
        int killCount,
        float swapCooldownRemaining,
        Monster selectedTarget)
    {
        bool isLowHealth = playerHp <= GameConstants.LowHealthWarningThreshold;
        Color normalHpColor = new Color(0.22f, 0.92f, 0.72f, 1.0f);
        Color warningHpColor = new Color(1.0f, 0.25f, 0.30f, 1.0f);
        Color activeHpColor = isLowHealth ? warningHpColor : normalHpColor;

        if (playerHealthLabelText != null)
        {
            playerHealthLabelText.text = $"PLAYER / 체력 (초당 -{currentDecayRate:F1})";
        }

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

        if (survivalTimeLabelText != null)
        {
            float remainToSwap = Mathf.Max(0f, nextSwapTime - survivalTime);
            survivalTimeLabelText.text = $"PHASE {phaseNumber} · 교환 {remainToSwap:F0}s";
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
                int arrivalDmg = Mathf.CeilToInt(selectedTarget.GetExpectedArrivalDamage());
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

    // 4. 기타 UI / 편의 기능: ESC 일시정지 시 자신과 상대가 보유하고 있는 효과를 상시 확인
    public void RefreshPauseAbilityStatus(AbilitySwapSystem swapSystem)
    {
        if (swapSystem == null)
        {
            return;
        }

        if (pausePlayerAbilitiesText != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<color=#33FFD1><b>[내 보유 능력 슬롯 (PLAYER)]</b></color>");
            IReadOnlyList<AbilityType> pSlots = swapSystem.PlayerSlots;
            for (int i = 0; i < pSlots.Count; i++)
            {
                AbilityType ability = pSlots[i];
                sb.AppendLine($"• <b>{AbilitySwapSystem.GetAbilityName(ability)}</b> : {AbilitySwapSystem.GetPlayerEffectDescription(ability)}");
            }
            pausePlayerAbilitiesText.text = sb.ToString();
        }

        if (pauseMonsterAbilitiesText != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<color=#FF6B78><b>[몬스터 진영 보유 능력 슬롯 (MONSTER)]</b></color>");
            IReadOnlyList<AbilityType> mSlots = swapSystem.MonsterSlots;
            for (int i = 0; i < mSlots.Count; i++)
            {
                AbilityType ability = mSlots[i];
                sb.AppendLine($"• <b>{AbilitySwapSystem.GetAbilityName(ability)}</b> : {AbilitySwapSystem.GetMonsterEffectDescription(ability)}");
            }
            pauseMonsterAbilitiesText.text = sb.ToString();
        }
    }

    // 3. 상대 교환 AI 시스템 UI 팝업 열기 및 갱신
    public void OpenAbilitySwapModal(AbilitySwapSystem swapSystem, int currentPhaseNumber, float survivalTime)
    {
        if (swapModalHeaderText != null)
        {
            swapModalHeaderText.text = $"능력 카드 맞교환 이벤트 (생존 {Mathf.FloorToInt(survivalTime)}초 · PHASE {currentPhaseNumber})";
        }

        if (swapInspectedCardDetailText != null)
        {
            swapInspectedCardDetailText.text = "카드를 클릭하면 [플레이어 보유 시 효과]와 [몬스터 진영 보유 시 효과]를 모두 확인할 수 있습니다.";
        }

        RefreshAbilitySwapModal(swapSystem);
    }

    public void RefreshAbilitySwapModal(AbilitySwapSystem swapSystem)
    {
        if (swapSystem == null)
        {
            return;
        }

        int selectedMonsterIdx = swapSystem.SelectedMonsterSlotIndex;
        int aiSelectedPlayerIdx = swapSystem.AiSelectedPlayerSlotIndex;

        // 1) 상대(몬스터) 슬롯 3개 갱신
        if (monsterSlotTitleTexts != null)
        {
            for (int i = 0; i < monsterSlotTitleTexts.Length && i < swapSystem.MonsterSlots.Count; i++)
            {
                AbilityType ability = swapSystem.GetMonsterSlotAbility(i);
                bool isSelectedByPlayer = (i == selectedMonsterIdx);

                if (monsterSlotTitleTexts[i] != null)
                {
                    monsterSlotTitleTexts[i].text = AbilitySwapSystem.GetAbilityName(ability);
                }

                if (monsterSlotDescTexts != null && i < monsterSlotDescTexts.Length && monsterSlotDescTexts[i] != null)
                {
                    monsterSlotDescTexts[i].text =
                        $"<color=#8AE8FF>[플레이어 획득 시]</color>\n{AbilitySwapSystem.GetPlayerEffectDescription(ability)}\n\n" +
                        $"<color=#FF8E99>[몬스터 보유 시 (현재)]</color>\n{AbilitySwapSystem.GetMonsterEffectDescription(ability)}";
                }

                if (monsterSlotBadgeTexts != null && i < monsterSlotBadgeTexts.Length && monsterSlotBadgeTexts[i] != null)
                {
                    monsterSlotBadgeTexts[i].text = isSelectedByPlayer ? "★ 내가 가져올 카드 선택됨" : "클릭하여 선택";
                    monsterSlotBadgeTexts[i].color = isSelectedByPlayer
                        ? new Color(0.20f, 1.0f, 0.82f, 1f)
                        : new Color(0.75f, 0.82f, 0.92f, 0.85f);
                }

                if (monsterSlotCardImages != null && i < monsterSlotCardImages.Length && monsterSlotCardImages[i] != null)
                {
                    monsterSlotCardImages[i].color = isSelectedByPlayer
                        ? new Color(0.14f, 0.36f, 0.36f, 0.98f)
                        : new Color(0.16f, 0.12f, 0.18f, 0.95f);
                }
            }
        }

        // 2) 내(플레이어) 슬롯 3개 갱신
        if (playerSlotTitleTexts != null)
        {
            for (int i = 0; i < playerSlotTitleTexts.Length && i < swapSystem.PlayerSlots.Count; i++)
            {
                AbilityType ability = swapSystem.GetPlayerSlotAbility(i);
                bool isSelectedByAi = (i == aiSelectedPlayerIdx);

                if (playerSlotTitleTexts[i] != null)
                {
                    playerSlotTitleTexts[i].text = AbilitySwapSystem.GetAbilityName(ability);
                }

                if (playerSlotDescTexts != null && i < playerSlotDescTexts.Length && playerSlotDescTexts[i] != null)
                {
                    playerSlotDescTexts[i].text =
                        $"<color=#8AE8FF>[플레이어 보유 시 (현재)]</color>\n{AbilitySwapSystem.GetPlayerEffectDescription(ability)}\n\n" +
                        $"<color=#FF8E99>[몬스터 획득 시]</color>\n{AbilitySwapSystem.GetMonsterEffectDescription(ability)}";
                }

                if (playerSlotBadgeTexts != null && i < playerSlotBadgeTexts.Length && playerSlotBadgeTexts[i] != null)
                {
                    playerSlotBadgeTexts[i].text = isSelectedByAi ? "★ 상대 AI가 가져갈 카드 (33.3%)" : "내 보유 슬롯 (클릭 시 상세 확인)";
                    playerSlotBadgeTexts[i].color = isSelectedByAi
                        ? new Color(1.0f, 0.42f, 0.48f, 1f)
                        : new Color(0.65f, 0.82f, 0.95f, 0.85f);
                }

                if (playerSlotCardImages != null && i < playerSlotCardImages.Length && playerSlotCardImages[i] != null)
                {
                    playerSlotCardImages[i].color = isSelectedByAi
                        ? new Color(0.38f, 0.14f, 0.20f, 0.98f)
                        : new Color(0.10f, 0.16f, 0.24f, 0.95f);
                }
            }
        }

        bool canConfirm = swapSystem.CanConfirmSwap;

        if (swapModalStepGuideText != null)
        {
            if (!canConfirm)
            {
                swapModalStepGuideText.text = "STEP 1. 위쪽 [상대 슬롯 (3개)] 중 가져올 효과 카드 1개를 먼저 클릭하세요.";
                swapModalStepGuideText.color = new Color(1.0f, 0.88f, 0.42f, 1f);
            }
            else
            {
                AbilityType takeAbility = swapSystem.GetMonsterSlotAbility(selectedMonsterIdx);
                AbilityType giveAbility = swapSystem.GetPlayerSlotAbility(aiSelectedPlayerIdx);
                swapModalStepGuideText.text =
                    $"STEP 2. 상대 AI가 내 [{AbilitySwapSystem.GetAbilityName(giveAbility)}] 카드를 지목했습니다! 아래 [교환 확정]을 눌러 [{AbilitySwapSystem.GetAbilityName(takeAbility)}] 카드와 1:1 맞교환하세요.";
                swapModalStepGuideText.color = new Color(0.20f, 1.0f, 0.82f, 1f);
            }
        }

        if (confirmAbilitySwapButton != null)
        {
            confirmAbilitySwapButton.interactable = canConfirm;
        }

        if (confirmAbilitySwapButtonImage != null)
        {
            confirmAbilitySwapButtonImage.color = canConfirm
                ? new Color(0.18f, 0.88f, 0.72f, 1.0f)
                : new Color(0.22f, 0.26f, 0.32f, 0.85f);
        }

        if (confirmAbilitySwapButtonText != null)
        {
            confirmAbilitySwapButtonText.text = canConfirm ? "교환 확정 (1:1 맞교환)" : "상대 슬롯을 먼저 선택하세요";
            confirmAbilitySwapButtonText.color = canConfirm
                ? new Color(0.04f, 0.08f, 0.12f, 1.0f)
                : new Color(0.65f, 0.72f, 0.80f, 0.85f);
        }
    }

    public void ShowInspectedAbilityDetail(AbilityType ability)
    {
        if (swapInspectedCardDetailText == null)
        {
            return;
        }

        string name = AbilitySwapSystem.GetAbilityName(ability);
        string pDesc = AbilitySwapSystem.GetPlayerEffectDescription(ability);
        string mDesc = AbilitySwapSystem.GetMonsterEffectDescription(ability);

        swapInspectedCardDetailText.text =
            $"<b>[{name}]</b>  |  <color=#33FFD1>플레이어 보유 시:</color> {pDesc}   ·   <color=#FF7582>몬스터 진영 보유 시:</color> {mDesc}";
    }

    public void ShowSwapResultAndCountdown(AbilityType acquiredByPlayer, AbilityType takenByAi, float remainingSeconds)
    {
        if (countdownResultSummaryText != null)
        {
            countdownResultSummaryText.text =
                $"능력 1:1 맞교환 완료!\n" +
                $"<color=#33FFD1>획득한 능력 : [{AbilitySwapSystem.GetAbilityName(acquiredByPlayer)}]</color> ({AbilitySwapSystem.GetPlayerEffectDescription(acquiredByPlayer)})\n" +
                $"<color=#FF7582>상대에게 넘어간 능력 : [{AbilitySwapSystem.GetAbilityName(takenByAi)}]</color> ({AbilitySwapSystem.GetMonsterEffectDescription(takenByAi)})";
        }

        UpdateResumeCountdownDisplay(remainingSeconds);
    }

    public void UpdateResumeCountdownDisplay(float remainingSeconds)
    {
        if (countdownNumberText != null)
        {
            int ceilSeconds = Mathf.Clamp(Mathf.CeilToInt(remainingSeconds), 1, 3);
            countdownNumberText.text = $"{ceilSeconds}";
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
