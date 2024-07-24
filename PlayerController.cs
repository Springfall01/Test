using Godot;
using System;


public partial class PlayerController : CharacterBody2D
{
	Vector2 velocity = new Vector2();
	private int speed = 300;
	private int gravity = 500;
	private float friction = .1f;
	private float acceleration = .25f;
	private int jumpHeight = 300;
	private int dashSpeed = 100;
	private bool isDashing = false;
	private float dashTimer = .2f;
	private float dashTimerReset = .2f;
	private bool isDashAvailable = true;
	private bool isWallJumping = false;
	private float wallJumpTimer = .45f;
	private float wallJumpTimerReset = .45f;
	private bool canClimb = true;
	private int climbSpeed = 500;
	private bool isClimbing = false;
	private float climbTimer = 5f;
	private float climbTimerReset = 5f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (!isDashing && !isWallJumping)
		{
			processMovement((float)delta);
		}
		if (!IsOnFloor())
		{
			processWallJump((float)delta);
		}
		if (canClimb)
		{
			processClimb((float)delta);
		}

		// Input for dash
		if (isDashAvailable)
		{
			processDash((float)delta);
		}

		// Dash cooldown
		if (isDashing)
		{
			dashTimer -= (float)delta;
			if (dashTimer <= 0)
			{
				isDashing = false;
				velocity = new Vector2(0, 0);
			}
		}
		else if (!isClimbing)
		{
			velocity.Y += gravity * (float)delta;
		}
		else
		{
			climbTimer -= (float)delta;
			if (climbTimer <= 0)
			{
				isClimbing = false;
				canClimb = false;
				climbTimer = climbTimerReset;
				}
		}

		Velocity = velocity;
		MoveAndSlide();
	}



	private void processMovement(float delta)
	{
		int direction = 0;
		// Get input for moving left and right
		if (Input.IsActionPressed("move_right"))
		{
			direction += 1;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction -= 1;
		}
		if (direction != 0)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction * speed, acceleration);
		}
		else
		{
			velocity.X = Mathf.Lerp(velocity.X, 0, friction);
		}

		// Input for jump
		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("jump")) { velocity.Y = -jumpHeight; }
			isDashAvailable = true;
			canClimb = true;
		}
	}

	private void processWallJump(float delta)
	{
		// Wall jumping
		if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("raycast_left").IsColliding())
		{
			velocity.Y = -jumpHeight;
			velocity.X = jumpHeight;
			isWallJumping = true;
		}
		else if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("raycast_right").IsColliding())
		{
			velocity.Y = -jumpHeight;
			velocity.X = -jumpHeight;
			isWallJumping = true;
		}
		if (isWallJumping)
		{
			wallJumpTimer -= (float)delta;
			if (wallJumpTimer <= 0)
			{
				isWallJumping = false;
				wallJumpTimer = wallJumpTimerReset;
			}
		}
	}

	private void processDash(float delta)
	{
		if (Input.IsActionJustPressed("dash"))
		{
			if (Input.IsActionPressed("move_left")) { velocity.X = -dashSpeed; isDashing = true; }
			if (Input.IsActionPressed("move_right")) { velocity.X = dashSpeed; isDashing = true; }
			if (Input.IsActionPressed("jump")) { velocity.Y = -dashSpeed; isDashing = true; }
			if (Input.IsActionPressed("move_left") && Input.IsActionPressed("jump")) { velocity.X = -dashSpeed; velocity.Y = -dashSpeed; isDashing = true; }
			if (Input.IsActionPressed("move_right") && Input.IsActionPressed("jump")) { velocity.X = dashSpeed; velocity.Y = -dashSpeed; isDashing = true; }
			dashTimer = dashTimerReset;
			isDashAvailable = false;
		}
	}

	private void processClimb(float delta)
	{
		if (Input.IsActionPressed("climb") && (GetNode<RayCast2D>("raycastClimbLeft").IsColliding() || GetNode<RayCast2D>("raycastClimbRight").IsColliding()
		|| GetNode<RayCast2D>("raycast_left").IsColliding() || GetNode<RayCast2D>("raycast_right").IsColliding()))
		{
			if (canClimb && !isWallJumping)
			{
				isClimbing = true;
				if (Input.IsActionPressed("ui_up"))
				{
					velocity.Y = -climbSpeed;
				}
				else if (Input.IsActionPressed("move_down"))
				{
					velocity.Y = climbSpeed;
				}
				else
				{
					velocity = new Vector2(0, 0);
				}
			}
			else { isClimbing = false; }
		}
		else { isClimbing = false; }
	}

}
