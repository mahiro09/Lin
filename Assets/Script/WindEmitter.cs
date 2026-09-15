using UnityEngine;
using Cainos.PixelArtTopDown_Basic;

public class WindEmitter : MonoBehaviour
{
    [Header("•—‚Ì‹­‚³")]
    public float windSpeed = 3f;

    [Header("•—‚Ì•ûŒü")]
    public Vector2 windDirection = Vector2.right;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        TopDownCharacterController player =
            other.GetComponent<TopDownCharacterController>();

        if (player == null)
        {
            return;
        }

        Vector2 windVelocity =
            windDirection.normalized * windSpeed;

        player.SetWindVelocity(windVelocity);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        TopDownCharacterController player =
            other.GetComponent<TopDownCharacterController>();

        if (player != null)
        {
            player.ClearWindVelocity();
        }
    }
}