using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
	private PlayerController player;
	private PlayerState playerState = PlayerState.idle;
	public IdleState(PlayerController player)
	{
		this.player = player;
	}
	public void Enter()
	{
		player.anim.SetInteger("state", (int)playerState);
	}
	public void Update()
	{
		// Not touching ground - idle->jump
		if (!player.isTouchingGround)
		{
			player.playerStateMachine.TransitionTo(player.playerStateMachine.jumpState);
		}

		// Moving horizontally - idle->run
		if (Mathf.Abs(player.rb.velocity.x) > 2f)
        {
			player.playerStateMachine.TransitionTo(player.playerStateMachine.runState);
        }

		// Not moving - idle->wait
		if (!player.movementScript.hasMoved)
        {
			player.playerStateMachine.TransitionTo(player.playerStateMachine.waitState);
        }

		if (player.isHurt)
        {
			player.playerStateMachine.TransitionTo(player.playerStateMachine.hurtState);
        }
	}
	public void Exit(){}
}