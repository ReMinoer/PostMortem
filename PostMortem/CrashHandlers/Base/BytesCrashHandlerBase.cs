using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class BytesCrashHandlerBase : SinglePartCrashHandlerBase, IReportBytes
    {
        public byte[] Bytes { get; private set; }
        public string SuggestedFileName { get; set; }
        public override bool CanReport => Bytes != null;

        protected override sealed async Task CreatePartAsync(CrashPathProvider pathProvider, ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            SuggestedFileName = pathProvider.GetName(crashContext);
            Bytes = await GetBytesAsync(crashContext, cancellationToken);
            await report.AddBytesAsync(this, cancellationToken);
        }

        protected abstract Task<byte[]> GetBytesAsync(ICrashContext crashContext, CancellationToken cancellationToken);
    }
}