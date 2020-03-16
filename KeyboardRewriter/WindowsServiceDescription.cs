using System.Configuration.Install;
using System.ServiceProcess;

namespace KeyboardRewriter
{
    internal static class WindowsServiceDescription
    {
        public static readonly ServiceInstaller ServiceInstaller = new ServiceInstaller
        {
            DelayedAutoStart = true,
            Description = "Intercepts and substitutes keyboard keystrokes.",
            DisplayName = "Keyboard Rewriter",
            ServiceName = "KeyboardRewriter",
            StartType = ServiceStartMode.Automatic
        };

        public static Installer[] Installers =
        {
            ServiceInstaller,
            new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalService,
                Password = null,
                Username = null
            }
        };
    }
}