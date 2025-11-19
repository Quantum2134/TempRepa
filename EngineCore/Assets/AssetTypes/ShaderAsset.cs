using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Assets.AssetTypes
{
    public class ShaderAsset : Asset, IDisposable
    {
        public string ShaderSource { get; set; }
        
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
                // Освобождаем строку шейдера
                ShaderSource = null;
                _disposed = true;
            }
        }
    }
}
