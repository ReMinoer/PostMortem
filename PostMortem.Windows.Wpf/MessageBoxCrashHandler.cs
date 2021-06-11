using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.Windows.Wpf
{
    public class MessageBoxCrashHandler : CrashHandlerBase
    {
        public Window Owner { get; set; }
        public string Message { get; set; } = "An execution error occurred and the program must close.";
        public string Caption { get; set; } = "Error";
        public MessageBoxButton Buttons { get; set; } = MessageBoxButton.OK;
        public MessageBoxImage Icon { get; set; } = MessageBoxImage.Error;
        public MessageBoxResult DefaultResult { get; set; } = MessageBoxResult.OK;
        public MessageBoxResult ExpectedResult { get; set; } = MessageBoxResult.OK;
        public MessageBoxOptions Options { get; set; }

        static public MessageBoxCrashHandler InformUser => new MessageBoxCrashHandler();
        static public MessageBoxCrashHandler AskUser => new MessageBoxCrashHandler
        {
            Message = "An execution error occurred and the program must close." + Environment.NewLine +
                      "Do you want to generate a report to help developers to fix the issue ?",
            Buttons = MessageBoxButton.YesNo,
            DefaultResult = MessageBoxResult.No,
            ExpectedResult = MessageBoxResult.Yes
        };
        
        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            var result = Owner != null
                ? MessageBox.Show(Owner, Message, Caption, Buttons, Icon, DefaultResult, Options)
                : MessageBox.Show(Message, Caption, Buttons, Icon, DefaultResult, Options);

            return Task.FromResult(result == ExpectedResult);
        }
    }
}