using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPowerup : MonoBehaviour
{
    protected Animator anim;

    protected AudioSource[] allAudios;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        allAudios = GetComponents<AudioSource>();
    }

    public void Collected()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        anim.SetTrigger("collected");
        allAudios[0].Play(); // explode sound
    }

    private void Disappear()
    {
        Destroy(this.gameObject);
    }

    private void ShowBroken()
    {
        allAudios[1].Play(); // cherry sound
        anim.SetTrigger("broken");
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
