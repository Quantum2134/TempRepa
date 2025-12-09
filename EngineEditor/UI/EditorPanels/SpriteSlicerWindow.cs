using EngineCore.Platform.OpenGL;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Mathematics;
using EngineEditor.Utils;

namespace EngineEditor.UI.EditorPanels
{
    public class SpriteSlicerWindow
    {
        public bool IsOpen;
        private Texture _texture;
        private Vector2 _textureSize;
        private bool _isCreatingRegion = false;

        public List<Rectangle> SelectedRegions { get; } = new();
        private int _activeRegionIndex = -1;

        // Режимы взаимодействия
        private enum EditMode { None, Move, ResizeTopLeft, ResizeBottomRight }
        private EditMode _currentMode = EditMode.None;

        private Vector2 _dragStart; // начальная позиция мыши при drag'е
        private Vector2 _regionDragStart; // начальное положение региона

        // Grid snapping
        private bool _enableGrid = true;
        private int _gridSize = 8;

        public SpriteSlicerWindow()
        {
        }

        public void Open(Texture texture)
        {
            _texture = texture;
            _textureSize = new Vector2(texture.Width, texture.Height);
            SelectedRegions.Clear();
            _activeRegionIndex = -1;
            _currentMode = EditMode.None;
            IsOpen = true;
        }

        public void Render()
        {
            if (!IsOpen) return;

            ImGui.SetNextWindowSize(new Vector2(1200, 800).ToNumerics(), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Sprite Slicer###SpriteSlicer", ref IsOpen))
            {
                if (_texture == null)
                {
                    ImGui.Text("No texture loaded.");
                }
                else
                {
                    // Управление сеткой
                    ImGui.Checkbox("Enable Grid", ref _enableGrid);
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(80);
                    ImGui.DragInt("Grid Size", ref _gridSize, 1, 1, 64);

                    RenderSlicerArea();

                    // Панель информации об активном регионе
                    if (_activeRegionIndex >= 0 && _activeRegionIndex < SelectedRegions.Count)
                    {
                        ImGui.Separator();
                        var r = SelectedRegions[_activeRegionIndex];
                        ImGui.Text($"Active Region: X={r.X}, Y={r.Y}, W={r.Width}, H={r.Height}");
                        ImGui.SameLine();
                        if (ImGui.Button("Delete Region"))
                        {
                            SelectedRegions.RemoveAt(_activeRegionIndex);
                            _activeRegionIndex = -1;
                        }
                    }
                }

                ImGui.Separator();
                if (ImGui.Button("Clear All"))
                {
                    SelectedRegions.Clear();
                    _activeRegionIndex = -1;
                }
                ImGui.SameLine();
                if (ImGui.Button("Save Sprites"))
                {
                    OnSave?.Invoke(GetSpriteDataList());
                    IsOpen = false;
                }
            }
            ImGui.End();

            // Глобальная обработка Delete (даже если фокус не на окне ImGui)
            // Но ImGui не перехватывает системные клавиши вне своих окон, поэтому делаем внутри окна
            if (IsOpen && ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows))
            {
                if (ImGui.IsKeyPressed(ImGuiKey.Delete) && _activeRegionIndex >= 0)
                {
                    SelectedRegions.RemoveAt(_activeRegionIndex);
                    _activeRegionIndex = -1;
                }
            }
        }

        private void RenderSlicerArea()
        {
            Vector2 available = ImGui.GetContentRegionAvail().ToOpenTK();
            available.Y -= 100; // место под управление и инфо

            float scaleX = available.X / _textureSize.X;
            float scaleY = available.Y / _textureSize.Y;
            float scale = Math.Min(scaleX, scaleY);
            scale = Math.Max(scale, 0.1f);

            Vector2 displaySize = _textureSize * scale;
            Vector2 cursorPos = ImGui.GetCursorScreenPos().ToOpenTK();
            ImGui.Image(_texture.Handle, displaySize.ToNumerics(), Vector2.UnitY.ToNumerics(), new Vector2(1, 0).ToNumerics());

            Vector2 imageMin = cursorPos;
            Vector2 imageMax = cursorPos + displaySize;

            Vector2 mousePos = ImGui.GetMousePos().ToOpenTK();
            bool isHoveringImage = mousePos.X >= imageMin.X && mousePos.X <= imageMax.X &&
                                   mousePos.Y >= imageMin.Y && mousePos.Y <= imageMax.Y;

            Vector2 ScreenToTexture(Vector2 screen)
            {
                return new Vector2(
                    (screen.X - imageMin.X) / displaySize.X * _textureSize.X,
                    (screen.Y - imageMin.Y) / displaySize.Y * _textureSize.Y
                );
            }

            // Завершение drag'а
            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                _currentMode = EditMode.None;
            }

