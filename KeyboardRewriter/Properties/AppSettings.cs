﻿using System.Collections.Generic;

namespace KeyboardRewriter.Properties
{
    public class AppSettings
    {
        public List<string> HardwareIds { get; set; }

        public int KeyStrokeTimeMilliseconds { get; set; }

        public SequenceStartRuleSetting SequenceStartRule { get; set; }

        public List<RewriteRuleSetting> RewriteRules { get; set; }
    }

    public class SequenceStartRuleSetting
    {
        public int TimeoutMilliseconds { get; set; }

        public string SendKeys { get; set; }
    }

    public class RewriteRuleSetting
    {
        public string ReceiveKeys { get; set; }

        public string SendKeys { get; set; }
    }
}