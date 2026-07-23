using System.Collections;
using UnityEngine;

public class TimeFreeze : MonoBehaviour
{
    public void FreezeTime(float duration)
    {
        StartCoroutine(DoTimeFreeze(duration));
    }

    private IEnumerator DoTimeFreeze(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
}
