using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GemCollection : MonoBehaviour 
{
    [SerializeField] private GameObject gem;
    private bool dropGems = false;
    private float gemTimer = 0f;
    private float gemDuration = 1f;    
    private int defaultSpawnCount = 6;  
    private List<GameObject> gems = new List<GameObject>();

    private void Update() {

        ScatterGems();

    }

    public void ScatterGems() 
    {
        // Keep track of when gems are suspended
        gemTimer += Time.deltaTime;
        if (gemTimer >= gemDuration) {
            gemTimer = 0f;
            dropGems = true;
        }

        // Continously scatter gems outward from player each frame, them drop at the same time
        float radiusIncreaseRate = 0.5f;
        for (int i = gems.Count - 1; i >= 0; i--)
        {     

            // Remove destroyed gems from gem collection
            if (gems[i] == null || gems[i].GetComponent<Gem>() == null)
            {
                gems.RemoveAt(i);
                continue;
            }

            // Increase the radius of the circle gradually
            float angle = i * Mathf.PI * 2 / gems.Count;
            float x = Mathf.Cos(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);
            float y = Mathf.Sin(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);

            // Continuously update each gem position 
            Gem gem = gems[i].GetComponent<Gem>();
            Vector3 newPos = gem.transform.position + new Vector3(x, y, 0);
            gem.transform.position = newPos;

            // Drop gems to ground
            if (dropGems) 
                gem.ChangeCollider();
            
        }

    }

    public void LoseGems(int collectedGems)
    {
        // Spawn gems
        SpawnGems(collectedGems);
    }

    // Instantiates several gem game objects in a semicircle from the player's current position
    public void SpawnGems(int collectedGems)
    {   
        Vector3 playerPosition = transform.position;

        int spawnCount = collectedGems < defaultSpawnCount && collectedGems > 0 ? collectedGems : defaultSpawnCount;

        // Spawn gems in a semicircle
        for (int i = 1; i < spawnCount; i++)
        {
            // semicircle (180 degrees)
            float radius = 2.5f;
            float angle = i * Mathf.PI / (spawnCount - 1);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            
            Vector3 pos = playerPosition + new Vector3(x, y, 0);
            gems.Add((GameObject)Instantiate(gem, pos, Quaternion.identity));
        }  

    }

}