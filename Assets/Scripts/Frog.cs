using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    [SerializeField] private Transform leftCap;
    [SerializeField] private Transform rightCap;
    [SerializeField] private float jumpLength = 10f;
    [SerializeField] private float jumpHeight = 15f;
	private bool facingLeft;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
		AnimationState();
    }

	private void AnimationState()
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
            if (isTouchingGround)
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
			MoveLeft();
		}
		else
		{
			MoveRight();
		}
	}

	private void MoveLeft()
	{
 		// Check if we are past the left cap
		if (IsPastLeftCap(leftCap))
		{
			// Check if sprite is facing right
			if (!IsFacingRight())
			{
				// Face the right direction
				FaceRight();
			}
			JumpLeft();
		}
		else
		{
			facingLeft = false;
		}
	}

	private void MoveRight()
	{
		if (IsPastRightCap(rightCap))
		{
			if (!IsFacingLeft())
			{
				FaceLeft();
			}
			JumpRight();
		}
		else
		{
			facingLeft = true;
		}
	}

	private void JumpLeft()
	{
		if (isTouchingGround)
		{
			rb.velocity = new Vector2(-jumpLength, jumpHeight);
			anim.SetBool("jumping", true);
		}
	}

	private void JumpRight()
	{
		if (isTouchingGround)
		{
			rb.velocity = new Vector2(jumpLength, jumpHeight);
			anim.SetBool("jumping", true);
		}
	}
}
