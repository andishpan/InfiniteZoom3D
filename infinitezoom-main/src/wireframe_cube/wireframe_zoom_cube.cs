using Godot;
using System;

public partial class wireframe_zoom_cube : MeshInstance3D
{	
	private Vector3 MovementDirection;
	public static float minMovement = 0.1f;
	public static float maxMovement = 2f;
	public static float minPosition = 0.1f;
	public static float maxPosition = 8f;
	public static float speed = 2f;
	
	private static float minScale = 0.01f;
	private static float maxScale = 0.05f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		float seed = rng.RandfRange(-2, 2);

		Scale = Vector3.One * 0.01f;

		if (seed >= 1) 
		{
			MovementDirection = new Vector3(rng.RandfRange(minMovement, maxMovement), rng.RandfRange(minMovement, maxMovement), 1);
			Position = new Vector3(rng.RandfRange(minPosition, maxPosition), rng.RandfRange(minPosition, maxPosition), -10);
		} 
		else if (seed >= 0) 
		{
			MovementDirection = new Vector3(rng.RandfRange(minMovement, maxMovement), rng.RandfRange(-minMovement, -maxMovement), 1);
			Position = new Vector3(rng.RandfRange(minPosition, maxPosition), rng.RandfRange(-minPosition, -maxPosition), -10);
		} 
		else if (seed >= -1) 
		{
			MovementDirection = new Vector3(rng.RandfRange(-minMovement, -maxMovement), rng.RandfRange(minMovement, maxMovement), 1);
			Position = new Vector3(rng.RandfRange(-minPosition, -maxPosition), rng.RandfRange(minPosition, maxPosition), -10);
		} 
		else 
		{
			MovementDirection = new Vector3(rng.RandfRange(-minMovement, -maxMovement), rng.RandfRange(-minMovement, -maxMovement), 1);
			Position = new Vector3(rng.RandfRange(-minPosition, -maxPosition), rng.RandfRange(-minPosition, -maxPosition), -10);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += MovementDirection * (float)delta * speed;

		if (Position.Z < 0) 
		{
			Scale = Vector3.One * (10 + Position.Z) * 0.1f;
		}
		else 
		{
			Scale = Vector3.One + Vector3.One * Position.Z * 0.1f;
		}

		// Clamp the scale to ensure it stays within the min and max bounds
		Scale = new Vector3(
			Mathf.Clamp(Scale.X, minScale, maxScale),
			Mathf.Clamp(Scale.Y, minScale, maxScale),
			Mathf.Clamp(Scale.Z, minScale, maxScale)
		);

		if (Position.Z > 15 || Position.X > 15 || Position.X < -15 || Position.Y > 15 || Position.Y < -15) 
		{
			QueueFree();
		}
	}
}