            // Обработка drag'а
            if (_currentMode != EditMode.None && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                if (isHoveringImage && _activeRegionIndex >= 0)
                {
                    Vector2 texMouse = ScreenToTexture(mousePos);
                    var rect = SelectedRegions[_activeRegionIndex];

                    if (_currentMode == EditMode.Move)
                    {
                        Vector2 offset = texMouse - _dragStart;
                        Vector2 newPos = _regionDragStart + offset;
                        if (_enableGrid)
                        {
                            newPos.X = Snap(newPos.X, _gridSize);
                            newPos.Y = Snap(newPos.Y, _gridSize);
                        }
                        rect.X = (int)Math.Max(0, Math.Min(newPos.X, _textureSize.X - rect.Width));
                        rect.Y = (int)Math.Max(0, Math.Min(newPos.Y, _textureSize.Y - rect.Height));
                    }
                    else if (_currentMode == EditMode.ResizeTopLeft)
                    {
                        Vector2 newTopLeft = texMouse;
                        if (_enableGrid)
                        {
                            newTopLeft.X = Snap(newTopLeft.X, _gridSize);
                            newTopLeft.Y = Snap(newTopLeft.Y, _gridSize);
                        }
                        int newRight = rect.X + rect.Width;
                        int newBottom = rect.Y + rect.Height;
                        int newLeft = (int)Math.Min(newTopLeft.X, newRight - 4);
                        int newTop = (int)Math.Min(newTopLeft.Y, newBottom - 4);
                        rect.X = newLeft;
                        rect.Y = newTop;
                        rect.Width = newRight - newLeft;
                        rect.Height = newBottom - newTop;
                    }
                    else if (_currentMode == EditMode.ResizeBottomRight)
                    {
                        Vector2 newBottomRight = texMouse;
                        if (_enableGrid)
                        {
                            newBottomRight.X = Snap(newBottomRight.X, _gridSize);
                            newBottomRight.Y = Snap(newBottomRight.Y, _gridSize);
                        }
                        int newLeft = rect.X;
                        int newTop = rect.Y;
                        int newRight = (int)Math.Max(newBottomRight.X, newLeft + 4);
                        int newBottom = (int)Math.Max(newBottomRight.Y, newTop + 4);
                        rect.Width = newRight - newLeft;
                        rect.Height = newBottom - newTop;
                    }

                    SelectedRegions[_activeRegionIndex] = rect;
                }
            }

            // Обработка нажатия
            if (isHoveringImage && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                Vector2 texMouse = ScreenToTexture(mousePos);
                _activeRegionIndex = -1;

                // Проверяем клик по существующим регионам
                for (int i = SelectedRegions.Count - 1; i >= 0; i--)
                {
                    var r = SelectedRegions[i];
                    const float hitSize = 8.0f;
                    Vector2 tl = new(r.X, r.Y);
                    Vector2 br = new(r.X + r.Width, r.Y + r.Height);
                    float edgeSize = hitSize / scale;

                    if (Vector2.Distance(texMouse, tl) <= edgeSize)
                    {
                        _activeRegionIndex = i;
                        _currentMode = EditMode.ResizeTopLeft;
                        _regionDragStart = new Vector2(r.X, r.Y);
                        break;
                    }
                    if (Vector2.Distance(texMouse, br) <= edgeSize)
                    {
                        _activeRegionIndex = i;
                        _currentMode = EditMode.ResizeBottomRight;
                        _regionDragStart = new Vector2(r.X + r.Width, r.Y + r.Height);
                        break;
                    }
                    if (texMouse.X >= r.X && texMouse.X <= r.X + r.Width &&
                        texMouse.Y >= r.Y && texMouse.Y <= r.Y + r.Height)
                    {
                        _activeRegionIndex = i;
                        _currentMode = EditMode.Move;
                        _dragStart = texMouse;
                        _regionDragStart = new Vector2(r.X, r.Y);
                        break;
                    }
                }

                if (_activeRegionIndex == -1)
                {
                    // Начинаем новое выделение
                    _isCreatingRegion = true;
                    _dragStart = texMouse;
                }
            }

            // Новое выделение (если не попали в существующий регион)
            // Визуализация временного выделения
            if (_isCreatingRegion && ImGui.IsMouseDown(ImGuiMouseButton.Left) && isHoveringImage)
            {
                Vector2 endPos = ScreenToTexture(mousePos);
                var rect = CreateNormalizedRect(_dragStart, endPos);
                if (_enableGrid)
                {
                    rect.X = (int)Snap(rect.X, _gridSize);
                    rect.Y = (int)Snap(rect.Y, _gridSize);
                    rect.Width = Math.Max(1, (int)Snap(rect.Width, _gridSize));
                    rect.Height = Math.Max(1, (int)Snap(rect.Height, _gridSize));
                }
                var drawLis = ImGui.GetWindowDrawList();
                DrawRectangle(drawLis, imageMin, displaySize, rect, new Vector4(1, 1, 0, 1));
            }

