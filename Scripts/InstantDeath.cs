using UnityEngine;

public class InstantDeath : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.collider.GetComponent<PlayerMovement>();
        if (player != null)
        {
            CameraManager.instance.ChangeTarget(transform);
            player.Die();
        }
    }
}
