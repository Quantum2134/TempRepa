using EngineCore.Core;
using EngineCore.Platform.OpenGL;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.UI.EditorPanels
{
    internal class ViewportPanel : EditorPanel
    {
        private FrameBuffer fbo;
        private Application Application;
        public ViewportPanel(Application application) : base("Viewport")
        {
            Application = application;
        }
        public void SetFBO(FrameBuffer frameBuffer)
        {
            fbo = frameBuffer;
        }
        protected override void OnRender()
        {
            ImGui.Image(fbo.ColorTexture.Handle, new System.Numerics.Vector2(Application.ClientSize.X, Application.ClientSize.Y), new System.Numerics.Vector2(1, 0), new System.Numerics.Vector2(1, 0));
        }
    }
}
