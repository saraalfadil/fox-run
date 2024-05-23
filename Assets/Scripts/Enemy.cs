using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Enemy : MonoBehaviour
{
	public Collider2D coll;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected AudioSource explodeSound;
    public GameObject damageText;
    private GameObject score;
	[SerializeField] protected LayerMask ground;
    public bool isTouchingGround { get { return coll.IsTouchingLayers(ground); } }

    private void OnEnable()
    {
        PlayerController.OnEnemyDefeated += JumpedOn;
    }

	private void OnDisable()
    {   
        PlayerController.OnEnemyDefeated -= JumpedOn;
    }

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        explodeSound = GetComponent<AudioSource>();
		coll = GetComponent<Collider2D>();
    }

    public void JumpedOn(Enemy enemy)
    {
		if (enemy != this)
			return;
			
        score = Instantiate(damageText, transform.position, transform.rotation);

        anim.SetTrigger("explode");
        explodeSound.Play();
        if (rb)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        coll.enabled = false;
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
