using EngineCore.Core;
using EngineCore.ECS;
using EngineCore.ECS.Components;
using EngineCore.Graphics.GraphicsManagers;
using EngineEditor.Utils;
using ImGuiNET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.UI.EditorPanels
{
    internal class InspectorPanel : EditorPanel
    {
        private Entity _selectedEntity;
        private Application Application;

        public InspectorPanel(Application application) : base("Inspector")
        {
            Application = application;
        }

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

            //name
            string currentName = _selectedEntity.Name ?? "";
            if (ImGui.InputText("##EntityName", ref currentName, 256))
            {
                _selectedEntity.Name = currentName;
            }

            ImGui.Separator();           

            //transform
            var transform = _selectedEntity.Transform;
            if (transform != null)
            {
                RenderTransform(transform);
            }

            //renderer
            var tr = _selectedEntity.GetComponents<TextureRenderer>();      
            for(int i = 0; i < tr.Length; i++)
            {
                if (tr != null)
                {
                    ImGui.Separator();

                    ImGui.PushID(i);
                    RenderSpriteRenderer(tr[i]);
                    ImGui.PopID();
                }
            }

            //script
            var scripts = _selectedEntity.GetComponents<ScriptComponent>();
            for (int i = 0; i < scripts.Length; i++)
            {
                if (scripts != null)
                {
                    ImGui.Separator();

                    ImGui.PushID(i);
                    RenderScriptComponent(scripts[i]);
                    ImGui.PopID();
                }
            }

            ImGui.Separator();

            // Add component

            List<(string, Type)> availableComponents = new List<(string, Type)>();

            
            availableComponents.Add(("Texture Renderer", typeof(TextureRenderer)));

            foreach(var script in Application.ScriptSystem.Scripts)
            {
                availableComponents.Add((script.Key, script.Value));
            }

            if (ImGui.BeginCombo("##AddComponent", "Add Component"))
            {
                foreach (var (name, type) in availableComponents)
                {
                    
                    if (ImGui.Selectable(name, false))
                    {
                        Component component = (Component)Activator.CreateInstance(type);
                        if(component is ScriptComponent)
                        {
                            ((ScriptComponent)component).name = type.Name;
                            ((ScriptComponent)component).Application = Application;
                        }
                        _selectedEntity.AddComponent(component);

                    }

                }
                ImGui.EndCombo();
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
                transform.Rotation = rotation;
            }

            // Scale
            var scale = transform.Scale.ToNumerics();
            if (ImGui.DragFloat2("Scale", ref scale, 0.01f, 0.01f, float.MaxValue))
            {
                transform.Scale = scale.ToOpenTK();
            }

            ImGui.Unindent();
        }

        private void RenderSpriteRenderer(TextureRenderer spriteRenderer)
        {
            //name
            ImGui.TextColored(new System.Numerics.Vector4(0.8f, 0.8f, 1.0f, 1.0f), "Texture Renderer");

            //delete button
            ImGui.SameLine();
            ImGui.PushID("TextureRendererRemoveButton");
            if (ImGui.Button("×", new System.Numerics.Vector2(20, 20)))
            {
                _selectedEntity.RemoveComponent<TextureRenderer>();               
            }
            ImGui.PopID();

            ImGui.Indent();


            //texture picker
            var allTextures = Application.GraphicsSystem.GraphicsContext.TextureManager.Textures;
            string[] textureNames = allTextures.Keys.ToArray();
            int currentIndex = -1;
            for (int i = 0; i < allTextures.Count; i++)
            {
                if (ReferenceEquals(allTextures.Values.ElementAt(i), spriteRenderer.Texture))
                {
                    currentIndex = i;
                    break;
                }
            }
            if (ImGui.BeginCombo("Texture", currentIndex == -1 ? "<None>" : textureNames[currentIndex]))
            {
                for (int i = 0; i < textureNames.Length; i++)
                {
                    bool isSelected = currentIndex == i;
                    if (ImGui.Selectable(textureNames[i], isSelected))
                    {
                        spriteRenderer.Texture = allTextures.Values.ElementAt(i);
                        currentIndex = i;
                    }
                    
                }
                ImGui.EndCombo();
            }

            //Preview
            if (spriteRenderer.Texture != null)
            {
                ImGui.Image(spriteRenderer.Texture.Handle, new System.Numerics.Vector2(100, 100),
                    new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
            }

            //Color picker         
            System.Numerics.Vector4 color = new System.Numerics.Vector4(spriteRenderer.Color.R, spriteRenderer.Color.G, spriteRenderer.Color.B, spriteRenderer.Color.A);
            if (ImGui.ColorEdit4("Color", ref color))
            {
                spriteRenderer.Color = new Color4(color.X, color.Y, color.Z, color.W);
            }

            //flip x
            bool x = spriteRenderer.FlipX;
            if(ImGui.Checkbox("Flip x", ref x))
            {
                spriteRenderer.FlipX = x;
            }

            //flip y
            bool y = spriteRenderer.FlipY;
            if (ImGui.Checkbox("Flip y", ref y))
            {
                spriteRenderer.FlipY = y;
            }

            //layer
            int z = spriteRenderer.Layer;
            if (ImGui.DragInt("Layer", ref z))
            {
                spriteRenderer.Layer = z;
            }
            ImGui.Unindent();          
        }

        private void RenderScriptComponent(ScriptComponent script)
        {
            ImGui.TextColored(new System.Numerics.Vector4(0.8f, 0.8f, 1.0f, 1.0f), $"{script.name}");
            ImGui.Indent();

            //delete button
            ImGui.SameLine();
            ImGui.PushID("ScriptRemoveButton");
            if (ImGui.Button("×", new System.Numerics.Vector2(20, 20)))
            {
                _selectedEntity.RemoveComponent<ScriptComponent>();
            }
            ImGui.PopID();

            //тут делаем инспектор полей
            var type = script.GetType();

            // 1. Поля
            foreach (var field in type.GetFields())
            {
                RenderFieldOrProperty(script, field.Name, () => field.GetValue(script), (v) => field.SetValue(script, v), field.FieldType);
            }

            // 2. Свойства с публичным сеттером
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (prop.CanRead && prop.CanWrite && prop.GetSetMethod() is { IsPublic: true })
                {
                    RenderFieldOrProperty(script, prop.Name, () => prop.GetValue(script), (v) => prop.SetValue(script, v), prop.PropertyType);
                }
            }


            ImGui.Unindent();
        }

        private void RenderFieldOrProperty(
    object target,
    string label,
    Func<object> getter,
    Action<object> setter,
    Type type)
        {
            ImGui.PushID($"{target.GetHashCode()}_{label}");

            object value = getter();

            if (type == typeof(bool))
            {
                bool val = (bool)value;
                if (ImGui.Checkbox(label, ref val))
                    setter(val);
            }
            else if (type == typeof(int))
            {
                int val = (int)value;
                if (ImGui.InputInt(label, ref val))
                    setter(val);
            }
            else if (type == typeof(float))
            {
                float val = (float)value;
                if (ImGui.InputFloat(label, ref val))
                    setter(val);
            }
            else if (type == typeof(string))
            {
                string currentValue = (string)value ?? "";
                const int bufSize = 256;

                // Используем byte[], а не Span
                byte[] buffer = new byte[bufSize];
                int written = Encoding.UTF8.GetBytes(currentValue, 0, currentValue.Length, buffer, 0);
                if (written < bufSize) buffer[written] = 0;

                // ImGui.InputText принимает ref byte (первый элемент)
                if (ImGui.InputText(label, buffer, bufSize))
                {
                    int len = 0;
                    while (len < bufSize && buffer[len] != 0) len++;
                    string newStr = Encoding.UTF8.GetString(buffer, 0, len);
                    setter(newStr);
                }
            }
            else if (type == typeof(Vector2))
            {
                Vector2 val = (Vector2)value;
                System.Numerics.Vector2 vec = val.ToNumerics();
                if (ImGui.InputFloat2(label, ref vec))
                    setter(val);
            }
            else if (type == typeof(Vector3))
            {
                Vector3 val = (Vector3)value;
                System.Numerics.Vector3 vec = val.ToNumerics();
                if (ImGui.InputFloat3(label, ref vec))
                    setter(val);
            }
            else if (type == typeof(Vector4))
            {
                Vector4 val = (Vector4)value;
                System.Numerics.Vector4 vec = val.ToNumerics();
                if (ImGui.InputFloat4(label, ref vec))
                    setter(val);
            }
            else if (type.IsEnum)
            {
                // Поддержка enum: отображаем как выпадающий список
                Array enumValues = Enum.GetValues(type);
                string[] names = Enum.GetNames(type);
                int currentIndex = Array.IndexOf(enumValues, value);

                if (ImGui.Combo(label, ref currentIndex, names, names.Length))
                {
                    setter(enumValues.GetValue(currentIndex));
                }
            }
            else
            {
                // Неизвестный тип — просто отображаем как readonly текст
                ImGui.Text($"{label}: <{type.Name}> (unsupported)");
            }

            ImGui.PopID();
        }
    }
}
