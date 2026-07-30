using UnityEngine;

public class SegmentedBody : MonoBehaviour
{

    [SerializeField] private int lenght;
    [SerializeField][Range(0f, 1f)] private float targetDist;
    [SerializeField][Range(0f, 1f)] private float smoothSpeed;
    [Space(10)]
    [SerializeField] private float wiggleSpeed;
    [SerializeField] private float wiggleMagnetude;
    [Space(10)]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Vector3[] segmentPoses;
    private Vector3[] segmentV;
    [SerializeField] private Transform targetDir;
    [SerializeField] private Transform wiggleDir;
    [Space(10)]
    [SerializeField] private Transform[] bodyParts;

    private void Start()
    {
        lineRenderer.positionCount = lenght;
        segmentPoses = new Vector3[lenght];
        segmentV = new Vector3[lenght];
    }

    public void Update()
    {
        wiggleDir.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnetude);

        segmentPoses[0] = targetDir.position;

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            Vector3 targetPos = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i - 1]).normalized * targetDist;
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentV[i], smoothSpeed);
            bodyParts[i-1].transform.position = segmentPoses[i];
        }
        lineRenderer.SetPositions(segmentPoses);
    }
}
