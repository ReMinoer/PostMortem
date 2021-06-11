using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class CompositeCrashHandlerBase : CrashHandlerBase, IEnumerable
    {
        public List<ICrashHandler> CrashHandlers { get; } = new List<ICrashHandler>();
        public bool AlwaysContinue { get; set; }
        
        public override async Task ConfigureReportAsync(IReport compositeReport, CancellationToken cancellationToken)
        {
            foreach (var crashHandler in CrashHandlers)
                await crashHandler.ConfigureReportAsync(compositeReport, cancellationToken);
        }
        
        public void Add(ICrashHandler crashHandler) => CrashHandlers.Add(crashHandler);
        public IEnumerator GetEnumerator() => CrashHandlers.GetEnumerator();
    }
}