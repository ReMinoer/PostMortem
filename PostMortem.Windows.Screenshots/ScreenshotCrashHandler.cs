using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        private Screenshot[] _screenshots;
        public ImageFormat ImageFormat { get; set; } = ImageFormat.Png;
        public CrashMultiPathProvider PathProvider { get; }

        public ScreenshotCrashHandler()
        {
            PathProvider = new CrashMultiPathProvider
            {
                NameProvider = (c, i) => c.GetDefaultFileName("screenshot", ImageFormat.ToString().ToLower(), i)
            };
        }

        public override async Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            _screenshots = await Task.WhenAll(Screen.AllScreens.Select((screen, index) => Task.Run(() =>
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

                        using (var memoryStream = new MemoryStream())
                        {
                            bitmap.Save(memoryStream, ImageFormat);
                            return new Screenshot(suggestedFileName, memoryStream.ToArray());
                        }
                    }
                }
                catch (Exception)
                {
                    return new Screenshot(suggestedFileName, Array.Empty<byte>());
                }
            }, cancellationToken)));

            foreach (var screenshot in _screenshots)
                await report.AddBytesAsync(screenshot, cancellationToken);

            return true;
        }

        public class Screenshot : IReportBytes
        {
            public string SuggestedFileName { get; }
            public byte[] Bytes { get; }
            public bool CanReport => Bytes != null;

            public Screenshot(string suggestedFileName, byte[] bytes)
            {
                SuggestedFileName = suggestedFileName;
                Bytes = bytes;
            }
        }
    }
}