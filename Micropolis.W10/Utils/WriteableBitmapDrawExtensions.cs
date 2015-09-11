namespace Micropolis.Lib.graphics
{
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

    using System;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Graphics.Display;
    using Windows.Graphics.Imaging;
    using Windows.Storage.Streams;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// Provides extensions for WriteableBitmap to allow for string rendering and drawing of graphics data according to MicropolisJ instructions.
    /// </summary>
    public static class WriteableBitmapDrawExtensions
    {
        /// <summary>Draws a string into a WriteableBitmap at specific coordinates in a specific color.
        /// </summary>
        /// <param name="dest">The WriteableBitmap to draw into</param>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The x-coordinate (top left) to draw to.</param>
        /// <param name="y">The y-coordinate (top left) to draw to.</param>
        /// <param name="color">The color to render the text in.</param>
        /// <param name="renderTextBlock">The textBlock in visible Visual Tree to render with.</param>
        /// <param name="SPToRender">The StackPanel to render with.</param>
        /// <returns>An awaitable task.</returns>
        /// <remarks>Thanks to http://geoffwebbercross.blogspot.de/2013/06/windows-81-rendertargetbitmap.html</remarks>
        public static async Task DrawString(this WriteableBitmap dest, string text, int x, int y, Color color,
            TextBlock renderTextBlock, UIElement SPToRender = null)
        {
            if (SPToRender == null)
            {
                SPToRender = renderTextBlock;
            }
            renderTextBlock.Foreground = new SolidColorBrush(color);
            renderTextBlock.Text = text;
            renderTextBlock.UpdateLayout();

            var renderTargetBitmap = new RenderTargetBitmap();
            try
            {
                var bitmap = new WriteableBitmap(1, 1);

                await
                    renderTargetBitmap.RenderAsync(SPToRender, (int) ((FrameworkElement) SPToRender).ActualWidth,
                        (int) ((FrameworkElement) SPToRender).ActualHeight);
                var stream = new InMemoryRandomAccessStream();
                using (stream)
                {
                    BitmapEncoder encoder = await
                        BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);

                    byte[] bytes = (await renderTargetBitmap.GetPixelsAsync()).ToArray();

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                        (uint) renderTargetBitmap.PixelWidth, (uint) renderTargetBitmap.PixelHeight,
                        DisplayProperties.LogicalDpi, DisplayProperties.LogicalDpi, bytes);

                    await encoder.FlushAsync();

                    bitmap = await bitmap.FromStream(stream);
                }
                DrawInto(dest, bitmap, x, y, WriteableBitmapExtensions.BlendMode.None);
            }
            catch (Exception e)
            {
                string p = "";
            }
        }

        /// <summary>
        /// Blits a WriteableBitmap into another.
        /// </summary>
        /// <param name="dest">The destination WriteableBitmap to blit into.</param>
        /// <param name="backgroundImage">The WriteableBitmap to blit into the destination WriteableBitmap.</param>
        /// <param name="xDestOffset">The x-coordinate in dest to blit to.</param>
        /// <param name="yDestOffset">The y-coordinate in dets to blit to.</param>
        /// <param name="mode">The mode to blit with (transparency).</param>
        public static void DrawInto(this WriteableBitmap dest, WriteableBitmap backgroundImage, int xDestOffset,
            int yDestOffset, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.None)
        {
            dest.DrawInto(backgroundImage, xDestOffset, yDestOffset, 0, 0, backgroundImage.PixelWidth,
                backgroundImage.PixelHeight, mode);
        }

        /// <summary>
        /// Blits a WriteableBitmap into another.
        /// </summary>
        /// <param name="dest">The destination WriteableBitmap to blit into.</param>
        /// <param name="backgroundImage">The WriteableBitmap to blit into the destination WriteableBitmap.</param>
        /// <param name="xDestOffset">The x-coordinate in dest to blit to.</param>
        /// <param name="yDestOffset">The y-coordinate in dets to blit to.</param>
        /// <param name="xstart">Crop x-coordinate (top left) in backgroundImage</param>
        /// <param name="ystart">Crop y-coordinate (top left) in backgroundImage</param>
        /// <param name="xend">Crop x-coordinate (bottom right) in backgroundImage</param>
        /// <param name="yend">Crop y-coordinate (bottom right) in backgroundImage</param>
        /// <param name="mode">The mode to blit with (transparency).</param>
        public static void DrawInto(this WriteableBitmap dest, WriteableBitmap backgroundImage, int xDestOffset,
            int yDestOffset, int xstart, int ystart, int xend, int yend,
            WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.None)
        {
            if (xDestOffset + xend - xstart > dest.PixelWidth)
            {
                xend = dest.PixelWidth - xDestOffset + xstart;
            }
            if (yDestOffset + yend - ystart > dest.PixelHeight)
            {
                yend = dest.PixelHeight - yDestOffset + ystart;
            }
            if (xstart < 0 || ystart < 0 || xend > backgroundImage.PixelWidth || yend > backgroundImage.PixelHeight)
            {
                throw new ArgumentException("Region not within source image");
            }
            if (xend-xstart < 0 || yend-ystart < 0)
            {
                return;
            }
            var destRect = new Rect(xDestOffset, yDestOffset, xend - xstart, yend - ystart);
            var srcRect = new Rect(xstart, ystart, xend - xstart, yend - ystart);

            dest.Blit(destRect, backgroundImage, srcRect, mode);
        }
    }
}