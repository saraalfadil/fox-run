using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    private bool isFlashing = false;
    public bool preventDamage = false;
    private float preventDamageDuration = 3f;
    [SerializeField] private GameObject shield;
    [SerializeField] private GemCollection gemCollection;
    [SerializeField] PlayerController playerController;

    private void Update()
    {
        PermanentUI.perm.healthStat.text = PermanentUI.perm.health.ToString();
    }

    private void Start()
    {
        gemCollection = GetComponent<GemCollection>();
    }

    public void DecreaseHealth()
    {

        // Temporarily protected from taking damage
        if (preventDamage)
            return;

        if (PermanentUI.perm.health > 0)
            HandleTakeDamage();
        else
            // Show game over
            PermanentUI.perm.GameOver();

    }

	// If shield is enabled
	// player doesn't take any damage

    private void HandleTakeDamage()
    {

        StartCoroutine(TemporarilyPreventDamage());

        playerController.state = PlayerState.hurt;

        if (shield.activeSelf)
            // Disable shield
            shield.SetActive(false);
		else
            TakeDamage();

    }

    private IEnumerator TemporarilyPreventDamage()
    {
        preventDamage = true;

        yield return new WaitForSeconds(preventDamageDuration);

        preventDamage = false;
    }

    private void TakeDamage()
	{
		LoseLives(1);

		LoseGems();
	}

    private void LoseGems()
    {
        // Lose collected gems
        int collectedGems = PermanentUI.perm.gems;
        gemCollection.LoseGems(collectedGems);

        // Reset gems to 0
        PermanentUI.perm.gems = 0;
    }

	// If player doesn't have any gems, decrease health
    private void LoseLives(int lifeCount)
    {
		if (PermanentUI.perm.gems > 1)
			return;

		PermanentUI.perm.health -= lifeCount;
    }

    public IEnumerator ResumeIdleAfterHurt()
    {
        StartPlayerFlashAnimation();
        yield return new WaitForSeconds(.5f);

        if (Mathf.Abs(playerController.rb.velocity.x) < .1f)
        {
            playerController.state = PlayerState.idle;
        }
    }

    public void StartPlayerFlashAnimation()
    {
        if (!isFlashing)
        {
            StartCoroutine(PlayerFlash());
        }
    }

    private IEnumerator PlayerFlash()
    {
        isFlashing = true;

        float elapsedTime = 0f;
        float flashDuration = 3f;
        float flashSpeed = 5.0f;
        Color originalColor = playerController.sprite.color;

        while (elapsedTime < flashDuration)
        {
            float newColor = Mathf.PingPong(elapsedTime * flashSpeed, 1f);
            UpdatePlayerColor(originalColor, newColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerController.sprite.color = originalColor;
        isFlashing = false;

    }

    private void UpdatePlayerColor(Color oColor, float newColor)
    {
      Color flashColor = new Color(oColor.r, oColor.g, oColor.b, newColor);
      playerController.sprite.color = flashColor;
    }

}
