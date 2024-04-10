using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Fish : Enemy
{

	/*[SerializeField] private float speed;
    [SerializeField] private int startingPoint;
    [SerializeField] private Transform[] points;
    private int i;

    private void Start()
    {
        transform.position = points[startingPoint].position;
    }

    private void Update()
    {
        // check the distance of the platform and the point
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;

            // check if the platform was on the last point after the index increase
            if(i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }*/

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
		Debug.Log("Move!!");
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
		Debug.Log("MoveDown!!");
		Debug.Log("UP CAP Y POSITION: " + upCap.localPosition.y);
		Debug.Log("FISH Y POSITION: " + transform.localPosition.y);
		if (IsPastDownCap(downCap))
		{
			Debug.Log("IsPastUpCap!!");

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
		Debug.Log("MoveUp!!");
		Debug.Log("DOWN CAP Y POSITION: " + downCap.localPosition.y);
		Debug.Log("FISH Y POSITION: " + transform.localPosition.y);
		if (IsPastUpCap(upCap))
		{
			Debug.Log("IsPastDownCap!!");
			
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
		Debug.Log("SWIM UP");
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
