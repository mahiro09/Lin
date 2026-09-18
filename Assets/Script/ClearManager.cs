using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ClearManager : MonoBehaviour
{
    public static ClearManager Instance;

    [Header("クリアUI")]
    public GameObject clearPanel;
    public RectTransform clearText;
    public TextMeshProUGUI inkResultText;

    [Header("アニメーション")]
    public float animationTime = 0.4f;

    [Header("ステージセレクト")]
    public string stageSelectSceneName = "StageSelect";

    private bool isCleared = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void Clear()
    {
        if (isCleared)
        {
            return;
        }

        isCleared = true;

        Debug.Log("ステージCLEAR!");

        // 使用インク表示
        if (InkManager.Instance != null &&
            inkResultText != null)
        {
            float usedInk =
                InkManager.Instance.GetTotalUsedInk();

            inkResultText.text =
                "使用インク\n" +
                Mathf.CeilToInt(usedInk);
        }

        // クリア画面表示
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }

        // CLEAR文字を小さくしてアニメーション開始
        if (clearText != null)
        {
            clearText.localScale = Vector3.zero;
        }

        StartCoroutine(ClearAnimation());

        Time.timeScale = 0f;
    }

    private IEnumerator ClearAnimation()
    {
        if (clearText == null)
        {
            yield break;
        }

        float timer = 0f;

        while (timer < animationTime)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                timer / animationTime;

            float scale =
                Mathf.Lerp(0f, 1.2f, t);

            clearText.localScale =
                new Vector3(
                    scale,
                    scale,
                    1f
                );

            yield return null;
        }

        timer = 0f;

        float startScale = 1.2f;
        float endScale = 1.0f;

        while (timer < 0.15f)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                timer / 0.15f;

            float scale =
                Mathf.Lerp(
                    startScale,
                    endScale,
                    t
                );

            clearText.localScale =
                new Vector3(
                    scale,
                    scale,
                    1f
                );

            yield return null;
        }
    }

    // =========================
    // NEXT STAGE
    // =========================

    public void NextStage()
    {
        Time.timeScale = 1f;

        int currentScene =
            SceneManager.GetActiveScene().buildIndex;

        int nextScene =
            currentScene + 1;

        Debug.Log(
            "次のステージへ：Build Index "
            + nextScene
        );

        if (nextScene <
            SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning(
                "次のステージがありません。"
            );
        }
    }

    // =========================
    // STAGE SELECT
    // =========================

    public void StageSelect()
    {
        Time.timeScale = 1f;

        Debug.Log(
            "ステージセレクトへ移動"
        );

        SceneManager.LoadScene(
            stageSelectSceneName
        );
    }

    // =========================
    // RETRY
    // =========================

    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}