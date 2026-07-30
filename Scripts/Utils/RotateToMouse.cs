using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private Vector2 direction;
    
    void Update()
    {
        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (Mathf.Abs(direction.x) > 0.001f && Mathf.Abs(direction.y) > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
        
    }
}
