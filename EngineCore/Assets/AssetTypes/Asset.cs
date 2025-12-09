using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace EngineCore.Assets.AssetTypes
{
    public enum AssetType
    {
        Texture = 0,
        Shader = 1,
    }
    public abstract class Asset
    {
        public string Path { get; set; }
        public string Name { get; set; }

        public AssetType Type { get; set; }
        
    }
}
