using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.EditorPanels
{
    internal abstract class EditorPanel
    {
        private bool isVisible;
        public string Title { get; protected set; }
        public bool IsVisible
        {
            get => isVisible;
            set => isVisible = value;
        }

        protected EditorPanel(string title)
        {
            Title = title;
            isVisible = true;
        }

        public void Render()
        {
            if (!IsVisible) return;

            if (ImGui.Begin(Title, ref isVisible))
            {
                OnRender();
            }
            ImGui.End();
        }

        protected abstract void OnRender();
    }
}
