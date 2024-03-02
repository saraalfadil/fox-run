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
		PlayerController.OnLifeCollected += Increasehealth;
		PlayerController.OnEnemyDefeated += IncreaseScore;
    }

    private void OnDisable()
    {   
        PlayerController.OnGemCollected -= IncreaseGemCount;
		PlayerController.OnLifeCollected -= Increasehealth;
		PlayerController.OnEnemyDefeated -= IncreaseScore;
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

        UpdateLabelColor();
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

	private void UpdateLabelColor()
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

    private void IncreaseScore(int defeatEnemyScore)
    {
    	score += defeatEnemyScore;
    }

    private void Increasehealth(int lifeCount)
    {
    	health += lifeCount;
    }
}
