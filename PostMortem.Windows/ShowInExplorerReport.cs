using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.Reports.Base;

namespace PostMortem.Windows
{
    public class ShowInExplorerReport : ReportDecoratorBase
    {
        public IReport Report { get; set; }
        public IReportFile FileToShow { get; set; }
        protected override IReport BaseReport => Report;

        public override event EventHandler Reported;
        public override event EventHandler Cancelled;

        public ShowInExplorerReport()
        {
        }

        public ShowInExplorerReport(IReport report, IReportFile fileToShow)
        {
            Report = report;
            FileToShow = fileToShow;
        }

        public override async Task ReportAsync(CancellationToken cancellationToken)
        {
            await base.ReportAsync(cancellationToken);

            if (!File.Exists(FileToShow.FilePath))
                throw new FileNotFoundException($"Cannot found report file path \"{FileToShow.FilePath}\".");

            Process.Start("explorer", $"/select,\"{FileToShow.FilePath}\"");

            Reported?.Invoke(this, EventArgs.Empty);
        }

        public override async Task CancelAsync()
        {
            await base.CancelAsync();
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}