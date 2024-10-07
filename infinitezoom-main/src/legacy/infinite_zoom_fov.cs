using Godot;
using System;

public partial class infinite_zoom_fov : Node3D
{
	[Export]
	private float zoomSpeed = 5.0f; 

	private Camera3D camera;
	private Node3D[] zoomObjects;
	private float initialZ = -50.0f; 

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Camera");

		zoomObjects = new Node3D[3];
		zoomObjects[0] = GetNode<Node3D>("Cylinder");
		zoomObjects[1] = GetNode<Node3D>("Prism");
		zoomObjects[2] = GetNode<Node3D>("Cube");

		
		for (int i = 0; i < zoomObjects.Length; i++)
		{
			var basis = new Basis();
			var position = new Vector3(0, 0, initialZ);
			zoomObjects[i].GlobalTransform = new Transform3D(basis, position);
			zoomObjects[i].Scale = new Vector3(1 - 0.2f * i, 1 - 0.2f * i, 1 - 0.2f * i);
		}
	}

	public override void _Process(double delta)
	{
		foreach (var obj in zoomObjects)
		{
			Transform3D transform = obj.GlobalTransform;
			
			Vector3 newPosition = transform.Origin;
			newPosition.Z += zoomSpeed * (float)delta; 

			
			float scaleIncrease = 1.0f + zoomSpeed * (float)delta / 100.0f;
			Vector3 newScale = obj.Scale * scaleIncrease;

			
			if (newPosition.Z >= 0)
			{
				newPosition.Z = initialZ;
				newScale = new Vector3(1 - 0.2f * Array.IndexOf(zoomObjects, obj), 1 - 0.2f * Array.IndexOf(zoomObjects, obj), 1 - 0.2f * Array.IndexOf(zoomObjects, obj));
			}

			obj.GlobalTransform = new Transform3D(transform.Basis, newPosition);
			obj.Scale = newScale;
		}
	}
}
