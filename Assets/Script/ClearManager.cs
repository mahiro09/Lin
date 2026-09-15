using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ClearManager : MonoBehaviour
{
    public static ClearManager Instance;

    [Header("CLEAR UI")]
    public GameObject clearPanel;

    [Header("CLEARテキスト")]
    public RectTransform clearText;

    [Header("インク結果")]
    public TextMeshProUGUI inkResultText;

    [Header("演出時間")]
    public float animationTime = 0.4f;

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

    // =========================
    // CLEAR
    // =========================

    public void Clear()
    {
        if (isCleared)
        {
            return;
        }

        isCleared = true;

        Debug.Log("ステージCLEAR!");

        // -------------------------
        // 使用インクを取得
        // -------------------------

        if (InkManager.Instance != null &&
            inkResultText != null)
        {
            float usedInk =
                InkManager.Instance.GetTotalUsedInk();

            inkResultText.text =
                "使用インク\n" +
                Mathf.CeilToInt(usedInk);
        }

        // -------------------------
        // CLEARパネル表示
        // -------------------------

        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }

        // -------------------------
        // CLEAR文字を小さくする
        // -------------------------

        if (clearText != null)
        {
            clearText.localScale = Vector3.zero;
        }

        // -------------------------
        // CLEAR演出開始
        // -------------------------

        StartCoroutine(ClearAnimation());

        // -------------------------
        // ゲーム停止
        // -------------------------

        Time.timeScale = 0f;
    }

    // =========================
    // CLEAR演出
    // =========================

    private IEnumerator ClearAnimation()
    {
        if (clearText == null)
        {
            yield break;
        }

        // -------------------------
        // 0 → 1.2
        // -------------------------

        float timer = 0f;

        while (timer < animationTime)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                timer / animationTime;

            float scale =
                Mathf.Lerp(
                    0f,
                    1.2f,
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

        // -------------------------
        // 1.2 → 1.0
        // -------------------------

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
    // 次のステージ
    // =========================

    public void NextStage()
    {
        Time.timeScale = 1f;

        int nextScene =
            SceneManager.GetActiveScene().buildIndex + 1;

        SceneManager.LoadScene(nextScene);
    }

    // =========================
    // リトライ
    // =========================

    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}