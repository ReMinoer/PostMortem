﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.Windows.Forms
{
    public class WinFormsMessageBoxCrashHandler : CrashHandlerBase
    {
        public string Text { get; set; } = "An execution error occurred and the program must close.";
        public string Caption { get; set; } = "Error";
        public MessageBoxButtons Buttons { get; set; } = MessageBoxButtons.OK;
        public MessageBoxIcon Icon { get; set; } = MessageBoxIcon.Error;
        public MessageBoxDefaultButton DefaultResult { get; set; } = MessageBoxDefaultButton.Button1;
        public DialogResult ExpectedResult { get; set; } = DialogResult.OK;
        public MessageBoxOptions Options { get; set; } = MessageBoxOptions.ServiceNotification;

        static public WinFormsMessageBoxCrashHandler InformUser => new WinFormsMessageBoxCrashHandler();
        static public WinFormsMessageBoxCrashHandler AskUser => new WinFormsMessageBoxCrashHandler
        {
            Text = "An execution error occurred and the program must close." + Environment.NewLine +
                      "Do you want to generate a report to help developers to fix the issue ?",
            Buttons = MessageBoxButtons.YesNo,
            DefaultResult = MessageBoxDefaultButton.Button2,
            ExpectedResult = DialogResult.Yes
        };

        public override bool HandleCrashImmediately(ICrashContext crashContext)
        {
            DialogResult result = MessageBox.Show(Text, Caption, Buttons, Icon, DefaultResult, Options);
            return result == ExpectedResult;
        }

        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken) => Task.FromResult(true);
        public override Task CleanAfterCancelAsync() => Task.CompletedTask;
    }
}