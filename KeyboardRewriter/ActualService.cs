using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;
using KeyboardRewriter.Interception;
using KeyboardRewriter.Properties;
using NLog;

namespace KeyboardRewriter
{
    internal class ActualService
    {
        private static readonly Logger __logger = LogManager.GetCurrentClassLogger();

        private List<RewriteRule> _rewriteRules;

        private Context _context;
        private CancellationTokenSource _cancellationTokenSource;
        private Thread _thread;

        public void Start(string[] args)
        {
            __logger.Info("Starting");

            AppSettings appSettings;
            using (var fileStream = File.Open(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), FileMode.Open))
                appSettings = (AppSettings) new DataContractJsonSerializer(typeof(AppSettings)).ReadObject(fileStream);

            _rewriteRules = appSettings.RewriteRules.Select(x => new RewriteRule(x.ReceiveKeys, x.SendKeys)).ToList();

            _context = Context.Create();
            if (_context.IsInvalid)
                throw new InvalidOperationException("Interception is not available!");

            var deviceIds = new int[Context.MaxDevices + 1];
            __logger.Info("Input devices detected: {0}", _context.HardwareIds.Count);
            foreach (var hardwareId in _context.HardwareIds)
            {
                __logger.Info("\t{0}\t{1}", hardwareId.DeviceId, hardwareId.HardwareId);

                if (appSettings.HardwareIds.Any(x => string.Equals(x, hardwareId.HardwareId, StringComparison.OrdinalIgnoreCase)))
                    deviceIds[hardwareId.DeviceId] = 1;
            }

            _context.SetFilter(deviceId => deviceIds[deviceId], Filter.KeyDown | Filter.KeyUp);

            _cancellationTokenSource = new CancellationTokenSource();
            _thread = new Thread(RewriteKeyboard);
            _thread.Start();

            __logger.Info("Started");
        }

        private void RewriteKeyboard()
        {
            KeyStroke[] keyStrokes = new KeyStroke[1];

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var deviceIdentifier = _context.WaitWithTimeout(5000);
                if (deviceIdentifier.Id == 0)
                    continue;

                if (_context.Receive(deviceIdentifier, keyStrokes) <= 0)
                    break;

                var keyStroke = keyStrokes[0];

                __logger.Debug("Received code={0}, state={1}", keyStroke.Code, keyStroke.State);

                var fitKeyStrokes = 0;
                RewriteRule fitRewriteRule = null;
                var unfitKeyStrokes = 0;
                RewriteRule unfitRewriteRule = null;
                foreach (var rewriteRule in _rewriteRules)
                {
                    var nextKeyStroke = rewriteRule.ReceiveKeyStrokes[rewriteRule.ReceivePosition];

                    if (nextKeyStroke.Code == keyStroke.Code && nextKeyStroke.State == keyStroke.State)
                    {
                        rewriteRule.ReceivePosition++;

                        if (fitKeyStrokes < rewriteRule.ReceivePosition)
                            fitKeyStrokes = rewriteRule.ReceivePosition;

                        if (fitRewriteRule == null && rewriteRule.ReceiveKeyStrokes.Length == rewriteRule.ReceivePosition)
                            fitRewriteRule = rewriteRule;
                    }
                    else
                    {
                        if (unfitKeyStrokes < rewriteRule.ReceivePosition)
                        {
                            unfitKeyStrokes = rewriteRule.ReceivePosition;
                            unfitRewriteRule = rewriteRule;
                        }

                        rewriteRule.ReceivePosition = 0;
                    }
                }

                unfitKeyStrokes -= fitKeyStrokes - 1;

                if (unfitRewriteRule != null && unfitKeyStrokes > 0)
                {
                    if (__logger.IsDebugEnabled)
                    {
                        for (int i = 0; i < unfitKeyStrokes; i++)
                        {
                            var sendingKeyStroke = unfitRewriteRule.ReceiveKeyStrokes[i];
                            __logger.Debug("Sending code={0}, state={1}", sendingKeyStroke.Code, sendingKeyStroke.State);
                        }
                    }

                    _context.Send(deviceIdentifier, unfitRewriteRule.ReceiveKeyStrokes, unfitKeyStrokes);
                }

                if (fitKeyStrokes == 0)
                {
                    __logger.Debug("Sending code={0}, state={1}", keyStroke.Code, keyStroke.State);

                    _context.Send(deviceIdentifier, keyStrokes, 1);
                }
                else if (fitRewriteRule != null)
                {
                    if (__logger.IsDebugEnabled)
                    {
                        for (int i = 0; i < fitRewriteRule.SendKeyStrokes.Length; i++)
                        {
                            var sendingKeyStroke = fitRewriteRule.SendKeyStrokes[i];
                            __logger.Debug("Sending code={0}, state={1}", sendingKeyStroke.Code, sendingKeyStroke.State);
                        }
                    }

                    _context.Send(deviceIdentifier, fitRewriteRule.SendKeyStrokes);

                    foreach (var rewriteRule in _rewriteRules)
                        rewriteRule.ReceivePosition = 0;
                }
            }
        }

        public void Stop()
        {
            __logger.Info("Stopping");

            _cancellationTokenSource.Cancel();
            _thread.Join();

            __logger.Info("Stopped");

            LogManager.Shutdown();
        }
    }
}