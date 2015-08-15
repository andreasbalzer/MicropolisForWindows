using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Engine.Libs
{
    public static class LoadFiles
    {
        /// <summary>
        /// Gets the packaged file from resources.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static async Task<StorageFile> GetPackagedFile(string folderName, string fileName)
        {
            StorageFolder installFolder = Package.Current.InstalledLocation;

            if (folderName != null)
            {
                StorageFolder subFolder = await installFolder.GetFolderAsync(folderName);
                return await subFolder.GetFileAsync(fileName);
            }
            else
            {
                return await installFolder.GetFileAsync(fileName);
            }
        }
    }
}
