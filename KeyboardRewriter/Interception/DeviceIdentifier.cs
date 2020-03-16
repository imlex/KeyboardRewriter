namespace KeyboardRewriter.Interception
{
    public struct DeviceIdentifier
    {
        public DeviceIdentifier(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}