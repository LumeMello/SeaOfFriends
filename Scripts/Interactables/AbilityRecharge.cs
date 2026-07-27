using UnityEngine;

public class AbilityRecharge : MonoBehaviour
{
    private PlayerMovement _playerMovement = null;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            if (_playerMovement != null)
            {
                _playerMovement._canDash = true;
                Destroy(this.gameObject);
            }
        }
    }
}
