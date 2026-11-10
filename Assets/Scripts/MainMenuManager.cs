using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI References")]
    [SerializeField] private UnityEngine.UI.Button startGameButton;
    [SerializeField] private UnityEngine.UI.Button toggleGuideButton;
    [SerializeField] private UnityEngine.UI.Button closeGuideButton;
    [SerializeField] private UnityEngine.UI.Button quitGameButton;
    [SerializeField] private GameObject guideModalPanel;

    public void SetupReferences(
        string targetSceneName,
        UnityEngine.UI.Button startBtn,
        UnityEngine.UI.Button guideBtn,
        UnityEngine.UI.Button closeGuideBtn,
        UnityEngine.UI.Button quitBtn,
        GameObject guidePanel)
    {
        gameSceneName = targetSceneName;
        startGameButton = startBtn;
        toggleGuideButton = guideBtn;
        closeGuideButton = closeGuideBtn;
        quitGameButton = quitBtn;
        guideModalPanel = guidePanel;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(StartGame);
        }

        if (toggleGuideButton != null)
        {
            toggleGuideButton.onClick.RemoveAllListeners();
            toggleGuideButton.onClick.AddListener(OpenGuidePanel);
        }

        if (closeGuideButton != null)
        {
            closeGuideButton.onClick.RemoveAllListeners();
            closeGuideButton.onClick.AddListener(CloseGuidePanel);
        }

        if (quitGameButton != null)
        {
            quitGameButton.onClick.RemoveAllListeners();
            quitGameButton.onClick.AddListener(QuitGame);
        }

        if (guideModalPanel != null)
        {
            guideModalPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenGuidePanel()
    {
        if (guideModalPanel != null)
        {
            guideModalPanel.SetActive(true);
        }
    }

    public void CloseGuidePanel()
    {
        if (guideModalPanel != null)
        {
            guideModalPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
