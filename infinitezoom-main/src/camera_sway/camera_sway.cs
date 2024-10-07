using Godot;
using System;

public partial class camera_sway : Node3D
{
	[Export]
	Node3D container;

	float zoomEffectSpeed = 0.1f;

	float elapsedTime = 0;

	float spawnCooldown = 2.0f;

	float distanceBetween = 10.0f;

	MeshInstance3D obstacleMesh;

	float lastDuplicatePositionZ;


	public override void _Ready()
	{
		return;
		obstacleMesh = container.GetChild<MeshInstance3D>(0);
		lastDuplicatePositionZ = obstacleMesh.Position.Z;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		return;
		float deltaF = (float)delta;

		//obstacle spawning
		elapsedTime += deltaF;

		if (elapsedTime >= spawnCooldown)
		{

			MeshInstance3D duplicate = (MeshInstance3D)obstacleMesh.Duplicate();
			Random r = new Random();
			double scaleMax = 1;
			double scaleMin = 1;

			float scale = (float)(r.NextDouble() * (scaleMax - scaleMin) + scaleMin);
			duplicate.Scale = new Vector3(scale, scale, scale);
			//container.AddChild(duplicate);

			OrmMaterial3D material = new OrmMaterial3D
			{
				AlbedoColor = new Color((float)r.NextDouble(),
			(float)r.NextDouble(),
			(float)r.NextDouble())
			};

			duplicate.SetSurfaceOverrideMaterial(0, material);

			double max = 1;
			double min = -1;


			float randomX = (float)(duplicate.Position.X + (r.NextDouble() * (max - min) + min));
			float randomY = (float)(duplicate.Position.Y + (r.NextDouble() * (max - min) + min));

			duplicate.Position = new Vector3(randomX, randomY,
				lastDuplicatePositionZ - distanceBetween);
			lastDuplicatePositionZ -= distanceBetween;
			elapsedTime = 0;
		}

		// move all obstacles in "container" to the camera
		foreach (Node node in container.GetChildren())
		{
			MeshInstance3D obstacleMesh = (MeshInstance3D)node;

			float x = obstacleMesh.Position.X;
			float y = obstacleMesh.Position.Y;
			float z = obstacleMesh.Position.Z + zoomEffectSpeed;

			obstacleMesh.Position = new Vector3(x, y, z);

			Random r = new Random();
			obstacleMesh.RotateX(r.Next(-1, 1) * 0.05f);
			obstacleMesh.RotateY(r.Next(-1, 1) * 0.05f);
			obstacleMesh.RotateZ(r.Next(-1, 1) * 0.05f);
		}

	}
}

