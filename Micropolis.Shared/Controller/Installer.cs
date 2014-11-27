namespace Micropolis.Controller
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.ApplicationModel;
    using Windows.Storage;
    using Windows.UI.Core;

    using Micropolis.Utils;

    /// <summary>
    /// Provides methods to run at first app start
    /// </summary>
    public class Installer
    {
        #region Methods

        /// <summary>
        /// Creates the CityThumbs folder and generates images for prepackaged city files
        /// </summary>
        /// <param name="cancelToken">Cancel token to notify of cancellation</param>
        /// <returns>task to await</returns>
        internal static async Task CreateCityFolderAndThumbnails(CancellationToken cancelToken)
        {
            ThreadCancellation.CheckCancellation(cancelToken);
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            IStorageItem cityThumbs=null;
#if WINDOWS_PHONE_APP
            try
            {
                cityThumbs = await localFolder.GetFolderAsync("cityThumbs");
            }
            catch (Exception exp)
            {
            }
#else
            cityThumbs = await localFolder.TryGetItemAsync("cityThumbs");
#endif
            bool cityThumbsExists = cityThumbs != null;
            bool installComplete = false;
            if (cityThumbsExists)
            {
#if WINDOWS_PHONE_APP
            try
            {

                installComplete = (await ((StorageFolder)cityThumbs).GetFileAsync("installComplete.txt"))!=null;
            }
            catch (Exception exp)
            {
            }
#else
                installComplete = (await ((StorageFolder) cityThumbs).TryGetItemAsync("installComplete.txt"))
                                  != null;
#endif
            }

            bool cityThumbImagesExist = cityThumbsExists
                                        && installComplete;
            if (!cityThumbsExists || !cityThumbImagesExist)
            {
                cityThumbs = await localFolder.CreateFolderAsync("cityThumbs", CreationCollisionOption.OpenIfExists);
                //ToDo: copy precreated images there from resources

                StorageFolder installFolder = Package.Current.InstalledLocation;
                StorageFolder cityFolder = await installFolder.GetFolderAsync("resources");
                cityFolder = await cityFolder.GetFolderAsync("cities");

                ThreadCancellation.CheckCancellation(cancelToken);


                var completionSource = new TaskCompletionSource<bool>();
                App.LoadPageReference.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        foreach (StorageFile file in await cityFolder.GetFilesAsync())
                        {
                            if (file.FileType == ".cty")
                            {
#if WINDOWS_PHONE_APP
                                ///Bug:add this
#else
                                await App.LoadPageReference.ThumbRender.RenderAndSaveToDisk(file);
#endif
                            }
                        }
                        completionSource.SetResult(true);
                    });

                await ((StorageFolder) cityThumbs).CreateFileAsync(
                    "installComplete.txt",
                    CreationCollisionOption.ReplaceExisting);

                await (Task)completionSource.Task;
            }
        }

#endregion
    }
}