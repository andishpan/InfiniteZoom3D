using Godot;
using System;
using System.Collections.Generic;

public partial class FractalZoom : Node3D
{
	ShaderMaterial _shaderMaterial;
	float _zoom = 1.0f;
	float AutoZoomSpeed = 0.99999999f;
	float MoveSpeed = 0.1f;
	Vector3 _center = new Vector3(0, 0, 0);
	float _time = 0.0f;

	float obstacleMoveSpeed = 20;

	Vector3 _baseColor;

	MeshInstance3D obstacleLeft;
	MeshInstance3D obstacleRight;

	Node3D containerLeft;
	Node3D containerRight;

	GpuParticles3D particlesLeft;
	GpuParticles3D particlesRight;

	float counter = 0;

	float spawnCooldown = 5;


	[Export]
	Color color = new Color(1, 1, 1, 1);

	Random random = new Random();

	float counter1 = 0;
	float counter2 = 0;

	MeshInstance3D crystal;

	float counterC = 0;
	float spawnCooldownC = 0.3f; //for small crystals

	Node3D crystalContainer;

	float bigCrystalMoveSpeed = 7;

	float smallCrystalMoveSpeed = 10;

	Dictionary<MeshInstance3D, int> bigCrystalRotations = new();



	public override void _Ready()
	{
		MeshInstance3D meshInstance = GetNode<MeshInstance3D>("FractalTunnel");
		_shaderMaterial = (ShaderMaterial)meshInstance.GetActiveMaterial(0);
		SetProcess(true);

		obstacleLeft = GetNode<MeshInstance3D>("Obstacle2");
		obstacleRight = GetNode<MeshInstance3D>("Obstacle1");

		containerLeft = GetNode<Node3D>("ContainerLeft");
		containerRight = GetNode<Node3D>("ContainerRight");

		particlesLeft = GetNode<GpuParticles3D>("SmokeLeft");
		particlesRight = GetNode<GpuParticles3D>("SmokeRight");

		crystal = GetNode<MeshInstance3D>("Crystal");
		crystalContainer = GetNode<Node3D>("CrystalContainer");
	}

	public override void _Process(double _delta)
	{
		double delta = (double) GetNode("/root/SpeedController").Get("delta");
		_zoom *= AutoZoomSpeed;

		_time += (float) delta;

		_center = new Vector3(
			MoveSpeed * Mathf.Sin(_time),
			MoveSpeed * Mathf.Cos(_time),
			0.0f
		);

		_shaderMaterial.SetShaderParameter("zoom", _zoom);
		_shaderMaterial.SetShaderParameter("center", _center);
		
		_baseColor = new Vector3(color.R, color.G, color.B);
		_shaderMaterial.SetShaderParameter("base_color", _baseColor);

		if(counter >= spawnCooldown){
			counter = 0;
			spawnObstacle();
		}else{
			counter += (float)delta;
		}
	


		foreach(MeshInstance3D obstacle in containerLeft.GetChildren()){
			float zTranslation = (float)(bigCrystalMoveSpeed * delta);
			float xyTranslation = (float)(10 * delta);
			if(obstacle.Position.Y > 0){
				xyTranslation = 0;
				obstacle.RotateZ((float)(bigCrystalRotations[obstacle] * _delta));
			}
			obstacle.Position = new Vector3(obstacle.Position.X + xyTranslation, obstacle.Position.Y + xyTranslation, obstacle.Position.Z + zTranslation);
		}

		foreach(MeshInstance3D obstacle in containerRight.GetChildren()){
			float zTranslation = (float)(bigCrystalMoveSpeed * delta);
			float xyTranslation = (float)(10 * delta);
			if(obstacle.Position.Y > 0){
				xyTranslation = 0;
				obstacle.RotateZ((float)(bigCrystalRotations[obstacle] * _delta));
			}
			obstacle.Position = new Vector3(obstacle.Position.X - xyTranslation, obstacle.Position.Y + xyTranslation, obstacle.Position.Z + zTranslation);
		}

		if(particlesLeft.Emitting){
			counter1 += (float)delta;
		}
		if(particlesRight.Emitting){
			counter2 += (float)delta;
		}
		if(counter1 >= 1){
			particlesLeft.Emitting = false;
			counter1 = 0;
		}
		if(counter2 >= 1){
			particlesRight.Emitting = false;
			counter2 = 0;
		}

		if(counterC >= spawnCooldownC){
			counterC = 0;
			spawnCrystal();
		}else{
			counterC += (float)delta;
		}
		

		foreach(MeshInstance3D crystal in crystalContainer.GetChildren()){
			crystal.Position = new Vector3(crystal.Position.X, crystal.Position.Y, (float)(crystal.Position.Z + delta * smallCrystalMoveSpeed));
			float rotation = random.Next(-1, 10) * (float)delta;
			crystal.RotateX(rotation);
			crystal.RotateY(rotation);
			crystal.RotateZ(rotation);

		}
	}

	void spawnCrystal(){
		MeshInstance3D duplicate = (MeshInstance3D) crystal.Duplicate();
		float x = random.Next(-24, 24);
		float y = random.Next(-24, 24);
		duplicate.Position = new Vector3(x, y, duplicate.Position.Z);
		crystalContainer.AddChild(duplicate);
	}

	void spawnObstacle(){
		MeshInstance3D duplicate;
		int randomInt = random.Next(-1, 1);
		if(randomInt < 0){
			duplicate = (MeshInstance3D) obstacleLeft.Duplicate();
			duplicate.Visible = true;
			containerLeft.AddChild(duplicate);
			particlesLeft.Emitting = true;
		}else {
			duplicate = (MeshInstance3D) obstacleRight.Duplicate();
			duplicate.Visible = true;
			containerRight.AddChild(duplicate);
			particlesRight.Emitting = true;
		}
		int rotation = random.Next() % 2;
		if (rotation == 0) {
			rotation = -1;
		}
		bigCrystalRotations.Add(duplicate, rotation);
	}

	void moveBigCrystals(Node3D container){

	}
}
