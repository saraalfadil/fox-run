using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CherryCollection : MonoBehaviour 
{

    private bool dropCherries = false;
    private float cherriesTimer = 0f;
    private float cherriesDuration = 1f;
    [SerializeField] private GameObject collectable;
    private List<GameObject> collectables = new List<GameObject>();

    private void Update() {

        // Keep track of when cherries are suspended
        cherriesTimer += Time.deltaTime;
        if (cherriesTimer >= cherriesDuration) {
            cherriesTimer = 0f;
            dropCherries = true;
        }

        float radiusIncreaseRate = 0.5f;
        for (int i = collectables.Count - 1; i >= 0; i--)
        {     

            if (collectables[i] == null || collectables[i].GetComponent<Cherry>() == null)
            {
                collectables.RemoveAt(i);
                continue;
            }

            // Increase the radius of the circle gradually
            float angle = i * Mathf.PI * 2 / collectables.Count;
            float x = Mathf.Cos(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);
            float y = Mathf.Sin(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);

            Cherry cherry = collectables[i].GetComponent<Cherry>();

            // Continuously update position
            Vector3 newPos = cherry.transform.position + new Vector3(x, y, 0);
            cherry.transform.position = newPos;

            // Drop cherries to ground
            if(dropCherries) 
                cherry.ChangeCollider();

        }
    }

    // Instantiates several cherry game objects in a semicircle from the player's current position
    public void ScatterCherries(Vector3 player_position)
    {   

        int numberOfCherries = 6;
        
        for (int i = 0; i < numberOfCherries; i++)
        {
            // semicircle (180 degrees)
            float angle = i * Mathf.PI / (numberOfCherries - 1);
            
            float radius = 2.5f;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            
            Vector3 pos = player_position + new Vector3(x, y, 0);

            collectables.Add((GameObject)Instantiate(collectable, pos, Quaternion.identity));
        }  

    }

}