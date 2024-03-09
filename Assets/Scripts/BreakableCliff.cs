using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableCliff : MonoBehaviour
{
	public GameObject brokenCliff;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
		{
			BreakIt();
		}
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
		{
			BreakIt();
		}
    }

	private void BreakIt()
	{
		
		if (brokenCliff != null) {
			brokenCliff.SetActive(true);
			brokenCliff.GetComponent<BrokenCliff>().DestroyBrokenCliff();
		}

		Destroy(this.gameObject);

	}

}
