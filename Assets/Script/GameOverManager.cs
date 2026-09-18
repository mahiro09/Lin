using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;

    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        // チュートリアルを非表示
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        // Game Over画面を表示
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;

        Debug.Log("GAME OVER！");
    }

    public void Retry()
    {
        Debug.Log("リスタートボタンが押されました");

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}