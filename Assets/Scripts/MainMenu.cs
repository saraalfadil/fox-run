using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PermanentUI.perm.Reset();
    }

    public void Quit() 
    {
        Application.Quit();
    }
    public void LoadMainMenu() 
    {
        SceneManager.LoadScene("Menu");
    }
    
    public void LoadNextLevel() 
    {
        SceneManager.LoadScene("Level2");
    }
}
