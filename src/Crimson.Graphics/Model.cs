using System.Diagnostics;
using System.Numerics;
using Crimson.Graphics.Materials;
using Crimson.Math;
using SharpGLTF.Schema2;
using Material = Crimson.Graphics.Materials.Material;

namespace Crimson.Graphics;

public class Model
{
    public ModelMesh[] Meshes;

    public Material[] Materials;

    public Model(ModelMesh[] meshes, Material[] materials)
    {
        Meshes = meshes;
        Materials = materials;
    }

    public static Model FromGltf(Renderer renderer, string path)
    {
        ModelRoot root = ModelRoot.Load(path);

        Dictionary<SharpGLTF.Schema2.Material, Material> materialMap = [];

        foreach (SharpGLTF.Schema2.Material material in root.LogicalMaterials)
        {
            MaterialDefinition definition = new MaterialDefinition();
            
            if (material.FindChannel("BaseColor") is { } baseColor)
            {
                Image? primaryImage = baseColor.Texture?.PrimaryImage;

                if (primaryImage != null)
                {
                    Texture albedo = new Texture(renderer, new Bitmap(primaryImage.Content.Content.ToArray()));
                    definition.Albedo = albedo;
                }
            }
            
            materialMap.Add(material, new Material(renderer, in definition));
        }

        List<ModelMesh> meshes = [];

        foreach (Node node in root.DefaultScene.VisualChildren)
            meshes.Add(ProcessNode(node, materialMap));

        return new Model(meshes.ToArray(), materialMap.Values.ToArray());
    }

    private static ModelMesh ProcessNode(Node node, Dictionary<SharpGLTF.Schema2.Material, Material> materialMap)
    {
        SharpGLTF.Schema2.Mesh mesh = node.Mesh;
        
        List<Mesh> meshes = [];

        if (mesh != null)
        {
            foreach (MeshPrimitive primitive in mesh.Primitives)
            {
                IList<Vector3> positions = primitive.GetVertexAccessor("POSITION").AsVector3Array();
                IList<Vector2>? texCoords = primitive.GetVertexAccessor("TEXCOORD_0")?.AsVector2Array();
                IList<Vector3>? normals = primitive.GetVertexAccessor("NORMAL")?.AsVector3Array();

                Debug.Assert(texCoords == null || texCoords.Count == positions.Count);
                Debug.Assert(normals == null || normals.Count == positions.Count);

                Vertex[] vertices = new Vertex[positions.Count];

                for (int i = 0; i < positions.Count; i++)
                {
                    vertices[i] = new Vertex
                    {
                        Position = positions[i],
                        TexCoord = texCoords == null ? Vector2.Zero : texCoords[i],
                        Color = Color.White,
                        Normal = normals == null ? Vector3.Zero : normals[i]
                    };
                }

                Material material = materialMap[primitive.Material];
                uint[]? indices = primitive.GetIndices()?.ToArray();

                // Crimson can't handle meshes without indices, so fake indices here by just having an incrementing index
                // buffer.
                // TODO: Crimson should natively support missing indices.
                if (indices == null)
                {
                    indices = new uint[positions.Count];

                    for (uint i = 0; i < indices.Length; i++)
                        indices[i] = i;
                }

                meshes.Add(new Mesh(vertices, indices, material));
            }
        }

        ModelMesh modelMesh = new ModelMesh(meshes, node.LocalMatrix);

        foreach (Node childNode in node.VisualChildren)
            modelMesh.Children.Add(ProcessNode(childNode, materialMap));

        return modelMesh;
    }
}