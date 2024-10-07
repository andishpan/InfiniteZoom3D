using Godot;
using Godot.Collections;
using System;
using System.IO;
using System.Runtime.CompilerServices;

public partial class scene_transition  : Node3D
{
	// nodes
	private Label transitionMessage;
	private player_container player;
	private Node3D start_scene;
	private Node3D end_scene;

	// cameras
	private Camera3D player_camera;
	private Camera3D start_scene_camera;
	private Camera3D end_scene_camera;

	// lights
	Array<Node> start_scene_lights;
	Array<Node> end_scene_lights;
	float[] start_scene_lights_energies;
	float[] end_scene_lights_energies;

	// environments
	private Godot.Environment player_environment;
	private Godot.Environment start_scene_environment;
	private Godot.Environment end_scene_environment;

	// transition parameters
	private float transition_timer;
	private float transition_duration;
	private float start_fov;
	private float end_fov;
	private string start_scene_camera_path;
	private string end_scene_camera_path;

	// scale parameters
	private float player_initial_scale;
	private float start_scene_initial_scale;
	private float end_scene_initial_scale;
	private float player_target_scale;
	private float start_scene_target_scale;
	private float end_scene_target_scale;

	// player parameters
	private float player_initial_speed;
	private float player_initial_accel;
	private float player_initial_movement_bound;
	private float player_target_speed;
	private float player_target_accel;
	private float player_target_movement_bound;

	// variables
	private int transition_state;
	private float passed_transition_time;
	private float end_scene_initial_z;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// get nodes
		transitionMessage = GetChildOrNull<Label>(0);
		camera_sway camera_sway_node = GetChildOrNull<camera_sway>(1);
		if(camera_sway_node != null) player = camera_sway_node.GetChildOrNull<player_container>(0);
		start_scene = GetChildOrNull<Node3D>(2);
		end_scene = GetChildOrNull<Node3D>(3);

