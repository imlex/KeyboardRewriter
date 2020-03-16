using System.ServiceProcess;

namespace KeyboardRewriter
{
    internal class WindowsService : ServiceBase
    {
        public WindowsService()
        {
            ServiceName = WindowsServiceDescription.ServiceInstaller.ServiceName;
        }

        private readonly ActualService _actualService = new ActualService();

        protected override void OnStart(string[] args)
        {
            _actualService.Start(args);
        }

        protected override void OnStop()
        {
            _actualService.Stop();
        }
    }
}