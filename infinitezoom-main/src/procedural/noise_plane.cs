using Godot;
using System;

public partial class noise_plane : CsgMesh3D
{
    public override void _Ready()
    {
        GD.Randomize();
        FastNoiseLite noiseGenerator = new FastNoiseLite();
        noiseGenerator.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        noiseGenerator.Seed = (int)GD.Randi();

        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoTexture = ImageTexture.CreateFromImage(noiseGenerator.GetImage(500, 500));
        Material = material;
    }

    public override void _Process(double delta)
    {
    }
}
