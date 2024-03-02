using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    private bool isGameOver = false;
    [SerializeField] private Canvas gameOver;
    private CanvasGroup gameOverCanvasGroup;

    private void OnEnable()
    {
        PlayerHealth.OnGameOver += HandleEndGame;
    }

    private void OnDisable()
    {   
        PlayerHealth.OnGameOver -= HandleEndGame;
    }

    public void HandleEndGame()
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
        PermanentUI.perm.ResetStats();
	}

}
