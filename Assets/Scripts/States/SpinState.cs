using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinState : IState
{
	private PlayerController player;

	private PlayerState playerState = PlayerState.spin;
	public SpinState(PlayerController player)
	{
		this.player = player;
	}
	public void Enter()
	{
		player.anim.SetInteger("state", (int)playerState);
		player.movementScript.speed = 15f;
		player.movementScript.MovePlayerRight();
	}
	public void Update()
	{
		if (!player.enteredSlide)
        {
			player.playerStateMachine.TransitionTo(player.playerStateMachine.idleState);
        }
	}
	public void Exit()
	{
		player.movementScript.speed = 5f;
	}
}