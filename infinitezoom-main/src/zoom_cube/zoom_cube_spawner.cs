using Godot;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

public partial class zoom_cube_spawner : Node3D
{
	private float spawnCooldown;
	private int spawnCount;
	private float rotationSpeed;
	private double counter;
	private PackedScene zoomCubeScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spawnCooldown = (float)GetMeta("spawnCooldown");
		spawnCount = (int)GetMeta("spawnCount");
		rotationSpeed = (float)GetMeta("rotationSpeed");
		zoomCubeScene = (PackedScene) GetMeta("zoomCube");
		counter = spawnCooldown;

		zoomCubeScene.SetMeta("minMovement", GetMeta("minMovement"));
		zoomCubeScene.SetMeta("maxMovement", GetMeta("maxMovement"));
		zoomCubeScene.SetMeta("minPosition", GetMeta("minPosition"));
		zoomCubeScene.SetMeta("maxPosition", GetMeta("maxPosition"));
		zoomCubeScene.SetMeta("speed", GetMeta("speed"));
		zoomCubeScene.SetMeta("cubeRotationSpeed", GetMeta("cubeRotationSpeed"));
		// GD.Print(zoomCubeScene.GetMeta("minMovement"));
		// zoom_cube.minMovement = (float)GetMeta("minMovement");
		// zoom_cube.maxMovement = (float)GetMeta("maxMovement");
		// zoom_cube.minPosition = (float)GetMeta("minPosition");
		// zoom_cube.maxPosition = (float)GetMeta("maxPosition");
		// zoom_cube.speed = (float)GetMeta("speed");
		// zoom_cube.rotationSpeed = (float)GetMeta("cubeRotationSpeed");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double _)
	{
		double delta = (double) GetNode("/root/SpeedController").Get("delta");
		Rotation += Vector3.Forward * rotationSpeed * (float)delta;
		counter += delta;

		if(counter >= spawnCooldown) {
			for(int i = 0; i < spawnCount; i++) {
				var zoomCube = zoomCubeScene.Instantiate();
				AddChild(zoomCube);
			}

			counter = 0;
		}
	}
}
