using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    protected Animator anim;
    protected AudioSource gemSound;
    protected SpriteRenderer sprite;
    private Collider2D coll;
    private Rigidbody2D rb;
    public bool temporary = false;
	[SerializeField] private FadeOutEffect fadeOutEffect;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        gemSound = GetComponent<AudioSource>();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        if (temporary)
            StartCoroutine(SelfDestruct());
		
    }

    public void Collected()
    {
        anim.SetTrigger("collected");
        gemSound.Play();
    }

    private void Disappear()
    {
        Destroy(this.gameObject);
    }

    public void ChangeCollider()
    {
        coll.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(3f);
        fadeOutEffect.FadeOut(sprite);
        Disappear();
    }


}
