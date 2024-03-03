using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutEffect : MonoBehaviour
{
    public void FadeOut(SpriteRenderer sprite)
    {
        float startAlpha = sprite.color.a;
        float targetAlpha = 0f;
        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);

            UpdateSpriteAlpha(sprite, newAlpha);

            elapsedTime += Time.deltaTime;
        }

        // Ensure the final alpha is set
        UpdateSpriteAlpha(sprite, targetAlpha);
    }

    private void UpdateSpriteAlpha(SpriteRenderer sprite, float alpha)
    {
		Color color = new Color(
			sprite.color.r, 
			sprite.color.g, 
			sprite.color.b, 
			alpha
		);

      	sprite.color = color;
    }
}
