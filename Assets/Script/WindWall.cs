using UnityEngine;

public class WindWall : MonoBehaviour
{
    [Header("•Ç‚Æ‚µ‚Ä”»’è‚·‚é‹——£")]
    public float wallDistance = 0.5f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Rigidbody2D rb =
            other.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            return;
        }

        // ƒV[ƒ“‚É‚ ‚éü‚ğ’T‚·
        LineRenderer[] lines =
            FindObjectsOfType<LineRenderer>();

        foreach (LineRenderer line in lines)
        {
            if (line.positionCount < 2)
            {
                continue;
            }

            for (int i = 0; i < line.positionCount - 1; i++)
            {
                Vector2 start =
                    line.GetPosition(i);

                Vector2 end =
                    line.GetPosition(i + 1);

                float distance =
                    DistanceToLine(
                        other.transform.position,
                        start,
                        end
                    );

                if (distance <= wallDistance)
                {
                    // ‰E•ûŒü‚Ö‚Ì•—‚ğ~‚ß‚é
                    if (rb.velocity.x > 0)
                    {
                        rb.velocity = new Vector2(
                            0,
                            rb.velocity.y
                        );
                    }

                    return;
                }
            }
        }
    }

    private float DistanceToLine(
        Vector2 point,
        Vector2 start,
        Vector2 end)
    {
        Vector2 line =
            end - start;

        float length =
            line.sqrMagnitude;

        if (length == 0)
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
            ) / length;

        t = Mathf.Clamp01(t);

        Vector2 closestPoint =
            start + line * t;

        return Vector2.Distance(
            point,
            closestPoint
        );
    }
}