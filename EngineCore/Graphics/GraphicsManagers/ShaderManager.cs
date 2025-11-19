using EngineCore.Platform.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EngineCore.Assets.AssetTypes;
using EngineCore.Logging;

namespace EngineCore.Graphics.GraphicsManagers
{
    public class ShaderManager
    {
        private GraphicsContext graphicsContext;

        private Dictionary<string, ShaderProgram> _shaders;

        public ShaderManager(GraphicsContext graphicsContext)
        {
            this.graphicsContext = graphicsContext;

            _shaders = new Dictionary<string, ShaderProgram>();
        }

        public ShaderProgram GetShader(string name)
        {
            if(_shaders.TryGetValue(name, out var shaderProgram)) return shaderProgram;

            return null;
        }

        public ShaderProgram LoadShader(string shaderProgramName, string vertexShaderPath, string fragmentShaderPath)
        {
            if(_shaders.ContainsKey(shaderProgramName)) return _shaders[shaderProgramName];

            ShaderAsset vertexShaderAsset = graphicsContext.GraphicsSystem.Application.AssetManager.LoadAsset<ShaderAsset>(vertexShaderPath);
            ShaderAsset fragmentShaderAsset = graphicsContext.GraphicsSystem.Application.AssetManager.LoadAsset<ShaderAsset>(fragmentShaderPath);

            if (vertexShaderAsset == null || fragmentShaderAsset == null)
            {
                Logger.Log($"Could not load shader assets for {shaderProgramName}", LogLevel.Error);
                return null;
            }

            ShaderProgram shader = new ShaderProgram(vertexShaderAsset.ShaderSource, fragmentShaderAsset.ShaderSource);
            _shaders.Add(shaderProgramName, shader);

            Logger.Log($"Shader program {shaderProgramName} created successfully", LogLevel.Info);

            return shader;
        }

        public void RemoveShader(string name)
        {
            if(_shaders.TryGetValue(name, out var shaderProgram))
            {
                _shaders.Remove(name);               
            }
        }
    }
}
