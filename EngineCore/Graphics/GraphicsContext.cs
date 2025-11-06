using EngineCore.Graphics.GraphicsManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    public class GraphicsContext
    {
        public StateManager StateManager {  get; private set; }
        public ShaderManager ShaderManager { get; private set; }
        public TextureManager TextureManager { get; private set; }

        public GraphicsContext()
        {
            StateManager = new StateManager();
            ShaderManager = new ShaderManager();
            TextureManager = new TextureManager();
        }

    }
}
