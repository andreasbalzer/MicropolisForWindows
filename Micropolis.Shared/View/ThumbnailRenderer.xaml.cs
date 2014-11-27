 // The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Micropolis.View
{
    using System;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;

    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;

    using Micropolis.Controller;

    /// <summary>
    /// Renders thumbnails of citys to be used in advertizing and main menu
    /// </summary>
    public sealed partial class ThumbnailRenderer : UserControl
    {
        #region Fields

        /// <summary>
        /// Link to Controller
        /// </summary>
        private readonly ThumbnailCreator _creator;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Sets up link to controller
        /// </summary>
        public ThumbnailRenderer()
        {
            this.InitializeComponent();
            this._creator = new ThumbnailCreator(this);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Renders a Micropolis savegame
        /// </summary>
        /// <param name="file">The savegame to render</param>
        /// <returns>A rendered version as a RenderTargetBitmap</returns>
        public async Task<RenderTargetBitmap> Render(IStorageFile file)
        {
            await this._creator.Render(file);
            var bitmap = new RenderTargetBitmap();
            this.RenderPane.UpdateLayout();

            await bitmap.RenderAsync(this.RenderPane);

            RenderTargetBitmap renderTargetBitmap = bitmap;

            return renderTargetBitmap;
        }

        /// <summary>
        /// Renders a Micropolis savegame and saves the rendered image to disk
        /// </summary>
        /// <param name="fileToRender">The savegame to render</param>
        /// <returns>awaitable task</returns>
        public async Task RenderAndSaveToDisk(IStorageFile fileToRender)
        {
            var renderTargetBitmap = await this.Render(fileToRender);
            
            IBuffer pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder cityThumbs = await localFolder.GetFolderAsync("cityThumbs");

            StorageFile file =
                await cityThumbs.CreateFileAsync(fileToRender.Name + ".png", CreationCollisionOption.ReplaceExisting);

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    96d,
                    96d,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
            }
        }

        #endregion

    }
}