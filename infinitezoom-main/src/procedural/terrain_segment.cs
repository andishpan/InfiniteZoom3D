using Godot;
using System;

public partial class terrain_segment : Node3D
{
    private FastNoiseLite noiseGenerator;

    [Export]
    private uint samples = 9;

    [Export]
    private float segment_size = 8f;

    [Export]
    private float noise_intensity = 12f;

    public override void _Ready()
    {
        GD.Randomize();
        noiseGenerator = new FastNoiseLite();
        noiseGenerator.Seed = (int)GD.Randi();
        noiseGenerator.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;

        var strip = generateStrip(0f);
        var meshNode = new MeshInstance3D();
        meshNode.Mesh = strip;
        AddChild(meshNode);

        var strip2 = generateStrip(segment_size);
        var meshNode2 = new MeshInstance3D();
        meshNode2.Mesh = strip2;
        AddChild(meshNode2);
    }

    public ArrayMesh generateStrip(float z)
    {
        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);

        float x = (-segment_size * (float)(samples - 2)) / 2;
        for (int vert = 0; vert < samples * 2; vert++)
        {
            if (vert > 1)
            {
                if (vert % 2 == 0)
                {
                    x += segment_size;
                    st.AddIndex(vert - 2);
                    st.AddIndex(vert - 1);
                }
                else
                {
                    st.AddIndex(vert - 1);
                    st.AddIndex(vert - 2);
                }

                st.AddIndex(vert);
            }

            float z_offset = segment_size * (1 - vert % 2);
            st.AddVertex(new Vector3(
                x,
                noiseGenerator.GetNoise2D(x, z + z_offset) * segment_size,
                z + z_offset
            ));
        }

        st.GenerateNormals();

        return st.Commit();
    }

    public override void _Process(double delta)
    {
    }
}
