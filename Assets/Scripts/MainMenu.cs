using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PermanentUI.perm.health = 5;
        PermanentUI.perm.cherries = 0;
        PermanentUI.perm.score = 0;
    }

    public void Quit() 
    {
        Application.Quit();
    }
    public void LoadMainMenu() 
    {
        SceneManager.LoadScene("Menu");
    }
}
