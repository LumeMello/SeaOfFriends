using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform targetTransform;
    public Vector2 direction;

    void Update()
    {
        direction = targetTransform.position - transform.position;
        if (Mathf.Abs(direction.x) > 0.001f && Mathf.Abs(direction.y) > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }
}
