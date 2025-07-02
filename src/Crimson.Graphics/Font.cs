using System.Text;
using FreeTypeSharp;
using static FreeTypeSharp.FT;

namespace Crimson.Graphics;

public unsafe class Font : IDisposable
{
    private readonly FT_FaceRec_* _face;
    
    public Font(string path)
    {
        byte[] pathBytes = Encoding.UTF8.GetBytes(path);
        
        fixed (byte* pPath = pathBytes)
        fixed (FT_FaceRec_** face = &_face)
            FT_New_Face(_library, pPath, 0, face).Check("New face");
    }
    
    public void Dispose()
    {
        FT_Done_Face(_face).Check("Done face");
    }

    private static FT_LibraryRec_* _library;

    static Font()
    {
        fixed (FT_LibraryRec_** library = &_library)
            FT_Init_FreeType(library).Check("Init freetype");
    }
}

file static class FreeTypeExtensions
{
    public static void Check(this FT_Error error, string operation)
    {
        if (error != FT_Error.FT_Err_Ok)
            throw new Exception($"FreeType operation '{operation}' failed: {error}");
    }
}