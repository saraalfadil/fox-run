using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : IState
{
	private PlayerController player;
	private PlayerState playerState = PlayerState.hurt;
	private float hurtTimer = 0f;
    private float hurtDuration = .5f;
	public HurtState(PlayerController player)
	{
		this.player = player;
	}
	public void Enter()
	{
		player.anim.SetInteger("state", (int)playerState);
		player.healthScript.StartPlayerFlashAnimation();
	}
	public void Update()
	{
		// Leave state after certain duration - hurt->idle
		hurtTimer += Time.deltaTime;
        if (hurtTimer >= hurtDuration) 
		{
            hurtTimer = 0f;
			player.playerStateMachine.TransitionTo(player.playerStateMachine.idleState);
        }
	}

	public void Exit()
	{
		player.isHurt = false;
	}
}