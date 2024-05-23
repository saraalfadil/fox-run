using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Fish : Enemy
{

    [SerializeField] private Transform upCap;
    [SerializeField] private Transform downCap;
    [SerializeField] private float swimLength = 2f;
	private bool facingUp = true;

    protected override void Start() 
    {
        base.Start();
    }
	
	private void Move()
	{
		if (facingUp)
		{
			MoveUp();
		}
		else
		{
			MoveDown();
		}
	}
	
	private void MoveDown()
	{
		if (IsPastDownCap(downCap))
		{
			// Check if sprite is facing down
			if (!IsFacingDown())
			{
				FaceDown();
			}

			SwimDown();
		}
		else 
		{
			facingUp = true;
		}
	}

	private void MoveUp()
	{
		if (IsPastUpCap(upCap))
		{
			// Check if sprite is facing up
			if (!IsFacingUp())
			{
				FaceUp();
			}
			SwimUp();
		}
		else 
		{
			facingUp = false;
		}
	}

	private void SwimUp()
	{
		rb.velocity = new UnityEngine.Vector2(rb.velocity.x, swimLength);
	}

	private void SwimDown()
	{
		rb.velocity = new UnityEngine.Vector2(rb.velocity.x, -swimLength);
	}

	public bool IsFacingUp()
	{
		return transform.localRotation.z == 0;
	}

	public bool IsFacingDown()
	{
		return transform.localRotation.z == -180;
	}

	public bool IsPastUpCap(Transform upCap)
	{
		return transform.localPosition.y < upCap.localPosition.y;
	}

	public bool IsPastDownCap(Transform downCap)
	{
		return transform.localPosition.y > downCap.localPosition.y;
	}

	public void FaceUp()
	{
		transform.localRotation = UnityEngine.Quaternion.Euler(0, 0, 0);
	}

	public void FaceDown()
	{
		transform.localRotation = UnityEngine.Quaternion.Euler(0, 0, 180);
	}

}
