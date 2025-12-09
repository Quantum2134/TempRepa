using EngineCore.Core;
using EngineCore.ECS;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.UI.EditorPanels
{
    internal class HierarchyPanel : EditorPanel
    {
        private Scene _scene;
        private Entity _selectedEntity;


        public event Action<Entity> OnEntitySelected;

        public HierarchyPanel() : base("Scene Hierarchy")
        {          
        }

        public void SetScene(Scene scene)
        {
            _scene = scene;
        }

        protected override void OnRender()
        {
            if(_scene != null)
            {
                // Кнопка "Create Entity" над списком
                if (ImGui.Button("Create Entity"))
                {
                    _scene.AddEntity(new Entity("New entity")); // Должен возвращать Entity
                                                                // Опционально: сразу выбрать новую сущность
                                                                // SetSelectedEntity(newEntity);
                                                                // OnEntitySelected?.Invoke(newEntity);
                }

                ImGui.Separator(); // визуальное отделение кнопки от списка



                //for(int i = 0; i < _scene.Entities.Count; i++)
                //{
                //    Entity entity = _scene.Entities[i];
                //    var isSelected = _selectedEntity == entity;
                //    ImGui.PushID(i);
                //    if (ImGui.Selectable(entity.Name, isSelected, ImGuiSelectableFlags.None))
                //    {
                //        _selectedEntity = entity;
                //        OnEntitySelected?.Invoke(entity);
                //    }
                //    ImGui.PopID();
                //}

                bool entityDeleted = false;
                Entity deletedEntity = null;

                // Используем ToList() для безопасного удаления
                var entitiesSnapshot = _scene.Entities.ToList();

                for (int i = 0; i < entitiesSnapshot.Count; i++)
                {
                    Entity entity = entitiesSnapshot[i];
                    var isSelected = _selectedEntity == entity;

                    ImGui.PushID(i); // Уникальный ID по индексу — отлично!

                    if (ImGui.Selectable(entity.Name, isSelected))
                    {
                        _selectedEntity = entity;
                        OnEntitySelected?.Invoke(entity);
                    }

                    // Контекстное меню по ПКМ на этом элементе
                    if (ImGui.BeginPopupContextItem())
                    {
                        if (ImGui.Selectable("Delete"))
                        {
                            deletedEntity = entity;
                            entityDeleted = true;
                        }
                        ImGui.EndPopup();
                    }

                    ImGui.PopID();
                }

                // Удаляем сущность после завершения цикла
                if (entityDeleted)
                {
                    _scene.RemoveEntity(deletedEntity); // ← убедитесь, что метод называется именно так

                    if (_selectedEntity == deletedEntity)
                    {
                        _selectedEntity = null;
                        OnEntitySelected?.Invoke(null);
                    }
                }
            }
            else
            {
                ImGui.Text("No scene");
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
