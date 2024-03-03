using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : Enemy
{
    [SerializeField] private Transform leftCap;
    [SerializeField] private Transform rightCap;
    [SerializeField] private float jumpLength = 10f;
    [SerializeField] private float jumpHeight = 15f;
    private bool facingRight = true;

    protected override void Start()
    {
        base.Start();
    }

	private void Move()
	{
		if (facingRight)
		{
			MoveRight();
		}
		else
		{
			MoveLeft();
		}
	}

	private void MoveLeft()
	{
		if (IsPastLeftCap(leftCap))
		{
			if (!IsFacingLeft())
			{
				FaceLeft();
			}
			JumpLeft();
		}
		else
		{
			facingRight = true;
		}
	}

	private void MoveRight()
	{
		if (IsPastRightCap(rightCap))
		{
			if (!IsFacingRight())
			{
				FaceRight();
			}
			JumpRight();
		}
		else
		{

			facingRight = false;
		}
	}

	private void JumpLeft()
	{
		if (isTouchingGround)
		{
			rb.velocity = new Vector2(-jumpLength, jumpHeight);
		}
	}
	
	private void JumpRight()
	{
		if (isTouchingGround)
		{
			rb.velocity = new Vector2(jumpLength, jumpHeight);
		}
	}

}
