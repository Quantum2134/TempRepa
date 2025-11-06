using EngineCore.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.EditorPanels
{
    internal class ConsolePanel : EditorPanel
    {
        private readonly List<LogEntry> _entries = new();
        private readonly List<LogEntry> _filteredEntries = new();

        // Флаги для фильтрации
        private bool _showVerbose = true;
        private bool _showInfo = true;
        private bool _showWarning = true;
        private bool _showError = true;

        private string _textBuffer = "";

        private class LogEntry
        {
            public string Message;
            public LogLevel Level;
        }

        public ConsolePanel() : base("Console")
        {
            Logger.OnLogMessage += OnNewLogMessage;
        }

        protected override void OnRender()
        {
            DrawToolbar();
            DrawLogList();
        }

        private void OnNewLogMessage(string message, LogLevel level)
        {
            _entries.Add(new LogEntry { Message = message, Level = level });
            // Ограничим размер, чтобы не утечь память
            if (_entries.Count > 2000)
                _entries.RemoveAt(0);
            RebuildFilter();
        }

        private void RebuildFilter()
        {
            _filteredEntries.Clear();
            foreach (var entry in _entries)
            {
                if (ShouldShow(entry.Level))
                    _filteredEntries.Add(entry);
            }
        }

        private bool ShouldShow(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => _showVerbose,
                LogLevel.Info => _showInfo,
                LogLevel.Warning => _showWarning,
                LogLevel.Error => _showError,
                _ => true
            };
        }

        private void DrawToolbar()
        {
            // Фильтры
            bool changed = false;
            changed |= ImGui.Checkbox("Trace", ref _showVerbose);
            ImGui.SameLine();
            changed |= ImGui.Checkbox("Info", ref _showInfo);
            ImGui.SameLine();
            changed |= ImGui.Checkbox("Warning", ref _showWarning);
            ImGui.SameLine();
            changed |= ImGui.Checkbox("Error", ref _showError);

            if(changed)
            {
                RebuildFilter();
            }

            ImGui.SameLine();
            if (ImGui.Button("Clear"))
            {
                _entries.Clear();
                _filteredEntries.Clear();
            }

            // Поле ввода (опционально: для будущих команд)
            ImGui.InputText("##consoleInput", ref _textBuffer, 256, ImGuiInputTextFlags.EnterReturnsTrue);
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                // Можно добавить поддержку команд типа "clear"
                if (_textBuffer == "clear")
                {
                    _entries.Clear();
                    _filteredEntries.Clear();
                }
                _textBuffer = "";
            }
        }

        private void DrawLogList()
        {
            var size = ImGui.GetContentRegionAvail();
            if (ImGui.BeginChild("Scrolling", size, ImGuiChildFlags.None))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(4, 1));

                foreach (var entry in _filteredEntries)
                {
                    var color = entry.Level switch
                    { 
                        LogLevel.Trace => new System.Numerics.Vector4(0.6f, 0.6f, 0.6f, 1.0f),
                        LogLevel.Info => new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                        LogLevel.Warning => new System.Numerics.Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        LogLevel.Error => new System.Numerics.Vector4(1.0f, 0.3f, 0.3f, 1.0f),
                        _ => new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                    };

                    ImGui.PushStyleColor(ImGuiCol.Text, color);
                    ImGui.TextUnformatted(entry.Message);
                    ImGui.PopStyleColor();
                }

                // Автопрокрутка вниз, если пользователь уже внизу
                if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                    ImGui.SetScrollHereY(1.0f);

                ImGui.PopStyleVar();
            }
            ImGui.EndChild();
        }

        
    }
}
