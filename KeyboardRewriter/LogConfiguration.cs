using System.Linq;
using NLog;
using NLog.Targets;

namespace KeyboardRewriter
{
    internal static class LogConfiguration
    {
        public static bool LogToConsole
        {
            set
            {
                var configuration = LogManager.Configuration;

                var consoleTarget = configuration.ConfiguredNamedTargets.SingleOrDefault(x => x is ConsoleTarget || x is ColoredConsoleTarget);

                if (value)
                {
                    if (consoleTarget == null)
                    {
                        consoleTarget = new ConsoleTarget("console")
                        {
                            Layout = "${longdate} ${level:uppercase=true} ${logger} ${message}${exception:format=ToString}"
                        };

                        configuration.AddTarget(consoleTarget);

                        configuration.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);

                        LogManager.ReconfigExistingLoggers();
                    }
                }
                else
                {
                    if (consoleTarget != null)
                    {
                        foreach (var rule in configuration.LoggingRules.Where(x => x.Targets.Any(y => y== consoleTarget)).ToList())
                        {
                            if (rule.Targets.Count == 1)
                                configuration.LoggingRules.Remove(rule);
                            else
                                rule.Targets.Remove(consoleTarget);
                        }

                        configuration.RemoveTarget(consoleTarget.Name);

                        LogManager.ReconfigExistingLoggers();
                    }
                }
            }
        }
    }
}