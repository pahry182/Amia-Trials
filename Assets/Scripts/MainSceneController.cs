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
    public TextMeshProUGUI defeatText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI checkpointText;
    public GameObject resultText;
    public Button RespawnButton;


    private void Update()
    {
        waveText.text = "Wave: " + GameManager.Instance.currentWave;
        lifeText.text = "Life: " + GameManager.Instance.currentLife;
        checkpointText.text = "Checkpoint: " + GameManager.Instance.currentCheckpoint;
    }

    private IEnumerator StartGame()
    {
        StartCoroutine(FadeOut(backgroundPanel, 1f));
        StartCoroutine(FadeOut(startMenuPanel, 0.4f));
        StartCoroutine(FadeIn(ingamePanel, 1f));
        GameManager.Instance.PlayBgm("Battle_Normal");
        StartCoroutine(GameManager.Instance._enemySpawnManager.SpawnEnemy());
        //GameManager.Instance.currentLife = GameManager.Instance.setLife;

        yield return new WaitForSeconds(1f);

        GameManager.Instance.isBattleStarted = true;
    }

    public IEnumerator GameOver()
    {
        GameManager.Instance.isBattleStarted = false;
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
                return "Bego ah";
            case 1:
                return "Maen yang bener tod";
            default:
                return "Jangan ampe kalah tod";
        }
    }

    public void SurrenderGameButton()
    {
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
}
