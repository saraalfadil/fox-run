using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class VariableCollisionTile : MonoBehaviour
{
    protected TilemapCollider2D m_Renderer;

    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GetComponent<TilemapCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        // When the player enters its collider space
        if (other.gameObject.tag == "Player")
        {       
            Debug.Log("Player touched");
            
        }
    }

}
