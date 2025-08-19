using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DieselTools_ExileAPI
{
    public class DisposableStyle : IDisposable
    {
        private readonly Dictionary<ImGuiStyleVar, float> _originalFloatStyles = new();
        private readonly Dictionary<ImGuiStyleVar, Vector2> _originalVector2Styles = new();
        private readonly Dictionary<ImGuiCol, Vector4> _originalColorStyles = new();

        public DisposableStyle()
        {
        }

        public void SetAlpha(float value)
        {
            var style = ImGui.GetStyle();
            _originalFloatStyles[ImGuiStyleVar.Alpha] = style.Alpha;
            style.Alpha = value;
        }

        public void SetWindowRounding(float value)
        {
            var style = ImGui.GetStyle();
            _originalFloatStyles[ImGuiStyleVar.WindowRounding] = style.WindowRounding;
            style.WindowRounding = value;
        }

        public void SetWindowBorderSize(float value)
        {
            var style = ImGui.GetStyle();
            _originalFloatStyles[ImGuiStyleVar.WindowBorderSize] = style.WindowBorderSize;
            style.WindowBorderSize = value;
        }

        public void SetChildRounding(float value)
        {
            var style = ImGui.GetStyle();
            _originalFloatStyles[ImGuiStyleVar.ChildRounding] = style.ChildRounding;
            style.ChildRounding = value;
        }

        public void SetFrameRounding(float value)
        {
            var style = ImGui.GetStyle();
            _originalFloatStyles[ImGuiStyleVar.FrameRounding] = style.FrameRounding;
            style.FrameRounding = value;
        }

        public void SetPopupRounding(float value)
        {
            var style = ImGui.GetStyle();
            _originalFloatStyles[ImGuiStyleVar.PopupRounding] = style.PopupRounding;
            style.PopupRounding = value;
        }

        public void SetScrollbarRounding(float value)
        {
            var style = ImGui.GetStyle();
            _originalFloatStyles[ImGuiStyleVar.ScrollbarRounding] = style.ScrollbarRounding;
            style.ScrollbarRounding = value;
        }

        public void SetGrabRounding(float value)
        {
            var style = ImGui.GetStyle();
            _originalFloatStyles[ImGuiStyleVar.GrabRounding] = style.GrabRounding;
            style.GrabRounding = value;
        }

        public void SetTabRounding(float value)
        {
            var style = ImGui.GetStyle();
            _originalFloatStyles[ImGuiStyleVar.TabRounding] = style.TabRounding;
            style.TabRounding = value;
        }

        public void SetWindowPadding(Vector2 value)
        {
            var style = ImGui.GetStyle();
            _originalVector2Styles[ImGuiStyleVar.WindowPadding] = style.WindowPadding;
            style.WindowPadding = value;
        }

        public void SetFramePadding(Vector2 value)
        {
            var style = ImGui.GetStyle();
            _originalVector2Styles[ImGuiStyleVar.FramePadding] = style.FramePadding;
            style.FramePadding = value;
        }

        public void SetCellPadding(Vector2 value)
        {
            var style = ImGui.GetStyle();
            _originalVector2Styles[ImGuiStyleVar.CellPadding] = style.CellPadding;
            style.CellPadding = value;
        }

        public void SetItemSpacing(Vector2 value)
        {
            var style = ImGui.GetStyle();
            _originalVector2Styles[ImGuiStyleVar.ItemSpacing] = style.ItemSpacing;
            style.ItemSpacing = value;
        }

        public void SetItemInnerSpacing(Vector2 value)
        {
            var style = ImGui.GetStyle();
            _originalVector2Styles[ImGuiStyleVar.ItemInnerSpacing] = style.ItemInnerSpacing;
            style.ItemInnerSpacing = value;
        }

        public void SetButtonTextAlign(Vector2 value)
        {
            var style = ImGui.GetStyle();
            _originalVector2Styles[ImGuiStyleVar.ButtonTextAlign] = style.ButtonTextAlign;
            style.ButtonTextAlign = value;
        }

        public void SetSelectableTextAlign(Vector2 value)
        {
            var style = ImGui.GetStyle();
            _originalVector2Styles[ImGuiStyleVar.SelectableTextAlign] = style.SelectableTextAlign;
            style.SelectableTextAlign = value;
        }

        public void SetColor(ImGuiCol colorVar, SharpDX.Color value)
        {
            var style = ImGui.GetStyle();
            _originalColorStyles[colorVar] = style.Colors[(int)colorVar];
            style.Colors[(int)colorVar] = new Vector4(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f);
        }

        public void Dispose()
        {
            var style = ImGui.GetStyle();
            foreach (var kvp in _originalFloatStyles)
            {
                switch (kvp.Key)
                {
                    case ImGuiStyleVar.Alpha:
                        style.Alpha = kvp.Value;
                        break;
                    case ImGuiStyleVar.WindowRounding:
                        style.WindowRounding = kvp.Value;
                        break;
                    case ImGuiStyleVar.WindowBorderSize:
                        style.WindowBorderSize = kvp.Value;
                        break;
                    case ImGuiStyleVar.ChildRounding:
                        style.ChildRounding = kvp.Value;
                        break;
                    case ImGuiStyleVar.FrameRounding:
                        style.FrameRounding = kvp.Value;
                        break;
                    case ImGuiStyleVar.PopupRounding:
                        style.PopupRounding = kvp.Value;
                        break;
                    case ImGuiStyleVar.ScrollbarRounding:
                        style.ScrollbarRounding = kvp.Value;
                        break;
                    case ImGuiStyleVar.GrabRounding:
                        style.GrabRounding = kvp.Value;
                        break;
                    case ImGuiStyleVar.TabRounding:
                        style.TabRounding = kvp.Value;
                        break;
                }
            }
            foreach (var kvp in _originalVector2Styles)
            {
                switch (kvp.Key)
                {
                    case ImGuiStyleVar.WindowPadding:
                        style.WindowPadding = kvp.Value;
                        break;
                    case ImGuiStyleVar.FramePadding:
                        style.FramePadding = kvp.Value;
                        break;
                    case ImGuiStyleVar.CellPadding:
                        style.CellPadding = kvp.Value;
                        break;
                    case ImGuiStyleVar.ItemSpacing:
                        style.ItemSpacing = kvp.Value;
                        break;
                    case ImGuiStyleVar.ItemInnerSpacing:
                        style.ItemInnerSpacing = kvp.Value;
                        break;
                    case ImGuiStyleVar.ButtonTextAlign:
                        style.ButtonTextAlign = kvp.Value;
                        break;
                    case ImGuiStyleVar.SelectableTextAlign:
                        style.SelectableTextAlign = kvp.Value;
                        break;
                }
            }
            foreach (var kvp in _originalColorStyles)
            {
                style.Colors[(int)kvp.Key] = kvp.Value;
            }
        }
    }
}
