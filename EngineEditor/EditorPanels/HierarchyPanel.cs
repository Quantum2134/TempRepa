using EngineCore.ECS;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.EditorPanels
{
    internal class HierarchyPanel : EditorPanel
    {
        private Scene _scene;
        private Entity _selectedEntity;


        public event Action<Entity> OnEntitySelected;

        public HierarchyPanel(Scene scene) : base("Scene Hierarchy")
        {
            _scene = scene;
        }

        protected override void OnRender()
        {
            foreach (var entity in _scene.entities)
            {
                var isSelected = _selectedEntity == entity;

                if (ImGui.Selectable(entity.Name, isSelected, ImGuiSelectableFlags.None))
                {
                    _selectedEntity = entity;
                    OnEntitySelected?.Invoke(entity);
                }
            }
        }

        public void SetSelectedEntity(Entity entity)
        {
            _selectedEntity = entity;
        }

        public Entity GetSelectedEntity()
        {
            return _selectedEntity;
        }
    }
}
