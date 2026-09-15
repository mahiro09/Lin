using UnityEngine;

public class DoorGimmick : MonoBehaviour
{
    [Header("閉じたドア")]
    public GameObject closedDoor;

    [Header("開いたドア")]
    public GameObject openedDoor;

    private bool isOpen = false;

    private void Start()
    {
        // 最初は閉じた状態
        CloseDoor();
    }

    public void OpenDoor()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;

        if (closedDoor != null)
        {
            closedDoor.SetActive(false);
        }

        if (openedDoor != null)
        {
            openedDoor.SetActive(true);
        }

        Debug.Log("ドアが開いた！");
    }

    public void CloseDoor()
    {
        isOpen = false;

        if (closedDoor != null)
        {
            closedDoor.SetActive(true);
        }

        if (openedDoor != null)
        {
            openedDoor.SetActive(false);
        }

        Debug.Log("ドアが閉じた！");
    }
}