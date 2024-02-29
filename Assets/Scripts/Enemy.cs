using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;
    protected AudioSource explodeSound;
    public GameObject damageText;
    private GameObject score;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        explodeSound = GetComponent<AudioSource>();
    }

    public void JumpedOn()
    {

        score = Instantiate(damageText, transform.position, transform.rotation);

        anim.SetTrigger("explode");
        explodeSound.Play();
        if (rb)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        GetComponent<Collider2D>().enabled = false;

    }

    private void Death()
    {
        Destroy(this.gameObject);
        Destroy(score);
    }

	public bool IsFacingLeft()
	{
		return transform.localScale.x == -1;
	}

	public bool IsFacingRight()
	{
		return transform.localScale.x == 1;
	}
	
	public void FaceLeft()
	{
		transform.localScale = new Vector3(-1, 1);
	}

	public void FaceRight()
	{
		transform.localScale = new Vector3(1, 1, 1);
	}

	public bool IsPastLeftCap(Transform leftCap)
	{
		return transform.position.x > leftCap.position.x;
	}

	public bool IsPastRightCap(Transform rightCap)
	{
		return transform.position.x < rightCap.position.x;
	}

}
