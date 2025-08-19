using ExileCore.Shared.Enums;
using ExileCore.Shared;
using ExileCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;

namespace DieselTools_ExileAPI;

public class Crafting
{
    public static int KeyDownDelay { get; set; } = 30;
    public static int KeyUpDelay { get; set; } = 30;
    public static int MouseMoveDelay { get; set; } = 30;
    public static int MouseDownDelay { get; set; } = 30;
    public static int MouseUpDelay { get; set; } = 30;

    public static async SyncTask<TaskResult> PickupCurrency(PlayerItem currencyItem, TaskCancellations taskCancellations) {
        while (true) {

            TaskResult taskResult = taskCancellations.Evaluate();
            if (!taskResult.Success) return taskResult;

            // pickup currency
            SharpDX.Vector2 item_position = currencyItem.Position.Center;
            item_position += Exile.WindowOffset;
            Keyboard.KeyDown(Keys.LShiftKey);
            await Task.Wait(KeyDownDelay);
            Mouse.moveMouse(item_position);
            await Task.Wait(MouseMoveDelay);
            Mouse.RightDown();
            await Task.Wait(MouseDownDelay);
            Mouse.RightUp();
            await Task.Wait(MouseUpDelay);

            if (Exile.IngameState.IngameUi.Cursor.Action == MouseActionType.UseItem) {
                return new TaskResult(true, $"{currencyItem.BaseName} picked up!");
            }
        }
    }
    public static async SyncTask<TaskResult> HoverItem(Vector2 position, TaskCancellations taskCancellations) {      
        Mouse.moveMouse(position);
        TaskResult taskResult = await Task.Wait(MouseMoveDelay, taskCancellations);
        if (!taskResult.Success) return taskResult;

        return new TaskResult(true);
    }

    public static async SyncTask<TaskResult> ClickItem(TaskCancellations taskCancellations) {

        Mouse.LeftDown();
        TaskResult taskResult = await Task.Wait(MouseDownDelay, taskCancellations);
        if (!taskResult.Success) return taskResult;

        Mouse.LeftUp();
        taskResult = await Task.Wait(MouseUpDelay, taskCancellations);
        if (!taskResult.Success) return taskResult;

        return new TaskResult(true);
    }





}
