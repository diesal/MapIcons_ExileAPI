using ExileCore.Shared;
using System.Diagnostics;

namespace DieselTools_ExileAPI
{
    public class TaskResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public TaskResult(bool success, string message = null) {
            Success = success;
            Message = message ?? string.Empty;
        }
    }

    public class TaskCondition
    {
        public string Message { get; }
        private readonly Func<bool> _condition;

        public TaskCondition(string message, Func<bool> condition) {
            Message = message;
            _condition = condition;
        }

        public bool Evaluate() {
            return _condition();
        }
    }

    public class TaskCancellations
    {
        private readonly List<TaskCondition> _conditions;

        public TaskCancellations() {
            _conditions = new List<TaskCondition>();
        }

        public void AddCondition(TaskCondition condition) {
            _conditions.Add(condition);
        }

        public TaskResult Evaluate() {
            foreach (var condition in _conditions) {
                if (condition.Evaluate()) {
                    return new TaskResult(false, condition.Message);
                }
            }
            return new TaskResult(true, string.Empty);
        }
    }

    public class Task
    {
        /// <summary>
        /// Waits for the specified number of milliseconds, yielding control back to the caller on each frame.
        /// </summary>
        /// <param name="ms">The number of milliseconds to wait.</param>
        /// <returns>A <see cref="SyncTask{Boolean}"/> that represents the asynchronous operation. The result is <c>true</c> when the wait is complete.</returns>
        public static async SyncTask<bool> Wait(int ms) {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < ms) {
                await TaskUtils.NextFrame();
            }

            return true;
        }

        public static async SyncTask<TaskResult> Wait(int ms, TaskCancellations taskCancellations) {
            var stopwatch = Stopwatch.StartNew();
            TaskResult taskResult;

            while (stopwatch.ElapsedMilliseconds < ms) {
                taskResult = taskCancellations.Evaluate();
                if (!taskResult.Success) return taskResult;
                await TaskUtils.NextFrame();
            }

            return new TaskResult(true);
        }


    }

}
