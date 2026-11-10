using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Title,
    Playing,
    Paused,
    AbilitySwap,
    ResumeCountdown,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [Header("Scene & Startup Settings")]
    [SerializeField] private bool startImmediatelyOnLoad = true;
    [SerializeField] private string mainMenuSceneName = "StartMenuScene";

    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Playing;
    [SerializeField] private float survivalTime;
    [SerializeField] private float nextAbilitySwapTime = GameConstants.AbilitySwapCycleInterval;
    [SerializeField] private float resumeCountdownTimer;
    [SerializeField] private int killCount;

    [Header("System References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private VFXManager vfxManager;
    [SerializeField] private AbilitySwapSystem abilitySwapSystem;
    [SerializeField] private AudioManager audioManager;

    public GameState CurrentState => currentState;
    public float SurvivalTime => survivalTime;
    public float NextAbilitySwapTime => nextAbilitySwapTime;
    public float ResumeCountdownTimer => resumeCountdownTimer;
    public int KillCount => killCount;
    public AbilitySwapSystem AbilitySwap => abilitySwapSystem;

    public int CurrentPhaseNumber => waveSpawner != null
        ? waveSpawner.GetPhaseIndex(survivalTime) + 1
        : 1;

    public void SetupReferences(
        PlayerController player,
        WaveSpawner spawner,
        CombatSystem combat,
        UIManager ui,
        VFXManager vfx,
        AbilitySwapSystem swapSystem = null,
        AudioManager audio = null)
    {
        playerController = player;
        waveSpawner = spawner;
        combatSystem = combat;
        uiManager = ui;
        vfxManager = vfx;
        abilitySwapSystem = swapSystem;
        audioManager = audio;
        startImmediatelyOnLoad = true;
        mainMenuSceneName = "StartMenuScene";
    }

    private void Start()
    {
        if (uiManager != null)
        {
            uiManager.BindButtons(this);
        }

        if (startImmediatelyOnLoad)
        {
            StartGame();
        }
        else
        {
            EnterTitleState();
        }
    }

    private void Update()
    {
        HandleEscapeInput();

        if (currentState == GameState.ResumeCountdown)
        {
            TickResumeCountdown(Time.unscaledDeltaTime);
            return;
        }

        if (currentState != GameState.Playing)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        survivalTime += deltaTime;

        if (waveSpawner != null)
        {
            waveSpawner.TickSpawner(deltaTime, survivalTime, playerController);
        }

        // Player might have died from defense line arrival damage during TickSpawner
        if (currentState != GameState.Playing)
        {
            return;
        }

        if (playerController != null)
        {
            playerController.TickPlayer(deltaTime);
        }

        if (currentState != GameState.Playing)
        {
            return;
        }

        if (vfxManager != null)
        {
            vfxManager.TickVFX(deltaTime);
        }

        RefreshHUD();

        // 생존 시간 기준 매 30.0초 주기(30s, 60s, 90s...)마다 카드 교환 팝업 발동
        if (survivalTime >= nextAbilitySwapTime)
        {
            TriggerAbilitySwapEvent();
        }
    }

    private void TickResumeCountdown(float unscaledDeltaTime)
    {
        resumeCountdownTimer -= unscaledDeltaTime;

        if (uiManager != null)
        {
            uiManager.UpdateResumeCountdownDisplay(resumeCountdownTimer);
        }

        if (resumeCountdownTimer <= 0f)
        {
            resumeCountdownTimer = 0f;
            currentState = GameState.Playing;
            Time.timeScale = 1f;

            if (uiManager != null)
            {
                uiManager.UpdatePanelVisibility(currentState);
            }

            if (vfxManager != null)
            {
                vfxManager.ShowSystemNotice($"Phase {CurrentPhaseNumber} 전투 재개! 다음 능력 교환까지 30.0초");
            }

            RefreshHUD();
        }
    }

    private void HandleEscapeInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    public void EnterTitleState()
    {
        Time.timeScale = 0f;
        currentState = GameState.Title;
        survivalTime = 0f;
        nextAbilitySwapTime = GameConstants.AbilitySwapCycleInterval;
        resumeCountdownTimer = 0f;
        killCount = 0;

        if (abilitySwapSystem != null)
        {
            abilitySwapSystem.ResetAbilities();
        }
        if (waveSpawner != null)
        {
            waveSpawner.ResetSpawner();
        }
        if (combatSystem != null)
        {
            combatSystem.ClearAllBullets();
        }
        if (vfxManager != null)
        {
            vfxManager.ClearAllEffects();
        }
        if (playerController != null)
        {
            playerController.ResetPlayer();
        }
        if (uiManager != null)
        {
            uiManager.UpdatePanelVisibility(currentState);
        }
        RefreshHUD();
    }

    public void StartGame()
    {
        survivalTime = 0f;
        nextAbilitySwapTime = GameConstants.AbilitySwapCycleInterval;
        resumeCountdownTimer = 0f;
        killCount = 0;

        if (abilitySwapSystem != null)
        {
            abilitySwapSystem.ResetAbilities();
        }
        if (waveSpawner != null)
        {
            waveSpawner.ResetSpawner();
        }
        if (combatSystem != null)
        {
            combatSystem.ClearAllBullets();
        }
        if (vfxManager != null)
        {
            vfxManager.ClearAllEffects();
        }
        if (playerController != null)
        {
            playerController.ResetPlayer();
        }
        if (audioManager != null)
        {
            audioManager.PlayMainBgm();
        }

        currentState = GameState.Playing;
        Time.timeScale = 1f;

        if (uiManager != null)
        {
            uiManager.UpdatePanelVisibility(currentState);
        }
        RefreshHUD();
    }

    public void PauseGame()
    {
        if (currentState != GameState.Playing)
        {
            return;
        }

        currentState = GameState.Paused;
        Time.timeScale = 0f;

        if (uiManager != null)
        {
            uiManager.RefreshPauseAbilityStatus(abilitySwapSystem);
            uiManager.UpdatePanelVisibility(currentState);
        }
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Paused)
        {
            return;
        }

        currentState = GameState.Playing;
        Time.timeScale = 1f;

        if (uiManager != null)
        {
            uiManager.UpdatePanelVisibility(currentState);
        }
    }

    public void TriggerAbilitySwapEvent()
    {
        if (currentState != GameState.Playing)
        {
            return;
        }

        currentState = GameState.AbilitySwap;
        Time.timeScale = 0f;
        nextAbilitySwapTime += GameConstants.AbilitySwapCycleInterval;

        if (abilitySwapSystem != null)
        {
            abilitySwapSystem.BeginSwapDraft();
        }

        if (uiManager != null)
        {
            uiManager.OpenAbilitySwapModal(abilitySwapSystem, CurrentPhaseNumber, survivalTime);
            uiManager.UpdatePanelVisibility(currentState);
        }
    }

    public void SelectMonsterAbilitySlot(int monsterSlotIndex)
    {
        if (currentState != GameState.AbilitySwap || abilitySwapSystem == null)
        {
            return;
        }

        if (abilitySwapSystem.SelectMonsterSlotForSwap(monsterSlotIndex))
        {
            if (uiManager != null)
            {
                uiManager.RefreshAbilitySwapModal(abilitySwapSystem);
            }
        }
    }

    public void InspectAbilityCardInSwapModal(bool isMonsterSide, int slotIndex)
    {
        if (currentState != GameState.AbilitySwap || abilitySwapSystem == null || uiManager == null)
        {
            return;
        }

        AbilityType ability = isMonsterSide
            ? abilitySwapSystem.GetMonsterSlotAbility(slotIndex)
            : abilitySwapSystem.GetPlayerSlotAbility(slotIndex);

        uiManager.ShowInspectedAbilityDetail(ability);
    }

    public void ConfirmAbilitySwapAndStartCountdown()
    {
        if (currentState != GameState.AbilitySwap || abilitySwapSystem == null)
        {
            return;
        }

        if (!abilitySwapSystem.ConfirmSwap(out AbilityType acquiredByPlayer, out AbilityType takenByAi))
        {
            return;
        }

        if (waveSpawner != null)
        {
            waveSpawner.RefreshActiveMonstersForAbilityChanges();
        }

        currentState = GameState.ResumeCountdown;
        resumeCountdownTimer = GameConstants.AbilitySwapResumeCountdown;

        if (uiManager != null)
        {
            uiManager.ShowSwapResultAndCountdown(acquiredByPlayer, takenByAi, resumeCountdownTimer);
            uiManager.UpdatePanelVisibility(currentState);
        }

        RefreshHUD();
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    public void TriggerGameOver()
    {
        if (currentState == GameState.GameOver)
        {
            return;
        }

        currentState = GameState.GameOver;
        Time.timeScale = 0f;

        RefreshHUD();

        if (uiManager != null)
        {
            uiManager.ShowGameOverSummary(survivalTime, killCount);
            uiManager.UpdatePanelVisibility(currentState);
        }
    }

    public void NotifyMonsterKilledByPlayer()
    {
        killCount++;

        if (abilitySwapSystem != null && playerController != null)
        {
            float killHeal = abilitySwapSystem.GetPlayerKillHealAmount();
            if (killHeal > 0f)
            {
                playerController.HealHealth(killHeal);
            }
        }
    }

    public void AddKillCount()
    {
        NotifyMonsterKilledByPlayer();
    }

    private void RefreshHUD()
    {
        if (uiManager == null || playerController == null)
        {
            return;
        }

        Monster selected = combatSystem != null ? combatSystem.SelectedTarget : null;
        uiManager.UpdateHUD(
            playerController.CurrentHealth,
            playerController.CurrentHealthDecayRate,
            survivalTime,
            nextAbilitySwapTime,
            CurrentPhaseNumber,
            killCount,
            playerController.SwapCooldownRemaining,
            selected
        );
    }
}
