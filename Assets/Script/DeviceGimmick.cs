using UnityEngine;

public class DeviceGimmick : MonoBehaviour
{
    [Header("装置の見た目")]
    public SpriteRenderer spriteRenderer;

    private bool isOn = false;

    public void SetOn(bool value)
    {
        if (isOn == value)
        {
            return;
        }

        isOn = value;

        if (isOn)
        {
            Debug.Log("装置 ON！");
        }
        else
        {
            Debug.Log("装置 OFF！");
        }

        // 見た目を変更
        if (spriteRenderer != null)
        {
            if (isOn)
            {
                spriteRenderer.color = Color.green;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}