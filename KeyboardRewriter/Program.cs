using System;
using System.Collections;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace KeyboardRewriter
{
    internal static class Program
    {
        internal const string RunAsConsoleArgument = "--console";
        internal const string InstallServiceArgument = "--install";
        internal const string RunAsServiceArgument = "--service";
        internal const string UninstallServiceArgument = "--uninstall";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            string firstArgument = args?.FirstOrDefault()?.ToLowerInvariant();
            switch (firstArgument)
            {
                case RunAsConsoleArgument:
                    RunAsConsoleApp(args);
                    break;
                case InstallServiceArgument:
                    InstallService();
                    break;
                case RunAsServiceArgument:
                    RunAsService();
                    break;
                case UninstallServiceArgument:
                    UninstallService();
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }

        private static void RunAsConsoleApp(string[] args)
        {
            LogConfiguration.LogToConsole = true;

            ActualService actualService = new ActualService();
            actualService.Start(args);

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(@"Press any key to stop service...");
            Console.ResetColor();
            Console.WriteLine();
            Console.ReadKey(true);

            actualService.Stop();
        }

        private static void InstallService()
        {
            using (TransactedInstaller transactedInstaller = CreateTransactedInstaller())
                transactedInstaller.Install(new Hashtable());
        }

        private static void RunAsService()
        {
            WindowsService windowsService = new WindowsService();

            LogConfiguration.LogToConsole = false;

            ServiceBase.Run(windowsService);
        }

        private static void UninstallService()
        {
            using (TransactedInstaller transactedInstaller = CreateTransactedInstaller())
                transactedInstaller.Uninstall(null);
        }

        private static TransactedInstaller CreateTransactedInstaller()
        {
            TransactedInstaller transactedInstaller = new TransactedInstaller();
            transactedInstaller.Installers.Add(new AssemblyInstaller(Assembly.GetExecutingAssembly(), null));
            return transactedInstaller;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\t" + AppDomain.CurrentDomain.FriendlyName +
                              " { " + RunAsConsoleArgument + " | " + InstallServiceArgument + " | " + RunAsServiceArgument + " | " + UninstallServiceArgument + " }");
        }
    }
}