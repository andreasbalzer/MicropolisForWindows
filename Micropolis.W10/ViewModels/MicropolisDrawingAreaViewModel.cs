using System;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Engine;
using Micropolis.Lib.graphics;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Micropolis.ViewModels
{
    public class MicropolisDrawingAreaViewModel : IMapListener
    {
        private static readonly int DEFAULT_TILE_SIZE = 32;

        /// <summary>
        ///     Earthquake shake steps
        /// </summary>
        public static int SHAKE_STEPS = 40;

        private readonly int FRAMES_PER_SECOND = 24;
        private readonly GraphicsBuffer _buffer;
        private readonly CoreDispatcher _dispatcher;
        private readonly Image _imageOutput;
        private readonly Grid _layoutRoot;
        private readonly Grid _sPToRender;
        private readonly TextBlock _textBlockToRender;

        private Stream _imageStream;

        /// <summary>
        ///     height of one tile in pixel
        /// </summary>
        public int TILE_HEIGHT;

        /// <summary>
        ///     width of one tile in pixel
        /// </summary>
        public int TILE_WIDTH;

        private bool _initialized;

        private bool _isCurrentEarthquake;
        private MainGamePageViewModel _mainPage;

        private bool blink;
        private DispatcherTimer blinkTimer;
        private bool blinkUnpoweredZones = true;
        private int dragX, dragY;
        private bool dragging;
        private long lastRepaintTicks;
        private Engine.Micropolis m;
        private bool needsBlinking;
        private bool repaintNow;
        private int shakeStep;

        private TileImages tileImages;
        private ToolCursor toolCursor;
        private ToolPreview toolPreview;

        public MicropolisDrawingAreaViewModel(Grid layoutRoot, Image imageOutput, Image imageCursor,
            Grid stackPanelToRender, TextBlock textBlockToRender, CoreDispatcher dispatcher)
        {
            _buffer = new GraphicsBuffer();
            _layoutRoot = layoutRoot;
            _imageOutput = imageOutput;

            _sPToRender = stackPanelToRender;
            _textBlockToRender = textBlockToRender;
            _dispatcher = dispatcher;

        }

        /// <summary>
        ///     Image drawn containing all tiles and sprites of the game
        /// </summary>
        public WriteableBitmap Image { get; private set; }

        /// <summary>
        ///     Fires whenever mapoverlay data changed
        /// </summary>
        /// <param name="overlayDataType"></param>
        public void MapOverlayDataChanged(MapState overlayDataType)
        {
        }

        /// <summary>
        ///     Fired when sprite moved
        /// </summary>
        /// <param name="sprite">sprite moved</param>
        public void SpriteMoved(Sprite sprite)
        {
            //repaint(getSpriteBounds(sprite, sprite.lastX, sprite.lastY));
            //repaint(getSpriteBounds(sprite, sprite.x, sprite.y));

            _buffer.SetBuffer(sprite.LastY, sprite.LastY, sprite.Width, sprite.Height, TILE_WIDTH, TILE_HEIGHT);
            _buffer.SetBuffer(sprite.X, sprite.Y, sprite.Width, sprite.Height, TILE_WIDTH, TILE_HEIGHT);

            Repaint();
        }

        /// <summary>
        ///     Fired when tile changed.
        /// </summary>
        /// <param name="xpos">xpos of tile</param>
        /// <param name="ypos">ypos of tile</param>
        public void TileChanged(int xpos, int ypos)
        {
            Repaint();
        }

        /// <summary>
        ///     Fired when whole map changed.
        /// </summary>
        public void WholeMapChanged()
        {
            Repaint();
        }

        /// <summary>
        ///     Gets size of tool in pixel
        /// </summary>
        /// <returns>Size of tool in pixel</returns>
        public Size GetToolCursorSizeInPixel()
        {
            if (toolCursor == null || toolCursor.Rect == null)
            {
                return new Size(0, 0);
            }
            return new Size(toolCursor.Rect.Width*TILE_WIDTH, toolCursor.Rect.Height*TILE_HEIGHT);
        }

        /// <summary>
        ///     Sets up the control after basic initialization to allow for faster startup during load of control.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="mainPage"></param>
        public void SetUpAfterBasicInit(Engine.Micropolis engine, MainGamePageViewModel mainPage)
        {
            m = engine;
            _mainPage = mainPage;
            SelectTileSize(DEFAULT_TILE_SIZE);
            m.AddMapListener(this);

            StartBlinkTimer();
            _initialized = true;
        }

        /// <summary>
        ///     Renders the map every now and then (see FRAMES_PER_SECOND).
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        internal void Render(object sender, object e)
        {
            if (!_initialized)
            {
                return;
            }

            var x = new TimeSpan(0, 0, 0, 0, (int) (1.0/FRAMES_PER_SECOND*1000));
            long diff = DateTime.Now.Ticks - lastRepaintTicks;
            if (diff > x.Ticks)
            {
                if (repaintNow || UndrawnTilesInView())
                {
                    repaintNow = false;
                    PaintComponentInView();
                }

                lastRepaintTicks = DateTime.Now.Ticks;
            }
        }


        private void PaintComponentInView()
        {
            var paintX = (int) (_mainPage.HorizontalMapOffset/_mainPage.ZoomFactor/TILE_WIDTH);
            var paintY = (int) (_mainPage.VerticalMapOffset/_mainPage.ZoomFactor/TILE_HEIGHT);
            int paintXMax = (int) _mainPage.MapWidth/TILE_WIDTH;
            int paintYMax = (int) _mainPage.MapHeight/TILE_HEIGHT;
            PaintComponent(paintX, paintY, paintXMax, paintYMax);
        }

        private bool UndrawnTilesInView()
        { //ToDo: implement version based on bounding boxes
            var paintX = (int)(_mainPage.HorizontalMapOffset / _mainPage.ZoomFactor / TILE_WIDTH);
            var paintY = (int)(_mainPage.VerticalMapOffset / _mainPage.ZoomFactor / TILE_HEIGHT);
            int paintXMax = (int)_mainPage.MapWidth / TILE_WIDTH;
            int paintYMax = (int)_mainPage.MapHeight / TILE_HEIGHT;

            for (var x = paintX; x < paintXMax; x++)
            {
                for (var y = paintY; y < paintYMax; y++)
                {
                    if (_buffer.Get(x, y) == -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        ///     Handles PointerMoved event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void LayoutRoot_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            Pointer ptr = e.Pointer;
            if (ptr.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint ptrPt = e.GetCurrentPoint(_layoutRoot);

                if (ptrPt.Properties.IsRightButtonPressed)
                {
                    ContinueDrag((int) e.GetCurrentPoint(_layoutRoot).Position.X,
                        (int) e.GetCurrentPoint(_layoutRoot).Position.Y);
                }

                if (!ptrPt.Properties.IsLeftButtonPressed && !ptrPt.Properties.IsRightButtonPressed)
                {
                    PositionToolCursor(e.GetCurrentPoint(_layoutRoot).Position.X,
                        e.GetCurrentPoint(_layoutRoot).Position.Y);
                }
                else if (dragging)
                {
                    PositionToolCursor(Math.Min(dragX, e.GetCurrentPoint(_layoutRoot).Position.X),
                        Math.Min(dragY, e.GetCurrentPoint(_layoutRoot).Position.Y));
                }
            }
        }

        Point toolCursorPosition = new Point(0, 0);

        /// <summary>
        ///     Positions tool cursor at specified coordinate.
        /// </summary>
        /// <param name="mouseX">xpos</param>
        /// <param name="mouseY">ypos</param>
        public void PositionToolCursor(double mouseX, double mouseY)
        {
            if (toolCursor == null) {
                return;
            }

            if (toolCursor.Rect.Height == 3)
            {
                // big cursor like residential zone
                toolCursorPosition = new Point(Math.Ceiling(mouseX / TILE_WIDTH - 2) * TILE_WIDTH, Math.Ceiling(mouseY / TILE_HEIGHT - 2) * TILE_HEIGHT);
            }
            else if (toolCursor.Rect.Height == 1)
            {
                // small cursor like park
                toolCursorPosition = new Point(Math.Ceiling(mouseX / TILE_WIDTH - 1) * TILE_WIDTH, Math.Ceiling(mouseY / TILE_HEIGHT - 1) * TILE_HEIGHT);
            }
            else if (toolCursor.Rect.Height == 4)
            {
                // huge cursor like power plant
                toolCursorPosition = new Point(Math.Ceiling(mouseX / TILE_WIDTH - 2) * TILE_WIDTH, Math.Ceiling(mouseY / TILE_HEIGHT - 2) * TILE_HEIGHT);
            }
            else if (toolCursor.Rect.Height == 6)
            {
                // huger than huge cursor like airport
                toolCursorPosition = new Point(Math.Ceiling(mouseX / TILE_WIDTH - 2) * TILE_WIDTH, Math.Ceiling(mouseY / TILE_HEIGHT - 2) * TILE_HEIGHT);
            }
        }

        /// <summary>
        ///     Handles PointerReleased event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void LayoutRoot_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            Pointer ptr = e.Pointer;
            if (ptr.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint ptrPt = e.GetCurrentPoint(_layoutRoot);

                EndDrag((int) e.GetCurrentPoint(_layoutRoot).Position.X,
                    (int) e.GetCurrentPoint(_layoutRoot).Position.Y);
            }
        }

        /// <summary>
        ///     Handles PointerPressed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void LayoutRoot_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            Pointer ptr = e.Pointer;
            if (ptr.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint ptrPt = e.GetCurrentPoint(_layoutRoot);

                if (ptrPt.Properties.IsRightButtonPressed || ptrPt.Properties.IsLeftButtonPressed)
                {
                    StartDrag((int) e.GetCurrentPoint(_layoutRoot).Position.X,
                        (int) e.GetCurrentPoint(_layoutRoot).Position.Y);
                }
            }
            else if (ptr.PointerDeviceType == PointerDeviceType.Pen || ptr.PointerDeviceType == PointerDeviceType.Touch)
            {
                PointerPoint ptrPt = e.GetCurrentPoint(_layoutRoot);
                PositionToolCursor(ptrPt.Position.X, ptrPt.Position.Y);
            }
        }

        /// <summary>
        ///     Selects a new tile size (zooming via Tile size).
        /// </summary>
        /// <param name="newTileSize">new tile size</param>
        public void SelectTileSize(int newTileSize)
        {
            tileImages = TileImages.GetInstance(newTileSize);
            TILE_WIDTH = tileImages.TileWidth;
            TILE_HEIGHT = tileImages.TileHeight;

            int width = m.GetWidth();
            int height = m.GetHeight();
            _imageOutput.Height = height*TILE_HEIGHT;
            _imageOutput.Width = width*TILE_WIDTH;

            _imageOutput.UpdateLayout();

            _buffer.Reset(); //Bug: check whether here is an issue. if so just remove it
            Repaint();
        }

        /// <summary>
        ///     Gets the current tile size.
        /// </summary>
        /// <returns></returns>
        public int GetTileSize()
        {
            return TILE_WIDTH;
        }

        /// <summary>
        ///     Gets city location at specified coordinates.
        /// </summary>
        /// <param name="x">xpos</param>
        /// <param name="y">ypos</param>
        /// <returns></returns>
        public CityLocation GetCityLocation(int x, int y)
        {
            return new CityLocation(x/TILE_WIDTH, y/TILE_HEIGHT);
        }

        /// <summary>
        ///     Gets preferred Size.
        /// </summary>
        /// <returns>preferred size</returns>
        public Size GetPreferredSize()
        {
            //assert this.m != null;
            return new Size(TILE_WIDTH*m.GetWidth(), TILE_HEIGHT*m.GetHeight());
        }

        /// <summary>
        ///     Sets the new engine.
        /// </summary>
        /// <param name="newEngine">new engine to use</param>
        public void SetEngine(Engine.Micropolis newEngine)
        {
            //assert newEngine != null;

            if (m != null)
            {
                //old engine
                m.RemoveMapListener(this);
            }
            m = newEngine;
            if (m != null)
            {
                //new engine
                m.AddMapListener(this);
            }

            // size may have changed
            _buffer.Reset(); //Bug: check whether here is an issue. if so, just remove it.
            Repaint();
        }

        /// <summary>
        ///     Queues a map repaint so the map gets repainted once the next tick.
        /// </summary>
        public void Repaint()
        {
            repaintNow = true;
        }

        /// <summary>
        ///     Immediately repaints the drawing area.
        ///     This does not wait for the FPS timer, so dont call it regularly!
        /// </summary>
        internal void RepaintNow()
        {
            PaintComponent();
        }

        /// <summary>
        ///     Draws a sprite to the graphics.
        /// </summary>
        /// <param name="gr">The graphics to draw into.</param>
        /// <param name="sprite">The sprite to draw.</param>
        /// <returns>An awaitable task.</returns>
        private async Task DrawSprite(Sprite sprite)
        {
            //assert sprite.isVisible();

            var p = new Point(
                (sprite.X + sprite.Offx)*TILE_WIDTH/16,
                (sprite.Y + sprite.Offy)*TILE_HEIGHT/16
                );

            byte[] img = tileImages.GetSpriteImage(sprite.Kind, sprite.Frame - 1);
            if (img != null)
            {
                int dimension = (int)Math.Floor(Math.Sqrt(img.Length / 4));
                DrawSprite(_imageStream, (int)p.X, (int)p.Y, 0, img, dimension);
                _buffer.SetBuffer(p.X, p.Y, (int)2 * dimension, (int)2 * dimension, TILE_WIDTH, TILE_HEIGHT);
            }
        }

        /// <summary>
        ///     Clears the map so it gets redrawn.
        /// </summary>
        public void ClearMap()
        {
            _buffer.Reset();
            repaintNow = true;
        }


        /// <summary>
        ///     Paints the map component (does the actual drawing)
        /// </summary>
        /// <param name="paintX">smallest X in map to draw to</param>
        /// <param name="paintY">smallest Y in map to draw to</param>
        /// <param name="paintWidth">width of the draw rectangle to draw to</param>
        /// <param name="paintHeight">height of the draw rectangle to draw to</param>
        private void PaintComponent(int paintX = 0, int paintY = 0,
            int paintWidth = int.MaxValue/2,
            int paintHeight = int.MaxValue/2)
        {
            needsBlinking = false;

            int width = m.GetWidth();
            int height = m.GetHeight();

            paintWidth = Math.Min(paintWidth, width);
            paintHeight = Math.Min(paintHeight, height);

            int maxX = width;
            int maxY = height;

            bool imageNotCreated = Image == null;
            bool imageHasDifferentsize = !imageNotCreated &&
                                         (Image.PixelWidth != width*TILE_WIDTH ||
                                          Image.PixelHeight != height*TILE_HEIGHT);

            _buffer.CreateBuffer(maxX, maxY);

            bool resetBuffer = false;

            if (imageNotCreated || imageHasDifferentsize)
            {
                Image = new WriteableBitmap(width*TILE_WIDTH, height*TILE_HEIGHT);
                _imageStream = WindowsRuntimeBufferExtensions.AsStream(this.Image.PixelBuffer);

                _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _imageOutput.Height = height*TILE_HEIGHT;
                    _imageOutput.Width = width*TILE_WIDTH;
                });

                repaintNow = true;
                resetBuffer = true;
            }
            resetBuffer = resetBuffer || _isCurrentEarthquake;
            if (resetBuffer)
            {
                _buffer.Reset(maxX, maxY);
                repaintNow = true;
                _isCurrentEarthquake = false;
            }


            for (int y = paintY; y < paintHeight; y++)
            {
                for (int x = paintX; x < paintWidth; x++)
                {
                    int cell = m.GetTile(x, y);
                    if (blinkUnpoweredZones &&
                        TileConstants.IsZoneCenter(cell) &&
                        !m.IsTilePowered(x, y))
                    {
                        needsBlinking = true;
                        if (blink)
                            cell = TileConstants.LIGHTNINGBOLT;
                        _buffer.Set(x, y, -1);
                    }

                    if (toolPreview != null)
                    {
                        int c = toolPreview.GetTile(x, y);
                        if (c != TileConstants.CLEAR)
                        {
                            cell = c;
                            _buffer.Set(x, y, -1);
                        }
                    }

                    bool needsUpdate = cell != _buffer.Get(x, y);
                    if (needsUpdate)
                    {
                        _buffer.Set(x, y, cell);
                        byte[] img = tileImages.GetTileImage(cell);
                        DrawTile(_imageStream, x, y, (shakeStep != 0 ? GetShakeModifier(y) : 0), img, (int)_imageOutput.Width);
                    }
                }
            }

            foreach (Sprite sprite in m.AllSprites())
            {
                if (sprite.IsVisible())
                {
                    DrawSprite(sprite);
                }
            }

            if (toolCursor != null)
            {
                int x0 = toolCursor.Rect.X * TILE_WIDTH;
                int x1 = (toolCursor.Rect.X + toolCursor.Rect.Width) * TILE_WIDTH;
                int y0 = toolCursor.Rect.Y * TILE_HEIGHT;
                int y1 = (toolCursor.Rect.Y + toolCursor.Rect.Height) * TILE_HEIGHT;

                /*
                DrawLine(_imageStream, (int)x0 - 1, (int)y0 - 1, (int)x0 - 1, (int)y1 - 1, Colors.Black);
                DrawLine(_imageStream, (int)x0 - 1, (int)y0 - 1, (int)x1 - 1, (int)y0 - 1, Colors.Black);
                DrawLine(_imageStream, (int)x1 + 3, (int)y0 - 3, (int)x1 + 3, (int)y1 + 3, Colors.Black);
                DrawLine(_imageStream, (int)x0 - 3, (int)y1 + 3, (int)x1 + 3, (int)y1 + 3, Colors.Black);

                DrawLine(_imageStream, (int)x0 - 4, (int)y0 - 4, (int)x1 + 3, (int)y0 - 4, Colors.White);
                DrawLine(_imageStream, (int)x0 - 4, (int)y0 - 4, (int)x0 - 4, (int)y1 + 3, Colors.White);
                DrawLine(_imageStream, (int)x1, (int)y0 - 1, (int)x1, (int)y1, Colors.White);
                DrawLine(_imageStream, (int)x0 - 1, (int)y1, (int)x1, (int)y1, Colors.White);*/

                DrawRectangle(_imageStream, (int)x0 - 2, (int)y0 - 2, (int)x1 + 2, (int)y1 + 2, toolCursor.BorderColor);
                DrawRectangle(_imageStream, (int)x0 - 1, (int)y0 - 1, (int)x1 + 1, (int)y1 + 1, toolCursor.BorderColor);

                if (toolCursor.FillColor != null && x0 >= 0 && y0 >= 0)
                {
                    FillRectangle(_imageStream, (int)x0, (int)y0, (int)x1, (int)y1, toolCursor.FillColor);
                }

                // now we need to set our buffer contents to empty, so we'll draw the tiles next time again
                int yy = (toolCursor.Rect.Y >= 0 ? toolCursor.Rect.Y : 0);
                int xx = (toolCursor.Rect.X >= 0 ? toolCursor.Rect.X : 0);
                int minyy = yy - 5 > 0 ? yy - 5 : yy;
                int minxx = xx - 5 > 0 ? xx - 5 : xx;
                int maxyya = yy + 5 < maxY ? yy + 5 : yy;
                int maxxxa = xx + 5 < maxX ? xx + 5 : xx;

                int maxyyb = maxyya + toolCursor.Rect.Height < maxY ? maxyya + toolCursor.Rect.Height : maxyya;
                int maxxxb = maxxxa + toolCursor.Rect.Width < maxX ? maxxxa + toolCursor.Rect.Width : maxxxa;

                for (var o = minyy; o < maxyyb; o++)
                {
                    for (int i = minxx; i < maxxxb; i++)
                    {
                        _buffer.Set(i, o, -1);
                    }
                }
            }
            
            Image.Invalidate();

            if (imageNotCreated || imageHasDifferentsize)
            {
                _imageOutput.Source = Image;
            }
        }

        private void FillRectangle(Stream _imageStream, int xStart, int yStart, int xEnd, int yEnd, Color color)
        {
            if (xStart < 0)
            {
                xStart = 0;
            }

            if (xEnd > _imageOutput.Width)
            {
                xEnd = (int)_imageOutput.Width - 1;
            }

            if (yStart < 0)
            {
                yStart = 0;
            }

            if (yEnd > _imageOutput.Height)
            {
                yEnd = (int)_imageOutput.Height - 1;
            }

            var startInStream = xStart * 4 + yStart * (int)_imageOutput.Width * 4;
            _imageStream.Seek(startInStream, SeekOrigin.Begin);

            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = xStart; x < xEnd; x++)
                {
                    byte[] backgroundPixel = new byte[4];
                    _imageStream.Read(backgroundPixel, 0, 4);
                    Color background = Color.FromArgb(backgroundPixel[3], backgroundPixel[2], backgroundPixel[1], backgroundPixel[0]);
                    Color newColor = BlendColors(background, color);
                    byte[] pixelToWrite = new byte[4] { newColor.B, newColor.G, newColor.R, newColor.A };
                    _imageStream.Seek(-4, SeekOrigin.Current);
                    _imageStream.Write(pixelToWrite, 0, pixelToWrite.Length);
                }

                _imageStream.Seek(((int)_imageOutput.Width - (xEnd - xStart)) * 4, SeekOrigin.Current);
            }

        }

        private Color BlendColors(Color background, Color foreground)
        {
            Color r = new Color();
            r.A = (byte)(255);
            r.R = (byte)((foreground.R * foreground.A / 255) + (background.R * background.A * (255 - foreground.A) / (255 * 255)));
            r.G = (byte)((foreground.G * foreground.A / 255) + (background.G * background.A * (255 - foreground.A) / (255 * 255)));
            r.B = (byte)((foreground.B * foreground.A / 255) + (background.B * background.A * (255 - foreground.A) / (255 * 255)));


            return r;
        }

        private void DrawRectangle(Stream _imageStream, int xStart, int yStart, int xEnd, int yEnd, Color color)
        {
            DrawLine(_imageStream, xStart, yStart, xStart, yEnd, color);
            DrawLine(_imageStream, xEnd, yStart, xEnd, yEnd, color);
            DrawLine(_imageStream, xStart, yStart, xEnd, yStart, color);
            DrawLine(_imageStream, xStart, yEnd, xEnd, yEnd, color);
        }

        private void DrawLine(Stream _imageStream, int xStart, int yStart, int xEnd, int yEnd, Color color)
        {
            if (xStart < 0)
            {
                xStart = 0;
            }

            if (xEnd > _imageOutput.Width)
            {
                xEnd = (int)_imageOutput.Width - 1;
            }

            if (yStart < 0)
            {
                yStart = 0;
            }

            if (yEnd > _imageOutput.Height)
            {
                yEnd = (int)_imageOutput.Height - 1;
            }

            var startInStream = xStart * 4 + yStart * (int)_imageOutput.Width * 4;

            byte[] pixelToWrite = new byte[4] { color.B, color.G, color.R, color.A };

            bool horizontalLine = xStart != xEnd;
            if (horizontalLine)
            {
                if (startInStream > _imageStream.Length)
                {
                    return;
                }
                _imageStream.Seek(startInStream, SeekOrigin.Begin);
                for (int x = xStart; x < xEnd; x++)
                {
                    _imageStream.Write(pixelToWrite, 0, pixelToWrite.Length);
                }
            }
            else
            {
                if (startInStream > _imageStream.Length)
                {
                    return;
                }
                _imageStream.Seek(startInStream, SeekOrigin.Begin);
                for (int y = yStart; y < yEnd; y++)
                {
                    _imageStream.Write(pixelToWrite, 0, pixelToWrite.Length);
                    int position = ((int)_imageOutput.Width - 1) * 4;
                    if (position > _imageStream.Length)
                    {
                        return;
                    }
                    _imageStream.Seek(position, SeekOrigin.Current);
                }
            }
        }

        private void DrawTile(Stream output, int x, int y, int shake, byte[] tilePixelEncoded, int width)
        {
            if (x < 0 || y < 0 || x > _mainPage.MapWidth / TILE_WIDTH || y > _mainPage.MapHeight / TILE_HEIGHT)
            {
                return;
            }

            int startPosInMap = y * TILE_HEIGHT * width * 4 + x * TILE_WIDTH * 4;

            for (int index = 0; index < TILE_HEIGHT; ++index)
            {
                int startPosForRowOfImg = startPosInMap + 4 * (index * width);
                
                if (shake != 0 && (y != 0 || x != 0) && (y != 99 || x != 119))
                    startPosForRowOfImg += shake * 4;
                output.Seek((long)startPosForRowOfImg, SeekOrigin.Begin);

                int offset = index * 4 * TILE_WIDTH;
                output.Write(tilePixelEncoded, offset, TILE_WIDTH * 4);
            }
        }

        /// <summary>
        /// Crop image
        /// </summary>
        /// <param name="img"></param>
        /// <param name="imgWidth"></param>
        /// <param name="xStart"></param>
        /// <param name="yStart"></param>
        /// <param name="xEnd"></param>
        /// <param name="yEnd"></param>
        /// <returns>cropped image</returns>
        private byte[] Crop(byte[] img, int imgWidth, int xStart, int yStart, int xEnd, int yEnd)
        {
            int width = xEnd - xStart;
            int height = yEnd - yStart;
            byte[] result = new byte[width * height * 4];

            for (var currentY = yStart; currentY < yEnd; currentY++)
            {
                Array.Copy(img, 
                    (xStart + (imgWidth * currentY)) * 4,
                    result, 
                    width * (currentY - yStart) * 4,
                    width * 4);
            }

            return result;
        }


        /// <summary>
        /// Draws sprite into output
        /// </summary>
        /// <param name="output"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="shake"></param>
        /// <param name="tilePixelEncoded"></param>
        /// <param name="width"></param>
        private void DrawSprite(Stream output, int x, int y, int shake, byte[] tilePixelEncoded, int width)
        {
            int minX = (x < 0 ? x * (-1) : 0);
            int minY = (y < 0 ? y * (-1) : 0);
            int maxY = (y + width > _imageOutput.Height ? width - (y + width - (int)_imageOutput.Height) : width);
            int maxX = (x + width > _imageOutput.Width ? width - (x + width - (int)_imageOutput.Width) : width);
            byte[] tilePixelEncodedToDraw = Crop(tilePixelEncoded,
                                                width,
                                                minX,
                                                minY,
                                                maxX,
                                                maxY);

            int newHeight = maxY - minY;
            int newWidth = maxX - minX;
            
            int startPosInMap = y * 4 * (int)_imageOutput.Width + x * 4;

            for (int index = 0; index < newHeight; ++index)
            {
                int startPosForRowOfImg = startPosInMap + 4 * (index * (int)_imageOutput.Width);

                if (shake != 0 && (y != 0 || x != 0) && (y != 99 || x != 119))
                {
                    startPosForRowOfImg += shake * 4;
                }
                
                int offset = index * 4 * newWidth;
                int length = (x + width > _imageOutput.Width ? width - (x + width - (int)_imageOutput.Width) : (x < 0 ? width + x : width)) * 4;
                TransparentWrite(output, (long)startPosForRowOfImg, tilePixelEncodedToDraw, offset, length);
            }
        }

        /// <summary>
        /// Blips one line of tilePixelEncodedToDraw into output respecting alpha transparency.
        /// </summary>
        /// <param name="output">destination</param>
        /// <param name="mapOffset">offset in destination</param>
        /// <param name="tilePixelEncodedToDraw">source</param>
        /// <param name="offset">offset in source</param>
        /// <param name="length">length of source to Blip</param>
        private void TransparentWrite(Stream output, long mapOffset, byte[] tilePixelEncodedToDraw, int offset, int length)
        {
            for (int i = offset; i < offset + length; i += 4)
            {
                if (tilePixelEncodedToDraw[i + 3] != 0)
                {
                    output.Seek((long)(mapOffset + i - offset), SeekOrigin.Begin);
                        output.Write(tilePixelEncodedToDraw, i, 4);
                }
            }
        }

     
        /// <summary>
        ///     Sets the tool cursor
        /// </summary>
        /// <param name="newRect">new rect</param>
        /// <param name="tool">new tool</param>
        public void SetToolCursor(CityRect newRect, MicropolisTool tool)
        {
            var tp = new ToolCursor
            {
                Rect = newRect,
                BorderColor = ColorParser.ParseColor(
                    Strings.ContainsKey("tool." + tool.Name + ".border")
                        ? Strings.GetString("tool." + tool.Name + ".border")
                        : Strings.GetString("tool.*.border")
                    ),
                FillColor = ColorParser.ParseColor(
                    Strings.ContainsKey("tool." + tool.Name + ".bgcolor")
                        ? Strings.GetString("tool." + tool.Name + ".bgcolor")
                        : Strings.GetString("tool.*.bgcolor")
                    )
            };
            SetToolCursor(tp);
        }

        /// <summary>
        ///     Sets tool cursor
        /// </summary>
        /// <param name="newCursor">new cursor</param>
        public void SetToolCursor(ToolCursor newCursor)
        {
            if (toolCursor == newCursor)
                return;
            if (toolCursor != null && toolCursor.Equals(newCursor))
                return;

            if (toolCursor != null)
            {
                Repaint();
            }
            toolCursor = newCursor;
            if (toolCursor != null)
            {
                Repaint();
            }
        }

        /// <summary>
        ///     Sets Tool preview
        /// </summary>
        /// <param name="newPreview">new tool preview</param>
        public void SetToolPreview(ToolPreview newPreview)
        {
            if (toolPreview != null)
            {
                CityRect b = toolPreview.GetBounds();
            }

            toolPreview = newPreview;
            if (toolPreview != null)
            {
                CityRect b = toolPreview.GetBounds();
            }

            Repaint();
        }

        /// <summary>
        ///     Gets tile bounds as a rectangle
        /// </summary>
        /// <param name="xpos">xpos of rect</param>
        /// <param name="ypos">ypos of rect</param>
        /// <returns></returns>
        public Rect GetTileBoundsAsRect(int xpos, int ypos)
        {
            var rect = new Rect
            {
                Width = TILE_WIDTH,
                Height = TILE_HEIGHT,
                X = xpos*TILE_WIDTH,
                Y = ypos*TILE_HEIGHT
            };

            return rect;
        }

        /// <summary>
        ///     Starts drag at specified position
        /// </summary>
        /// <param name="x">xpos</param>
        /// <param name="y">ypos</param>
        protected void StartDrag(int x, int y)
        {
            dragging = true;
            dragX = x;
            dragY = y;
        }

        /// <summary>
        ///     Ends drag at specified position
        /// </summary>
        /// <param name="x">xpos</param>
        /// <param name="y">ypos</param>
        protected void EndDrag(int x, int y)
        {
            dragging = false;
        }

        /// <summary>
        ///     Updates drag with specified position.
        /// </summary>
        /// <param name="x">xpos</param>
        /// <param name="y">ypos</param>
        protected void ContinueDrag(int x, int y)
        {
            int dx = x - dragX;
            int dy = y - dragY;
            _mainPage.ScrollGameFieldBy(dx, dy);
        }

        /// <summary>
        ///     Does a blink when tiles do not have power.
        /// </summary>
        private void DoBlink()
        {
            blink = !blink;

            if (needsBlinking)
            {
                needsBlinking = false;
                Repaint();
            }
        }

        /// <summary>
        ///     Starts the blink timer so tiles can blink if they do not have power.
        /// </summary>
        private void StartBlinkTimer()
        {
            blinkTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 500)};
            blinkTimer.Tick += (a, b) => DoBlink();
            blinkTimer.Start();
        }

        /// <summary>
        ///     Stops the blink timer.
        /// </summary>
        private void StopBlinkTimer()
        {
            if (blinkTimer != null)
            {
                blinkTimer.Stop();
                blinkTimer = null;
            }
        }

        /// <summary>
        ///     Does an earthquake shake.
        /// </summary>
        /// <param name="i">shake step</param>
        public void Shake(int i)
        {
            shakeStep = i;
            _isCurrentEarthquake = true;
            Repaint();
        }

        /// <summary>
        ///     Gets the shake modifier for specified row
        /// </summary>
        /// <param name="row">row</param>
        /// <returns></returns>
        private int GetShakeModifier(int row)
        {
            return (int) Math.Round(4.0*Math.Sin((shakeStep + row/2)/2.0));
        }

        /// <summary>
        ///     Gets a cropped part of the map to be used in NotificationPanel.
        /// </summary>
        /// <param name="xpos">xpos of rect</param>
        /// <param name="ypos">ypos of rect</param>
        /// <param name="notificationPanelViewportSize">viewport size to fill</param>
        /// <returns></returns>
        internal WriteableBitmap GetLandscape(int xpos, int ypos, Size notificationPanelViewportSize)
        {
            Rect r = GetTileBoundsAsRect(xpos, ypos);

            var xstart = (int) (r.X + r.Width/2 - notificationPanelViewportSize.Width/2);
            var ystart = (int) (r.Y + r.Height/2 - notificationPanelViewportSize.Height/2);

            var width = (int) notificationPanelViewportSize.Width;
            int xend = xstart + width;
            var height = (int) notificationPanelViewportSize.Height;
            int yend = ystart + height;

            if (xstart < 0)
            {
                xend += (0 - xstart);
                xstart = 0;
            }

            if (ystart < 0)
            {
                yend += (0 - ystart);
                ystart = 0;
            }

            if (xend > _imageOutput.Width)
            {
                xstart -= xend - (int)_imageOutput.Width;
                xend = (int)_imageOutput.Width - 1;
            }

            if (yend > _imageOutput.Height)
            {
                ystart -= yend - (int)_imageOutput.Height;
                yend = (int)_imageOutput.Height - 1;
            }

            PaintComponent(xstart, ystart, xend, yend);

            WriteableBitmap clonedImg = Image.Crop(xstart, ystart, width, height);

            return clonedImg;
        }

        internal void StopRendering()
        {
            CompositionTarget.Rendering -= Render;
            StopBlinkTimer();
        }

        internal void StartRendering()
        {
            CompositionTarget.Rendering += Render;
            StartBlinkTimer(); //Bug: check. might lead to double execution, if so remove here and in start method 
        }
    }
}