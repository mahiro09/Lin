using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    public Button stage01Button;
    public Button stage02Button;
    public Button stage03Button;
    public Button stage04Button;

    private void Start()
    {
        UpdateStageButtons();
    }

    private void UpdateStageButtons()
    {
        // STAGE 01
        // 最初からプレイ可能
        stage01Button.interactable = true;

        // STAGE 02
        // STAGE 01をクリアしたら解放
        stage02Button.interactable =
            PlayerPrefs.GetInt("Stage01Clear", 0) == 1;

        // STAGE 03
        // STAGE 02をクリアしたら解放
        stage03Button.interactable =
            PlayerPrefs.GetInt("Stage02Clear", 0) == 1;

        // STAGE 04
        // STAGE 03をクリアしたら解放
        stage04Button.interactable =
            PlayerPrefs.GetInt("Stage03Clear", 0) == 1;
    }

    // =========================
    // STAGE 01
    // =========================
    public void OpenStage01()
    {
        SceneManager.LoadScene("Tutorial");
    }

    // =========================
    // STAGE 02
    // =========================
    public void OpenStage02()
    {
        if (PlayerPrefs.GetInt("Stage01Clear", 0) == 1)
        {
            SceneManager.LoadScene("Switch");
        }
    }

    // =========================
    // STAGE 03
    // =========================
    public void OpenStage03()
    {
        if (PlayerPrefs.GetInt("Stage02Clear", 0) == 1)
        {
            SceneManager.LoadScene("Wind");
        }
    }

    // =========================
    // STAGE 04
    // =========================
    public void OpenStage04()
    {
        if (PlayerPrefs.GetInt("Stage03Clear", 0) == 1)
        {
            SceneManager.LoadScene("Laser");
        }
    }
}