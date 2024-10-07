using Godot;
using System;

public partial class tunnel_object : Node3D
{
	private Camera3D camera;
	private MeshInstance3D[] sceneObjects;
	private int currentTargetIndex = 0;
	private const float Speed = 10.0f;
	private const float RotationSpeed = 2.0f;  
	private const float MinScale = 0.1f;
	private const float ScaleFactor = 5.0f;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Path3D/CameraPath/Camera");
		sceneObjects = new MeshInstance3D[]
		{
			GetNode<MeshInstance3D>("Object1"),
			GetNode<MeshInstance3D>("Object2"),
			GetNode<MeshInstance3D>("Object3"),
			GetNode<MeshInstance3D>("Object4"),
			GetNode<MeshInstance3D>("Object5"),
			GetNode<MeshInstance3D>("Object6"),
			GetNode<MeshInstance3D>("Object7"),
			GetNode<MeshInstance3D>("Object8"),
			GetNode<MeshInstance3D>("Object9"),
			GetNode<MeshInstance3D>("Object10"),
			GetNode<MeshInstance3D>("Object11"),
			GetNode<MeshInstance3D>("Object12"),
			GetNode<MeshInstance3D>("Object13")
		};
	}

	public override void _Process(double delta)
	{
		/* Vector3 cameraPos = camera.GlobalTransform.Origin;
	Vector3 targetPos = sceneObjects[currentTargetIndex].GlobalTransform.Origin;
	float distance = cameraPos.DistanceTo(targetPos);
	
	Vector3 direction = (targetPos - cameraPos).Normalized();
	camera.GlobalTransform = new Transform3D(camera.GlobalTransform.Basis, cameraPos + direction * Speed * (float)delta);*/
		
		Vector3 cameraPos = camera.GlobalTransform.Origin;
		MeshInstance3D currentObject = sceneObjects[currentTargetIndex];
		Vector3 objectPos = currentObject.GlobalTransform.Origin;
		float distance = objectPos.DistanceTo(cameraPos);

		
		/* Vector3 direction = (targetPos - cameraPos).Normalized();
	camera.GlobalTransform = new Transform3D(camera.GlobalTransform.Basis, cameraPos + direction * Speed * (float)delta);*/
		if (distance > 1.0)
		{
			
			Vector3 direction = (cameraPos - objectPos).Normalized();
			currentObject.GlobalTransform = new Transform3D(currentObject.GlobalTransform.Basis, objectPos + direction * Speed * (float)delta);
		}
		else
		{
			currentTargetIndex = (currentTargetIndex + 1) % sceneObjects.Length;
		}

	
		currentObject.RotateZ(RotationSpeed * (float)delta);
		currentObject.RotateY(RotationSpeed * (float)delta);

		UpdateObjectScales(cameraPos);
	}

	private void UpdateObjectScales(Vector3 cameraPos)
	{
		foreach (var obj in sceneObjects)
		{
			float objDistance = cameraPos.DistanceTo(obj.GlobalTransform.Origin);
			float scaleValue = Mathf.Max(MinScale, ScaleFactor / Mathf.Max(objDistance, 1.0f));
			obj.Scale = new Vector3(scaleValue, scaleValue, scaleValue);
		}
	}
}
