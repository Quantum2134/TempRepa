using EngineCore.Platform.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            //return _shaders.TryGetValue(name, out var shaderProgram) ? shaderProgram : null;

            if(_shaders.TryGetValue(name, out var shaderProgram))
            {
                Console.WriteLine("Good");
                return shaderProgram;
            }
            else
            {
                return null;
            }
        }

        public ShaderProgram LoadShader(string name, string vertexPath, string fragmentPath)
        {
            if(_shaders.ContainsKey(name)) return _shaders[name];
           
            ShaderProgram shader = new ShaderProgram(vertexPath, fragmentPath);
            _shaders.Add(name, shader);

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
