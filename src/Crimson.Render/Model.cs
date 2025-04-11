using System.Numerics;
using Crimson.Math;
using SharpGLTF.Schema2;

namespace Crimson.Render;

public class Model
{
    public Mesh[] Meshes;

    public Material[] Materials;

    public Model(Mesh[] meshes, Material[] materials)
    {
        Meshes = meshes;
        Materials = materials;
    }

    public static Model FromGltf(Graphics graphics, string path)
    {
        ModelRoot root = ModelRoot.Load(path);

        Dictionary<SharpGLTF.Schema2.Material, Material> materialMap = [];

        foreach (SharpGLTF.Schema2.Material material in root.LogicalMaterials)
        {
            MaterialDefinition definition = new MaterialDefinition();
            
            if (material.FindChannel("BaseColor") is { } baseColor)
            {
                Image primaryImage = baseColor.Texture.PrimaryImage;
                Texture albedo = new Texture(graphics, new Bitmap(primaryImage.Content.Content.ToArray()));
                definition.Albedo = albedo;
            }
            
            materialMap.Add(material, new Material(graphics, in definition));
        }

        List<Mesh> meshes = [];
        
        foreach (Node node in root.DefaultScene.VisualChildren)
        {
            SharpGLTF.Schema2.Mesh mesh = node.Mesh;

            foreach (MeshPrimitive primitive in mesh.Primitives)
            {
                IList<Vector3> positions = primitive.GetVertexAccessor("POSITION").AsVector3Array();
                IList<Vector2> texCoords = primitive.GetVertexAccessor("TEXCOORD_0").AsVector2Array();
                
                Vertex[] vertices = new Vertex[positions.Count];

                for (int i = 0; i < positions.Count; i++)
                    vertices[i] = new Vertex(positions[i], texCoords[i], Color.White, Vector3.Zero);
                
                meshes.Add(new Mesh(vertices, primitive.GetIndices().ToArray(), materialMap[primitive.Material]));
            }
        }

        return new Model(meshes.ToArray(), materialMap.Values.ToArray());
    }
}