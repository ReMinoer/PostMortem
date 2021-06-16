using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;
using PostMortem.Utils;
using WpfScreenHelper;
using Size = System.Drawing.Size;

namespace PostMortem.Windows.Screenshots
{
    public class ScreenshotCrashHandler : CrashHandlerBase
    {
        public ImageFormat ImageFormat { get; set; } = ImageFormat.Png;
        public CrashMultiPathProvider PathProvider { get; }
        public object PartId { get; set; }

        public ScreenshotCrashHandler()
        {
            PathProvider = new CrashMultiPathProvider
            {
                NameProvider = (c, i) => c.GetDefaultFileName("screenshot", ImageFormat.ToString().ToLower(), i)
            };
        }

        public override async Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            await Task.WhenAll(Screen.AllScreens.Select(async (screen, index) =>
            {
                var suggestedFileName = PathProvider.GetName(crashContext, index.ToString());
                try
                {
                    int x = (int)screen.Bounds.X;
                    int y = (int)screen.Bounds.Y;
                    int width = (int)screen.Bounds.Width;
                    int height = (int)screen.Bounds.Height;

                    using (Bitmap bitmap = new Bitmap(width, height))
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                            graphics.CopyFromScreen(x, y, 0, 0, new Size(width, height));
                        
                        using (IReportPart reportPart = await report.CreatePartAsync(suggestedFileName, PartId, cancellationToken))
                            bitmap.Save(reportPart.Stream, ImageFormat);
                    }
                }
                catch (Exception)
                {
                    (await report.CreatePartAsync(suggestedFileName, PartId, cancellationToken)).Dispose();
                }

            }));

            return true;
        }
    }
}