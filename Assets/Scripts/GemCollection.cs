using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GemCollection : MonoBehaviour 
{

    private bool dropGems = false;
    private float gemTimer = 0f;
    private float gemDuration = 1f;
    [SerializeField] private GameObject collectable;
    private List<GameObject> collectables = new List<GameObject>();

    private void Update() {

        // Keep track of when gems are suspended
        gemTimer += Time.deltaTime;
        if (gemTimer >= gemDuration) {
            gemTimer = 0f;
            dropGems = true;
        }

        float radiusIncreaseRate = 0.5f;
        for (int i = collectables.Count - 1; i >= 0; i--)
        {     

            if (collectables[i] == null || collectables[i].GetComponent<Gem>() == null)
            {
                collectables.RemoveAt(i);
                continue;
            }

            // Increase the radius of the circle gradually
            float angle = i * Mathf.PI * 2 / collectables.Count;
            float x = Mathf.Cos(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);
            float y = Mathf.Sin(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);

            Gem gem = collectables[i].GetComponent<Gem>();

            // Continuously update position
            Vector3 newPos = gem.transform.position + new Vector3(x, y, 0);
            gem.transform.position = newPos;

            // Drop gems to ground
            if (dropGems) 
                gem.ChangeCollider();

        }
    }

    // Instantiates several gem game objects in a semicircle from the player's current position
    public void ScatterGems(Vector3 player_position, int prevGemCount)
    {   
        
        int numberOfGems = 6;
        if (prevGemCount < 6 && prevGemCount > 0)
        {
            numberOfGems = prevGemCount; //limit gems dropped
        }
        
        for (int i = 1; i < numberOfGems; i++)
        {
            // semicircle (180 degrees)
            float angle = i * Mathf.PI / (numberOfGems - 1);
            
            float radius = 2.5f;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            
            Vector3 pos = player_position + new Vector3(x, y, 0);

            collectables.Add((GameObject)Instantiate(collectable, pos, Quaternion.identity));
        }  

    }

}