using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{

    [SerializeField] private string sceneToLoad;
    [SerializeField] private Canvas endLevel;
    private CanvasGroup endLevelCanvasGroup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //SceneManager.LoadScene(sceneToLoad);

            ShowEndLevel();

        }
    }

    private void ShowEndLevel()
    {
        // stop main audio
        AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
        allAudios[0].Stop();

        // Play end level audio
        AudioSource endLevelAudio = endLevel.GetComponent<AudioSource>();
        endLevelAudio.Play();

        // Display end level canvas overlay
        endLevelCanvasGroup = endLevel.GetComponent<CanvasGroup>();
        endLevelCanvasGroup.alpha = 1;

        StartCoroutine(StopInput());

    }

    private IEnumerator StopInput()
    {
        yield return new WaitForSeconds(.8f);

        PermanentUI.perm.endLevel = true;
    }

}
