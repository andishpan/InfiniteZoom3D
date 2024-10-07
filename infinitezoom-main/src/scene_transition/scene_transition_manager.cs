using Godot;
using Godot.Collections;
using System;

public partial class scene_transition_manager : Node3D
{
	[Export]
	Array<Resource> scene_list;
	
	[Export]
	scene_transition scene_transition;

	RandomNumberGenerator rng;
	int previous_end_scene_index;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng = new RandomNumberGenerator();

		int start_scene_index = rng.RandiRange(0, scene_list.Count - 1);
		var start_scene = ResourceLoader.Load<PackedScene>(scene_list[start_scene_index].ResourcePath).Instantiate();

		int end_scene_index = rng.RandiRange(0, scene_list.Count - 1);
		while(start_scene_index == end_scene_index) end_scene_index = rng.RandiRange(0, scene_list.Count - 1);
		previous_end_scene_index = end_scene_index;
		var end_scene = ResourceLoader.Load<PackedScene>(scene_list[end_scene_index].ResourcePath).Instantiate();
		
		scene_transition.AddChild(start_scene);
		scene_transition.AddChild(end_scene);
		scene_transition.GetChild<camera_sway>(1).GetChild<player_container>(0).activateShield(2, true);
		scene_transition._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(scene_transition.get_transition_state() == 2) {
			Node start_scene = scene_transition.GetChild(2);

			int end_scene_index = rng.RandiRange(0, scene_list.Count - 1);
			while(previous_end_scene_index == end_scene_index) end_scene_index = rng.RandiRange(0, scene_list.Count - 1);
			previous_end_scene_index = end_scene_index;
			var end_scene = ResourceLoader.Load<PackedScene>(scene_list[end_scene_index].ResourcePath).Instantiate();
			
			scene_transition.RemoveChild(start_scene);
			scene_transition.AddChild(end_scene);
			scene_transition._Ready();
		}
	}
}
