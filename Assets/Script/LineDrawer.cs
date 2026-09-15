using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [Header("線のPrefab")]
    public GameObject linePrefab;

    [Header("線を追加する間隔")]
    public float minDistance = 0.1f;

    [Header("線の太さ")]
    public float thinWidth = 0.05f;
    public float normalWidth = 0.1f;
    public float thickWidth = 0.15f;

    [Header("線を消せる距離")]
    public float deleteDistance = 0.5f;

    private LineRenderer currentLine;

    private List<Vector2> points =
        new List<Vector2>();

    private Camera mainCamera;

    // 0 = 細い
    // 1 = 普通
    // 2 = 太い
    private int currentThickness = 1;

    // 今描いている線の使用インク
    private float currentLineInk = 0f;

    // 完成した線
    private List<LineData> lines =
        new List<LineData>();

    private class LineData
    {
        public GameObject gameObject;
        public LineRenderer lineRenderer;
        public float usedInk;
    }


    private void Start()
    {
        mainCamera = Camera.main;
    }


    private void Update()
    {
        // =====================================
        // チュートリアル中
        // =====================================

        if (TutorialManager.IsTutorialActive)
        {
            // Step 2「線を描こう！」だけ描画可能
            if (TutorialManager.CurrentStep != 2)
            {
                if (currentLine != null)
                {
                    CancelCurrentDrawing();
                }

                return;
            }

            // Step 2でも「次へ」を押すまでは描画不可
            if (!TutorialManager.IsOperationStarted)
            {
                if (currentLine != null)
                {
                    CancelCurrentDrawing();
                }

                return;
            }
        }


        // =====================================
        // 線の太さ変更
        // =====================================

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentThickness = 0;

            Debug.Log("線の太さ：細い");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentThickness = 1;

            Debug.Log("線の太さ：普通");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentThickness = 2;

            Debug.Log("線の太さ：太い");
        }


        // =====================================
        // 右クリックで線を消す
        // =====================================

        if (Input.GetMouseButtonDown(1))
        {
            DeleteLine();

            return;
        }


        // =====================================
        // 左クリックで描画開始
        // =====================================

        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }


        // =====================================
        // 左クリック中
        // =====================================

        if (Input.GetMouseButton(0))
        {
            ContinueDrawing();
        }


        // =====================================
        // 左クリックを離す
        // =====================================

        if (Input.GetMouseButtonUp(0))
        {
            FinishDrawing();
        }
    }


    // =====================================
    // 描画開始
    // =====================================

    private void StartDrawing()
    {
        if (linePrefab == null)
        {
            Debug.LogError(
                "Line Prefabが設定されていません！"
            );

            return;
        }

        if (InkManager.Instance == null)
        {
            Debug.LogError(
                "InkManagerがありません！"
            );

            return;
        }

        points.Clear();

        currentLineInk = 0f;

        GameObject newLine =
            Instantiate(linePrefab);

        currentLine =
            newLine.GetComponent<LineRenderer>();

        if (currentLine == null)
        {
            Debug.LogError(
                "Line PrefabにLine Rendererがありません！"
            );

            Destroy(newLine);

            return;
        }

        float width =
            GetCurrentWidth();

        currentLine.startWidth =
            width;

        currentLine.endWidth =
            width;

        Vector2 mousePosition =
            GetMouseWorldPosition();

        points.Add(mousePosition);

        currentLine.positionCount = 1;

        currentLine.SetPosition(
            0,
            new Vector3(
                mousePosition.x,
                mousePosition.y,
                0
            )
        );
    }


    // =====================================
    // 描画中
    // =====================================

    private void ContinueDrawing()
    {
        if (currentLine == null)
        {
            return;
        }

        if (points.Count == 0)
        {
            return;
        }

        Vector2 mousePosition =
            GetMouseWorldPosition();

        Vector2 lastPosition =
            points[points.Count - 1];

        float distance =
            Vector2.Distance(
                lastPosition,
                mousePosition
            );

        if (distance >= minDistance)
        {
            float multiplier =
                GetInkMultiplier();

            float inkCost =
                distance * multiplier;

            if (!InkManager.Instance.UseInk(inkCost))
            {
                Debug.Log(
                    "インクが足りません！"
                );

                FinishDrawing();

                return;
            }

            currentLineInk +=
                inkCost;

            points.Add(
                mousePosition
            );

            currentLine.positionCount =
                points.Count;

            currentLine.SetPosition(
                points.Count - 1,
                new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    0
                )
            );
        }
    }


    // =====================================
    // 描画終了
    // =====================================

    private void FinishDrawing()
    {
        if (currentLine == null)
        {
            return;
        }

        // 点が2つ未満なら線として扱わない
        if (points.Count < 2)
        {
            Destroy(
                currentLine.gameObject
            );

            currentLine = null;

            points.Clear();

            currentLineInk = 0f;

            return;
        }

        LineData data =
            new LineData();

        data.gameObject =
            currentLine.gameObject;

        data.lineRenderer =
            currentLine;

        data.usedInk =
            currentLineInk;

        lines.Add(data);

        Debug.Log(
            "線を作成しました。使用インク: "
            + currentLineInk
        );

        // =================================
        // チュートリアル Step 2
        // =================================

        if (TutorialManager.IsTutorialActive &&
            TutorialManager.CurrentStep == 2)
        {
            Debug.Log(
                "チュートリアル：線を描きました！"
            );

            TutorialManager.Instance.OnLineDrawn();
        }

        currentLine = null;

        points.Clear();

        currentLineInk = 0f;
    }


    // =====================================
    // 描きかけの線をキャンセル
    // =====================================

    private void CancelCurrentDrawing()
    {
        if (currentLine == null)
        {
            return;
        }

        Destroy(
            currentLine.gameObject
        );

        // 描きかけで使用したインクを全額返す
        if (currentLineInk > 0f &&
            InkManager.Instance != null)
        {
            InkManager.Instance.AddInk(
                currentLineInk
            );
        }

        currentLine = null;

        points.Clear();

        currentLineInk = 0f;
    }


    // =====================================
    // 線を削除
    // =====================================

    private void DeleteLine()
    {
        if (lines.Count == 0)
        {
            return;
        }

        Vector2 mousePosition =
            GetMouseWorldPosition();

        LineData closestLine = null;

        float closestDistance =
            deleteDistance;

        foreach (LineData line in lines)
        {
            if (line == null ||
                line.lineRenderer == null)
            {
                continue;
            }

            float distance =
                DistanceToLine(
                    mousePosition,
                    line.lineRenderer
                );

            if (distance < closestDistance)
            {
                closestDistance =
                    distance;

                closestLine =
                    line;
            }
        }

        if (closestLine == null)
        {
            return;
        }

        // 80%返却
        float refund =
            closestLine.usedInk * 0.8f;

        if (InkManager.Instance != null)
        {
            InkManager.Instance.AddInk(
                refund
            );
        }

        Debug.Log(
            "線を削除しました。返却インク: "
            + refund
        );

        Destroy(
            closestLine.gameObject
        );

        lines.Remove(
            closestLine
        );
    }


    // =====================================
    // 線までの距離
    // =====================================

    private float DistanceToLine(
        Vector2 point,
        LineRenderer line)
    {
        float closestDistance =
            float.MaxValue;

        for (
            int i = 0;
            i < line.positionCount - 1;
            i++
        )
        {
            Vector2 start =
                line.GetPosition(i);

            Vector2 end =
                line.GetPosition(i + 1);

            float distance =
                DistanceToLineSegment(
                    point,
                    start,
                    end
                );

            if (distance < closestDistance)
            {
                closestDistance =
                    distance;
            }
        }

        return closestDistance;
    }


    // =====================================
    // 線分までの距離
    // =====================================

    private float DistanceToLineSegment(
        Vector2 point,
        Vector2 start,
        Vector2 end)
    {
        Vector2 line =
            end - start;

        float lengthSquared =
            line.sqrMagnitude;

        if (lengthSquared == 0)
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

        t =
            Mathf.Clamp01(t);

        Vector2 closestPoint =
            start + line * t;

        return Vector2.Distance(
            point,
            closestPoint
        );
    }


    // =====================================
    // 現在の線の太さ
    // =====================================

    private float GetCurrentWidth()
    {
        switch (currentThickness)
        {
            case 0:
                return thinWidth;

            case 1:
                return normalWidth;

            case 2:
                return thickWidth;
        }

        return normalWidth;
    }


    // =====================================
    // インク消費倍率
    // =====================================

    private float GetInkMultiplier()
    {
        switch (currentThickness)
        {
            case 0:
                return 1f;

            case 1:
                return 2f;

            case 2:
                return 3f;
        }

        return 2f;
    }


    // =====================================
    // マウスのワールド座標
    // =====================================

    private Vector2 GetMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            mainCamera =
                Camera.main;
        }

        Vector3 mousePosition =
            Input.mousePosition;

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(
                mousePosition
            );

        return new Vector2(
            worldPosition.x,
            worldPosition.y
        );
    }
}