using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class NewTestScript
{
    public GameObject ui;
    public GameObject character;

    [Test]
    public void UI_SetStartingUI()
    {
        ui = GameObject.Find("PlayerUI");
        PermanentUI script = ui.GetComponent<PermanentUI>();
    
        Assert.IsTrue(script.cherries == 0);
    }

    [Test]
    public void TestPlayerJumps()
    {

        character = GameObject.Find("Player");
        PlayerController controller = this.character.GetComponent<PlayerController>();
        //controller.movementScript.Jump();
        //yield return new WaitForSeconds(0.5f);
        //Assert.IsTrue(controller.movementScript.isJumping);
        //yield return new WaitForSeconds(1f);
        //Assert.IsFalse(controller.isJumping);
    }

}

