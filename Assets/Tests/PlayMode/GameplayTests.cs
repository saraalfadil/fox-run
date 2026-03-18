using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class GameplayTests
{
    public GameObject ui;
    public GameObject player;

    [SetUp]
    public void Setup()
    {
        // PermanentUI persists across tests via DontDestroyOnLoad — reset before each test
        if (PermanentUI.perm != null)
            PermanentUI.perm.ResetStats();
    }

    [UnityTest]
    public IEnumerator UI_SetStartingUI()
    {
        yield return SceneManager.LoadSceneAsync("Level1");
        ui = GameObject.Find("==== UI ====");
        PermanentUI script = ui.GetComponent<PermanentUI>();

        Assert.IsTrue(script.gems == 0);
		Assert.IsTrue(script.score == 0);
		Assert.IsTrue(script.health == 3);
    }

    [UnityTest]
    public IEnumerator TestPlayerJumps()
    {
        yield return SceneManager.LoadSceneAsync("Level1");
        player = GameObject.Find("Player");
        PlayerController controller = this.player.GetComponent<PlayerController>();
        controller.movementScript.Jump();
        yield return new WaitForSeconds(0.25f);
        Assert.IsTrue(controller.movementScript.isJumping);
        yield return new WaitForSeconds(1f);
        Assert.IsFalse(controller.movementScript.isJumping);
    }

    // Taking damage while holding gems should lose the gems but NOT decrease health
    [UnityTest]
    public IEnumerator TakeDamage_WithGems_LosesGemsNotHealth()
    {
        yield return SceneManager.LoadSceneAsync("Level1");
        yield return null; // allow Start() to run so PermanentUI.perm is set

        player = GameObject.Find("Player");
        PermanentUI permUI = PermanentUI.perm;
        PlayerHealth healthScript = player.GetComponent<PlayerHealth>();

        permUI.gems = 5;
        int startingHealth = permUI.health;

        healthScript.preventDamage = false;
        healthScript.DecreaseHealth();
        yield return null;

        Assert.AreEqual(startingHealth, permUI.health, "Health should not decrease when player has gems");
        Assert.AreEqual(0, permUI.gems, "Gems should be lost when taking damage");
    }

    // When health hits 0, the game over event should fire
    [UnityTest]
    public IEnumerator Health_ReachesZero_TriggersGameOver()
    {
        yield return SceneManager.LoadSceneAsync("Level1");
        yield return null; // allow Start() to run so PermanentUI.perm is set

        player = GameObject.Find("Player");
        PermanentUI permUI = PermanentUI.perm;
        PlayerHealth healthScript = player.GetComponent<PlayerHealth>();

        // Disable EndGame so its OnGameOver handler doesn't throw (audio/scene ops)
        // and doesn't freeze time or load the Menu scene mid-test
        EndGame endGame = UnityEngine.Object.FindObjectOfType<EndGame>();
        if (endGame != null) endGame.enabled = false;

        permUI.gems = 0;
        permUI.health = 1;

        bool gameOverFired = false;
        Action gameOverHandler = () => gameOverFired = true;
        PermanentUI.OnGameOver += gameOverHandler;

        // First hit: health 1 → 0
        healthScript.preventDamage = false;
        healthScript.DecreaseHealth();
        yield return null;

        // Second hit: health is 0, should trigger game over
        healthScript.preventDamage = false;
        healthScript.DecreaseHealth();
        yield return null;

        PermanentUI.OnGameOver -= gameOverHandler;

        Assert.IsTrue(gameOverFired, "OnGameOver should fire when health reaches 0");
    }

    // After losing gems they scatter into the world and the player should be able to recollect them
    [UnityTest]
    public IEnumerator LostGems_CanBeRecollected()
    {
        yield return SceneManager.LoadSceneAsync("Level1");
        yield return null; // allow Start() to run so PermanentUI.perm is set

        player = GameObject.Find("Player");
        PermanentUI permUI = PermanentUI.perm;
        PlayerHealth healthScript = player.GetComponent<PlayerHealth>();

        permUI.gems = 3;
        permUI.health = 3;

        // Trigger gem loss
        healthScript.preventDamage = false;
        healthScript.DecreaseHealth();
        yield return null;

        Assert.AreEqual(0, permUI.gems, "Gems should be reset after taking damage");

        // Wait for gem suspension period (1s) to end so gem colliders become active
        yield return new WaitForSeconds(1.5f);

        GameObject[] spawnedGems = GameObject.FindGameObjectsWithTag("Gem");
        Assert.Greater(spawnedGems.Length, 0, "Gems should be present in the world after losing them");

        // Move the player to the gem using the physics engine so collision events fire
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        playerRb.MovePosition(spawnedGems[0].transform.position);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.Greater(permUI.gems, 0, "Player should be able to recollect lost gems");
    }

    // Colliding with an enemy while falling should defeat it and award 100 points
    [UnityTest]
    public IEnumerator JumpOnEnemy_WhileFalling_DefeatsEnemyAndAddsScore()
    {
        yield return SceneManager.LoadSceneAsync("Level1");
        yield return null; // allow Start() to run so PermanentUI.perm is set

        player = GameObject.Find("Player");
        PermanentUI permUI = PermanentUI.perm;
        PlayerController controller = player.GetComponent<PlayerController>();

        permUI.score = 0;
        controller.isFalling = true;

        // Create a minimal enemy object — no Enemy component needed since
        // PermanentUI.IncreaseScore doesn't use the enemy reference
        GameObject enemyObj = new GameObject("TestEnemy");
        enemyObj.tag = "Enemy";

        // HandleEnemyCollision is private, so call it via reflection
        var method = typeof(PlayerController).GetMethod(
            "HandleEnemyCollision",
            BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(controller, new object[] { enemyObj });

        yield return null;

        UnityEngine.Object.Destroy(enemyObj);

        Assert.AreEqual(100, permUI.score, "Score should increase by 100 when defeating an enemy while falling");
    }
}
