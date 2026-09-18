using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    [Header("レーザー")]
    public LineRenderer laserLine;

    [Header("レーザーの開始位置")]
    public Transform laserStart;

    [Header("レーザーの最大距離")]
    public float maxDistance = 10f;

    [Header("線を検知する距離")]
    public float blockerDistance = 0.15f;

    [Header("レーザーの当たり判定")]
    public BoxCollider2D laserCollider;


    private void Start()
    {
        // Line Rendererを取得
        if (laserLine == null)
        {
            laserLine = GetComponent<LineRenderer>();
        }

        // Box Colliderを取得
        if (laserCollider == null)
        {
            laserCollider = GetComponent<BoxCollider2D>();
        }

        // 開始位置が設定されていなければ自分自身を使う
        if (laserStart == null)
        {
            laserStart = transform;
        }

        // レーザーを最初に更新
        UpdateLaser();
    }


    private void Update()
    {
        UpdateLaser();
    }


    // ==================================================
    // レーザーを更新
    // ==================================================

    private void UpdateLaser()
    {
        if (laserLine == null)
        {
            return;
        }

        Vector2 startPosition = laserStart.position;

        Vector2 direction = transform.right.normalized;

        float closestDistance = maxDistance;


        // シーン内のレーザー遮断線を探す
        LaserBlocker[] blockers =
            FindObjectsOfType<LaserBlocker>();


        foreach (LaserBlocker blocker in blockers)
        {
            if (blocker == null)
            {
                continue;
            }

            float distance =
                GetLaserBlockDistance(
                    startPosition,
                    direction,
                    blocker
                );

            if (distance >= 0f &&
                distance < closestDistance)
            {
                closestDistance = distance;
            }
        }


        // レーザーの終点
        Vector2 endPosition =
            startPosition +
            direction *
            closestDistance;


        // Line Rendererを更新
        if (laserLine.useWorldSpace)
        {
            laserLine.SetPosition(
                0,
                startPosition
            );

            laserLine.SetPosition(
                1,
                endPosition
            );
        }
        else
        {
            laserLine.SetPosition(
                0,
                transform.InverseTransformPoint(
                    startPosition
                )
            );

            laserLine.SetPosition(
                1,
                transform.InverseTransformPoint(
                    endPosition
                )
            );
        }


        // Box Colliderもレーザーの長さに合わせる
        if (laserCollider != null)
        {
            UpdateLaserCollider(
                closestDistance
            );
        }
    }


    // ==================================================
    // 線がレーザーを遮る位置を探す
    // ==================================================

    private float GetLaserBlockDistance(
        Vector2 start,
        Vector2 direction,
        LaserBlocker blocker
    )
    {
        float checkStep = 0.05f;

        for (
            float distance = 0f;
            distance <= maxDistance;
            distance += checkStep
        )
        {
            Vector2 checkPosition =
                start +
                direction *
                distance;

            if (
                blocker.IsPointNearLine(
                    checkPosition,
                    blockerDistance
                )
            )
            {
                return distance;
            }
        }

        return -1f;
    }


    // ==================================================
    // レーザーの当たり判定を長さに合わせる
    // ==================================================

    private void UpdateLaserCollider(
        float laserLength
    )
    {
        laserCollider.size =
            new Vector2(
                laserLength,
                laserCollider.size.y
            );

        laserCollider.offset =
            new Vector2(
                laserLength * 0.5f,
                laserCollider.offset.y
            );
    }


    // ==================================================
    // プレイヤーがレーザーに触れた
    // ==================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Debug.Log(
            "レーザーに当たった！"
        );

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.GameOver();
        }
    }
}