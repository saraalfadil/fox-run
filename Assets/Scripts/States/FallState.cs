using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : IState
{
	private PlayerController player;
	private PlayerState playerState = PlayerState.falling;
	public FallState(PlayerController player)
	{
		this.player = player;
	}
	public void Enter()
	{
		player.anim.SetInteger("state", (int)playerState);
		player.isFalling = true;
	}
	public void Update()
	{
		// On ground - fall->idle
		if (player.isTouchingGround)
		{
			player.playerStateMachine.TransitionTo(player.playerStateMachine.idleState);
		}
	}
	public void Exit()
	{
		player.isFalling = false;
	}
}