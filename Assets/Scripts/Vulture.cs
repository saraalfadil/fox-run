using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulture : Enemy
{
    [SerializeField] private Transform leftCap;
    [SerializeField] private Transform rightCap;
    [SerializeField] private float flyLength = 2f;
    private bool facingLeft = true;

    protected override void Start() 
    {
        base.Start();
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
		if (IsPastLeftCap(leftCap))
		{
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

	private void MoveRight()
	{
		if (IsPastRightCap(rightCap))
		{
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

	private void FlyLeft()
	{
		rb.velocity = new Vector2(-flyLength, rb.velocity.y);
	}

	private void FlyRight()
	{
		rb.velocity = new Vector2(flyLength, rb.velocity.y);
	}
	
}
