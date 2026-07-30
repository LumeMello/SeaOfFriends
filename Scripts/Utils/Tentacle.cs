using Unity.VisualScripting;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    [SerializeField]private int lenght;
    [SerializeField][Range(0f, 1f)] private float targetDist;
    [SerializeField][Range(0f, 1f)] private float smoothSpeed;
    [SerializeField] private float trailSpeed;
    [Space(10)]
    [SerializeField] private float wiggleSpeed;
    [SerializeField] private float wiggleMagnetude;
    [Space(10)]
    [SerializeField]private LineRenderer lineRenderer;
    [SerializeField] private Vector3[] segmentPoses;
    private Vector3[] segmentV;
    [SerializeField] private Transform targetDir;
    [SerializeField] private Transform wiggleDir;

    private void Start()
    {
        lineRenderer.positionCount = lenght;
        segmentPoses = new Vector3[lenght];
        segmentV = new Vector3[lenght];
    }

    public void Update()
    {
        wiggleDir.localRotation = Quaternion.Euler(0,0,Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnetude);

        segmentPoses[0] = targetDir.position;

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i-1] + targetDir.right * targetDist, ref segmentV[i], smoothSpeed + i / trailSpeed);
        }
        lineRenderer.SetPositions(segmentPoses);
    }
}
