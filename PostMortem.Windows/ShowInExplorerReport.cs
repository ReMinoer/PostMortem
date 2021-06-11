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

        public ShowInExplorerReport()
        {
        }

        public ShowInExplorerReport(IReport report, IReportFile fileToShow)
        {
            Report = report;
            FileToShow = fileToShow;
        }

        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await base.ProcessAsync(cancellationToken);

            if (!File.Exists(FileToShow.FilePath))
                throw new FileNotFoundException($"Cannot found report file path \"{FileToShow.FilePath}\".");

            Process.Start("explorer", $"/select,\"{FileToShow.FilePath}\"");
        }
    }
}