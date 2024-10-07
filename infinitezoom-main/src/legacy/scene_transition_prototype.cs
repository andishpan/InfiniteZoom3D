using Godot;
using System;

public partial class scene_transition_prototype : Node3D
{
	private zoom_cube_spawner zoomCubeSpawner;
	private tunnel blenderScene;
	private Node3D player;
	private Camera3D playerCamera;
	private Camera3D blenderSceneCamera;
	private const float transitionSpeed = 2.0f;
	private const float transitionStartZ = -4.0f;
	private const float transitionEndZ = -0f;
	private int blenderSceneInit = 0;
	private int transitionState = 0;
	private float playerCameraStartFov = 90;
	private float blenderSceneCameraFov = 90;
	private float previousBlenderSceneZ = transitionStartZ;
	private float zProgress = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get zoom cube spawner
		zoomCubeSpawner = GetNode<zoom_cube_spawner>("ZoomCubeSpawner");

		// Configure blender scene
		blenderScene = GetNode<tunnel>("Tunnel");
		blenderScene.Scale = Vector3.One * 0.01f;
		blenderScene.Visible = false;

		blenderSceneCamera = blenderScene.GetNode<Camera3D>("Path3D/CameraPath/Camera");
		blenderSceneCameraFov = blenderSceneCamera.Fov;

		// Get player
		player = GetNode<Node3D>("PlayerContainer");
		playerCamera = GetNode<Camera3D>("PlayerContainer/Camera/Camera3D");
		playerCameraStartFov = playerCamera.Fov;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(transitionState == 0) {
			if(blenderSceneInit == 0) {
				blenderSceneInit = 1;
			}

			if(blenderSceneInit == 1) {
				blenderScene.SetProcess(false);
				blenderSceneInit = 2;
			}
			
			if(blenderScene.Position.Z > transitionStartZ) {
				zoomCubeSpawner.SetProcess(false);
				blenderScene.Visible = true;
				transitionState = 1;
			}
		}

		if(transitionState == 1) {
			//if(transitionEndZ != 0) zProgress += Math.Abs((blenderScene.Position.Z - previousBlenderSceneZ) * (transitionStartZ / (transitionStartZ - transitionEndZ))); else
			zProgress += Math.Abs(blenderScene.Position.Z - previousBlenderSceneZ);
			float transitionProgress = Math.Min(Math.Abs(zProgress / transitionStartZ), 1);
			float transitionRemainder = 1 - transitionProgress;
			previousBlenderSceneZ = blenderScene.Position.Z;
			
			if(blenderScene.Position.Z >= transitionEndZ) {
				player.Scale = 1/16f * Vector3.One;
				playerCamera.Fov = blenderSceneCameraFov;
				blenderScene.Scale = Vector3.One;

				blenderScene.Position = Vector3.Zero;
				blenderScene.SetProcess(true);
				transitionState = 2;
			} else {
				player.Position = transitionRemainder * player.Position + transitionProgress * new Vector3(player.Position.X, player.Position.Y, blenderSceneCamera.Position.Z + 5f);
				player.Scale = Math.Max(transitionRemainder, 1/16f) * Vector3.One;
				playerCamera.Fov = blenderSceneCameraFov + transitionRemainder * (playerCameraStartFov - blenderSceneCameraFov);
				blenderScene.Scale = transitionProgress * Vector3.One;
			}
		}

		if(transitionState == 2) {
			player.Position = new Vector3(player.Position.X, player.Position.Y, blenderSceneCamera.Position.Z + 5f);
		} else blenderScene.Position += new Vector3(0, 0, 1) * (float)delta * transitionSpeed;
	}
}
