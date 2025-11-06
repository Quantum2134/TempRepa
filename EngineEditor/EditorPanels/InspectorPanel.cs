using EngineCore.ECS;
using EngineCore.ECS.Components;
using EngineEditor.Utils;
using ImGuiNET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.EditorPanels
{
    internal class InspectorPanel : EditorPanel
    {
        private Entity _selectedEntity;

        public InspectorPanel() : base("Inspector") { }

        public void SetSelectedEntity(Entity entity)
        {
            _selectedEntity = entity;
        }

        protected override void OnRender()
        {
            if (_selectedEntity == null)
            {
                ImGui.Text("No entity selected");
                return;
            }

            ImGui.Text($"Entity: {_selectedEntity.Name}");
            ImGui.Separator();

            var transform = _selectedEntity.transform;
            if (transform != null)
            {
                RenderTransform(transform);
            }           
        }

        private void RenderTransform(Transform transform)
        {
            ImGui.TextColored(new System.Numerics.Vector4(0.8f, 0.8f, 1.0f, 1.0f), "Transform");
            ImGui.Indent();

            // Position
            var position = transform.Position.ToNumerics();
            if (ImGui.DragFloat2("Position", ref position, 0.1f))
            {
                transform.Position = position.ToOpenTK();
            }

            // Rotation
            float rotation = transform.Rotation;
            if (ImGui.DragFloat("Rotation", ref rotation, 1f, -360f, 360f))
            {
                // Ограничиваем вращение
                transform.Rotation = MathHelper.Clamp(transform.Rotation, -360f, 360f);
            }

            // Scale
            var scale = transform.Scale.ToNumerics();
            if (ImGui.DragFloat2("Scale", ref scale, 0.01f, 0.01f, float.MaxValue))
            {
                transform.Scale = scale.ToOpenTK();
            }

            ImGui.Unindent();
        }
    }
}
