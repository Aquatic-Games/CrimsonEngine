using System.Numerics;
using Assimp;
using Crimson.Math;
using Material = Crimson.Graphics.Materials.Material;

namespace Crimson.Graphics;

public class Model
{
    public List<Mesh> Meshes;
    public List<Material> Materials;
    
    public Model(string path)
    {
        using AssimpContext context = new AssimpContext();
        Scene scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals);

        Meshes = [];
        foreach (Assimp.Mesh aMesh in scene.Meshes)
        {
            Vertex[] vertices = new Vertex[aMesh.VertexCount];
            uint[] indices = [];
            for (int i = 0; i < aMesh.VertexCount; i++)
            {
                Vector3 position = aMesh.Vertices[i];
                Vector2 texCoord = Vector2.Zero;
                Color color = Color.White;
                Vector3 normal = Vector3.Zero;
                
                if (aMesh.TextureCoordinateChannelCount > 0)
                {
                    Vector3 texCoord3d = aMesh.TextureCoordinateChannels[0][i];
                    texCoord = new Vector2(texCoord3d.X, texCoord3d.Y);
                }

                if (aMesh.VertexColorChannelCount > 0)
                {
                    Vector4 texColor = aMesh.VertexColorChannels[0][i];
                    color = new Color(texColor.X, texColor.Y, texColor.Z, texColor.W);
                }

                if (aMesh.HasNormals)
                    normal = aMesh.Normals[i];
                
                vertices[i] = new Vertex(position, texCoord, color, normal);
            }

            if (!aMesh.HasFaces)
                throw new NotImplementedException();

            foreach (Face face in aMesh.Faces)
            {
                if (!face.HasIndices)
                    throw new NotImplementedException();

                int arrayIndex = indices.Length;
                Array.Resize(ref indices, indices.Length + face.IndexCount);
                foreach (int index in face.Indices)
                    indices[arrayIndex++] = (uint) index;
            }
            
            Meshes.Add(new Mesh(vertices.ToArray(), indices.ToArray(), null));
        }
        
        ProcessNode(scene, scene.RootNode);
    }

    private void ProcessNode(Scene scene, Node node)
    {
        if (node.HasChildren)
        {
            foreach (Node childNode in node.Children)
                ProcessNode(scene, childNode);
        }

        if (!node.HasMeshes)
            return;
        
        //foreach (int meshIndex in node.MeshIndices)
            
    }
}