using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    public CinemachineCamera vcam;
    public CinemachineBasicMultiChannelPerlin _caremaNoise;
    
    public void ChangeTarget(Transform newTarget)
    {
        vcam.Follow = newTarget;
    }

    public void BasicScreenShake(float intensity, float frequency, float duration)
    {
        StartCoroutine(ScreenShake(intensity, frequency,duration));
    }

    private IEnumerator ScreenShake(float intensity, float frequency, float duration)
    {
        _caremaNoise.AmplitudeGain = intensity;
        _caremaNoise.FrequencyGain = frequency;
        yield return new WaitForSeconds(duration);
        _caremaNoise.AmplitudeGain = 0;
    }
}
