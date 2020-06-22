using KeyboardRewriter.Interception;

namespace KeyboardRewriter
{
    internal class RewriteRule : BaseRule
    {
        public RewriteRule(string receiveKeys, string sendKeys)
        {
            ReceiveKeyStrokes = GenerateReceiveKeyStrokes(receiveKeys);
            SendKeyStrokes = GenerateSendKeyStrokes(sendKeys);
        }

        public KeyStroke[] ReceiveKeyStrokes { get; }

        public int ReceivePosition { get; set; }

        public KeyStroke[] SendKeyStrokes { get; }
    }
}