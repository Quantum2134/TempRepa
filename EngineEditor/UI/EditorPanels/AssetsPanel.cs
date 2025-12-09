using EngineCore.Assets;
using EngineCore.Assets.Assets;
using EngineCore.Assets.AssetTypes;
using EngineCore.Core;
using EngineCore.ECS;
using EngineCore.Logging;
using EngineCore.Platform.OpenGL;
using EngineEditor.Platform.Windows;
using EngineEditor.Utils;
using ImGuiNET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.UI.EditorPanels
{
    internal class AssetsPanel : EditorPanel
    {
        public TextureInspectorWindow textureInspector;
        public ShaderInspectorWindow shaderInspector;

        private readonly Application _application;
        private readonly Vector2 _thumbnailSize = new(120, 120);
        private readonly float _padding = 10.0f;

        // Заглушка для иконки шейдера. Замени путь на реальный, когда найдёшь.
        private static int _shaderIconTextureId = 0;

        public AssetsPanel(Application application) : base("Assets")
        {
            _application = application;
            textureInspector = new TextureInspectorWindow();
            shaderInspector = new ShaderInspectorWindow();
        }
        public void SetShaderIcon(Texture texture)
        {
            _shaderIconTextureId = texture.Handle;
        }
        protected override void OnRender()
        {
            // Кнопка загрузки
            if (ImGui.Button("Load new asset"))
            {
                var sourceFiles = FileDialogWindows.OpenFiles(
                    title: "Select assets",
                    filter: "All files (*.*)\0*.*\0"
                );

                string relativeDir = "textures";
                foreach (var file in sourceFiles)
                {
                    if (string.IsNullOrEmpty(file)) continue;
                    string relativePath = _application.AssetSystem.CopyFileToResources(file, relativeDir);
                    if (relativePath == null) continue;

                    var textureAsset = _application.AssetSystem.LoadAsset<TextureAsset>(relativePath);
                    _application.GraphicsSystem.GraphicsContext.TextureManager.LoadTexture(textureAsset);
                }
            }

            ImGui.Separator();

            const float thumbnailSize = 120.0f;
            const float nameHeight = 20.0f;
            const float cellPadding = 6.0f;

            // Динамически определяем число колонок
            float availWidth = ImGui.GetContentRegionAvail().X;
            int columnCount = Math.Max(1, (int)(availWidth / (thumbnailSize + cellPadding * 2)));

            // Создаём таблицу без заголовков, без рамок
            if (!ImGui.BeginTable("##AssetGrid", columnCount,
                ImGuiTableFlags.SizingFixedFit |
                ImGuiTableFlags.NoBordersInBody |
                ImGuiTableFlags.NoHostExtendX |
                ImGuiTableFlags.NoKeepColumnsVisible))
            {
                return; // редко, но бывает
            }

            var assets = _application.AssetSystem.Assets;

            for (int i = 0; i < assets.Count; i++)
            {
                ImGui.TableNextColumn(); // переходим к следующей ячейке

                var asset = assets[i];
                ImGui.PushID(i);

                // Устанавливаем паддинг внутри ячейки
                ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(cellPadding, cellPadding).ToNumerics());

                // Имя ассета по центру под изображением
                float nameWidth = ImGui.CalcTextSize(asset.Name).X;
                float centerX = ImGui.GetCursorPosX() + (thumbnailSize - nameWidth) * 0.5f;
                if (centerX < ImGui.GetCursorPosX()) centerX = ImGui.GetCursorPosX(); // защита от переполнения

                // Невидимая кнопка — весь tile как кликабельная область
                Vector2 buttonSize = new(thumbnailSize, thumbnailSize + nameHeight);
                bool clicked = ImGui.InvisibleButton("##tile", buttonSize.ToNumerics());
                bool hovered = ImGui.IsItemHovered();

                // Получаем позицию для рисования
                Vector2 drawPos = ImGui.GetItemRectMin().ToOpenTK();

                // Фон при наведении
                if (hovered)
                {
                    var drawList = ImGui.GetWindowDrawList();
                    drawList.AddRectFilled(
                        drawPos.ToNumerics(),
                        (drawPos + new Vector2(thumbnailSize, thumbnailSize + nameHeight)).ToNumerics(),
                        ImGui.GetColorU32(ImGuiCol.ButtonHovered, 0.5f)
                    );
                }

                // Рисуем изображение
                if (asset.Type == AssetType.Texture)
                {
                    var texture = _application.GraphicsSystem.GraphicsContext.TextureManager.GetTexture(asset.Name);
                    if (texture != null)
                    {
                        ImGui.SetCursorScreenPos(drawPos.ToNumerics());
                        ImGui.Image(
                            texture.Handle,
                            new Vector2(thumbnailSize, thumbnailSize).ToNumerics(),
                            new Vector2(0, 1).ToNumerics(),
                            new Vector2(1, 0).ToNumerics()  
                        );
                    }
                    else
                    {
                        DrawPlaceholder(drawPos, thumbnailSize, "MISSING");
                    }
                }
                else if (asset.Type == AssetType.Shader)
                {
                    ImGui.SetCursorScreenPos(drawPos.ToNumerics());
                    ImGui.Image(_shaderIconTextureId, new Vector2(thumbnailSize, thumbnailSize).ToNumerics(), new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
                }
                else
                {
                    DrawPlaceholder(drawPos, thumbnailSize, "?");
                }

                // Имя под изображением
                ImGui.SetCursorScreenPos(new Vector2(centerX, drawPos.Y + thumbnailSize + 2).ToNumerics());
                ImGui.TextUnformatted(asset.Name);

                if (clicked)
                {
                    HandleAssetClick(asset);
                }

                ImGui.PopStyleVar(); // CellPadding
                ImGui.PopID();
            }

            ImGui.EndTable();

            textureInspector.Render();
            shaderInspector.Render();
        }
        private void DrawPlaceholder(Vector2 pos, float size, string label)
        {
            var drawList = ImGui.GetWindowDrawList();
            var rectMin = pos;
            var rectMax = pos + new Vector2(size, size);

            // Серый фон
            drawList.AddRectFilled(rectMin.ToNumerics(), rectMax.ToNumerics(), ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1.0f).ToNumerics()));
            // Рамка
            drawList.AddRect(rectMin.ToNumerics(), rectMax.ToNumerics(), ImGui.GetColorU32(ImGuiCol.Border));

            // Текст по центру
            var textSize = ImGui.CalcTextSize(label);
            var textPos = pos + new Vector2((size - textSize.X) * 0.5f, (size - textSize.Y) * 0.5f);
            drawList.AddText(textPos.ToNumerics(), ImGui.GetColorU32(ImGuiCol.Text), label);
        }
        private void HandleAssetClick(Asset asset)
        {
            switch (asset.Type)
            {
                case AssetType.Texture:

                    shaderInspector.Close();
                    textureInspector.Open(_application.GraphicsSystem.GraphicsContext.TextureManager.GetTexture(asset.Name));
                    break;
                case AssetType.Shader:
                    textureInspector.Close();
                    shaderInspector.Open((ShaderAsset)asset);
                    break;
                default:
                    break;
            }
        }
    }
}

