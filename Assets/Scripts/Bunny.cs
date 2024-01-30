using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : Enemy
{
    private Collider2D coll;
    [SerializeField] private Transform leftCap;
    [SerializeField] private Transform rightCap;
    [SerializeField] private float jumpLength = 10f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private LayerMask ground;
    private bool facingRight = true;

    protected override void Start() 
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    private void Move() 
    {

        if(facingRight)
        {

            // Check if we are past the right cap
            if(transform.position.x < rightCap.position.x)
            {
                // Check if sprite is facing right 
                if(transform.localScale.x != 1)
                {   
                    // Face the right direction
                    transform.localScale = new Vector3(1, 1, 1);
                }
                // If touching ground, then jump
                if(coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                } 
            }
            else 
            {

                facingRight = false;
            }
        }

        else 
        {

            // Check if we are past the left cap
            if(transform.position.x > leftCap.position.x)
            {

                // Check if sprite is facing left 
                if(transform.localScale.x != -1)
                {   
                    // Face the left direction
                    transform.localScale = new Vector3(-1, 1);
                }
                // If touching ground, then jump
                if(coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                }
            }
            else 
            {
                facingRight = true;
            }

        }
 
    }

}