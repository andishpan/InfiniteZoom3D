using Godot;
using Godot.Collections;
using System;

public partial class zoom_cube : MeshInstance3D
{	
	private Vector3 MovementDirection;
	private Vector3 rotationAxis;

	private float minMovement = 0.1f;
	private float maxMovement = 2f;
	private float minPosition = 0.1f;
	private float maxPosition = 8f;
	private float speed = 2f;
	private float rotationSpeed = 0f;

	[Export]
	Array<Mesh> mesh_list;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		minMovement = (float)GetMeta("minMovement");
		maxMovement = (float)GetMeta("maxMovement");
		minPosition = (float)GetMeta("minPosition");
		maxPosition = (float)GetMeta("maxPosition");
		speed = (float)GetMeta("speed");
		rotationSpeed = (float)GetMeta("rotationSpeed");
		
		RandomNumberGenerator rng = new RandomNumberGenerator();
		float seed = rng.RandfRange(-2, 2);

		Scale = Vector3.One * 0.01f;
		if(rotationSpeed > 0) rotationAxis = (Vector3.One * rng.RandfRange(-1, 1)).Normalized();
		if(mesh_list.Count != 0) Mesh = mesh_list.PickRandom();

		if(seed >= 1) {
			MovementDirection = new Vector3(rng.RandfRange(minMovement, maxMovement), rng.RandfRange(minMovement, maxMovement), 1);
			Position = new Vector3(rng.RandfRange(minPosition, maxPosition), rng.RandfRange(minPosition, maxPosition), -10);
		} else if(seed >= 0) {
			MovementDirection = new Vector3(rng.RandfRange(minMovement, maxMovement), rng.RandfRange(-minMovement, -maxMovement), 1);
			Position = new Vector3(rng.RandfRange(minPosition, maxPosition), rng.RandfRange(-minPosition, -maxPosition), -10);
		} else if(seed >= -1) {
			MovementDirection = new Vector3(rng.RandfRange(-minMovement, -maxMovement), rng.RandfRange(minMovement, maxMovement), 1);
			Position = new Vector3(rng.RandfRange(-minPosition, -maxPosition), rng.RandfRange(minPosition, maxPosition), -10);
		} else {
			MovementDirection = new Vector3(rng.RandfRange(-minMovement, -maxMovement), rng.RandfRange(-minMovement, -maxMovement), 1);
			Position = new Vector3(rng.RandfRange(-minPosition, -maxPosition), rng.RandfRange(-minPosition, -maxPosition), -10);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double _)
	{
		double delta = (double) GetNode("/root/SpeedController").Get("delta");
		Position += MovementDirection * (float)delta * speed;
		if(rotationSpeed > 0) Rotate(rotationAxis, (float)delta * rotationSpeed);

		if(Position.Z < 0) Scale = Vector3.One * (10 + Position.Z) * 0.1f;
		if(Position.Z > 0) Scale = Vector3.One + Vector3.One * Position.Z * 0.1f;
		if(Position.Z > 15) QueueFree();
	}
}
