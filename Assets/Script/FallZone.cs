using UnityEngine;

public class FallZone : MonoBehaviour
{
    private bool playerFalling = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (playerFalling)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        // =====================================
        // 橋の上ならセーフ
        // =====================================

        if (BridgeManager.Instance != null)
        {
            Vector2 playerPosition = other.transform.position;

            if (BridgeManager.Instance.IsOnBridge(playerPosition))
            {
                return;
            }
        }

        // =====================================
        // 川に落ちた
        // =====================================

        playerFalling = true;

        Debug.Log("川に落ちた！");

        FallAnimation fallAnimation =
            other.GetComponent<FallAnimation>();

        if (fallAnimation != null)
        {
            // 沈むアニメーション開始
            fallAnimation.StartFall();
        }
        else
        {
            // FallAnimationがなかった場合
            Debug.LogWarning(
                "PlayerにFallAnimationがありません！"
            );

            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.GameOver();
            }
        }
    }
}