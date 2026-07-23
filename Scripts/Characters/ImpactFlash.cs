using System.Collections;
using UnityEngine;

public class ImpactFlash : MonoBehaviour
{
    public void Flash(SpriteRenderer spriteRend, float duration, Color flashColor)
    {
        StartCoroutine(DoFlash(spriteRend, duration, flashColor));
    }

    private IEnumerator DoFlash(SpriteRenderer spriteRend, float duration, Color flashColor)
    {
        Color originalColor = spriteRend.color;
        spriteRend.color = flashColor;

        yield return new WaitForSeconds(duration);

        spriteRend.color = originalColor;
    }
}
