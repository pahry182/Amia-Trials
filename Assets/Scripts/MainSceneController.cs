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
    public CanvasGroup guideMenuPanel, guideGamePanel;
    public TextMeshProUGUI defeatText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI checkpointText;
    public GameObject resultText;
    public Button RespawnButton;

    private void Awake()
    {
        backgroundPanel.gameObject.SetActive(true);
        startMenuPanel.gameObject.SetActive(true);
        ingamePanel.gameObject.SetActive(false);
        losePanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.currentWave % GameManager.Instance.bossWave == 0 && GameManager.Instance.currentWave != 0)
        {
            waveText.text = "Wave: " + GameManager.Instance.currentWave + " (Boss)";
        }
        else
        {
            waveText.text = "Wave: " + GameManager.Instance.currentWave;
        }
        lifeText.text = "Life: " + GameManager.Instance.currentLife;
        checkpointText.text = "Checkpoint: " + GameManager.Instance.currentCheckpoint;
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

        yield return new WaitForSeconds(1f);

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
        resultText.SetActive(false);
        CalculateResult();
        StartCoroutine(FadeIn(losePanel, 0.4f));

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(FadeIn(resultText.GetComponent<CanvasGroup>(), 0.4f));
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
        StartCoroutine(FadeIn(startMenuPanel, 1f));
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
        resultText.GetComponent<TextMeshProUGUI>().text = "Current Wave: " + GameManager.Instance.currentWave +
            " Current Life: " + GameManager.Instance.currentLife +
            " Current Checkpoint: " + GameManager.Instance.currentCheckpoint;
    }

    private IEnumerator smoothFadeTransition(float duration, CanvasGroup panel1, CanvasGroup panel2)
    {
        StartCoroutine(FadeOut(panel1, duration));
        yield return new WaitForSeconds(duration);
        StartCoroutine(FadeIn(panel2, duration));
    }

    public void creditsButton()
    {
        StartCoroutine(smoothFadeTransition(0.3f, startMenuPanel, creditsPanel));
    }

    public void closeCreditsButton()
    {
        StartCoroutine(smoothFadeTransition(0.3f, creditsPanel, startMenuPanel));
    }

    public void settingButton()
    {
        StartCoroutine(smoothFadeTransition(0.3f, startMenuPanel, settingPanel));
    }

    public void closeSettingButton()
    {
        StartCoroutine(smoothFadeTransition(0.3f, settingPanel, startMenuPanel));
    }

    public void guideButton()
    {
        StartCoroutine(smoothFadeTransition(0.3f, settingPanel, guideMenuPanel));
    }

    public void guideIngameButton()
    {

    }

    public void closeGuideButtonInMenu()
    {
        StartCoroutine(smoothFadeTransition(0.3f, guideMenuPanel, settingPanel));
    }

    public void closeGuideButtonInGame()
    {

    }

    public void pauseButton()
    {

    }

    public void closePauseButton()
    {

    }
}
