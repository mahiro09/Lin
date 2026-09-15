using UnityEngine;

public class SwitchGimmick : MonoBehaviour
{
    [Header("接続するDevice")]
    public DeviceGimmick targetDevice;

    [Header("開けるドア")]
    public DoorGimmick targetDoor;

    [Header("接続判定の範囲")]
    public float connectionDistance = 0.5f;

    private bool isConnected = false;

    private void Update()
    {
        CheckConnection();
    }

    // =====================================
    // SwitchとDeviceが線でつながっているか確認
    // =====================================

    private void CheckConnection()
    {
        if (targetDevice == null)
        {
            return;
        }

        bool connected = false;

        // シーン内にあるLineRendererを取得
        LineRenderer[] lines =
            FindObjectsOfType<LineRenderer>();

        Vector2 switchPosition =
            transform.position;

        Vector2 devicePosition =
            targetDevice.transform.position;

        foreach (LineRenderer line in lines)
        {
            if (line == null)
            {
                continue;
            }

            // 点が2つ未満なら無視
            if (line.positionCount < 2)
            {
                continue;
            }

            // 線の最初の位置
            Vector2 startPosition;

            // 線の最後の位置
            Vector2 endPosition;

            if (line.useWorldSpace)
            {
                startPosition =
                    line.GetPosition(0);

                endPosition =
                    line.GetPosition(
                        line.positionCount - 1
                    );
            }
            else
            {
                startPosition =
                    line.transform.TransformPoint(
                        line.GetPosition(0)
                    );

                endPosition =
                    line.transform.TransformPoint(
                        line.GetPosition(
                            line.positionCount - 1
                        )
                    );
            }

            // =====================================
            // Switch → Device
            // =====================================

            bool normalConnection =
                Vector2.Distance(
                    startPosition,
                    switchPosition
                ) <= connectionDistance
                &&
                Vector2.Distance(
                    endPosition,
                    devicePosition
                ) <= connectionDistance;

            // =====================================
            // Device → Switch
            // =====================================

            bool reverseConnection =
                Vector2.Distance(
                    startPosition,
                    devicePosition
                ) <= connectionDistance
                &&
                Vector2.Distance(
                    endPosition,
                    switchPosition
                ) <= connectionDistance;

            if (normalConnection || reverseConnection)
            {
                connected = true;
                break;
            }
        }

        // =====================================
        // 状態が変わった時だけ処理
        // =====================================

        if (connected != isConnected)
        {
            isConnected = connected;

            if (isConnected)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }
    }

    // =====================================
    // ON
    // =====================================

    private void TurnOn()
    {
        Debug.Log("Switch ON！");

        // DeviceをON
        if (targetDevice != null)
        {
            targetDevice.SetOn(true);
        }

        // ドアを開く
        if (targetDoor != null)
        {
            targetDoor.OpenDoor();
        }
    }

    // =====================================
    // OFF
    // =====================================

    private void TurnOff()
    {
        Debug.Log("Switch OFF！");

        // DeviceをOFF
        if (targetDevice != null)
        {
            targetDevice.SetOn(false);
        }

        // ドアを閉じる
        if (targetDoor != null)
        {
            targetDoor.CloseDoor();
        }
    }
}