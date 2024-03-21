using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenCliff : MonoBehaviour
{
	[SerializeField] private FadeOutEffect fadeOutEffect;
	public AudioSource brokenCliffSound;

	public void DestroyBrokenCliff()
	{
		brokenCliffSound.Play();
		StartCoroutine(SelfDestruct());
	}

	private IEnumerator SelfDestruct()
	{
		
		yield return new WaitForSeconds(2f);

		foreach (Transform child in transform) 
		{
			SpriteRenderer sprite = child.GetComponent<SpriteRenderer>();
			fadeOutEffect.FadeOut(sprite);
		}
		
		Destroy(this.gameObject);
	}
}
