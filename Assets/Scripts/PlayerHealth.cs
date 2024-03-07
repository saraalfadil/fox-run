using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	public static event Action OnGameOver;
	public static event Action OnLifeLost;
	public static event Action OnGemsLost;
    public bool preventDamage = false;
    private float preventDamageDuration = 3f;
    [SerializeField] private GameObject shield;
    [SerializeField] private GemCollection gemCollection;
    [SerializeField] private PlayerController playerController;
	[SerializeField] private FlashEffect flashEffect;

    private void Start()
    {
        gemCollection = GetComponent<GemCollection>();
    }

    public void DecreaseHealth()
    {
        // Temporarily protected from taking damage
        if (preventDamage)
            return;

		if (PermanentUI.perm.health <= 0) 
		{
			OnGameOver?.Invoke(); 
			return;
		}

       	HandleTakeDamage();
    }

	// If shield is enabled
	// player doesn't take any damage
    private void HandleTakeDamage()
    {
        StartCoroutine(TemporarilyPreventDamage());

       	ShowHurt();

		// Protected by shield
        if (shield.activeSelf)
		{
			// Disable shield
            shield.SetActive(false);
		}
		else 
		{
			TakeDamage();
		}
    }

	private void ShowHurt()
	{
		playerController.isHurt = true;
	}

    private IEnumerator TemporarilyPreventDamage()
    {
        preventDamage = true;

        yield return new WaitForSeconds(preventDamageDuration);

        preventDamage = false;
    }

    private void TakeDamage()
	{
		OnLifeLost?.Invoke();

		LoseGems();
	}

    private void LoseGems()
    {
        // Lose collected gems
        int collectedGemCount = PermanentUI.perm.gems;
        gemCollection.LoseGems(collectedGemCount);

		OnGemsLost?.Invoke();
    }

    public void StartPlayerFlashAnimation()
    {
        if (!flashEffect.isFlashing)
        {
            StartCoroutine(flashEffect.Flash(playerController.sprite));
        }
    }

}
