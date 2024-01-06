using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fall : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PermanentUI.perm.Reset();
            
            // better for multiple levels
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
