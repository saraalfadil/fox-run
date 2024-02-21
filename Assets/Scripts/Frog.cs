using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    private Collider2D coll;
    [SerializeField] private Transform leftCap;
    [SerializeField] private Transform rightCap;
    [SerializeField] private float jumpLength = 10f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private LayerMask ground;
    private bool facingLeft = true;

    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // Transition from jump to fall
        if (anim.GetBool("jumping"))
        {
            if (rb.velocity.y < .1f)
            {
                anim.SetBool("falling", true);
                anim.SetBool("jumping", false);
            }
        }

        // Transition from fall to idle
        if (anim.GetBool("falling"))
        {
            if (coll.IsTouchingLayers(ground))
            {
                anim.SetBool("falling", false);
                anim.SetBool("jumping", false);
            }
        }
    }

    private void Move()
    {

        if (facingLeft)
        {

            // Check if we are past the left cap
            if (transform.position.x > leftCap.position.x)
            {
                // Check if sprite is facing right
                if (transform.localScale.x != 1)
                {
                    // Face the right direction
                    transform.localScale = new Vector3(1, 1, 1);
                }
                // If touching ground, then jump
                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                    anim.SetBool("jumping", true);
                }
            }
            else
            {
                facingLeft = false;
            }
        }

        else
        {
            // Check if we are past the right cap
            if (transform.position.x < rightCap.position.x)
            {
                // Check if sprite is facing left
                if (transform.localScale.x != -1)
                {
                    // Face the left direction
                    transform.localScale = new Vector3(-1, 1);
                }
                // If touching ground, jump
                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                    anim.SetBool("jumping", true);
                }
            }
            else
            {
                facingLeft = true;
            }
        }

    }

    private void Jump()
    {
        rb.velocity = new Vector2(-jumpLength, jumpHeight);
    }

}
