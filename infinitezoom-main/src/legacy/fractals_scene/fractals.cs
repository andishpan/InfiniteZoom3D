using Godot;
using System;

public partial class fractals : Node3D
{

	int width = 200;
	int height = 200;
	MeshInstance3D mesh;
	Camera3D camera;

	int maxIterations = 100;

	Color[,] colors;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		colors = new Color[width, height];

		colors = calculateFractals(colors);
						
		Random r = new Random();

		mesh = GetNode<MeshInstance3D>("MeshInstance3D");

		camera = GetNode<Camera3D>("Camera3D");

		//camera.Position = new Vector3(width/2, 100, height/2);

		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				var newMesh = mesh.Duplicate() as MeshInstance3D;
				newMesh.Position = new Vector3(x, 0, y);

                StandardMaterial3D mat = new StandardMaterial3D
                {
                    AlbedoColor = Color.FromHsv(colors[x, y].R, colors[x, y].G, colors[x, y].B),
                };
                mesh.MaterialOverride = mat;
				newMesh.Scale = new Vector3(1, 1, colors[x,y].R * 100);

				newMesh.SetSurfaceOverrideMaterial(0, mat);
				AddChild(newMesh);
			}
		}
	}

	public override void _Process(double delta)
	{
	}

	Color[,] calculateFractals(Color[,] colors)
	{
		float x0 = -2.0f;
		float x1 = 0.47f;
		float y0 = -1.12f;
		float y1 = 1.12f;

		for(int px = 0; px < width; px++){
			for(int py = 0; py < height; py++){
				float x0_scaled = x0 + (x1 - x0) * px / width;
				float y0_scaled = y0 + (y1 - y0) * py / height;
                float x = 0;
                float y = 0;
				int iteration = 0;
				while ((x*x) + (y*y) <= 4 && iteration < maxIterations){					
					float x_temp = (x*x) - (y*y) + x0_scaled;
					y = (2*x*y) + y0_scaled;
					x = x_temp;
					iteration++;
				}
				colors[px, py] = getHsvColorFromIteration(iteration);
			}
		}
		return colors;
	}


	Color getHsvColorFromIteration(int iteration){
		GD.Print(iteration +" " + maxIterations);
		if(iteration == maxIterations){
			return new Color(0, 0, 0);
		}
		float hue = (float)iteration / maxIterations;
		return new Color(hue, 1, 1);
	}

}

