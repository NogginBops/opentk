using System;
using System.Drawing;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Platform;

namespace OpenTK.Backends.Tests
{
    public class JoystickComponentView : View
    {
        public override string Title => "Joystick";

        public override bool IsVisible => Toolkit.Joystick != null;

        private float? LeftDeadzone;
        private float? RightDeadzone;

        public override void Initialize()
        {
            base.Initialize();

            try { LeftDeadzone = Toolkit.Joystick.LeftDeadzone; } catch { LeftDeadzone = null; }
            try { RightDeadzone = Toolkit.Joystick.RightDeadzone; } catch { RightDeadzone = null; }
        }

        private void DrawAxis(float x, float y, float deadzone)
        {
            // FIXME: Padding!
            const float RADIUS = 50;
            const float SMALL_RADIUS = 8;

            ImDrawListPtr draw_list = ImGui.GetWindowDrawList();
            Vector2 p = ImGui.GetCursorScreenPos().ToOpenTK();

            // XY axes
            draw_list.AddLine((p + new Vector2(0, RADIUS)).ToNumerics(), (p + new Vector2(2 * RADIUS, RADIUS)).ToNumerics(), ImGui.GetColorU32(ImGuiCol.TextDisabled));
            draw_list.AddLine((p + new Vector2(RADIUS, 0)).ToNumerics(), (p + new Vector2(RADIUS, 2 * RADIUS)).ToNumerics(), ImGui.GetColorU32(ImGuiCol.TextDisabled));

            // Deadzone circle
            draw_list.AddCircle((p + new Vector2(RADIUS, RADIUS)).ToNumerics(), RADIUS * deadzone, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.8f, 0.2f, 0, 0.8f)));

            // Outline
            draw_list.AddCircle((p + new Vector2(RADIUS, RADIUS)).ToNumerics(), RADIUS, ImGui.GetColorU32(ImGuiCol.Text));

            // Position circle
            draw_list.AddCircle((p + new Vector2((x * 0.5f + 0.5f) * RADIUS * 2, (y * 0.5f + 0.5f) * RADIUS * 2)).ToNumerics(), SMALL_RADIUS, 0xFFFFFFFF);
            draw_list.AddCircleFilled((p + new Vector2((x * 0.5f + 0.5f) * RADIUS * 2, (y * 0.5f + 0.5f) * RADIUS * 2)).ToNumerics(), 1.5f, 0xFF0000FF);

            string str = $"X:{x:0.00} Y:{y:0.00}";
            Vector2 strSize = ImGui.CalcTextSize(str).ToOpenTK();

            draw_list.AddText((p + new Vector2((2*RADIUS)/2 - strSize.X/2, 2*RADIUS)).ToNumerics(), ImGui.GetColorU32(ImGuiCol.Text), str);

