using UnityEngine;
using Cainos.PixelArtTopDown_Basic;

public class WindZone : MonoBehaviour
{
    [Header("•—‚Ì‹­‚³")]
    public float windForce = 3f;

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
            Debug.LogWarning(
                "Player‚ÉTopDownCharacterController‚ª‚ ‚è‚Ü‚¹‚ñ"
            );

            return;
        }

        Vector2 windVelocity =
            windDirection.normalized *
            windForce;

        player.SetWindVelocity(
            windVelocity
        );
    }

    private void OnTriggerExit2D(Collider2D other)
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

        player.ClearWindVelocity();
    }
}