﻿using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.Reports.Base;
using PostMortem.Utils;

namespace PostMortem.Reports
{
    public class ZipArchiveReport : ReportDecoratorBase, IReportFile
    {
        private readonly FolderReport _folderReport;
        protected override IReport BaseReport => _folderReport;

        public string FilePath { get; private set; }
        public string TemporaryFolderPath => _folderReport.FolderPath;

        public CrashPathProvider PathProvider { get; }
        public CrashPathProvider TemporaryFolderPathProvider => _folderReport.FolderPathProvider;

        public bool CanReport => !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath);

        public ZipArchiveReport()
        {
            _folderReport = new FolderReport();

            PathProvider = new CrashPathProvider
            {
                NameProvider = c => c.GetDefaultFileName("crash_report", "zip")
            };
        }

        public override Task PrepareAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            FilePath = PathProvider.GetPath(crashContext);
            return base.PrepareAsync(crashContext, cancellationToken);
        }

        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await base.ProcessAsync(cancellationToken);
            await Task.Run(() => ZipFile.CreateFromDirectory(TemporaryFolderPath, FilePath), cancellationToken);

            Directory.Delete(TemporaryFolderPath, recursive: true);
        }
    }
}