using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class TextCrashHandlerBase : SinglePartCrashHandlerBase
    {
        public string Text { get; private set; }

        protected override sealed async Task CreatePartAsync(CrashPathProvider pathProvider, ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            Text = await GetTextAsync(crashContext, cancellationToken);
            await report.AddTextPartAsync(Text, SuggestedFileName, PartId, cancellationToken);
        }

        protected abstract Task<string> GetTextAsync(ICrashContext crashContext, CancellationToken cancellationToken);
    }
}