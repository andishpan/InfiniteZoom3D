using Godot;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

public partial class zoom_wireframe_spawner : Node3D
{
	private float spawnCooldown;
	private int spawnCount;
	private float rotationSpeed;
	private double counter;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spawnCooldown = (float)GetMeta("spawnCooldown");
		spawnCount = (int)GetMeta("spawnCount");
		rotationSpeed = (float)GetMeta("rotationSpeed");
		counter = spawnCooldown;

		wireframe_zoom_cube.minMovement = (float)GetMeta("minMovement");
		wireframe_zoom_cube.maxMovement = (float)GetMeta("maxMovement");
		wireframe_zoom_cube.minPosition = (float)GetMeta("minPosition");
		wireframe_zoom_cube.maxPosition = (float)GetMeta("maxPosition");
		wireframe_zoom_cube.speed = (float)GetMeta("speed");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Rotation += Vector3.Forward * rotationSpeed * (float)delta;
		counter += delta;

		if(counter >= spawnCooldown) {
			for(int i = 0; i < spawnCount; i++) {
				PackedScene zoomCubeScene = (PackedScene) GetMeta("zoomCube");
				var zoomCube = zoomCubeScene.Instantiate();
				AddChild(zoomCube);
			}

			counter = 0;
		}

		//GD.Print(GetChildren().Count);
	}
}
