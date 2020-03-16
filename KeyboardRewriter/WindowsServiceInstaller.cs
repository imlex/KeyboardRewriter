using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace KeyboardRewriter
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        public WindowsServiceInstaller()
        {
            Installers.AddRange(WindowsServiceDescription.Installers);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            AppendRunAsServiceArgument();
            base.OnBeforeInstall(savedState);
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            AppendRunAsServiceArgument();
            base.OnBeforeInstall(savedState);
        }

        private void AppendRunAsServiceArgument()
        {
            string assemblyPath = Context.Parameters["assemblypath"];
            if (!string.IsNullOrEmpty(assemblyPath) && assemblyPath[0] != '\"')
                assemblyPath = "\"" + assemblyPath + "\"";
            assemblyPath += " " + Program.RunAsServiceArgument;
            Context.Parameters["assemblypath"] = assemblyPath;
        }
    }
}