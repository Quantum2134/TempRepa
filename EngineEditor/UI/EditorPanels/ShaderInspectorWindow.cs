using EngineCore.Platform.OpenGL;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using EngineEditor.Utils;
using EngineCore.Assets.AssetTypes;

namespace EngineEditor.UI.EditorPanels
{
    public class ShaderInspectorWindow
    {
        public bool IsOpen;
        private ShaderAsset _shader;

        private string path;
        private string name;

        public ShaderInspectorWindow()
        {
           
        }

        public void Open(ShaderAsset shader)
        {
            _shader = shader;
            if (_shader != null)
            {
                path = _shader.Path;
                name = _shader.Name;
                IsOpen = true;
            }
        }
        public void Close()
        {
            IsOpen = false;
        }
        public void Render()
        {
            if (!IsOpen) return;

            ImGui.SetNextWindowSize(new Vector2(400, 500).ToNumerics(), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Shader Inspector", ref IsOpen))
            {
                if (_shader == null)
                {
                    ImGui.Text("No shader selected.");
                }
                else
                {
                    ImGui.Text(name);
                    ImGui.Text(path);
                }

               
            }
            ImGui.End();
        }

        
    }
}
