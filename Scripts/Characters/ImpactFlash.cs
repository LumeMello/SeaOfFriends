using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class ImpactFlash : MonoBehaviour
{
    public static ImpactFlash instance;
    public bool bright = false;

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

    public void Flash(SpriteRenderer spriteRend, float duration, Color flashColor)
    {
        bright = true;
        StartCoroutine(DoFlash(spriteRend, duration, flashColor));
    }

    private IEnumerator DoFlash(SpriteRenderer spriteRend, float duration, Color flashColor)
    {
        Color originalColor = spriteRend.color;
        spriteRend.color = flashColor;

        yield return new WaitForSeconds(duration);

        spriteRend.color = originalColor;
        bright = false;
    }
}
