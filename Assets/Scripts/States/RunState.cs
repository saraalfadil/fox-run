using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : IState
{
	private PlayerController player;

	private PlayerState playerState = PlayerState.running;
	public RunState(PlayerController player)
	{
		this.player = player;
	}
	public void Enter()
	{
		player.anim.SetInteger("state", (int)playerState);
	}
	public void Update()
	{
		// Not touching ground - run->jump
		if (!player.isTouchingGround)
		{
			player.playerStateMachine.TransitionTo(player.playerStateMachine.jumpState);
		}

		// Moving horizontally - run->idle
		if (Mathf.Abs(player.rb.velocity.x) < .1f)
        {
			player.playerStateMachine.TransitionTo(player.playerStateMachine.idleState);
        }

		if (player.isHurt)
        {
			player.playerStateMachine.TransitionTo(player.playerStateMachine.hurtState);
        }
	}
	public void Exit(){}
}