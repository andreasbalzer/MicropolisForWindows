using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Engine;
using Micropolis.Lib.graphics;

namespace Micropolis
{
    using System.Threading;

    // This file is part of Micropolis for WinRT.
    // Copyright (C) 2014 Andreas Balzer, Felix Dietrich, Florian Thurnwald and Ivo Vutov
    // Portions Copyright (C) MicropolisJ by Jason Long
    // Portions Copyright (C) Micropolis Don Hopkins
    // Portions Copyright (C) 1989-2007 Electronic Arts Inc.
    //
    // Micropolis for WinRT is free software; you can redistribute it and/or modify
    // it under the terms of the GNU GPLv3, with Additional terms.
    // See the README file, included in this distribution, for details.
    // Project website: http://code.google.com/p/micropolis/

    /// <summary>
    ///     Stores all TileImages and their information
    /// </summary>
    public class TileImages
    {
        /// <summary>
        /// References Tile Images for various resolutions / zoom levels
        /// </summary>
        private static readonly Dictionary<int, TileImages> SAVED_INSTANCES = new Dictionary<int, TileImages>();

        /// <summary>
        ///     The tile height at the current zoom level
        /// </summary>
        public int TileHeight;

        /// <summary>
        ///     The tile width at the current zoom level
        /// </summary>
        public int TileWidth;

        private WriteableBitmap[] _images;
        private Dictionary<SpriteKind, Dictionary<int, WriteableBitmap>> _spriteImages;

        private TileImages(int size)
        {
            TileWidth = size;
            TileHeight = size;
        }

        /// <summary>
        ///     Initializes the TileImages by loading all tile images for all sizes
        /// </summary>
        /// <returns></returns>
        public static async Task Initialize(CancellationToken cancelToken)
        {
            Micropolis.Utils.ThreadCancellation.CheckCancellation(cancelToken);
            var im8 = new TileImages(8);
            var im16 = new TileImages(16);
            var im32 = new TileImages(32);

            SAVED_INSTANCES.Add(8, im8);
            SAVED_INSTANCES.Add(16, im16);
            SAVED_INSTANCES.Add(32, im32);
            
            Micropolis.Utils.ThreadCancellation.CheckCancellation(cancelToken);
            await im8.SetUp(8);
            Micropolis.Utils.ThreadCancellation.CheckCancellation(cancelToken);
            await im16.SetUp(16);
            Micropolis.Utils.ThreadCancellation.CheckCancellation(cancelToken);
            await im32.SetUp(32);
        }

        /// <summary>
        ///     Sets up this instance for the specified tile size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Awaitable task.</returns>
        private async Task SetUp(int size)
        {
            if (size != 16)
            {
                WriteableBitmap[] result =
                    await LoadTileImages("ms-appx:///resources/images/Game/tiles/tiles_" + size + "x" + size + ".png", size);
                App.LoadPageReference.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { _images = result; });
            }
            else
            {
                WriteableBitmap[] result = await LoadTileImages("ms-appx:///resources/images/Game/tiles/tiles.png", 16);
                App.LoadPageReference.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { _images = result; });
            }

