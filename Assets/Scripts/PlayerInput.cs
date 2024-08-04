using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	[SerializeField] PlayerController playerController;
	
	InputMaster controls;
	private Vector2 moveInput;

	public void Awake()
	{
		controls = new InputMaster();
	}

	public void OnEnable()
	{
		controls.Player.Enable();
	}

	public void OnDisable()
	{
		controls.Player.Disable();
	}

	void FixedUpdate()
	{
		moveInput = controls.Player.Movement.ReadValue<Vector2>();
	}

    private void Update()
    {

		if (playerController == null)
			return;

		// Ignore input if player is hurt, level has ended, or player is using the slide
        if (playerController.isHurt || PermanentUI.perm.endLevel || playerController.enteredSlide)
            return;

		float hDirection = Input.GetAxis("Horizontal");
        float vDirection = Input.GetAxis("Vertical");
		bool jumpButton = Input.GetButtonDown("Jump");

        if (hDirection < 0 || moveInput.x < 0)
        {
            playerController.movementScript.MovePlayerLeft();
        }
        else if (hDirection > 0 || moveInput.x > 0)
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

	// Mapped to the "jump" input action
	public void OnJump() 
	{
		if (playerController.isTouchingGround) 
		{
			playerController.movementScript.Jump();
		}
	}

}
