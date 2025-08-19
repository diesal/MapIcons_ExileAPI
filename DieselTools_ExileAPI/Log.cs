using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselTools_ExileAPI
{
    public class Log
    {

        private const int MAX_LOG_ENTRIES = 500;
        private readonly Queue<(DateTime Date, string Description, int Count)> _log = new();
        private string _lastMessage = null;
        private int _repeatCount = 1;

                public void Message(string entry) {
            if (_log.Count > 0 && entry == _lastMessage) {
                _repeatCount++;
                // Update the last entry in the queue
                var entries = _log.ToList();
                entries[entries.Count - 1] = (DateTime.Now, entry, _repeatCount);
                _log.Clear();
                foreach (var e in entries)
                    _log.Enqueue(e);
            }
            else {
                _repeatCount = 1;
                _lastMessage = entry;
                _log.Enqueue((DateTime.Now, entry, 1));
            }
            while (_log.Count > MAX_LOG_ENTRIES) { _log.Dequeue(); }
        }


        public void Clear() {
            _log.Clear();
            _lastMessage = null;
            _repeatCount = 1;
        }

        public IEnumerable<(DateTime Date, string Description, int Count)> GetEntries() {
            return _log;
        }

        public void Render(bool newestFirst = true) {
            ImGui.Begin("Log");
            if (ImGui.Button("Clear History")) { Clear(); }

            ImGui.BeginChild("Log");
            var entries = newestFirst ? _log.Reverse() : _log;
            foreach (var (dateTime, message, count) in entries) {
                var display = count > 1 ? $"[{count}] {message}" : message;
                ImGui.TextUnformatted($"{dateTime:HH:mm:ss.fff}: {display}");
            }
            ImGui.EndChild();
            ImGui.End();
        }

    }
}