using System.Numerics;

namespace Crimson.Graphics;

public class ModelMesh
{
    public List<ModelMesh> Children;

    public List<Mesh> Meshes;

    public Matrix4x4 Transform;

    public ModelMesh(List<Mesh> meshes, Matrix4x4 transform)
    {
        Children = [];
        Meshes = meshes;
        Transform = transform;
    }
}