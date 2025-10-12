using System.Numerics;
using Crimson.Engine.Entities.Components;
using Crimson.Graphics;

namespace Crimson.Engine.Entities;

public static class EntityExtensions
{
    /*public static void AddToEntity(this Model model, Entity entity)
    {
        int counter = 0;
        foreach (ModelMesh mesh in model.Meshes)
            AddMeshToEntity(mesh, entity, ref counter);
    }

    private static void AddMeshToEntity(ModelMesh mesh, Entity entity, ref int counter)
    {
        foreach (Mesh m in mesh.Meshes)
        {
            Matrix4x4.Decompose(mesh.Transform, out Vector3 scale, out Quaternion rotation, out Vector3 position);
            
            Entity child = new Entity($"Mesh{counter++}", new Transform(position, rotation)
            {
                Scale = scale
            });
            child.AddComponent(new MeshRenderer(m));
            
            entity.AddChild(child);
        }

        if (mesh.Children.Count > 0)
        {
            Entity submesh = new Entity($"Mesh{counter++}");
            entity.AddChild(submesh);
            
            foreach (ModelMesh m in mesh.Children)
                AddMeshToEntity(m, submesh, ref counter);
        }
    }*/
}