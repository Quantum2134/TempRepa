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
        private Dictionary<string, ShaderProgram> _shaders;

        public ShaderManager()
        {
            _shaders = new Dictionary<string, ShaderProgram>();
        }

        public ShaderProgram GetShader(string name)
        {
            if(_shaders.TryGetValue(name, out var shaderProgram)) return shaderProgram;

            return null;
        }

        public ShaderProgram LoadShader(string shaderProgramName, ShaderAsset vertex, ShaderAsset fragment)
        {
            if(_shaders.ContainsKey(shaderProgramName)) return _shaders[shaderProgramName];       

            ShaderProgram shader = new ShaderProgram(vertex.ShaderSource, fragment.ShaderSource);
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
