using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PermanentUI : MonoBehaviour
{
    public int gems = 0;
	public int health = 3;
	public int score = 0;
    public TextMeshProUGUI gemCountText;
    public TextMeshProUGUI gemLabel;
    public TextMeshProUGUI healthCountText;
    public TextMeshProUGUI scoreCountText;
    public static PermanentUI perm;
    private Color32 originalColor;
	private Color32 dangerColor;
    public bool endLevel;

    private void OnEnable()
    {
        PlayerController.OnGemCollected += IncreaseGemCount;
		PlayerController.OnLifeCollected += IncreaseHealth;
		PlayerController.OnEnemyDefeated += IncreaseScore;
		PlayerHealth.OnLifeLost += DecreaseHealth;
		PlayerHealth.OnGemsLost += ResetGemCount;
    }

    private void OnDisable()
    {   
        PlayerController.OnGemCollected -= IncreaseGemCount;
		PlayerController.OnLifeCollected -= IncreaseHealth;
		PlayerController.OnEnemyDefeated -= IncreaseScore;
		PlayerHealth.OnLifeLost -= DecreaseHealth;
		PlayerHealth.OnGemsLost -= ResetGemCount;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Singleton
        if(!perm)
        {
            perm = this;
        }
        else
        {
            Destroy(gameObject);
        }

        originalColor = gemLabel.color;
		dangerColor = new Color32(255, 0, 0, 255);
    }

    public void Update()
    {	
		GetStats();

        GetLabelColor();
    }

    private void GetStats()
    {
        healthCountText.text = health.ToString();
        gemCountText.text = gems.ToString();
        scoreCountText.text = score.ToString();
    }

    public void ResetStats()
    {
        health = 3;
        gems = 0;
        score = 0;
    }

	private void GetLabelColor()
	{
        if (gems == 0)
            gemLabel.color = dangerColor;
        else
            gemLabel.color = originalColor;
	}

	private void IncreaseGemCount(int gemCount)
	{
		gems += gemCount;
	}

	private void ResetGemCount()
	{
		gems = 0;
	}

    private void IncreaseScore(Enemy enemy)
    {
    	score += 100;
    }

    private void IncreaseHealth(int lifeCount)
    {
    	health += lifeCount;
    }

	// If player doesn't have any gems, decrease health
	private void DecreaseHealth()
    {
		if (gems > 1)
			return;

		health -= 1;
    }
}
