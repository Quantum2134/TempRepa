using System;
using OpenTK.Graphics.OpenGL4;


namespace EngineCore.Platform.OpenGL
{
    public enum WrapMode
    {
        ClampToEdge = (int)TextureWrapMode.ClampToEdge,
        Repeat = (int)TextureWrapMode.Repeat,
        MirroredRepeat = (int)TextureWrapMode.MirroredRepeat
    }

    public enum FilterMode
    {
        Nearest = (int)TextureMinFilter.Nearest,
        Linear = (int)TextureMinFilter.Linear,
    }

    public enum TextureFormat
    {
        RGB = 3,
        RGBA = 4
    }

    public struct TextureSettings
    {
        public WrapMode WrapMode;
        public FilterMode FilterMode;
        public bool GenerateMipmaps;
        public TextureFormat Format;

        public static readonly TextureSettings Default = new()
        {
            WrapMode = WrapMode.ClampToEdge,
            FilterMode = FilterMode.Linear,
            GenerateMipmaps = true,
            Format = TextureFormat.RGBA
        };

        public static TextureSettings ForRenderTarget => new()
        {
            WrapMode = WrapMode.ClampToEdge,
            FilterMode = FilterMode.Linear,
            GenerateMipmaps = false,
            Format = TextureFormat.RGBA
        };
    }

    public class Texture : IDisposable
    {
        private int _handle;
        private int _width;
        private int _height;
        private int _size;

        public int Handle => _handle;
        public int Width => _width;
        public int Height => _height;
        public int Size => _size;
        public string TextureName { get; set; }

        public readonly TextureTarget Target = TextureTarget.Texture2D;

        public WrapMode WrapMode { get; set; }
        public FilterMode FilterMode { get; set; }
        public bool GenerateMipmaps { get; set; }

        private bool _isDisposed;

        // Sprite constructor
        public Texture(int width, int height, byte[] data)
        {
            TextureSettings textureSettings = TextureSettings.Default;

            Init(width, height, data, textureSettings);
        }

        // FBO constructor
        public Texture(int width, int height)
        {
            TextureSettings textureSettings = TextureSettings.ForRenderTarget;

            Init(width, height, null, textureSettings);
        }

        private void Init(int width, int height, byte[]? data, in TextureSettings settings)
        {
            _handle = GL.GenTexture();
            _width = width;
            _height = height;
            
            _size = _width * _height * (int)settings.Format;

            Bind();

            // Определяем формат
            var (internalFormat, pixelFormat) = settings.Format switch
            {
                TextureFormat.RGB => (PixelInternalFormat.Rgb8, PixelFormat.Rgb),
                TextureFormat.RGBA => (PixelInternalFormat.Rgba8, PixelFormat.Rgba),
                _ => throw new InvalidOperationException("Unsupported format")
            };

            if (data != null)
                GL.TexImage2D(Target, 0, internalFormat, width, height, 0, pixelFormat, PixelType.UnsignedByte, data);
            else
                GL.TexImage2D(Target, 0, internalFormat, width, height, 0, pixelFormat, PixelType.UnsignedByte, IntPtr.Zero);

            // Применяем настройки
            WrapMode = settings.WrapMode;
            FilterMode = settings.FilterMode;
            GenerateMipmaps = settings.GenerateMipmaps;

            Apply();
        }

        public void Apply()
        {
            if (_isDisposed) return;
            Bind();

            // Прямой каст — безопасен, т.к. значения совпадают
            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)WrapMode);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)WrapMode);

            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)FilterMode);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)FilterMode);

            if (GenerateMipmaps)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Bind(TextureUnit unit = TextureUnit.Texture0)
        {
            if (_isDisposed) return;
            GL.ActiveTexture(unit);
            GL.BindTexture(Target, Handle);
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            GL.DeleteTexture(Handle);
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public override bool Equals(object? obj) => obj is Texture t && Handle == t.Handle;
        public override int GetHashCode() => Handle;
    }
}