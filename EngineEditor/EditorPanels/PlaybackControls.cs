using ImGuiNET;
using System;
using System.Collections.Generic;

public class PlaybackControls
{
    private PlayState _playState = PlayState.Stopped;
    private readonly List<PlayState> _allowedTransitions = new();
    private bool b;

    public PlaybackControls()
    {
        _allowedTransitions.Add(PlayState.Stopped);
        _allowedTransitions.Add(PlayState.Playing);
        _allowedTransitions.Add(PlayState.Paused);
    }

    public void Render()
    {
        ImGui.Begin("Playback");
        // Стилизация кнопок
        var buttonSize = new System.Numerics.Vector2(40, 30);

        // Кнопка Stop
        //ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(0.8f, 0.2f, 0.2f, 1.0f));
        if (ImGui.Button("Stop", buttonSize))
        {
            SetPlayState(PlayState.Stopped);
        }
        //ImGui.PopStyleColor();

        ImGui.SameLine();

        // Кнопка Play/Pause
        if (_playState == PlayState.Playing)
        {
            //ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(0.2f, 0.8f, 0.2f, 1.0f));
            if (ImGui.Button("Pause", buttonSize))
            {
                SetPlayState(PlayState.Paused);
            }
            //ImGui.PopStyleColor();
        }
        else
        {
            //ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(0.2f, 0.6f, 0.2f, 1.0f));
            if (ImGui.Button("Play", buttonSize))
            {
                SetPlayState(PlayState.Playing);
            }
            //ImGui.PopStyleColor();
        }

        ImGui.SameLine();

        // Кнопка Step (только в состоянии Pause)
        if (_playState == PlayState.Paused)
        {
            if (ImGui.Button("Step", buttonSize))
            {
                OnStep?.Invoke();
            }
        }
        else
        {
            // Сделать кнопку неактивной
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
            ImGui.Button("Step", buttonSize);
            ImGui.PopStyleVar();
        }

        // Показываем состояние
        ImGui.SameLine();
        ImGui.Text(GetStateText(_playState));

        
  
    }

    private void SetPlayState(PlayState newState)
    {
        if (_allowedTransitions.Contains(newState))
        {
            var oldState = _playState;
            _playState = newState;

            // Вызываем соответствующие события
            switch (newState)
            {
                case PlayState.Playing:
                    if (oldState == PlayState.Stopped)
                        OnPlay?.Invoke();
                    else if (oldState == PlayState.Paused)
                        OnResume?.Invoke();
                    break;

                case PlayState.Paused:
                    OnPause?.Invoke();
                    break;

                case PlayState.Stopped:
                    OnStop?.Invoke();
                    break;
            }
        }
    }

    private string GetStateText(PlayState state)
    {
        return state switch
        {
            PlayState.Stopped => "Stopped",
            PlayState.Playing => "Playing",
            PlayState.Paused => "Paused",
            _ => "Unknown"
        };
    }

    public PlayState GetPlayState() => _playState;
    public bool IsPlaying() => _playState == PlayState.Playing;
    public bool IsPaused() => _playState == PlayState.Paused;
    public bool IsStopped() => _playState == PlayState.Stopped;

    public event Action OnPlay;
    public event Action OnPause;
    public event Action OnStop;
    public event Action OnResume;
    public event Action OnStep;
}

public enum PlayState
{
    Stopped,
    Playing,
    Paused
}