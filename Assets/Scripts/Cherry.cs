using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cherry : MonoBehaviour
{
    protected Animator anim;
    protected AudioSource cherrySound;
    protected SpriteRenderer sprite;
    private Collider2D coll;
    private Rigidbody2D rb;
    public bool temporary = false;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        cherrySound = GetComponent<AudioSource>();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        if (temporary) 
            StartCoroutine(SelfDestruct());
    }

    public void Collected()
    {
        anim.SetTrigger("collected");
        cherrySound.Play();
        //PermanentUI.perm.cherries += 1;
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
        FadeOut();
        Disappear();
    }
    private void FadeOut() 
    {
        float startAlpha = sprite.color.a;
        float targetAlpha = 0f;
        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);

            elapsedTime += Time.deltaTime;
        }

        // Ensure the final alpha is set
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, targetAlpha);
    }

}
