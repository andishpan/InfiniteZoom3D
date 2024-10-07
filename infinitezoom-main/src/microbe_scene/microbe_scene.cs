using Godot;
using System;
using System.Collections;

public partial class microbe_scene : Node3D
{
	[Export]
	Node3D container;

	[Export]
	Node3D groundContainer;

	[Export]
	float zoomEffectSpeed = 4f;

	[Export]
	float microbeSpeed = 8.5f;

	float elapsedTime = 0;

	float spawnCooldown = 0.5f;

	float distanceBetween = 0.5f;

	ArrayList organisms;

	float lastDuplicatePositionZ;

	//how fast the ground scales => how fast the scene collapses
	float scaleFactor = 0.1f;

	int whichOrganism = 0;

	Random r = new Random();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		organisms = new ArrayList();
		foreach (Node node in container.GetChildren())
		{
			MeshInstance3D mesh = (MeshInstance3D)node;
			organisms.Add(mesh);
			lastDuplicatePositionZ = mesh.Position.Z;
		}
		InitGrounds();
	}


	void InitGrounds()
	{
		MeshInstance3D ground = groundContainer.GetChild<MeshInstance3D>(0);
		MeshInstance3D ceiling = groundContainer.GetChild<MeshInstance3D>(1);
		MeshInstance3D wallRight = groundContainer.GetChild<MeshInstance3D>(2);
		MeshInstance3D wallLeft = groundContainer.GetChild<MeshInstance3D>(3);

		for (int i = 0; i < 10; i++)
		{
			MeshInstance3D duplicate = (MeshInstance3D)ground.Duplicate();
			MeshInstance3D duplicate2 = (MeshInstance3D)ceiling.Duplicate();
			MeshInstance3D duplicate3 = (MeshInstance3D)wallRight.Duplicate();
			MeshInstance3D duplicate4 = (MeshInstance3D)wallLeft.Duplicate();

			groundContainer.AddChild(duplicate);
			groundContainer.AddChild(duplicate2);
			groundContainer.AddChild(duplicate3);
			groundContainer.AddChild(duplicate4);

			duplicate.Position = new Vector3(duplicate.Position.X, duplicate.Position.Y, -i * ground.Scale.Z);
			duplicate2.Position = new Vector3(duplicate2.Position.X, duplicate2.Position.Y, -i * ground.Scale.Z);
			duplicate3.Position = new Vector3(duplicate3.Position.X, duplicate3.Position.Y, -i * ground.Scale.Z);
			duplicate4.Position = new Vector3(duplicate4.Position.X, duplicate4.Position.Y, -i * ground.Scale.Z);
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double _delta)
	{
		double delta = (double)GetNode("/root/SpeedController").Get("delta");
		//obstacle spawning
		elapsedTime += (float)delta;

		MeshInstance3D obstacleMesh = (MeshInstance3D)organisms[whichOrganism];

		if (elapsedTime >= spawnCooldown)
		{

			MeshInstance3D duplicate = (MeshInstance3D)obstacleMesh.Duplicate();
			duplicate.Visible = true;
			Area3D obstacleArea = (Area3D)duplicate.GetNode("Area3D");
			obstacleArea.Monitorable = true;
			obstacleArea.Monitoring = true;
			double scaleMax = 0.4;
			double scaleMin = 0.3;

			float scale = (float)(r.NextDouble() * (scaleMax - scaleMin) + scaleMin);
			duplicate.Scale = new Vector3(scale, scale, scale);
			container.AddChild(duplicate);

			OrmMaterial3D material = new OrmMaterial3D
			{
				AlbedoColor = new Color(
					(float)r.NextDouble(),
					(float)r.NextDouble(),
					(float)r.NextDouble()
				)
			};

			duplicate.SetSurfaceOverrideMaterial(0, material);

			double max = 10;
			double min = -max;


			float randomX = (float)(duplicate.Position.X + (r.NextDouble() * (max - min) + min));
			float randomY = (float)(duplicate.Position.Y + (r.NextDouble() * (max - min) + min));

			lastDuplicatePositionZ -= distanceBetween;
			duplicate.Position = new Vector3(randomX, randomY,
				lastDuplicatePositionZ);
			elapsedTime = 0;

			whichOrganism = (whichOrganism + 1) % organisms.Count;
		}



		// move all obstacles in "container" to the camera
		foreach (Node node in container.GetChildren())
		{
			obstacleMesh = (MeshInstance3D)node;

			float x = obstacleMesh.Position.X;
			float y = obstacleMesh.Position.Y;
			float z = obstacleMesh.Position.Z + microbeSpeed * (float)delta;

			obstacleMesh.Position = new Vector3(x, y, z);

			obstacleMesh.RotateX(r.Next(-1, 1) * 1f * (float)delta);
			obstacleMesh.RotateY(r.Next(-1, 1) * 1f * (float)delta);
			obstacleMesh.RotateZ(r.Next(-1, 1) * 1f * (float)delta);
		}

		foreach (MeshInstance3D ground in groundContainer.GetChildren())
		{
			float x = ground.Position.X;
			float y = ground.Position.Y;
			float z = ground.Position.Z + zoomEffectSpeed * (float)delta;

			ground.Position = new Vector3(x, y, z);

			float _scaleFactor = scaleFactor * (float)delta;
			ground.Scale = new Vector3(ground.Scale.X + _scaleFactor, ground.Scale.Y + _scaleFactor, ground.Scale.Z + _scaleFactor);
		}

	}


}
