using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : IState
{
	private PlayerController player;
	private PlayerState playerState = PlayerState.crouching;

	public CrouchState(PlayerController player)
	{
		this.player = player;
	}
	public void Enter()
	{
		player.anim.SetInteger("state", (int)playerState);
	}
	public void Update()
	{
	}
	public void Exit(){}
}