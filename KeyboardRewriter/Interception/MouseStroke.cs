namespace KeyboardRewriter.Interception
{
    public struct MouseStroke
    {
        public MouseState State;
        public ushort Flags;
        public short Rolling;
        public int X;
        public int Y;
        public uint Information;
    }
}