            await LoadSpriteImages();
        }

        /// <summary>
        ///     Gets the TileImages instance for the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static TileImages GetInstance(int size)
        {
            return SAVED_INSTANCES[size];
        }

        /// <summary>
        ///     Gets the tile image for the specified tile.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        public WriteableBitmap GetTileImage(int cell)
        {
            int tile = (cell & TileConstants.LOMASK)%_images.Length;
            return _images[tile];
        }

        /// <summary>
        ///     Loads the tile images.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="srcSize">Size of the source.</param>
        /// <returns></returns>
        private async Task<WriteableBitmap[]> LoadTileImages(String resourceName, int srcSize)
        {
            var iconUrl = new Uri(resourceName, UriKind.RelativeOrAbsolute);
            var refImage = new WriteableBitmap(10, 10);
            refImage = await refImage.FromContent(iconUrl);

            var images = new List<WriteableBitmap>();

            int elements = refImage.PixelHeight/srcSize;


            using (refImage.GetBitmapContext())
            {
                for (int i = 0; i < elements; i++)
                {
                    var bi = new WriteableBitmap(TileWidth, TileHeight);
                    using (bi.GetBitmapContext())
                    {
                        bi.DrawInto(refImage, 0, 0,
                            0, i*srcSize,
                            srcSize, i*srcSize + srcSize);

                        images.Add(bi);
                    }
                }
            }

            return images.ToArray<WriteableBitmap>();
        }

        /// <summary>
        ///     Gets the sprite image for the specified kind at the specified animation frame.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="frameNumber">The frame number.</param>
        /// <returns></returns>
        public WriteableBitmap GetSpriteImage(SpriteKind kind, int frameNumber)
        {
            return _spriteImages[kind][frameNumber];
        }

        /// <summary>
        ///     Loads the sprite images.
        /// </summary>
        /// <returns></returns>
        private async Task LoadSpriteImages()
        {
            _spriteImages = new Dictionary<SpriteKind, Dictionary<int, WriteableBitmap>>();
            foreach (SpriteKind kind in SpriteKinds.SpriteKind.Values)
            {
                var imgs = new Dictionary<int, WriteableBitmap>();
                for (int i = 0; i < kind.NumFrames; i++)
                {
                    WriteableBitmap img = await LoadSpriteImage(kind, i);
                    if (img != null)
                    {
                        imgs.Add(i, img);
                    }
                }
                _spriteImages.Add(kind, imgs);
            }
        }

        /// <summary>
        ///     Loads the sprite image from various dynamically determined sources.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="frameNo">The frame no.</param>
        /// <returns></returns>
        private async Task<WriteableBitmap> LoadSpriteImage(SpriteKind kind, int frameNo)
        {
            String resourceName = "ms-appx:///resources/images/Game/sprites/obj" + kind.ObjectId + "-" + frameNo;

            // first, try to load specific size image
            var iconUri = new Uri(resourceName + "_" + TileWidth + "x" + TileHeight + ".png",
                UriKind.RelativeOrAbsolute);


            StorageFolder folder = Package.Current.InstalledLocation;
            folder = await folder.GetFolderAsync("resources");
            folder = await folder.GetFolderAsync("images");
            folder = await folder.GetFolderAsync("Game");
            folder = await folder.GetFolderAsync("sprites");
            IStorageItem file = null;

#if WINDOWS_PHONE_APP
            try
            {
                file = await folder.GetFileAsync(
                    iconUri.AbsoluteUri.Substring(iconUri.AbsoluteUri.LastIndexOf("/", StringComparison.Ordinal) + 1));
            }
            catch (FileNotFoundException)
            {
                file = null;
            }
#else

            file = await folder.TryGetItemAsync(iconUri.AbsoluteUri.Substring(iconUri.AbsoluteUri.LastIndexOf("/", StringComparison.Ordinal) + 1));
#endif
            if (file != null)
            {

                var image = new WriteableBitmap(10, 10);
                image = await image.FromContent(iconUri);

                return image;
            }

            iconUri = new Uri(resourceName + ".png", UriKind.RelativeOrAbsolute);
            
#if WINDOWS_PHONE_APP
            try
            {
                file = await folder.GetFileAsync(resourceName.Substring(resourceName.LastIndexOf("/", StringComparison.Ordinal) + 1) + ".png");
            }
            catch (FileNotFoundException)
            {
                file = null;
            }
#else
            file = await folder.TryGetItemAsync(resourceName.Substring(resourceName.LastIndexOf("/", StringComparison.Ordinal) + 1) + ".png");
#endif

            if (file == null)
            {
                return null;
            }

            if (TileWidth == 16 && TileHeight == 16)
            {
                var image = new WriteableBitmap(10, 10);
                image = await image.FromContent(iconUri);

                return image;
            }

            // scale the image ourselves
            var ii = new WriteableBitmap(10, 10);
            ii = await ii.FromContent(iconUri);


            int destWidth = ii.PixelWidth*TileWidth/16;
            int destHeight = ii.PixelHeight*TileHeight/16;

            WriteableBitmap bi = ii.Resize(destWidth, destHeight, WriteableBitmapExtensions.Interpolation.Bilinear);
            return bi;
        }
    }
}