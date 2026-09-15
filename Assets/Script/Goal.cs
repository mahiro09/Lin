using UnityEngine;

public class Goal : MonoBehaviour
{
    private bool isCleared = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // =====================================
        // プレイヤー以外は無視
        // =====================================

        if (!other.CompareTag("Player"))
        {
            return;
        }

        // =====================================
        // すでに処理済みなら無視
        // =====================================

        if (isCleared)
        {
            return;
        }

        // =====================================
        // チュートリアル中
        // =====================================

        if (TutorialManager.IsTutorialActive)
        {
            // ---------------------------------
            // Step 4
            // 「橋を渡ろう！」の状態
            // ---------------------------------

            if (TutorialManager.CurrentStep == 4)
            {
                // 「次へ」を押して操作開始しているか確認
                if (!TutorialManager.IsOperationStarted)
                {
                    return;
                }

                Debug.Log(
                    "チュートリアル：橋を渡ってゴールしました！"
                );

                isCleared = true;

                // チュートリアルを次のステップへ
                TutorialManager.Instance.OnGoalReached();

                return;
            }

            // ---------------------------------
            // Step 4以外ではゴールしない
            // ---------------------------------

            return;
        }

        // =====================================
        // 通常ゲーム
        // =====================================

        isCleared = true;

        Debug.Log("CLEAR!");

        if (ClearManager.Instance != null)
        {
            ClearManager.Instance.Clear();
        }
    }
}