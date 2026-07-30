using System.Collections;
using UnityEngine;

public class ActiveParticle : MonoBehaviour
{
    private Vector3 lastPosition;
    [SerializeField] private GameObject particle;
    private bool active = false;
    private bool checking = false;
    void Update()
    {
        if (checking == false)
        {
            StartCoroutine(checkMoviment());
        }

        particle.SetActive(active);
    }

    private IEnumerator checkMoviment()
    {
        checking = true;
        lastPosition = transform.position;
        yield return new WaitForSeconds(0.1f);
        if (Mathf.Abs(lastPosition.x - transform.position.x) > 0.0002f && Mathf.Abs(lastPosition.y - transform.position.y) > 0.0002f)
        {
            active = true;
        }
        else
        {
            active = false;
        }

        checking = false;
    }
}
