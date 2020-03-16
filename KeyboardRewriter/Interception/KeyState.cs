namespace KeyboardRewriter.Interception
{
    public enum KeyState : ushort
    {
        Down = 0x00,
        Up = 0x01,
        E0 = 0x02,
        E1 = 0x04,
        TermSrvSetLed = 0x08,
        TermSrvShadow = 0x10,
        TermSrvVkPacket = 0x20
    }
}