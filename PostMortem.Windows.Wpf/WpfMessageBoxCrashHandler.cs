using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.Windows.Wpf
{
    public class WpfMessageBoxCrashHandler : CrashHandlerBase
    {
        public string Message { get; set; } = "An execution error occurred and the program must close.";
        public string Caption { get; set; } = "Error";
        public MessageBoxButton Buttons { get; set; } = MessageBoxButton.OK;
        public MessageBoxImage Icon { get; set; } = MessageBoxImage.Error;
        public MessageBoxResult DefaultResult { get; set; } = MessageBoxResult.OK;
        public MessageBoxResult ExpectedResult { get; set; } = MessageBoxResult.OK;
        public MessageBoxOptions Options { get; set; } = MessageBoxOptions.ServiceNotification;

        static public WpfMessageBoxCrashHandler InformUser => new WpfMessageBoxCrashHandler();
        static public WpfMessageBoxCrashHandler AskUser => new WpfMessageBoxCrashHandler
        {
            Message = "An execution error occurred and the program must close." + Environment.NewLine +
                      "Do you want to generate a report to help developers to fix the issue ?",
            Buttons = MessageBoxButton.YesNo,
            DefaultResult = MessageBoxResult.No,
            ExpectedResult = MessageBoxResult.Yes
        };
        
        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            MessageBoxResult result = MessageBox.Show(Message, Caption, Buttons, Icon, DefaultResult, Options);
            return Task.FromResult(result == ExpectedResult);
        }

        public override Task ConfigureReportAsync(IReport report, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}