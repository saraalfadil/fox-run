using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PermanentUI : MonoBehaviour
{
    public int gems = 0;
	public int health = 3;
	public int score = 0;
    public TextMeshProUGUI gemText;
    public TextMeshProUGUI gemLabel;
    public TextMeshProUGUI healthStat;
    public TextMeshProUGUI scoreText;
    public static PermanentUI perm;
    private bool isGameOver = false;
    [SerializeField] private Canvas gameOver;
    private CanvasGroup gameOverCanvasGroup;
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
        healthStat.text = health.ToString();
        gemText.text = gems.ToString();
        scoreText.text = score.ToString();
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

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

		StartGameOverAudio();

        // Freeze time
        Time.timeScale = 0;

        ShowGameOverScreen();

        StartCoroutine(GameOverReset());
    }

	private void StartGameOverAudio()
	{
		// stop main audio
        AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
        allAudios[0].Stop();

        // Play game over audio
        AudioSource gameOverAudio = gameOver.GetComponent<AudioSource>();
        gameOverAudio.Play();
	}

	private void ShowGameOverScreen()
	{
		// Display game over canvas overlay
        gameOverCanvasGroup = gameOver.GetComponent<CanvasGroup>();
        gameOverCanvasGroup.alpha = 1;
	}

    private IEnumerator GameOverReset()
    {
        // Resume time
        float pauseEndTime = Time.realtimeSinceStartup + 6;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }

        ResetGame();
    }

	private void ResetGame()
	{
		Time.timeScale = 1;

        // Reset scene
        SceneManager.LoadScene("Menu");

        // Score, health, gems
        ResetStats();
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
