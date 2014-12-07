namespace Micropolis.Controller
{
    using System.Threading.Tasks;

    using Windows.Storage;
    using Windows.UI.Xaml.Media.Imaging;

    using Engine;

    using Micropolis.View;
    
    /// <summary>
    /// Controller to render thumbnail images of savegames. Should be initialized and used via ThumbnailRenderer
    /// </summary>
    public class ThumbnailCreator
    {
        #region Fields

        private readonly Micropolis _engine;

        private readonly ThumbnailRenderer _renderer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Sets up an empty engine and establishes the link to the renderer
        /// </summary>
        /// <param name="renderer"></param>
        public ThumbnailCreator(ThumbnailRenderer renderer)
        {
            this._engine = new Micropolis();
            this._renderer = renderer;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Renders a savegame and returns a RenderTargetImage
        /// </summary>
        /// <param name="file">The savegame to render</param>
        /// <returns>Rendered image as RenderTargetImage</returns>
        public async Task Render(IStorageFile file)
        {
            await this._engine.Load((StorageFile)file);
            this._renderer.RenderPane.ViewModel.SetEngine(this._engine);
            this._renderer.RenderPane.ViewModel.PaintComponent();

        }

        #endregion
    }
}