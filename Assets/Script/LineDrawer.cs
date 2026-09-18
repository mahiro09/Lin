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

    private float currentLineInk = 0f;

    private List<LineData> lines =
        new List<LineData>();


    // ==================================================
    // 線のデータ
    // ==================================================

    private class LineData
    {
        public GameObject gameObject;
        public LineRenderer lineRenderer;

        // 橋用
        public BridgeLine bridgeLine;

        // レーザー遮断用
        public LaserBlocker laserBlocker;

        // 使用したインク
        public float usedInk;
    }


    // ==================================================
    // Start
    // ==================================================

    private void Start()
    {
        mainCamera = Camera.main;
    }


    // ==================================================
    // Update
    // ==================================================

    private void Update()
    {
        // =========================
        // チュートリアル中の制御
        // =========================

        if (TutorialManager.IsTutorialActive)
        {
            // 線を描けるのはStep 2
            if (TutorialManager.CurrentStep != 2)
            {
                if (currentLine != null)
                {
                    CancelCurrentDrawing();
                }

                return;
            }

            if (!TutorialManager.IsOperationStarted)
            {
                if (currentLine != null)
                {
                    CancelCurrentDrawing();
                }

                return;
            }
        }


        // =========================
        // 線の太さ変更
        // =========================

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


        // =========================
        // 右クリック → 線を削除
        // =========================

        if (Input.GetMouseButtonDown(1))
        {
            DeleteLine();

            return;
        }


        // =========================
        // 左クリック → 線を描く
        // =========================

        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }

        if (Input.GetMouseButton(0))
        {
            ContinueDrawing();
        }

        if (Input.GetMouseButtonUp(0))
        {
            FinishDrawing();
        }
    }


    // ==================================================
    // 線を描き始める
    // ==================================================

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


        // =========================
        // Line Prefabを生成
        // =========================

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


        // =========================
        // 現在の太さを設定
        // =========================

        float width =
            GetCurrentWidth();

        currentLine.startWidth =
            width;

        currentLine.endWidth =
            width;


        // =========================
        // マウス位置取得
        // =========================

        Vector2 mousePosition =
            GetMouseWorldPosition();

        points.Add(mousePosition);


        currentLine.positionCount =
            1;

        currentLine.SetPosition(
            0,
            new Vector3(
                mousePosition.x,
                mousePosition.y,
                0f
            )
        );
    }


    // ==================================================
    // 線を描いている途中
    // ==================================================

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


        // =========================
        // マウス位置
        // =========================

        Vector2 mousePosition =
            GetMouseWorldPosition();

        Vector2 lastPosition =
            points[points.Count - 1];


        float distance =
            Vector2.Distance(
                lastPosition,
                mousePosition
            );


        // =========================
        // 近すぎる場合は追加しない
        // =========================

        if (distance < minDistance)
        {
            return;
        }


        // =========================
        // 太さによるインク倍率
        // =========================

        float multiplier =
            GetInkMultiplier();


        float inkCost =
            distance * multiplier;


        // =========================
        // インク不足
        // =========================

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


        // =========================
        // 点を追加
        // =========================

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
                0f
            )
        );
    }


    // ==================================================
    // 線を描き終わる
    // ==================================================

    private void FinishDrawing()
    {
        if (currentLine == null)
        {
            return;
        }


        // =========================
        // 2点未満なら線として確定しない
        // =========================

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


        // ==================================================
        // LineData作成
        // ==================================================

        LineData data =
            new LineData();


        data.gameObject =
            currentLine.gameObject;


        data.lineRenderer =
            currentLine;


        data.usedInk =
            currentLineInk;


        // ==================================================
        // BridgeLine取得
        // ==================================================

        data.bridgeLine =
            currentLine.GetComponent<BridgeLine>();


        // ==================================================
        // LaserBlocker取得
        // ==================================================

        data.laserBlocker =
            currentLine.GetComponent<LaserBlocker>();


        // ==================================================
        // Edge Collider 2Dを更新
        // ==================================================

        if (data.laserBlocker != null)
        {
            data.laserBlocker.UpdateCollider();

            Debug.Log(
                "LaserBlockerのColliderを更新しました！"
            );
        }
        else
        {
            Debug.LogWarning(
                "Line PrefabにLaserBlockerがありません！"
            );
        }


        // ==================================================
        // 線をリストに登録
        // ==================================================

        lines.Add(data);


        // ==================================================
        // BridgeManagerに登録
        // ==================================================

        if (data.bridgeLine != null)
        {
            if (BridgeManager.Instance != null)
            {
                BridgeManager.Instance.RegisterBridge(
                    data.bridgeLine
                );

                Debug.Log(
                    "線を橋として登録しました！"
                );
            }
            else
            {
                Debug.LogWarning(
                    "BridgeManagerが見つかりません！"
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "Line PrefabにBridgeLineがありません！"
            );
        }


        // ==================================================
        // デバッグ
        // ==================================================

        Debug.Log(
            "線を作成しました。使用インク: "
            + currentLineInk
        );


        // ==================================================
        // チュートリアル
        // ==================================================

        if (TutorialManager.IsTutorialActive &&
            TutorialManager.CurrentStep == 2)
        {
            Debug.Log(
                "チュートリアル：線を描きました！"
            );

            TutorialManager.Instance.OnLineDrawn();
        }


        // ==================================================
        // リセット
        // ==================================================

        currentLine = null;

        points.Clear();

        currentLineInk = 0f;
    }


    // ==================================================
    // 描いている途中の線をキャンセル
    // ==================================================

    private void CancelCurrentDrawing()
    {
        if (currentLine == null)
        {
            return;
        }


        Destroy(
            currentLine.gameObject
        );


        // =========================
        // 使用したインクを全額返す
        // =========================

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


    // ==================================================
    // 線を削除
    // ==================================================

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


        // =========================
        // 一番近い線を探す
        // =========================

        foreach (LineData line in lines)
        {
            if (line == null)
            {
                continue;
            }

            if (line.lineRenderer == null)
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


        // ==================================================
        // インク80%返却
        // ==================================================

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


        // ==================================================
        // BridgeManagerから削除
        // ==================================================

        if (closestLine.bridgeLine != null)
        {
            if (BridgeManager.Instance != null)
            {
                BridgeManager.Instance.UnregisterBridge(
                    closestLine.bridgeLine
                );
            }
        }


        // ==================================================
        // LaserBlockerは線のGameObjectと一緒に削除される
        // ==================================================

        if (closestLine.laserBlocker != null)
        {
            Debug.Log(
                "LaserBlockerを削除しました！"
            );
        }


        // ==================================================
        // GameObject削除
        // ==================================================

        Destroy(
            closestLine.gameObject
        );


        lines.Remove(
            closestLine
        );
    }


    // ==================================================
    // 線までの距離
    // ==================================================

    private float DistanceToLine(
        Vector2 point,
        LineRenderer line)
    {
        float closestDistance =
            float.MaxValue;


        for (
            int i = 0;
            i < line.positionCount - 1;
            i++)
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


    // ==================================================
    // 線の1区間までの距離
    // ==================================================

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


        t =
            Mathf.Clamp01(t);


        Vector2 closestPoint =
            start + line * t;


        return Vector2.Distance(
            point,
            closestPoint
        );
    }


    // ==================================================
    // 現在の線の太さ
    // ==================================================

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


    // ==================================================
    // インク消費倍率
    // ==================================================

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


    // ==================================================
    // マウスのワールド座標
    // ==================================================

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