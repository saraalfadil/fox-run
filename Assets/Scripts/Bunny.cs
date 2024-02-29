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

        if (facingRight)
        {
            // Check if we are past the right cap
            if (IsPastRightCap(rightCap))
            {
                // Check if sprite is facing right
                if (!IsFacingRight())
                {
                  // Face the right direction
                  FaceRight();
                }
                JumpRight();
            }
            else
            {

                facingRight = false;
            }
        }
        else
        {
            // Check if we are past the left cap
            if (IsPastLeftCap(leftCap))
            {
                // Check if sprite is facing left
                if (!IsFacingLeft())
                {
                    // Face the left direction
                    FaceLeft();
                }
                JumpLeft();
            }
            else
            {
                facingRight = true;
            }
        }
    }
	private void JumpLeft()
	{
		// If touching ground, then jump
		if (coll.IsTouchingLayers(ground))
		{
			rb.velocity = new Vector2(-jumpLength, jumpHeight);
		}
	}

	private void JumpRight()
	{
		// If touching ground, then jump
		if (coll.IsTouchingLayers(ground))
		{
			rb.velocity = new Vector2(jumpLength, jumpHeight);
		}
	}

}
