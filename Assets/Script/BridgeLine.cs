using UnityEngine;

public class BridgeLine : MonoBehaviour
{
    [Header("橋の判定の余裕")]
    public float extraWidth = 0.5f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // ========================================
    // プレイヤーが橋の上にいるか判定
    // ========================================
    public bool IsPointOnBridge(Vector2 point)
    {
        if (lineRenderer == null)
        {
            return false;
        }

        if (lineRenderer.positionCount < 2)
        {
            return false;
        }

        // 線の太さ
        float lineWidth =
            (lineRenderer.startWidth +
             lineRenderer.endWidth) * 0.5f;

        // 判定範囲を広めにする
        float judgeWidth =
            (lineWidth * 0.5f)
            + extraWidth
            + 0.2f;

        // 線の各区間をチェック
        for (int i = 0;
             i < lineRenderer.positionCount - 1;
             i++)
        {
            Vector2 start =
                lineRenderer.GetPosition(i);

            Vector2 end =
                lineRenderer.GetPosition(i + 1);

            float distance =
                DistanceToLineSegment(
                    point,
                    start,
                    end
                );

            if (distance <= judgeWidth)
            {
                return true;
            }
        }

        return false;
    }

    // ========================================
    // 線からプレイヤーまでの距離
    // 風の壁判定でも使用
    // ========================================
    public float GetDistanceToLine(Vector2 point)
    {
        if (lineRenderer == null)
        {
            return float.MaxValue;
        }

        if (lineRenderer.positionCount < 2)
        {
            return float.MaxValue;
        }

        float closestDistance =
            float.MaxValue;

        for (int i = 0;
             i < lineRenderer.positionCount - 1;
             i++)
        {
            Vector2 start =
                lineRenderer.GetPosition(i);

            Vector2 end =
                lineRenderer.GetPosition(i + 1);

            float distance =
                DistanceToLineSegment(
                    point,
                    start,
                    end
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        return closestDistance;
    }

    // ========================================
    // 線分と点の距離を計算
    // ========================================
    private float DistanceToLineSegment(
        Vector2 point,
        Vector2 start,
        Vector2 end)
    {
        Vector2 line =
            end - start;

        float lengthSquared =
            line.sqrMagnitude;

        // 線の長さが0の場合
        if (lengthSquared == 0f)
        {
            return Vector2.Distance(
                point,
                start
            );
        }

        // 点が線分のどこに一番近いか
        float t =
            Vector2.Dot(
                point - start,
                line
            ) / lengthSquared;

        // 線分の範囲内に限定
        t = Mathf.Clamp01(t);

        Vector2 closestPoint =
            start + line * t;

        return Vector2.Distance(
            point,
            closestPoint
        );
    }
}