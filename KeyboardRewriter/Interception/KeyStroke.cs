using System.Runtime.InteropServices;

namespace KeyboardRewriter.Interception
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyStroke
    {
        public const ushort SleepCode = ushort.MaxValue;

        public ushort Code;
        public KeyState State;
        public uint Information;
    }
}