using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[SerializeField] PlayerController playerController;
	
    private void Update()
    {

		if (playerController == null)
			return;

		// Ignore input if player is hurt or level has ended
        if (playerController.isHurt || PermanentUI.perm.endLevel || playerController.enteredSlide)
            return;

		float hDirection = Input.GetAxis("Horizontal");
        float vDirection = Input.GetAxis("Vertical");
		bool jumpButton = Input.GetButtonDown("Jump");

        if (hDirection < 0)
        {
            playerController.movementScript.MovePlayerLeft();
        }
        else if (hDirection > 0)
        {
            playerController.movementScript.MovePlayerRight();
        }

        if (vDirection < 0)
		{
            playerController.movementScript.MovePlayerDown();
        }

 		if (jumpButton && playerController.isTouchingGround)
        {
        	playerController.movementScript.Jump();
        }

    }

}
