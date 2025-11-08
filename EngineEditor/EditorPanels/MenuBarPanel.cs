using EngineCore.ECS;
using ImGuiNET;
using System;

public class MenuBarPanel
{  
    public void Render()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New Scene"))
                {
                    OnNewScene?.Invoke();
                }

                if (ImGui.MenuItem("Open Scene..."))
                {
                    OnOpenScene?.Invoke();
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Save Scene"))
                {
                    OnSaveScene?.Invoke();
                }

                if (ImGui.MenuItem("Save Scene As..."))
                {
                    OnSaveSceneAs?.Invoke();
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Undo", "Ctrl+Z"))
                {
                    OnUndo?.Invoke();
                }

                if (ImGui.MenuItem("Redo", "Ctrl+Y"))
                {
                    OnRedo?.Invoke();
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Cut", "Ctrl+X"))
                {
                    OnCut?.Invoke();
                }

                if (ImGui.MenuItem("Copy", "Ctrl+C"))
                {
                    OnCopy?.Invoke();
                }

                if (ImGui.MenuItem("Paste", "Ctrl+V"))
                {
                    OnPaste?.Invoke();
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("View"))
            {
                ImGui.MenuItem("Scene Hierarchy", "", ref _showSceneHierarchy);
                ImGui.MenuItem("Inspector", "", ref _showInspector);
                ImGui.MenuItem("Viewport", "", ref _showViewport);

                ImGui.EndMenu();
            }


            ImGui.EndMainMenuBar();
        }
    }

    // События для обработки в основном редакторе
    public event Action OnNewScene;
    public event Action OnOpenScene;
    public event Action OnSaveScene;
    public event Action OnSaveSceneAs;
    public event Action OnUndo;
    public event Action OnRedo;
    public event Action OnCut;
    public event Action OnCopy;
    public event Action OnPaste;

    // Показывать/скрывать панели
    private bool _showSceneHierarchy = true;
    private bool _showInspector = true;
    private bool _showViewport = true;

    public bool ShowSceneHierarchy => _showSceneHierarchy;
    public bool ShowInspector => _showInspector;
    public bool ShowViewport => _showViewport;
}