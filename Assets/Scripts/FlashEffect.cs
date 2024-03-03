using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{	
	public bool isFlashing = false;

    public IEnumerator Flash(SpriteRenderer sprite)
    {
        isFlashing = true;

        float elapsedTime = 0f;
        float flashDuration = 3f;
        float flashSpeed = 5.0f;
        Color originalColor = sprite.color;

        while (elapsedTime < flashDuration)
        {
            float newColor = Mathf.PingPong(elapsedTime * flashSpeed, 1f);
            UpdatePlayerColor(sprite, originalColor, newColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sprite.color = originalColor;
        isFlashing = false;

    }

    private void UpdatePlayerColor(SpriteRenderer sprite, Color originalColor, float newColor)
    {
      	Color flashColor = new Color(
			originalColor.r, 
			originalColor.g, 
			originalColor.b, 
			newColor
		);

      	sprite.color = flashColor;
    }
}
