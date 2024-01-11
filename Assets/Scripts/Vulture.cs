using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulture : Enemy
{
    private Collider2D coll;

    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    [SerializeField] private float jumpLength = 2f;
    [SerializeField] private LayerMask ground;
    private bool facingLeft = true;

    protected override void Start() 
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    private void Move() 
    {
        if(facingLeft)
        {

            // Check if we are past the left cap
            if(transform.localPosition.x > leftCap)
            {
                // Check if sprite is facing left
                if(transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }
                rb.velocity = new Vector2(-jumpLength, rb.velocity.y);
            }
            else 
            {
                facingLeft = false;
            }

        }

        else 
        {
            
            // Check if we are past the right cap
            if(transform.localPosition.x < rightCap)
            {
                // Check if sprite is facing left
                if(transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    
                }
                rb.velocity = new Vector2(jumpLength, rb.velocity.y);

            }
            else 
            {
                facingLeft = true;
            }
        }
 
    }

}
