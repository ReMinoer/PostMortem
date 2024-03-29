﻿using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;
using PostMortem.Utils;

namespace PostMortem.CrashHandlers
{
    public class ExceptionInfoCrashHandler : TextCrashHandlerBase
    {
        protected override string GetDefaultFileName(ICrashContext crashContext) => crashContext.GetDefaultFileName("exception", "txt");

        protected override Task<string> GetTextAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(crashContext.Exception.ToString());
        }

        public override Task CleanAfterCancelAsync() => Task.CompletedTask;
    }
}