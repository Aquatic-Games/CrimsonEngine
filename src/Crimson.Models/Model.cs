using Assimp;

namespace Crimson.Models;

public class Model
{
    public Model(string path)
    {
        using AssimpContext context = new AssimpContext();
        Scene scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals);
    }
}