using ImGuiNET;
using System.Numerics;
using System.Text;

namespace MapIcons;

public static class Log
{
    private const string WINDOW_NAME = "MapIcons Debug Log";
    private const int MAX_LOG_ENTRIES = 500;
    private const int BUFFER_SIZE = 65536; // 64 KB buffer size

    private static readonly Queue<(DateTime Date, string Description, int Count)> _logQueue = new();

    private static Action _customControlsAction;
    private static string _logText = string.Empty;
    private static string _lastMessage = null;
    private static int _repeatCount = 1;

    public static void Write(string entry)
    {

        if (_logQueue.Count > 0 && entry == _lastMessage) {
            _repeatCount++;
            // Update the last entry in the queue
            var entries = _logQueue.ToList();
            entries[entries.Count - 1] = (DateTime.Now, entry, _repeatCount);
            _logQueue.Clear();
            foreach (var e in entries) _logQueue.Enqueue(e);
        }
        else {
            _repeatCount = 1;
            _lastMessage = entry;
            _logQueue.Enqueue((DateTime.Now, entry, 1));
        }
        while (_logQueue.Count > MAX_LOG_ENTRIES) { _logQueue.Dequeue(); }

        UpdateLogText();
    }

    public static void Clear() {
        _logQueue.Clear();
        _lastMessage = null;
        _repeatCount = 1;
        UpdateLogText();
    }

    public static void Render(ref bool showLogWindow)
    {
        if (showLogWindow)
        {
            ImGui.Begin(WINDOW_NAME, ref showLogWindow);

            if (ImGui.Button("Clear History")) Clear();

            _customControlsAction?.Invoke();

            ImGui.BeginChild("Log");
            ImGui.InputTextMultiline("##logText", ref _logText, (uint)BUFFER_SIZE, new Vector2(-1, -1), ImGuiInputTextFlags.ReadOnly);

            ImGui.EndChild();
            ImGui.End();
        }
    }

    public static void SetCustomHeaderControls(Action customControlsAction)
    {
        _customControlsAction = customControlsAction;
    }

    private static void UpdateLogText()
    {
        var logString = new StringBuilder();
        foreach (var (dateTime, message, count) in _logQueue) {
            var display = count > 1 ? $"[{count}] {message}" : message;
            logString.AppendLine($"{dateTime:HH:mm:ss.fff}: {display}");
        }
        _logText = logString.ToString();
    }
}