using UnityEngine;

public class SeekTarget : MonoBehaviour
{
    [SerializeField] private float seekSpeed;
    [SerializeField] private Transform targetTransform;

    void Update()
    {
        Vector2 TargetPos = targetTransform.position;
        transform.position = Vector2.MoveTowards(transform.position, TargetPos, seekSpeed * Time.deltaTime);
    }
}
