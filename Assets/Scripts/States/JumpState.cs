using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IState
{
	private PlayerController player;

	private PlayerState playerState = PlayerState.jumping;

	public JumpState(PlayerController player)
	{
		this.player = player;
	}
	public void Enter()
	{
		player.anim.SetInteger("state", (int)playerState);

		Debug.Log("JumpState");
	}
	public void Update()
	{
		// Going down - jump->fall
		if (player.rb.velocity.y < .1f)
		{
			player.playerStateMachine.TransitionTo(player.playerStateMachine.fallState);
		}
	}
	public void Exit(){}
}