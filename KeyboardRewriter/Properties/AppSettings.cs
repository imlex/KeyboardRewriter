using System.Collections.Generic;

namespace KeyboardRewriter.Properties
{
    public class AppSettings
    {
        public List<string> HardwareIds { get; set; }

        public List<RewriteRuleSetting> RewriteRules { get; set; }
    }

    public class RewriteRuleSetting
    {
        public string ReceiveKeys { get; set; }

        public string SendKeys { get; set; }
    }
}