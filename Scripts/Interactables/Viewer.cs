using System.Collections;
using UnityEngine;

public class Viewer : MonoBehaviour
{
    [SerializeField] private GameObject seek;
    private Transform seekTransform;
    private Rigidbody2D seekRigidbody2D;
    private Vector2 originalPos;

    public float speed;
    public bool active = false;

    private void Awake()
    {
        seekTransform = seek.GetComponent<Transform>();
        seekRigidbody2D = seek.GetComponent<Rigidbody2D>();

        originalPos = seekTransform.position;
    }

    public void Update()
    {
        if (active)
        {
            float _moveInputX = Input.GetAxisRaw("Horizontal");
            float _moveInputY = Input.GetAxisRaw("Vertical");

            seekRigidbody2D.linearVelocity = new Vector2(_moveInputX * speed, _moveInputY * speed);

            if (Input.GetKeyDown(KeyCode.Space) && CameraManager.instance.vcam.Follow == seekTransform)
            {
                StopViewing();
            }
        }
    }

    public void StartViewing()
    {
        CameraManager.instance.ChangeTarget(seekTransform);
        StartCoroutine(StartViewerCoroutine());
    }

    public void StopViewing()
    {
        active = false;
        seekTransform.position = originalPos;
    }

    private IEnumerator StartViewerCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        active = true;
    }
}
