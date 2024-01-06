using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PermanentUI : MonoBehaviour
{
    // Player stats
    public int cherries = 0;
    public TextMeshProUGUI cherryText;
    public int health;
    public TextMeshProUGUI healthStat;
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public static PermanentUI perm;
    
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
    }

    public void Reset()
    {
        health = 5;
        cherries = 0;
        score = 0;
    }
    
}