            // Завершение нового выделения
            // Завершение нового выделения
            if (_isCreatingRegion && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                _isCreatingRegion = false;

                Vector2 endPos = isHoveringImage ? ScreenToTexture(mousePos) : _dragStart;
                var rect = CreateNormalizedRect(_dragStart, endPos);

                if (rect.Width >= 2 && rect.Height >= 2) // минимум 2x2
                {
                    if (_enableGrid)
                    {
                        rect.X = (int)Snap(rect.X, _gridSize);
                        rect.Y = (int)Snap(rect.Y, _gridSize);
                        rect.Width = Math.Max(2, (int)Snap(rect.Width, _gridSize));
                        rect.Height = Math.Max(2, (int)Snap(rect.Height, _gridSize));
                    }
                    SelectedRegions.Add(rect);
                    _activeRegionIndex = SelectedRegions.Count - 1;
                }
            }

            // Рисуем все регионы
            var drawList = ImGui.GetWindowDrawList();
            for (int i = 0; i < SelectedRegions.Count; i++)
            {
                var r = SelectedRegions[i];
                var color = i == _activeRegionIndex ? new Vector4(1, 0, 0, 1) : new Vector4(0, 1, 0, 1);
                DrawRectangle(drawList, imageMin, displaySize, r, color);

                if (i == _activeRegionIndex)
                {
                    // Рисуем углы для активного региона
                    float cornerSize = 6.0f;
                    Vector2 tlScreen = imageMin + new Vector2(r.X * scaleX, r.Y * scaleY);
                    Vector2 brScreen = tlScreen + new Vector2(r.Width * scaleX, r.Height * scaleY);

                    drawList.AddCircleFilled(tlScreen.ToNumerics(), cornerSize, ImGui.GetColorU32(new Vector4(1, 1, 1, 1).ToNumerics()), 8);
                    drawList.AddCircleFilled(brScreen.ToNumerics(), cornerSize, ImGui.GetColorU32(new Vector4(1, 1, 1, 1).ToNumerics()), 8);
                }
            }
        }

        private float Snap(float value, int gridSize)
        {
            return (float)Math.Round(value / gridSize) * gridSize;
        }

        private Rectangle CreateNormalizedRect(Vector2 p1, Vector2 p2)
        {
            float x1 = Math.Min(p1.X, p2.X);
            float y1 = Math.Min(p1.Y, p2.Y);
            float x2 = Math.Max(p1.X, p2.X);
            float y2 = Math.Max(p1.Y, p2.Y);
            return new Rectangle(
                (int)x1, (int)y1,
                (int)(x2 - x1), (int)(y2 - y1)
            );
        }

        private void DrawRectangle(ImDrawListPtr drawList, Vector2 imageMin, Vector2 displaySize, Rectangle rect, Vector4 color)
        {
            float scaleX = displaySize.X / _textureSize.X;
            float scaleY = displaySize.Y / _textureSize.Y;
            Vector2 p1 = imageMin + new Vector2(rect.X * scaleX, rect.Y * scaleY);
            Vector2 p2 = p1 + new Vector2(rect.Width * scaleX, rect.Height * scaleY);
            drawList.AddRect(p1.ToNumerics(), p2.ToNumerics(), ImGui.GetColorU32(color.ToNumerics()), 0, ImDrawFlags.None, 2.0f);
        }

        public List<SpriteData> GetSpriteDataList()
        {
            var list = new List<SpriteData>();
            foreach (var rect in SelectedRegions)
            {
                float u0 = (float)rect.X / _texture.Width;
                float v0 = (float)rect.Y / _texture.Height;
                float u1 = (float)(rect.X + rect.Width) / _texture.Width;
                float v1 = (float)(rect.Y + rect.Height) / _texture.Height;

                list.Add(new SpriteData
                {
                    Texture = _texture,
                    SourceRect = rect,
                    UVMin = new Vector2(u0, v0),
                    UVMax = new Vector2(u1, v1)
                });
            }
            return list;
        }

        public Action<List<SpriteData>> OnSave;

        public struct SpriteData
        {
            public Texture Texture;
            public Rectangle SourceRect;
            public Vector2 UVMin;
            public Vector2 UVMax;
        }
    }

    public struct Rectangle
    {
        public int X, Y, Width, Height;
        public Rectangle(int x, int y, int width, int height)
        {
            X = x; Y = y; Width = width; Height = height;
        }
    }
}
