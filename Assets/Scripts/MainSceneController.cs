using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainSceneController : UIController
{
    public CanvasGroup backgroundPanel;
    public CanvasGroup startMenuPanel;
    public CanvasGroup ingamePanel;
    public CanvasGroup losePanel;
    public CanvasGroup creditsPanel;
    public CanvasGroup settingPanel;
    public CanvasGroup pausePanel;
    public CanvasGroup guideMenuPanel, guideGamePanel;
    public CanvasGroup confirmWindow;
    public DescriptionWindowController _dscw;
    public TextMeshProUGUI defeatText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI checkpointText;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI ingameHighscoreText;
    public TextMeshProUGUI loseWaveText;
    public TextMeshProUGUI loseLifeText;
    public TextMeshProUGUI loseCheckpointText;
    public TextMeshProUGUI loseHighscoreText;
    public Button RespawnButton;

    private bool isStarted;

    private void Awake()
    {
        backgroundPanel.gameObject.SetActive(true);
        startMenuPanel.gameObject.SetActive(true);
        ingamePanel.gameObject.SetActive(false);
        losePanel.gameObject.SetActive(false);
    }

    private void Start()
    {
        isStarted = true;
        highscoreText.text = GameManager.Instance.highestScore.ToString();
    }

    private void Update()
    {
        if (GameManager.Instance.currentWave % GameManager.Instance.bossWave == 0 && GameManager.Instance.currentWave != 0)
        {
            waveText.text = GameManager.Instance.currentWave.ToString();
        }
        else
        {
            waveText.text = GameManager.Instance.currentWave.ToString();
        }

        lifeText.text = GameManager.Instance.currentLife.ToString();
        checkpointText.text = GameManager.Instance.currentCheckpoint.ToString();
        ingameHighscoreText.text = GameManager.Instance.highestScore.ToString();
    }

    private void OnEnable()
    {
        if (isStarted)
        {
            highscoreText.text = GameManager.Instance.highestScore.ToString();
        }
    }

    private IEnumerator StartGame()
    {
        StartCoroutine(FadeOut(backgroundPanel, 0.4f));
        StartCoroutine(FadeOut(startMenuPanel, 0.4f));
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FadeIn(ingamePanel, 1f));
        //GameManager.Instance.PlayBgm("Battle_Normal");
        StartCoroutine(GameManager.Instance._enemySpawnManager.SpawnEnemy());
        //GameManager.Instance.currentLife = GameManager.Instance.setLife;

        //yield return new WaitForSeconds(1f);

        GameManager.Instance.isBattleStarted = true;
    }

    public IEnumerator GameOver()
    {
        GameManager.Instance.isBattleStarted = false;
        GameManager.Instance.PlayBgm("Battle_Defeat");
        StartCoroutine(FadeOut(ingamePanel, 1f));
        StartCoroutine(FadeIn(backgroundPanel, 1f));

        yield return new WaitForSeconds(1f);

        defeatText.text = GenerateDefeatText();
        CalculateResult();
        StartCoroutine(FadeIn(losePanel, 0.4f));

        yield return new WaitForSeconds(0.2f);
    }

    public void StartGameButton()
    {
        
        StartCoroutine(StartGame());
    }

    private string GenerateDefeatText()
    {
        int chose = Random.Range(0, 3);
        switch (chose)
        {
            case 0:
                return "GAME OVER";
            case 1:
                return "TRY AGAIN";
            default:
                return "PONG?";
        }
    }

    public void SurrenderGameButton()
    {
        GameManager.Instance.PlayBgm("Menu_Main");
        StartCoroutine(SurrenderGame());
    }

    IEnumerator SurrenderGame()
    {
        //StartCoroutine(FadeOut(backgroundPanel, 1f));
        StartCoroutine(FadeOut(losePanel, 0.4f));
        StartCoroutine(FadeOut(ingamePanel, 0.4f));
        StartCoroutine(FadeOut(pausePanel, 0.4f));
        StartCoroutine(FadeOut(confirmWindow, 0.4f));

        StartCoroutine(FadeIn(backgroundPanel, 1f));
        StartCoroutine(FadeIn(startMenuPanel, 1f));
        Time.timeScale = 1f;
        //StartCoroutine(GameManager.Instance._enemySpawnManager.SpawnEnemy());

        yield return new WaitForSeconds(0f);

        //GameManager.Instance.isBattleStarted = true;
        GameManager.Instance.ResetState();
    }

    public void RespawnCheckpointButton()
    {
        
        StartCoroutine(RespawnCheckpoint());
    }


    IEnumerator RespawnCheckpoint()
    {
        StartCoroutine(FadeOut(backgroundPanel, 1f));
        StartCoroutine(FadeOut(losePanel, 0.4f));
        StartCoroutine(FadeIn(ingamePanel, 1f));

        yield return new WaitForSeconds(0f);

        GameManager.Instance.RespawnCheckpoint();
        GameManager.Instance.isBattleStarted = true;
        StartCoroutine(GameManager.Instance._enemySpawnManager.SpawnEnemy());
    }

    public void CalculateResult()
    {
        loseWaveText.text = GameManager.Instance.currentWave.ToString();
        loseLifeText.text = GameManager.Instance.currentLife.ToString();
        loseCheckpointText.text = GameManager.Instance.currentCheckpoint.ToString();
        loseHighscoreText.text = GameManager.Instance.highestScore.ToString();

    }

    public void creditsButton()
    {
        StartCoroutine(SmoothFadeTransition(0.3f, startMenuPanel, creditsPanel));
    }

    public void closeCreditsButton()
    {
        StartCoroutine(SmoothFadeTransition(0.3f, creditsPanel, startMenuPanel));
    }

    public void settingButton()
    {
        StartCoroutine(SmoothFadeTransition(0.3f, startMenuPanel, settingPanel));
    }

    public void closeSettingButton()
    {
        StartCoroutine(SmoothFadeTransition(0.3f, settingPanel, startMenuPanel));
    }

    public void guideButton()
    {
        StartCoroutine(FadeIn(guideMenuPanel, 0.3f));
    }

    public void guideIngameButton()
    {

    }

    public void CloseGuidePanel()
    {
        StartCoroutine(FadeOut(guideMenuPanel, 0.3f));
    }

    public void closeGuideButtonInGame()
    {

    }

    public void PauseButton()
    {
        StartCoroutine(SmoothFadeTransition(0.3f, ingamePanel, pausePanel));
        Time.timeScale = 0f;
    }

    public void ClosePauseButton()
    {
        StartCoroutine(SmoothFadeTransition(0.3f, pausePanel, ingamePanel));
        Time.timeScale = 1f;
    }

    public void OpenConfirmWindow()
    {
        StartCoroutine(FadeIn(confirmWindow, 0.3f));
    }

    public void CloseConfirmWindow()
    {
        StartCoroutine(FadeOut(confirmWindow, 0.3f));
    }
}
