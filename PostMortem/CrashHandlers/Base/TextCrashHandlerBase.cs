using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class TextCrashHandlerBase : SinglePartCrashHandlerBase, IReportText
    {
        public string Text { get; private set; }
        public string SuggestedFileName { get; private set; }

        public override bool CanReport => Text != null;

        protected override sealed async Task CreatePartAsync(CrashPathProvider pathProvider, ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            SuggestedFileName = pathProvider.GetName(crashContext);
            Text = await GetTextAsync(crashContext, cancellationToken);
            await report.AddTextAsync(this, cancellationToken);
        }

        protected abstract Task<string> GetTextAsync(ICrashContext crashContext, CancellationToken cancellationToken);
    }
}