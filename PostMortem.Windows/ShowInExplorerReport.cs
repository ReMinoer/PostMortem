using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.Reports.Base;

namespace PostMortem.Windows
{
    public class ShowInExplorerReport<TReport> : ReportDecoratorBase
        where TReport : IReport
    {
        public TReport Report { get; set; }
        public Func<TReport, string> ReportPathProvider { get; set; }

        protected override IReport BaseReport => Report;

        public override event EventHandler Reported;
        public override event EventHandler Cancelled;

        public ShowInExplorerReport()
        {
        }

        public ShowInExplorerReport(TReport report, Func<TReport, string> reportPathProvider)
        {
            Report = report;
            ReportPathProvider = reportPathProvider;
        }

        public override async Task ReportAsync(CancellationToken cancellationToken)
        {
            await base.ReportAsync(cancellationToken);

            string reportPath = ReportPathProvider(Report);
            if (!File.Exists(reportPath) && !Directory.Exists(reportPath))
                throw new FileNotFoundException($"Cannot found report path \"{reportPath}\".");

            Process.Start("explorer", $"/select,\"{reportPath}\"");

            Reported?.Invoke(this, EventArgs.Empty);
        }

        public override async Task CleanAfterCancelAsync()
        {
            await base.CleanAfterCancelAsync();
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}