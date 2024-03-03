using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class NewTestScript
{
    public GameObject ui;
    public GameObject player;

    [Test]
    public void UI_SetStartingUI()
    {
        ui = GameObject.Find("==== UI ====");
        PermanentUI script = ui.GetComponent<PermanentUI>();
    
        Assert.IsTrue(script.gems == 0);
		Assert.IsTrue(script.score == 0);
		Assert.IsTrue(script.health == 3);
    }

    [Test]
    public IEnumerator TestPlayerJumps()
    {

        player = GameObject.Find("Player");
        PlayerController controller = this.player.GetComponent<PlayerController>();
        controller.movementScript.Jump();
        yield return new WaitForSeconds(0.5f);
        Assert.IsTrue(controller.movementScript.isJumping);
        yield return new WaitForSeconds(1f);
        Assert.IsFalse(controller.movementScript.isJumping);
    }

}

