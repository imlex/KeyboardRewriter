using System.Runtime.InteropServices;

namespace KeyboardRewriter.Interception
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyStroke
    {
        public ushort Code;
        public KeyState State;
        public uint Information;
    }
}