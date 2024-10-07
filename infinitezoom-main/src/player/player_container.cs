using Godot;
using System;

public partial class player_container : Node3D
{
	[Export]
	public float speed = 30;

	[Export]
	public float accel = 350;

	public float movement_bound = 15;

	private CharacterBody3D player;
	private CharacterBody3D camera;
	private Area3D cameraCollision;

	private Node3D shield;

	const float swaySpeed = 10;
	double swayTimeout = 0.2;
	double swayTimer;

	bool shieldUp = false;
	bool shieldVisibility = false;

	double shieldTimer = 0;
	double shieldTimerTarget = 1;

	[Signal]
	public delegate void ShieldEventHandler(bool isUp);
	
	GpuParticles3D rocketParticles;

	Vector3 previousPosition;

	public override void _Ready()
	{
		player = GetNode<CharacterBody3D>("Player");
		camera = GetNode<CharacterBody3D>("Camera");
		cameraCollision = camera.GetNode<Area3D>("Area3D");
		shield = GetNode<Node3D>("Player/Shield");
		rocketParticles = GetNode<GpuParticles3D>("Player/Rocket/GPUParticles3D");

		swayTimer = swayTimeout;
		previousPosition = player.Position;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = new Vector3(
			Input.GetAxis("move_left", "move_right"),
			Input.GetAxis("move_down", "move_up"),
			0
		);
		
		int gameState = (int) GetNode("/root/GameState").Get("GAME_STATE");
		if(gameState != 1) direction = Vector3.Zero;

		if(Input.GetAxis("move_left", "move_right") != 0) player.RotateZ(-Input.GetAxis("move_left", "move_right") / 10);
		if(direction != Vector3.Zero) direction = direction.Normalized();

		player.Velocity = player.Velocity.MoveToward(direction * speed, (float)delta * accel);

		if (swayTimer < swayTimeout)
		{
			swayTimer += delta;
		}

		if (false && cameraCollision.HasOverlappingAreas())
		{
			Vector3 swayDirection = camera.Position - cameraCollision.GetOverlappingAreas()[0].Position;
			swayDirection.Z = 0;
			camera.Velocity = camera.Velocity
				.MoveToward(swayDirection.Normalized() * swaySpeed, (float)delta * 1000);
			swayTimer = 0;
		}
		else if (swayTimer >= swayTimeout)
		{
			camera.Velocity = camera.Velocity
				.MoveToward(player.Position - camera.Position, (float)delta * 1000);
		}

		player.MoveAndSlide();
		camera.MoveAndSlide();

		player.Position = player.Position.Clamp(
			new Vector3(-movement_bound, -movement_bound, player.Position.Z),
			new Vector3(movement_bound, movement_bound, player.Position.Z)
		);

		shield.RotateY((float)delta * 1);
		shield.RotateX((float)delta * 1);
		shield.RotateZ((float)delta * 1);

		if(shieldUp){
			((Material)shield.Get("material")).Set("shader_parameter/shield_uptime", shieldTimer / shieldTimerTarget);
			if(Double.IsNegative(shieldTimer)){
				shieldUp = false;
				shield.Visible = false;
			}else {
				shield.Visible = shieldVisibility;
				shieldTimer -= delta;
			}
		}

		EmitSignal("Shield", shieldUp);

		
		Vector3 currentPosition = player.GlobalPosition;
		float distanceTraveled = currentPosition.DistanceTo(previousPosition);
		float playerSpeed = (float)(distanceTraveled / delta);

		if(playerSpeed > 300){
			rocketParticles.Amount = 1;
		}else{
			rocketParticles.Amount = 100;
		}

		previousPosition = currentPosition;
	}

	public void playerGotHit(Area3D area)
	{
		if(!shieldUp) {
			activateShield(1, true);
		}
	}

	public void activateShield(float time, bool visible) {
		shieldUp = true;
		shieldTimer = time;
		shieldTimerTarget = time;
		shieldVisibility = visible;
	}
}

