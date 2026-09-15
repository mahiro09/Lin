using System.Collections;
using UnityEngine;

public class FallAnimation : MonoBehaviour
{
    private bool isFalling = false;

    public void StartFall()
    {
        if (isFalling)
        {
            return;
        }

        isFalling = true;

        StartCoroutine(FallCoroutine());
    }

    private IEnumerator FallCoroutine()
    {
        // プレイヤーの移動を止める
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;

        // 沈む時間
        float fallTime = 0.8f;

        // 沈む距離
        float fallDistance = 0.7f;

        // 揺れの強さ
        float shakeAmount = 0.12f;

        // 揺れる速さ
        float shakeSpeed = 25f;

        float timer = 0f;

        while (timer < fallTime)
        {
            timer += Time.deltaTime;

            float t = timer / fallTime;

            // =====================================
            // 下に沈む
            // =====================================

            Vector3 fallPosition = Vector3.Lerp(
                startPosition,
                startPosition + Vector3.down * fallDistance,
                t
            );

            // =====================================
            // 激しく左右に揺れる
            // =====================================

            float shakeX =
                Mathf.Sin(timer * shakeSpeed) * shakeAmount;

            // 少し上下にも揺れる
            float shakeY =
                Mathf.Sin(timer * shakeSpeed * 1.3f)
                * shakeAmount * 0.35f;

            transform.position =
                fallPosition +
                new Vector3(
                    shakeX,
                    shakeY,
                    0f
                );

            // =====================================
            // 徐々に小さくする
            // =====================================

            transform.localScale = Vector3.Lerp(
                startScale,
                startScale * 0.1f,
                t
            );

            yield return null;
        }

        // 完全に沈む
        transform.localScale = Vector3.zero;

        // 少し待つ
        yield return new WaitForSeconds(0.2f);

        // ゲームオーバー
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.GameOver();
        }
    }
}