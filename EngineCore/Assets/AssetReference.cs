using EngineCore.Assets.AssetTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngineCore.Assets
{
    internal class AssetReference
    {
        public Asset Asset { get; private set; }
        public int ReferenceCount { get; private set; }
        public string AssetPath { get; private set; }

        public AssetReference(Asset asset, string assetPath)
        {
            Asset = asset;
            AssetPath = assetPath;
            ReferenceCount = 1;
        }

        public void AddReference()
        {
            ReferenceCount++;
        }

        public bool RemoveReference()
        {
            ReferenceCount--;
            return ReferenceCount <= 0;
        }
    }
}