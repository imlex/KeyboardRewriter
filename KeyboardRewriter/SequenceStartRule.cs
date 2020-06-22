using KeyboardRewriter.Interception;

namespace KeyboardRewriter
{
    internal class SequenceStartRule : BaseRule
    {
        public SequenceStartRule(int timeoutMilliseconds, string sendKeys)
        {
            TimeoutTicks = 10000L * timeoutMilliseconds;

            SendKeyStrokes = GenerateSendKeyStrokes(sendKeys);
        }

        public long TimeoutTicks { get; }

        public KeyStroke[] SendKeyStrokes { get; }
    }
}