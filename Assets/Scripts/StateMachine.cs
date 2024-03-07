using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerState { idle, running, jumping, falling, hurt, waiting, push, crouching };

[Serializable]
public class StateMachine
{
	public IState CurrentState { get; private set; }
	public IdleState idleState;
	public RunState runState;
	public JumpState jumpState;
	public FallState fallState;
	public HurtState hurtState;
	public PushState pushState;
	public WaitState waitState;
	public CrouchState crouchState;

	public StateMachine(PlayerController player)
	{	
		this.idleState = new IdleState(player);
		this.runState = new RunState(player);
		this.jumpState = new JumpState(player);
		this.fallState = new FallState(player);
		this.hurtState = new HurtState(player);
		this.pushState = new PushState(player);
		this.waitState = new WaitState(player);
		this.crouchState = new CrouchState(player);
	}

	public void Initialize(IState startingState)
	{
		CurrentState = startingState;
		startingState.Enter();
	}

	public void TransitionTo(IState nextState)
	{
		CurrentState.Exit();
		CurrentState = nextState;
		nextState.Enter();
	}

	public void Update()
	{
		if (CurrentState != null)
		{
			CurrentState.Update();
		}
	}
}