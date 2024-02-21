using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PermanentUI : MonoBehaviour
{
    // Player stats
    public int gems = 0;
    public TextMeshProUGUI gemText;
    public TextMeshProUGUI gemLabel;
    public int health;
    public TextMeshProUGUI healthStat;
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public static PermanentUI perm;

    private bool isGameOver = false;
    [SerializeField] private Canvas gameOver;
    private CanvasGroup gameOverCanvasGroup;
    private Color32 originalColor;
    public bool endLevel;

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
    }

    public void Update()
    {
        // Change label to red
        if (gems == 0)
            gemLabel.color = new Color32(255, 0, 0, 255);
        else
            gemLabel.color = originalColor;

    }

    public void Reset()
    {
        health = 5;
        gems = 0;
        score = 0;
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        // stop main audio
        AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
        allAudios[0].Stop();

        // Play game over audio
        AudioSource gameOverAudio = gameOver.GetComponent<AudioSource>();
        gameOverAudio.Play();

        // Freeze time
        Time.timeScale = 0;

        // Display game over canvas overlay
        gameOverCanvasGroup = gameOver.GetComponent<CanvasGroup>();
        gameOverCanvasGroup.alpha = 1;

        StartCoroutine(GameOverReset());
    }

    private IEnumerator GameOverReset()
    {
        // Resume time
        float pauseEndTime = Time.realtimeSinceStartup + 6;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        Time.timeScale = 1;

        // Reset scene
        SceneManager.LoadScene("Menu");

        // Score, health, gems
        Reset();
    }

}
