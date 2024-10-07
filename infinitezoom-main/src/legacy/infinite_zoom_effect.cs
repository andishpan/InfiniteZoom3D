/*using Godot;
using System;

public partial class InfiniteZoomEffect : Node3D
{
	[Export]
	private NodePath PlayerPath = Player ; 

	[Export]
	private float DistanceBetweenObjects = 10.0f;

	private Camera3D camera;
	private Node3D[] zoomObjects;
	private Node3D player;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Camera");
		player = GetNode<Player>(PlayerPath);

		zoomObjects = new Node3D[3];
		zoomObjects[0] = GetNode<Node3D>("Cylinder");
		zoomObjects[1] = GetNode<Node3D>("Prism");
		zoomObjects[2] = GetNode<Node3D>("Cube");

		
		float baseScale = 1.0f; 
		for (int i = 0; i < zoomObjects.Length; i++)
		{
			float scale = Mathf.Pow(3, i) / Mathf.Pow(3, zoomObjects.Length - 1);
			zoomObjects[i].Scale = new Vector3(scale, scale, scale);
		}
	}

	public override void _Process(double delta)
	{
		// Determine the current position along the infinite zoom path
		float t = Mathf.PosMod(player.Translation.z, DistanceBetweenObjects) / DistanceBetweenObjects;
		t = Mathf.Pow(t, 1.3f); // Apply bias to smooth out the transition

		
		for (int i = 0; i < zoomObjects.Length; i++)
		{
			float minZoom = Mathf.Pow(3, i) / Mathf.Pow(3, zoomObjects.Length - 1);
			float maxZoom = Mathf.Pow(3, i + 1) / Mathf.Pow(3, zoomObjects.Length - 1);
			float s = Mathf.Lerp(minZoom, maxZoom, t);
			zoomObjects[i].Scale = new Vector3(s, s, s);
		}

		
		if (t < 0.5)
		{
			

			var material = (ShaderMaterial)zoomObjects[0].GetSurfaceMaterial(0);
			material.SetShaderParam("color", new Color(1, 1, 1, t));
		}
	}
}  */
