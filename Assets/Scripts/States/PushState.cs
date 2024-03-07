using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushState : IState
{
	private PlayerController player;
	private PlayerState playerState = PlayerState.push;

	public PushState(PlayerController player)
	{
		this.player = player;
	}
	public void Enter()
	{
		player.anim.SetInteger("state", (int)playerState);
	}
	public void Update()
	{
		if (Mathf.Abs(player.rb.velocity.x) < 2f)
		{
			player.playerStateMachine.TransitionTo(player.playerStateMachine.idleState);
		}
	}
	public void Exit(){}
}