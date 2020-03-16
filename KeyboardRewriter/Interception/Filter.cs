using System;

namespace KeyboardRewriter.Interception
{
    [Flags]
    public enum Filter : ushort
    {
        KeyNone = 0x0000,
        KeyAll = 0xFFFF,
        KeyDown = KeyState.Up,
        KeyUp = KeyState.Up << 1,
        KeyE0 = KeyState.E0 << 1,
        KeyE1 = KeyState.E1 << 1,
        KeyTermSrvSetLed = KeyState.TermSrvSetLed << 1,
        KeyTermSrvShadow = KeyState.TermSrvShadow << 1,
        KeyTermSrvVkpacket = KeyState.TermSrvVkPacket << 1,

        MouseNone = 0x0000,
        MouseAll = 0xFFFF,

        MouseLeftButtonDown = MouseState.LeftButtonDown,
        MouseLeftButtonUp = MouseState.LeftButtonUp,
        MouseRightButtonDown = MouseState.RightButtonDown,
        MouseRightButtonUp = MouseState.RightButtonUp,
        MouseMiddleButtonDown = MouseState.MiddleButtonDown,
        MouseMiddleButtonUp = MouseState.MiddleButtonUp,

        MouseButton1Down = MouseState.Button1Down,
        MouseButton1Up = MouseState.Button1Up,
        MouseButton2Down = MouseState.Button2Down,
        MouseButton2Up = MouseState.Button2Up,
        MouseButton3Down = MouseState.Button3Down,
        MouseButton3Up = MouseState.Button3Up,

        MouseButton4Down = MouseState.Button4Down,
        MouseButton4Up = MouseState.Button4Up,
        MouseButton5Down = MouseState.Button5Down,
        MouseButton5Up = MouseState.Button5Up,

        MouseWheel = MouseState.Wheel,
        MouseHorizontalWheel = MouseState.HorizontalWheel,

        MouseMove = 0x1000
    }
}