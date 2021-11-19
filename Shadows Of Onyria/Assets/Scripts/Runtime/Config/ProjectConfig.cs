using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType
{
    XboxController,
    KeyboardMouse
}

public static class ProjectConfig
{
    public const int TARGET_FPS = 60;
    
    public static float TargetFrameTime => 1f / TARGET_FPS;
    public static InputType inputType = InputType.KeyboardMouse;
}
