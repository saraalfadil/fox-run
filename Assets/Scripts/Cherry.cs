using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cherry : MonoBehaviour
{
    protected Animator anim;
    protected AudioSource cherrySound;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        cherrySound = GetComponent<AudioSource>();
    }

    public void Collected()
    {
        anim.SetTrigger("collected");
        cherrySound.Play();
        PermanentUI.perm.cherries += 1;
    }

    private void Disappear()
    {
        Destroy(this.gameObject);
    }
}
