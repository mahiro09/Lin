using UnityEngine;
using TMPro;

public class InkManager : MonoBehaviour
{
    public static InkManager Instance;

    [Header("最大インク")]
    public float maxInk = 100f;

    [Header("現在のインク")]
    public float currentInk = 100f;

    [Header("インク表示")]
    public TextMeshProUGUI inkText;

    // このステージで使用したインクの合計
    private float totalUsedInk = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // ステージ開始時に最大まで戻す
        currentInk = maxInk;

        // 使用インクをリセット
        totalUsedInk = 0f;

        UpdateInkText();
    }

    // =========================
    // インクを使う
    // =========================

    public bool UseInk(float amount)
    {
        if (currentInk < amount)
        {
            return false;
        }

        currentInk -= amount;

        // 今回使用したインクを記録
        totalUsedInk += amount;

        UpdateInkText();

        return true;
    }

    // =========================
    // インクを返す
    // =========================

    public void AddInk(float amount)
    {
        currentInk += amount;

        // 最大インクを超えない
        currentInk = Mathf.Min(
            currentInk,
            maxInk
        );

        UpdateInkText();
    }

    // =========================
    // 現在のインクを取得
    // =========================

    public float GetCurrentInk()
    {
        return currentInk;
    }

    // =========================
    // このステージで使用した
    // インクを取得
    // =========================

    public float GetTotalUsedInk()
    {
        return totalUsedInk;
    }

    // =========================
    // インク表示更新
    // =========================

    private void UpdateInkText()
    {
        if (inkText != null)
        {
            inkText.text =
                Mathf.CeilToInt(currentInk)
                + " / "
                + Mathf.CeilToInt(maxInk);
        }
    }
}