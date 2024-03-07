using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitState : IState
{
	private PlayerController player;
	private PlayerState playerState = PlayerState.waiting;

	public WaitState(PlayerController player)
	{
		this.player = player;
	}
	public void Enter()
	{
		player.anim.SetInteger("state", (int)playerState);
	}
	public void Update()
	{
		// Moving horizontally - idle->run
		if (Mathf.Abs(player.rb.velocity.x) > 2f)
        {
			player.playerStateMachine.TransitionTo(player.playerStateMachine.runState);
        }
	}
	public void Exit(){}
}