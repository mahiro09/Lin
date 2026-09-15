using UnityEngine;

public class BridgeLine : MonoBehaviour
{
    [Header("壁判定の余裕")]
    public float extraWidth = 0.1f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // ========================================
    // プレイヤーが線の上にいるか
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

        float lineWidth =
            (lineRenderer.startWidth +
             lineRenderer.endWidth) * 0.5f;

        float judgeWidth =
            (lineWidth * 0.5f) + extraWidth;

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
    // プレイヤーから線までの距離
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
    // 線分との距離を計算
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

        // 始点と終点が同じ場合
        if (lengthSquared == 0f)
        {
            return Vector2.Distance(
                point,
                start
            );
        }

        float t =
            Vector2.Dot(
                point - start,
                line
            ) / lengthSquared;

        t = Mathf.Clamp01(t);

        Vector2 closestPoint =
            start + line * t;

        return Vector2.Distance(
            point,
            closestPoint
        );
    }
}