using UnityEngine;

public class LaserBlocker : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    private void Awake()
    {
        lineRenderer =
            GetComponent<LineRenderer>();

        edgeCollider =
            GetComponent<EdgeCollider2D>();
    }

    public void UpdateCollider()
    {
        if (lineRenderer == null)
        {
            return;
        }

        if (edgeCollider == null)
        {
            return;
        }

        if (lineRenderer.positionCount < 2)
        {
            return;
        }

        Vector2[] points =
            new Vector2[
                lineRenderer.positionCount
            ];

        for (
            int i = 0;
            i < lineRenderer.positionCount;
            i++)
        {
            Vector3 position =
                lineRenderer.GetPosition(i);

            if (lineRenderer.useWorldSpace)
            {
                points[i] =
                    transform.InverseTransformPoint(
                        position
                    );
            }
            else
            {
                points[i] =
                    new Vector2(
                        position.x,
                        position.y
                    );
            }
        }

        edgeCollider.points =
            points;
    }


    // ==================================================
    // ƒŒ[ƒU[‚ªü‚É‹ß‚¢‚©
    // ==================================================

    public bool IsPointNearLine(
        Vector2 point,
        float distance)
    {
        if (lineRenderer == null)
        {
            return false;
        }

        if (lineRenderer.positionCount < 2)
        {
            return false;
        }

        for (
            int i = 0;
            i < lineRenderer.positionCount - 1;
            i++)
        {
            Vector2 start =
                lineRenderer.GetPosition(i);

            Vector2 end =
                lineRenderer.GetPosition(i + 1);

            float lineDistance =
                DistanceToLineSegment(
                    point,
                    start,
                    end
                );

            if (lineDistance <= distance)
            {
                return true;
            }
        }

        return false;
    }


    private float DistanceToLineSegment(
        Vector2 point,
        Vector2 start,
        Vector2 end)
    {
        Vector2 line =
            end - start;

        float lengthSquared =
            line.sqrMagnitude;

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