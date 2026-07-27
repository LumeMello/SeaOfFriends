using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public int levelDeathCount = 0;
    [SerializeField] private Text _deathCountText;

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

    public void NewDeath()
    {
        levelDeathCount++;
        if (levelDeathCount < 10)
        {
            _deathCountText.text = "X 0" + levelDeathCount.ToString();
        }
        else
        {
            _deathCountText.text = "X " + levelDeathCount.ToString();
        }
    }
    private IEnumerator ScreenShake(float intensity, float frequency, float duration)
    {
        _caremaNoise.AmplitudeGain = intensity;
        _caremaNoise.FrequencyGain = frequency;
        yield return new WaitForSeconds(duration);
        _caremaNoise.AmplitudeGain = 0;
    }
}
