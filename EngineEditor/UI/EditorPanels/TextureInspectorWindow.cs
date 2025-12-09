using EngineCore.Platform.OpenGL;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using EngineEditor.Utils;

namespace EngineEditor.UI.EditorPanels
{
    public class TextureInspectorWindow
    {
        public bool IsOpen;
        private Texture _texture;

        // Кэшируем текущие значения для ImGui (они должны быть mutable)
        private WrapMode _wrapMode;
        private FilterMode _filterMode;

        private SpriteSlicerWindow _slicer = new();

        public SpriteSlicerWindow SpriteSlicerWindow => _slicer;

        public TextureInspectorWindow()
        {
            // Подпишемся на сохранение из слайсера (опционально)
            _slicer.OnSave = (sprites) =>
            {
                // Например, можно создать новые ассеты или спрайты
                System.Diagnostics.Debug.WriteLine($"Sliced {sprites.Count} sprites from texture");
            };
        }

        public void Open(Texture texture)
        {
            _texture = texture;
            if (_texture != null)
            {
                _wrapMode = _texture.WrapMode;
                _filterMode = _texture.FilterMode;
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
            if (ImGui.Begin("Texture Inspector", ref IsOpen))
            {
                if (_texture == null)
                {
                    ImGui.Text("No texture selected.");
                }
                else
                {
                    RenderTextureInfo();
                    RenderSamplerSettings();
                    RenderActions();
                }

                if(ImGui.Button("Sprite slicer"))
                {
                    _slicer.Open(_texture);

                }
                _slicer.Render();
            }
            ImGui.End();
        }

        private void RenderTextureInfo()
        {
            // Превью
            Vector2 previewSize = new Vector2(150, 150);
            float aspect = (float)_texture.Width / _texture.Height;
            if (aspect > 1) previewSize.Y = previewSize.X / aspect;
            else previewSize.X = previewSize.Y * aspect;

            ImGui.Image(_texture.Handle, previewSize.ToNumerics(), Vector2.UnitY.ToNumerics(), new Vector2(1, 0).ToNumerics());
            ImGui.Text($"Size: {_texture.Width} x {_texture.Height}");
            ImGui.Text($"Memory: {_texture.Size} bytes ({_texture.Size / 1024f / 1024f} mb)");
            ImGui.Separator();
        }

        private void RenderSamplerSettings()
        {
            // Wrap Mode
            var wrapNames = Enum.GetNames<WrapMode>();
            int wrapIdx = Array.IndexOf(wrapNames, _wrapMode.ToString());
            if (ImGui.Combo("Wrap Mode", ref wrapIdx, wrapNames, wrapNames.Length))
            {
                _wrapMode = (WrapMode)Enum.Parse(typeof(WrapMode), wrapNames[wrapIdx]);
            }

            //Filter
            var minNames = Enum.GetNames<FilterMode>();
            int minIdx = Array.IndexOf(minNames, _filterMode.ToString());
            if (ImGui.Combo("Filter", ref minIdx, minNames, minNames.Length))
            {
                _filterMode = (FilterMode)Enum.Parse(typeof(FilterMode), minNames[minIdx]);
            }

            
        }

        private void RenderActions()
        {
            ImGui.Separator();

            if (ImGui.Button("Open in Sprite Slicer"))
            {
                _slicer.Open(_texture);
            }

            ImGui.SameLine();
            if (ImGui.Button("Apply Settings"))
            {
                _texture.WrapMode = _wrapMode;
                _texture.FilterMode = _filterMode;
                _texture.Apply();
            }
        }
    }
}
