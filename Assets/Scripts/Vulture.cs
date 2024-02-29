using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulture : Enemy
{
    private Collider2D coll;
    [SerializeField] private Transform leftCap;
    [SerializeField] private Transform rightCap;
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
        if (facingLeft)
        {
            // Check if we are past the left cap
            if (IsPastLeftCap())
            {
                // Check if sprite is facing left
                if (!IsFacingLeft())
                {
                    FaceLeft();
                }
                FlyLeft();
            }
            else 
            {
                facingLeft = false;
            }
        }
        else 
        {
            // Check if we are past the right cap
            if (IsPastRightCap())
            {
                // Check if sprite is facing right
                if (!IsFacingRight())
                {
                    FaceRight();
                }
               	FlyRight();
            }
            else 
            {
                facingLeft = true;
            }
        }
    }

	private void FlyLeft()
	{
		rb.velocity = new Vector2(-jumpLength, rb.velocity.y);
	}

	private void FlyRight()
	{
		rb.velocity = new Vector2(jumpLength, rb.velocity.y);
	}
	
}
