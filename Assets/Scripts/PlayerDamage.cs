using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    public bool preventDamage = false;
    private float preventDamageTimer = 0f;
    private float preventDamageDuration = 3f;
    private bool isFlashing = false;
    [SerializeField] private GemCollection gemCollection;
    [SerializeField] private GameObject shield;

    private void Update()
    {
      TrackTemporaryInvincibility();
    }

    private void Start()
    {
        gemCollection = GetComponent<GemCollection>();
    }

    private void TrackTemporaryInvincibility()
    {

        // Keep track of temporary invincibility
        preventDamageTimer += Time.deltaTime;
        if (preventDamageTimer >= preventDamageDuration) {
            preventDamageTimer = 0f;
            preventDamage = false;
        }

    }

    public void DecreaseHealth()
    {

        // Player is temporarily protected from taking damage
        if(preventDamage)
            return;

        PermanentUI.perm.healthStat.text = PermanentUI.perm.health.ToString();
        if(PermanentUI.perm.health <= 0)
        {
            // Show game over
            PermanentUI.perm.GameOver();
        }
        else
        {

            TakeDamage();

        }

    }

    private void TakeDamage()
    {

        if(preventDamage)
            return;

        preventDamage = true;
        playerController.state = PlayerState.hurt;

        // If shield is enabled
        // player doesn't take any damage
        if(shield.activeSelf) {

            // Disable shield
            shield.SetActive(false);

        } else {
            // If player doesn't have any gems, decrease health
            if(PermanentUI.perm.gems < 1)
                PermanentUI.perm.health -= 1;

            // Lose collected gems
            int collectedGems = PermanentUI.perm.gems;
            gemCollection.LoseGems(collectedGems);

            // Reset gems to 0
            PermanentUI.perm.gems = 0;
        }

    }

    public IEnumerator ResumeIdleAfterHurt()
    {
        StartPlayerFlashAnimation();
        yield return new WaitForSeconds(.5f);
        if(Mathf.Abs(playerController.rb.velocity.x) < .1f)
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
            Color flashColor = new Color(originalColor.r, originalColor.g, originalColor.b, newColor);
            playerController.sprite.color = flashColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerController.sprite.color = originalColor;
        isFlashing = false;

    }

}