            ImGui.Dummy(new Vector2(RADIUS * 2, RADIUS * 2 + ImGui.GetTextLineHeightWithSpacing()).ToNumerics());
        }

        private void DrawTrigger(float t)
        {
            const float WIDTH = 40;
            const float HEIGHT = 100;
            const float RADIUS = 10;

            ImDrawListPtr draw_list = ImGui.GetWindowDrawList();
            Vector2 p = ImGui.GetCursorScreenPos().ToOpenTK();

            // FIXME: When t is vert small the radius doesn't work correctly...
            // There is some code in imgui that is used for horizontal progress bars
            // which handles this but it is not public.
            // https://github.com/ocornut/imgui/blob/master/imgui_draw.cpp#L5881
            // - Noggin_bops 2025-07-13
            draw_list.AddRectFilled((p + new Vector2(0, (1 - t) * HEIGHT)).ToNumerics(), (p + new Vector2(WIDTH, HEIGHT)).ToNumerics(), ImGui.GetColorU32(ImGuiCol.Text), RADIUS);

            // Outline
            draw_list.AddRect(p.ToNumerics(), (p + new Vector2(WIDTH, HEIGHT)).ToNumerics(), ImGui.GetColorU32(ImGuiCol.Text), RADIUS);

            string str = $"{t:0.00}";
            Vector2 strSize = ImGui.CalcTextSize(str).ToOpenTK();

            draw_list.AddText((p + new Vector2(WIDTH / 2 - strSize.X / 2, HEIGHT + ImGui.GetStyle().ItemSpacing.Y)).ToNumerics(), ImGui.GetColorU32(ImGuiCol.Text), $"{t:0.00}");

            ImGui.Dummy(new Vector2(WIDTH, HEIGHT + ImGui.GetTextLineHeightWithSpacing()).ToNumerics());
        }

        private void DrawButton(string str, bool pressed, Color3<Hsv> hsv_color)
        {
            const float WIDTH = 50;
            const float HEIGHT = 50;
            const float RADIUS = 25;

            ImDrawListPtr draw_list = ImGui.GetWindowDrawList();
            Vector2 p = ImGui.GetCursorScreenPos().ToOpenTK();

            Color4<Rgba> color = hsv_color.ToRgb().ToRgba(1.0f);
            Color4<Rgba> bg_color = new Color3<Hsv>(hsv_color.X, hsv_color.Y * 0.5f, hsv_color.Z * 0.5f).ToRgb().ToRgba(1.0f);
            Color4<Rgba> pressed_color = new Color3<Hsv>(hsv_color.X, float.Clamp(hsv_color.Y * 1.5f, 0, 1), hsv_color.Z).ToRgb().ToRgba(1.0f);

            draw_list.AddCircleFilled((p + new Vector2(RADIUS, RADIUS)).ToNumerics(), RADIUS, (pressed ? pressed_color : bg_color).ToImGui());
            draw_list.AddCircle((p + new Vector2(RADIUS, RADIUS)).ToNumerics(), RADIUS, color.ToImGui());

            Vector2 strSize = ImGui.CalcTextSize(str).ToOpenTK();
            
            draw_list.AddText((p + new Vector2(WIDTH / 2 - strSize.X / 2, HEIGHT/2 + - strSize.Y/2)).ToNumerics(), ImGui.GetColorU32(ImGuiCol.Text), str);

            ImGui.Dummy(new Vector2(WIDTH, HEIGHT + ImGui.GetTextLineHeightWithSpacing()).ToNumerics());
        }

        public override void Paint(double deltaTime)
        {
            base.Paint(deltaTime);

            ImGui.SeparatorText("Component properties");

            ImGui.Text($"Left deadzone: {LeftDeadzone}");
            ImGui.Text($"Right deadzone: {RightDeadzone}");

            ImGui.SeparatorText("Joysticks");

            JoystickHandle handle = Toolkit.Joystick.Open(0);

            ImGui.Text($"Joystick 0: {Toolkit.Joystick.GetName(handle)}");

            ImGui.SeparatorText("Axes:");

            float leftX = Toolkit.Joystick.GetAxis(handle, JoystickAxis.LeftXAxis);
            float leftY = Toolkit.Joystick.GetAxis(handle, JoystickAxis.LeftYAxis);

            float rightX = Toolkit.Joystick.GetAxis(handle, JoystickAxis.RightXAxis);
            float rightY = Toolkit.Joystick.GetAxis(handle, JoystickAxis.RightYAxis);

            float leftTrigger = Toolkit.Joystick.GetAxis(handle, JoystickAxis.LeftTrigger);
            float rightTrigger = Toolkit.Joystick.GetAxis(handle, JoystickAxis.RightTrigger);

            DrawTrigger(leftTrigger);
            ImGui.SameLine();
            DrawAxis(leftX, leftY, LeftDeadzone ?? 0);
            ImGui.SameLine();
            DrawAxis(rightX, rightY, RightDeadzone ?? 0);
            ImGui.SameLine();
            DrawTrigger(rightTrigger);

            ImGui.SeparatorText("Buttons:");

            DrawButton("A", Toolkit.Joystick.GetButton(handle, JoystickButton.A), new Color3<Rgb>(0, 0.65f, 0).ToHsv());
            ImGui.SameLine();
            DrawButton("B", Toolkit.Joystick.GetButton(handle, JoystickButton.B), new Color3<Rgb>(0.65f, 0, 0).ToHsv());
            ImGui.SameLine();
            DrawButton("X", Toolkit.Joystick.GetButton(handle, JoystickButton.X), new Color3<Rgb>(0.3f, 0.3f, 0.65f).ToHsv());
            ImGui.SameLine();
            DrawButton("Y", Toolkit.Joystick.GetButton(handle, JoystickButton.Y), new Color3<Rgb>(0.65f, 0.65f, 0.0f).ToHsv());
            
            DrawButton("LB", Toolkit.Joystick.GetButton(handle, JoystickButton.LeftShoulder), new Color3<Hsv>(0, 0, 0.5f));
            ImGui.SameLine();
            DrawButton("RB", Toolkit.Joystick.GetButton(handle, JoystickButton.RightShoulder), new Color3<Hsv>(0, 0, 0.5f));
            ImGui.SameLine();
            DrawButton("LThumb", Toolkit.Joystick.GetButton(handle, JoystickButton.LeftThumb), new Color3<Hsv>(0, 0, 0.5f));
            ImGui.SameLine();
            DrawButton("RThumb", Toolkit.Joystick.GetButton(handle, JoystickButton.RightThumb), new Color3<Hsv>(0, 0, 0.5f));
            
            DrawButton("Start", Toolkit.Joystick.GetButton(handle, JoystickButton.Start), new Color3<Hsv>(0, 0, 0.5f));
            ImGui.SameLine();
            DrawButton("Back", Toolkit.Joystick.GetButton(handle, JoystickButton.Back), new Color3<Hsv>(0, 0, 0.5f));
            
            DrawButton("D-Up", Toolkit.Joystick.GetButton(handle, JoystickButton.DPadUp), new Color3<Hsv>(0, 0, 0.5f));
            ImGui.SameLine();
            DrawButton("D-Down", Toolkit.Joystick.GetButton(handle, JoystickButton.DPadDown), new Color3<Hsv>(0, 0, 0.5f));
            ImGui.SameLine();
            DrawButton("D-Left", Toolkit.Joystick.GetButton(handle, JoystickButton.DPadLeft), new Color3<Hsv>(0, 0, 0.5f));
            ImGui.SameLine();
            DrawButton("D-Right", Toolkit.Joystick.GetButton(handle, JoystickButton.DPadRight), new Color3<Hsv>(0, 0, 0.5f));

            Toolkit.Joystick.SetVibration(handle, leftTrigger, rightTrigger);

            Toolkit.Joystick.Close(handle);
        }
    }
}

