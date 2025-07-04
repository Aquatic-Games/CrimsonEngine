using System.Text;
using Crimson.Content;
using Crimson.Graphics.Renderers;
using Crimson.Math;
using FreeTypeSharp;
using static FreeTypeSharp.FT;

namespace Crimson.Graphics;

public unsafe class Font : IContentResource<Font>, IDisposable
{
    private readonly FT_FaceRec_* _face;
    private readonly List<Texture> _fontAtlases;
    private readonly Dictionary<(char c, uint size), Character> _characters;

    private Vector2T<int> _currentRegionOffset;
    
    public Font(string path)
    {
        byte[] pathBytes = Encoding.UTF8.GetBytes(path);
        
        fixed (byte* pPath = pathBytes)
        fixed (FT_FaceRec_** face = &_face)
            FT_New_Face(_library, pPath, 0, face).Check("New face");

        _fontAtlases = [new Texture(AtlasSize, null, PixelFormat.RGBA8)];
        _characters = [];
    }

    public Size<int> MeasureText(string text, uint size)
    {
        int currentWidth = 0;
        int width = 0;
        int height = 0;

        foreach (char c in text)
        {
            Character character = GetCharacter(c, size);

            if (c == '\n')
            {
                height += (int) size;
                currentWidth = 0;
                continue;
            }

            if (character.LineHeight > height)
                height =  character.LineHeight;

            currentWidth += character.Advance;
            if (currentWidth > width)
                width = currentWidth;
        }
        
        return new Size<int>(width, height);
    }

    internal void Draw(TextureBatcher batcher, Vector2T<int> position, uint size, string text, Color color)
    {
        Vector2T<int> currentPos = position;
        
        foreach (char c in text)
        {
            if (c == '\n')
            {
                currentPos = new Vector2T<int>(position.X, currentPos.Y + (int) size);
                continue;
            }
            
            Character character = GetCharacter(c, size);

            Vector2T<int> pos = currentPos +
                                new Vector2T<int>(character.Bearing.X, -character.Bearing.Y + character.Ascender);
            
            DrawCharacter(batcher, character, pos, color);

            currentPos += new Vector2T<int>(character.Advance, 0);
        }
    }
    
    public void Dispose()
    {
        FT_Done_Face(_face).Check("Done face");
        
        foreach (Texture texture in _fontAtlases)
            texture.Dispose();
        
        _fontAtlases.Clear();
        _characters.Clear();
    }

    private void DrawCharacter(TextureBatcher batcher, Character c, Vector2T<int> position, Color color)
    {
        Size<int> size = c.Region.Size;

        Vector2T<float> topLeft = position.As<float>();
        Vector2T<float> topRight = new Vector2T<float>(position.X + size.Width, position.Y);
        Vector2T<float> bottomLeft = new Vector2T<float>(position.X, position.Y + size.Height);
        Vector2T<float> bottomRight = new Vector2T<float>(position.X + size.Width, position.Y + size.Height);

        batcher.AddToDrawQueue(new TextureBatcher.Draw(c.Texture, topLeft, topRight, bottomLeft, bottomRight, c.Region,
            color));
    }

    private Character GetCharacter(char c, uint size)
    {
        if (!_characters.TryGetValue((c, size), out Character character))
        {
            FT_GlyphSlotRec_* slot = _face->glyph;
            
            FT_Set_Pixel_Sizes(_face, 0, size).Check("Set pixel sizes");
            FT_Load_Char(_face, c, FT_LOAD.FT_LOAD_RENDER).Check("Load char");

            Size<int> glyphSize = new Size<int>((int) slot->bitmap.width, (int) slot->bitmap.rows);

            if (_currentRegionOffset.X + glyphSize.Width >= AtlasSize.Width)
            {
                _currentRegionOffset = new Vector2T<int>(0, (int) (_currentRegionOffset.Y + glyphSize.Height));
                if (_currentRegionOffset.Y + glyphSize.Height >= AtlasSize.Height)
                {
                    _currentRegionOffset = Vector2T<int>.Zero;
                    _fontAtlases.Add(new Texture(AtlasSize, null, PixelFormat.RGBA8));
                }
            }

            Rectangle<int> region = new Rectangle<int>(_currentRegionOffset, glyphSize);
            _currentRegionOffset += new Vector2T<int>(glyphSize.Width, 0);
            
            Texture texture = _fontAtlases[^1];
            
            // The texture can only be updated if the glyph size is not 0.
            if (glyphSize != Size<int>.Zero)
            {
                byte[] bitmapBytes = new byte[glyphSize.Width * glyphSize.Height * 4];

                // Convert to RGBA texture.
                // TODO: PixelFormat.A8 or something? Or have it directly in the shader?
                for (int y = 0; y < glyphSize.Height; y++)
                {
                    for (int x = 0; x < glyphSize.Width; x++)
                    {
                        int location = y * glyphSize.Width + x;

                        bitmapBytes[(location * 4) + 0] = 255;
                        bitmapBytes[(location * 4) + 1] = 255;
                        bitmapBytes[(location * 4) + 2] = 255;
                        bitmapBytes[(location * 4) + 3] = slot->bitmap.buffer[location];
                    }
                }

                texture.Update(region, bitmapBytes);
            }

            character = new Character(texture, region, new Vector2T<int>(slot->bitmap_left, slot->bitmap_top),
                (int) (slot->advance.x >> 6), (int) (_face->size->metrics.ascender >> 6),
                (int) (_face->size->metrics.height >> 6));
            _characters.Add((c, size), character);
        }

        return character;
    }

    private static readonly FT_LibraryRec_* _library;

    /// <summary>
    /// Changes the size of each font atlas. Increasing the size will allow you to fit more text on a single atlas, but
    /// may produce more waste when more than one atlas is used. There should not normally be a need to change this value.
    /// </summary>
    public static Size<int> AtlasSize;

    static Font()
    {
        fixed (FT_LibraryRec_** library = &_library)
            FT_Init_FreeType(library).Check("Init freetype");

        AtlasSize = new Size<int>(1024);
    }

    private readonly struct Character
    {
        public readonly Texture Texture;
        public readonly Rectangle<int> Region;
        public readonly Vector2T<int> Bearing;
        public readonly int Advance;

        public readonly int Ascender;
        public readonly int LineHeight;
        
        public Character(Texture texture, Rectangle<int> region, Vector2T<int> bearing, int advance, int ascender, int lineHeight)
        {
            Texture = texture;
            Region = region;
            Bearing = bearing;
            Advance = advance;
            Ascender = ascender;
            LineHeight = lineHeight;
        }
    }

    public static Font LoadResource(string fullPath, bool hasExtension)
    {
        if (hasExtension)
            return new Font(fullPath);
        
        ReadOnlySpan<string> acceptedExtensions = [".ttf", ".odt"];

        foreach (string extension in acceptedExtensions)
        {
            fullPath = Path.ChangeExtension(fullPath, extension);
            if (File.Exists(fullPath))
                return new Font(fullPath);
        }

        throw new FileNotFoundException(
            $"No font with the accepted extensions ({string.Join(", ", acceptedExtensions!)}) was found.");
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