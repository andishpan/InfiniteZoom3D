using Godot;
using System;

public partial class tunnel : Node3D
{
	private Camera3D camera;
	private MeshInstance3D[] sceneObjects;
	private int currentTargetIndex = 0;
	private const float Speed = 20.0f * 16;
	private const float ObjectSpeed = 2.0f; 
	private const float RotationSpeed = 2.0f;
	private const float MinScale = 0.1f;
	private const float ScaleFactor = 75.0f;
	//private const string infiniteZoomBlenderPath = "res://src/tunnel/infinite_zoomBlender.tscn";
	//private const string wireframeCubePath = "res://src/WireframeCube.tscn";

	private PackedScene infiniteZoomBlenderScene;
	private PackedScene wireframeCubeScene;

	private bool wireframeEnabled = true;

	public override void _Ready()
	{
		//infiniteZoomBlenderScene = (PackedScene)ResourceLoader.Load(infiniteZoomBlenderPath);
		//wireframeCubeScene = (PackedScene)ResourceLoader.Load(wireframeCubePath);

		camera = GetNode<Camera3D>("Path3D/CameraPath/Camera");
		sceneObjects = new MeshInstance3D[]
		{
			GetNode<MeshInstance3D>("Objects/Object1"),
			GetNode<MeshInstance3D>("Objects/Object2"),
			GetNode<MeshInstance3D>("Objects/Object3"),
			GetNode<MeshInstance3D>("Objects/Object4"),
			GetNode<MeshInstance3D>("Objects/Object5"),
			GetNode<MeshInstance3D>("Objects/Object6"),
			GetNode<MeshInstance3D>("Objects/Object7"),
			GetNode<MeshInstance3D>("Objects/Object8"),
			GetNode<MeshInstance3D>("Objects/Object9"),
			GetNode<MeshInstance3D>("Objects/Object10"),
			GetNode<MeshInstance3D>("Objects/Object11"),
			GetNode<MeshInstance3D>("Objects/Object12"),
			GetNode<MeshInstance3D>("Objects/Object13")
		};
	}

	public override void _Process(double delta)
	{
		if (!IsInstanceValid(camera) || !camera.IsInsideTree())
			return;

		Vector3 cameraPos = camera.GlobalTransform.Origin;
		Vector3 targetPos = sceneObjects[12].GlobalTransform.Origin; 

		float distance = cameraPos.DistanceTo(targetPos);
		Vector3 direction = (targetPos - cameraPos).Normalized();
		camera.GlobalTransform = new Transform3D(camera.GlobalTransform.Basis, cameraPos + direction * Speed * (float)delta);

		
		Vector3 obj1Pos = sceneObjects[0].GlobalTransform.Origin;  
		for (int i = 1; i < sceneObjects.Length - 1; i++)  
		{
			MoveAndRotateObject(sceneObjects[i], obj1Pos, delta);
		}

		if (distance < 1.0)
		{
			// removed scene switcher
			//SceneSwitcher sceneSwitcher = (SceneSwitcher)GetNode("/root/SceneSwitcher");
			currentTargetIndex = (currentTargetIndex + 1) % sceneObjects.Length;
			if (currentTargetIndex == 0)
			{
				//GetTree().ChangeSceneToPacked(infiniteZoomBlenderScene);
				camera.GlobalTransform = new Transform3D(camera.GlobalTransform.Basis, sceneObjects[0].GlobalTransform.Origin);
			}
			/*else
			{
				GetTree().ChangeSceneToPacked(wireframeCubeScene);
			} 
			sceneSwitcher.SwitchToScene2 = !sceneSwitcher.SwitchToScene2;*/
		}

		UpdateObjectScales(cameraPos);
	}

	private void MoveAndRotateObject(MeshInstance3D obj, Vector3 targetPos, double delta)
	{
		if (!IsInstanceValid(obj) || !obj.IsInsideTree())
			return;

		
		Vector3 objPos = obj.GlobalTransform.Origin;
		Vector3 direction = (targetPos - objPos).Normalized();
		obj.GlobalTransform = new Transform3D(obj.GlobalTransform.Basis, objPos + direction * ObjectSpeed * (float)delta);

		
		obj.RotateZ(RotationSpeed * (float)delta);
		obj.RotateY(RotationSpeed * (float)delta);
	}

	private void UpdateObjectScales(Vector3 cameraPos)
	{
		foreach (var obj in sceneObjects)
		{
			if (!IsInstanceValid(obj) || !obj.IsInsideTree())
				continue;

			float objDistance = cameraPos.DistanceTo(obj.GlobalTransform.Origin);
			float scaleValue = Mathf.Max(MinScale, ScaleFactor / Mathf.Max(objDistance, 1.0f));
			obj.Scale = new Vector3(scaleValue, scaleValue, scaleValue);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.P)
		{
			Viewport viewport = GetViewport();
			if (wireframeEnabled)
			{
				viewport.DebugDraw = Viewport.DebugDrawEnum.Disabled;
			}
			else
			{
				viewport.DebugDraw = Viewport.DebugDrawEnum.Wireframe;
			}
			wireframeEnabled = !wireframeEnabled;
		}
	}
}
