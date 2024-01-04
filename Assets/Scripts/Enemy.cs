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
        if(rb) 
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
}
