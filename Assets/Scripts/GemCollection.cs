using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GemCollection : MonoBehaviour
{
    private bool dropGems = false;
    private float gemDuration = 1f;
    private int defaultSpawnCount = 6;
    float radiusIncreaseRate = 0.5f;
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

    public void ScatterGems()
    {

        // Continously scatter gems outward from player each frame, them drop at the same time
        for (int i = gems.Count - 1; i >= 0; i--)
        {

            // Remove destroyed gems from gem collection
            if (gems[i] == null)
            {
                gems.RemoveAt(i);
                continue;
            }

            Gem gem = gems[i].GetComponent<Gem>();

            // Update gem position in circular shape
            UpdateGemPosition(gem, i);

            // Drop gems to ground
            if (dropGems)
                gem.ChangeCollider();

        }

    }

    private void UpdateGemPosition(Gem gem, int gemIndex)
    {
      // Increase the radius of the circle gradually
      float angle = gemIndex * Mathf.PI * 2 / gems.Count;
      float x = Mathf.Cos(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);
      float y = Mathf.Sin(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);

      // Continuously update each gem position
      Vector3 newPos = gem.transform.position + new Vector3(x, y, 0);
      gem.transform.position = newPos;

    }

    private IEnumerator SuspendGems()
    {
      yield return new WaitForSeconds(gemDuration);
      dropGems = true;
    }

    public void LoseGems(int collectedGems)
    {
        // Spawn gems
        SpawnGems(collectedGems);
    }

    // Instantiates several gem game objects in a semicircle from the player's current position
    public void SpawnGems(int collectedGems)
    {

        int spawnCount = collectedGems < defaultSpawnCount && collectedGems > 0 ? collectedGems : defaultSpawnCount;

        // Spawn gems in a semicircle
        for (int i = 1; i < spawnCount; i++)
        {
            Vector3 gemPos = GetGemPosition(i, spawnCount);

            GameObject gameObject = Instantiate(gem, gemPos, Quaternion.identity);

            gems.Add(gameObject);
        }

    }

    private Vector3 GetGemPosition(int i, int spawnCount)
    {

        // semicircle (180 degrees)
        float radius = 2.5f;
        float angle = i * Mathf.PI / (spawnCount - 1);
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        Vector3 playerPosition = transform.position;
        Vector3 gemPos = playerPosition + new Vector3(x, y, 0);

        return gemPos;
    }

}
