using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GemCollection: MonoBehaviour
{
    private bool dropGems = false;
    private int defaultGemCount = 6;
    float radiusIncreaseRate = 0.5f;
	private float gemSuspensionDuration = 1f;
    [SerializeField] private GameObject gem;
    private List<GameObject> gems = new List<GameObject>();

    private void Start()
    {
      StartCoroutine(SuspendGems());
    }

    private void Update()
    {
      ScatterGems();
    }

    // Continously scatter gems outward from player each frame
	// then, drop at the same time
    public void ScatterGems()
    {

        for (int i = gems.Count - 1; i >= 0; i--)
        {

            // Remove destroyed gems from gem collection
            if (gems[i] == null)
            {
                gems.RemoveAt(i);
                continue;
            }

            Gem gem = gems[i].GetComponent<Gem>();

            ScatterGem(i, gem);

        }

    }

    private void ScatterGem(int gemIndex, Gem gem)
    {
      // Update gem position in circular shape
      UpdateGemPosition(gemIndex, gem);

      // Drop gems to ground
      if (dropGems)
          gem.ChangeCollider();
    }


    private void UpdateGemPosition(int gemIndex, Gem gem)
    {
      // Increase the radius of the circle gradually
      float angle = gemIndex * Mathf.PI * 2 / gems.Count;
      float radius = .01f + radiusIncreaseRate * Time.deltaTime;
      float x = Mathf.Cos(angle) * radius;
      float y = Mathf.Sin(angle) * radius;

      Vector3 newPos = gem.transform.position + new Vector3(x, y, 0);
      gem.transform.position = newPos;

    }

    private IEnumerator SuspendGems()
    {
      yield return new WaitForSeconds(gemSuspensionDuration);
      dropGems = true;
    }

    public void LoseGems(int collectedGems)
    {
        SpawnGems(collectedGems);
    }

    // Instantiates several gem game objects in a semicircle from the player's current position
    public void SpawnGems(int collectedGems)
    {

        int gemCount;
        if (collectedGems > 0 && collectedGems < defaultGemCount)
          gemCount = collectedGems;
        else
          gemCount = defaultGemCount;

        // Spawn gems in a semicircle
        for (int i = 1; i < gemCount; i++)
        {
          GameObject gemObject = SpawnGem(i, gemCount);
          gems.Add(gemObject);
        }

    }

    // Instantiates gem object
    private GameObject SpawnGem(int gemIndex, int gemCount)
    {
        Vector3 gemPos = GetGemPosition(gemIndex, gemCount);

        GameObject gemObject = Instantiate(gem, gemPos, Quaternion.identity);

        return gemObject;
    }

    // Given index, returns expected position of gem relative to player
    private Vector3 GetGemPosition(int gemIndex, int gemCount)
    {

        // semicircle (180 degrees)
        float radius = 2.5f;
        float angle = gemIndex * Mathf.PI / (gemCount - 1);
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        Vector3 playerPosition = transform.position;
        Vector3 gemPos = playerPosition + new Vector3(x, y, 0);

        return gemPos;
    }

}
