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
        public GraphicsSystem GraphicsSystem {  get; private set; }

        public StateManager StateManager {  get; private set; }
        public ShaderManager ShaderManager { get; private set; }
        public TextureManager TextureManager { get; private set; }

        public GraphicsContext(GraphicsSystem graphicsSystem)
        {
            GraphicsSystem = graphicsSystem;

            StateManager = new StateManager();
            ShaderManager = new ShaderManager(this);
            TextureManager = new TextureManager(this);
        }

    }
}
