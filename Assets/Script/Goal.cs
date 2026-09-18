using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private bool isCleared = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player以外は無視
        if (!other.CompareTag("Player")) return;

        // すでにクリアしていたら無視
        if (isCleared) return;

        // =========================
        // チュートリアル中
        // =========================
        if (TutorialManager.IsTutorialActive)
        {
            if (TutorialManager.CurrentStep == 4)
            {
                if (!TutorialManager.IsOperationStarted) return;

                Debug.Log("チュートリアル：橋を渡ってゴールしました！");

                // STAGE 01クリアを保存
                PlayerPrefs.SetInt("Stage01Clear", 1);
                PlayerPrefs.Save();

                Debug.Log("STAGE 01クリアを保存しました！");

                isCleared = true;

                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.OnGoalReached();
                }

                // Tutorial → Switch
                Invoke(nameof(GoToSwitch), 1.0f);

                return;
            }

            return;
        }

        // =========================
        // 通常ステージ
        // =========================

        isCleared = true;

        Debug.Log("CLEAR!");

        // -------------------------
        // Switchクリア
        // -------------------------
        string currentScene =
            SceneManager.GetActiveScene().name;

        if (currentScene == "Switch")
        {
            PlayerPrefs.SetInt("Stage02Clear", 1);
            PlayerPrefs.Save();

            Debug.Log("STAGE 02クリアを保存しました！");
        }

        // クリア画面を表示
        if (ClearManager.Instance != null)
        {
            ClearManager.Instance.Clear();
        }
    }

    // =========================
    // Tutorial → Switch
    // =========================

    private void GoToSwitch()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Switch");
    }
}