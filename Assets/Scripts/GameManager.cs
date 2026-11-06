using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    Title,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Title;
    [SerializeField] private float survivalTime;
    [SerializeField] private int killCount;

    [Header("System References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private VFXManager vfxManager;

    public GameState CurrentState => currentState;
    public float SurvivalTime => survivalTime;
    public int KillCount => killCount;

    public void SetupReferences(PlayerController player, WaveSpawner spawner, CombatSystem combat, UIManager ui, VFXManager vfx)
    {
        playerController = player;
        waveSpawner = spawner;
        combatSystem = combat;
        uiManager = ui;
        vfxManager = vfx;
    }

    private void Start()
    {
        if (uiManager != null)
        {
            uiManager.BindButtons(this);
        }
        EnterTitleState();
    }

    private void Update()
    {
        HandleEscapeInput();

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

        if (vfxManager != null)
        {
            vfxManager.TickVFX(deltaTime);
        }

        RefreshHUD();
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
        killCount = 0;

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
        killCount = 0;

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

    public void RestartGame()
    {
        StartGame();
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

    public void AddKillCount()
    {
        killCount++;
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
            survivalTime,
            killCount,
            playerController.SwapCooldownRemaining,
            selected
        );
    }
}
