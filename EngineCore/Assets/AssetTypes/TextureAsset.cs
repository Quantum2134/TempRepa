using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using EngineCore.Assets.AssetTypes;

namespace EngineCore.Assets.Assets
{
    public class TextureAsset : Asset, IDisposable
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] PixelData { get; set; }
        
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Освобождаем массив пикселей
                PixelData = null;
                _disposed = true;
            }
        }
    }
}