		//check nodes 
		if(transitionMessage == null || player == null || start_scene == null || end_scene == null) {
			SetProcess(false);
		} else {
			// initialize variables
			transition_state = 0;
			passed_transition_time = 0;
			end_scene_initial_z = 0;
			
			// get transition parameters
			transition_timer = (float) GetMeta("transition_timer");
			transition_duration = (float) GetMeta("transition_duration");
			start_fov = (float) GetMeta("start_fov");
			end_fov = (float) GetMeta("end_fov");
			start_scene_camera_path = (string) GetMeta("start_scene_camera_path");
			end_scene_camera_path = (string) GetMeta("end_scene_camera_path");

			// adjust transition timer with random time
			if(transition_timer == 0) {
				RandomNumberGenerator rng;
				rng = new RandomNumberGenerator();
				
				int max_time = 30;
				if(start_scene.Name.Equals("InfiniteZoom")) max_time = 17;
				if(start_scene.Name.Equals("MushroomSpawner")) max_time = 15;
				
				GD.Print(start_scene.Name);
				
				transition_timer = rng.RandfRange(Math.Min(10, max_time), max_time);
			}

			// get scale parameters
			player_initial_scale = (float) GetMeta("player_initial_scale");
			start_scene_initial_scale = (start_scene.Scale.X + start_scene.Scale.Y + start_scene.Scale.Z) / 3 * (float) GetMeta("start_scene_initial_scale");
			end_scene_initial_scale = (end_scene.Scale.X + end_scene.Scale.Y + end_scene.Scale.Z) / 3 * (float) GetMeta("end_scene_initial_scale");
			player_target_scale = (float) GetMeta("player_target_scale");
			start_scene_target_scale = (start_scene.Scale.X + start_scene.Scale.Y + start_scene.Scale.Z) / 3 * (float) GetMeta("start_scene_target_scale");
			end_scene_target_scale = (end_scene.Scale.X + end_scene.Scale.Y + end_scene.Scale.Z) / 3 * (float) GetMeta("end_scene_target_scale");

			// get lights
			start_scene_lights = start_scene.FindChildren("", "Light3D");
			end_scene_lights = end_scene.FindChildren("", "Light3D");
			start_scene_lights_energies = new float[start_scene_lights.Count];
			end_scene_lights_energies = new float[end_scene_lights.Count];

			for(int i = 0; i < start_scene_lights.Count; i++) {
				Light3D omni_light = (Light3D) start_scene_lights[i];
				start_scene_lights_energies[i] = omni_light.LightEnergy;
			}

			for(int i = 0; i < end_scene_lights.Count; i++) {
				Light3D light = (Light3D) end_scene_lights[i];
				end_scene_lights_energies[i] = light.LightEnergy;
			}

			// get player parameters
			player_initial_speed = (float) GetMeta("player_initial_speed");
			player_initial_accel = (float) GetMeta("player_initial_accel");
			player_initial_movement_bound = (float) GetMeta("player_initial_movement_bound");
			player_target_speed = (float) GetMeta("player_target_speed");
			player_target_accel = (float) GetMeta("player_target_accel");
			player_target_movement_bound = (float) GetMeta("player_target_movement_bound");

			// adjust scene scales with default values 
			if(start_scene_initial_scale <= 0) start_scene_initial_scale = (start_scene.Scale.X + start_scene.Scale.Y + start_scene.Scale.Z) / 3;
			if(end_scene_initial_scale <= 0) end_scene_initial_scale = (end_scene.Scale.X + end_scene.Scale.Y + end_scene.Scale.Z) / 3;
			if(start_scene_target_scale <= 0) start_scene_target_scale = (start_scene.Scale.X + start_scene.Scale.Y + start_scene.Scale.Z) / 3;
			if(end_scene_target_scale <= 0) end_scene_target_scale = (end_scene.Scale.X + end_scene.Scale.Y + end_scene.Scale.Z) / 3;

			// adjust camera path parameters with default values
			if(start_scene_camera_path.Equals("")) {
				//start_scene_camera_path = (string) start_scene_node.GetMeta("camera_path");
				Array<Node> cameras = start_scene.FindChildren("", "Camera3D");
				start_scene_camera = (Camera3D) cameras[0];
			} 

			if(end_scene_camera_path.Equals("")) {
				//end_scene_camera_path = (string) end_scene_node.GetMeta("camera_path");
				Array<Node> cameras = end_scene.FindChildren("", "Camera3D");
				end_scene_camera = (Camera3D) cameras[0];
			}

			// get cameras
			player_camera = player.GetNode<Camera3D>("Camera/Camera3D");

			if (start_scene.Name.Equals("WireframeCube")) player_camera.GetViewport().DebugDraw = Viewport.DebugDrawEnum.Wireframe;
			else player_camera.GetViewport().DebugDraw = Viewport.DebugDrawEnum.Disabled;

			if(!start_scene_camera_path.Equals("")) start_scene_camera = start_scene.GetNode<Camera3D>(start_scene_camera_path);
			if(!end_scene_camera_path.Equals("")) end_scene_camera = end_scene.GetNode<Camera3D>(end_scene_camera_path);

			// adjust fov parameters with default values
			if(start_fov <= 0) start_fov = start_scene_camera.Fov;
			if(end_fov <= 0) end_fov = end_scene_camera.Fov;

			// get environments
			if(start_scene.GetNodeOrNull<WorldEnvironment>("WorldEnvironment") != null) {
				WorldEnvironment worldEnvironment = (WorldEnvironment) start_scene.GetNode("WorldEnvironment").Duplicate();
				start_scene_environment = worldEnvironment.Environment;
			} else start_scene_environment = GD.Load<Godot.Environment>("res://assets/environments/default_env.tres");
			
			if(end_scene.GetNodeOrNull<WorldEnvironment>("WorldEnvironment") != null) {
				WorldEnvironment worldEnvironment = (WorldEnvironment) end_scene.GetNode("WorldEnvironment").Duplicate();
				end_scene_environment = worldEnvironment.Environment;
				
			} else end_scene_environment = GD.Load<Godot.Environment>("res://assets/environments/default_env.tres");
			//GD.Print(end_scene_environment);

			// configure start scene
			start_scene.Scale = Vector3.One * start_scene_initial_scale;

			// configure end scene
			end_scene.Scale = Vector3.One * end_scene_initial_scale;
			end_scene.Visible = false;
			end_scene.SetProcess(false);

			if(end_scene.Position.Z != 0) {
				end_scene_initial_z = end_scene.Position.Z;
			} else {
				end_scene_initial_z = -300 * end_scene_target_scale;
				end_scene.Position = new Vector3(0, 0, end_scene_initial_z);
			}

			// configure player
			player.Scale = Vector3.One * player_initial_scale;
			player.speed = player_initial_speed;
			player.accel = player_initial_accel;
			player.movement_bound = player_initial_movement_bound;
			player_camera.Fov = start_fov;
			player_camera.Current = true;

			if(start_scene_environment.AmbientLightSource == Godot.Environment.AmbientSource.Bg) start_scene_environment.AmbientLightColor = start_scene_environment.BackgroundColor;
			if(end_scene_environment.AmbientLightSource == Godot.Environment.AmbientSource.Bg) end_scene_environment.AmbientLightColor = end_scene_environment.BackgroundColor;

			player_camera.Environment = new Godot.Environment {
				BackgroundMode = Godot.Environment.BGMode.Color,
				BackgroundColor = start_scene_environment.BackgroundColor,
				BackgroundEnergyMultiplier = start_scene_environment.BackgroundEnergyMultiplier,
				AmbientLightSource = Godot.Environment.AmbientSource.Color,
				AmbientLightColor = start_scene_environment.AmbientLightColor,
				AdjustmentEnabled = true,
				AdjustmentContrast = start_scene_environment.AdjustmentContrast
			};

			// allow process method to run after everything has been succesfully initialized
			SetProcess(true);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(transition_state == 0) {
			// pre transition
			player.Position = new Vector3(player.Position.X, player.Position.Y, start_scene_camera.Position.Z * start_scene_initial_scale - 10);
			transition_timer -= (float) delta;
			
			if(transition_timer <= 0) {
				player.activateShield(transition_duration + 1, true);
				end_scene.Visible = true;
				transition_state = 1;
				GetNode("/root/SpeedController").Set("_active", false);
			}
		} else if(transition_state == 1) {
			// during transition
			passed_transition_time += (float) delta;
			float transition_progress = Math.Min(passed_transition_time / transition_duration, 1);
			float transition_remainder = 1 - transition_progress;
			int gameState = (int) GetNode("/root/GameState").Get("GAME_STATE");
			if(gameState == 1) transitionMessage.Visible = passed_transition_time % 1 > 0.5;
			
			if (transition_progress < 1) {
				// continue transition
				if(transition_progress < 0.1) {
					// reduce player movement freedom
					float player_movement_freedom = 1 - transition_progress * 9;
					player.speed = player_movement_freedom * player_initial_speed;
					player.accel = player_movement_freedom * player_initial_accel;
					player.movement_bound = player_movement_freedom * player_initial_movement_bound;
				} else if(transition_progress > 0.9) {
					// increase player movement freedom
					float player_movement_freedom = 1 - transition_remainder * 9;
					player.speed = player_movement_freedom * player_target_speed;
					player.accel = player_movement_freedom * player_target_accel;
					player.movement_bound = player_movement_freedom * player_target_movement_bound;
				}
				
				player.Position = 	transition_remainder * player.Position
								+	transition_progress * new Vector3(player.Position.X, player.Position.Y, end_scene_camera.Position.Z * end_scene_target_scale - 10);
				player.Scale = Vector3.One * (player_initial_scale * transition_remainder + player_target_scale * transition_progress);
				player_camera.Fov = end_fov + transition_remainder * (start_fov - end_fov);

				start_scene.Position = new Vector3(0, 0, -end_scene_initial_z * transition_progress);
				start_scene.Scale = Vector3.One * (start_scene_initial_scale * transition_remainder + start_scene_target_scale * transition_progress);

				end_scene.Position = new Vector3(0, 0, end_scene_initial_z * transition_remainder);
				end_scene.Scale = Vector3.One * (end_scene_initial_scale * transition_remainder + end_scene_target_scale * transition_progress);

				//adjust lights
				for(int i = 0; i < start_scene_lights.Count; i++) {
					Light3D light = (Light3D) start_scene_lights[i];
					light.LightEnergy = start_scene_lights_energies[i] * transition_remainder;
				}

				for(int i = 0; i < end_scene_lights.Count; i++) {
					Light3D light = (Light3D) end_scene_lights[i];
					light.LightEnergy = end_scene_lights_energies[i] * transition_progress;
				}

				// adjust player camera environment
				player_camera.Environment.BackgroundColor = start_scene_environment.BackgroundColor * transition_remainder
														  + end_scene_environment.BackgroundColor * transition_progress;
				player_camera.Environment.BackgroundEnergyMultiplier = start_scene_environment.BackgroundEnergyMultiplier * transition_remainder
																	 + end_scene_environment.BackgroundEnergyMultiplier * transition_progress;
				player_camera.Environment.AdjustmentContrast = start_scene_environment.AdjustmentContrast * transition_remainder
															 + end_scene_environment.AdjustmentContrast * transition_progress;
				player_camera.Environment.AmbientLightColor = start_scene_environment.AmbientLightColor * transition_remainder
															+ end_scene_environment.AmbientLightColor * transition_progress;
			} else {
				// end transition
				transitionMessage.Visible = false;
				
				player.Scale = Vector3.One * player_target_scale;
				player.speed = player_target_speed;
				player.accel = player_target_accel;
				player.movement_bound = player_target_movement_bound;
				player_camera.Fov = end_fov;

				player_camera.Environment.BackgroundColor = end_scene_environment.BackgroundColor;
				player_camera.Environment.BackgroundEnergyMultiplier = end_scene_environment.BackgroundEnergyMultiplier;
				player_camera.Environment.AdjustmentContrast = end_scene_environment.AdjustmentContrast;
				player_camera.Environment.AmbientLightColor = end_scene_environment.AmbientLightColor;

				start_scene.Visible = false;
				start_scene.SetProcess(false);

				for(int i = 0; i < end_scene_lights.Count; i++) {
					Light3D light = (Light3D) end_scene_lights[i];
					light.LightEnergy = end_scene_lights_energies[i];
				}

				end_scene.Position = Vector3.Zero;
				end_scene.Scale = Vector3.One * end_scene_target_scale;
				end_scene.SetProcess(true);
				
				transition_timer = (float) GetMeta("transition_timer");
				GetNode("/root/SpeedController").Set("_active", true);
				player.Position = new Vector3(player.Position.X, player.Position.Y, end_scene_camera.Position.Z * end_scene_target_scale - 10);
				transition_state = 2;
			}
		}

		// CharacterBody3D playerBody = (CharacterBody3D) player.GetChild(0);
		// GD.Print(playerBody.Position);
	}

	public int get_transition_state() {
		return transition_state;
	}
}
