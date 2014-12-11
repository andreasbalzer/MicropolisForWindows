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

namespace Micropolis.ViewModels
{
    public class MicropolisDrawingAreaViewModel : IMapListener
    {
        private static readonly int DEFAULT_TILE_SIZE = 16;

        /// <summary>
        ///     Earthquake shake steps
        /// </summary>
        public static int SHAKE_STEPS = 40;

        private readonly int FRAMES_PER_SECOND = 15;
        private readonly GraphicsBuffer _buffer;
        private readonly CoreDispatcher _dispatcher;
        private readonly Image _imageCursor;
        private readonly Image _imageOutput;
        private readonly Grid _layoutRoot;
        private readonly Grid _sPToRender;
        private readonly TextBlock _textBlockToRender;

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
        private WriteableBitmap cursor;
        private int dragX, dragY;
        private bool dragging;
        private long lastRepaintTicks;
        private Engine.Micropolis m;
        private bool needsBlinking;
        private bool repaintCursorNow;
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
            _imageCursor = imageCursor;
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
                if (repaintNow)
                {
                    repaintNow = false;
                    PaintComponentInView();
                    //paintCursor(); // does only need to be drawn when cursor has not been visible or when type of tool changed
                }
                if (repaintCursorNow)
                {
                    repaintCursorNow = false;
                    PaintCursor();
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
            PaintComponent(false, paintX, paintY, paintXMax, paintYMax);
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

        /// <summary>
        ///     Positions tool cursor at specified coordinate.
        /// </summary>
        /// <param name="mouseX">xpos</param>
        /// <param name="mouseY">ypos</param>
        public void PositionToolCursor(double mouseX, double mouseY)
        {
            double newX = 0;
            double newY = 0;
            TranslateTransform exTr = null;

            if (!(_imageCursor.RenderTransform is MatrixTransform))
            {
                exTr = (TranslateTransform) _imageCursor.RenderTransform;
                newX = exTr.X;
                newY = exTr.Y;
            }

            if (toolCursor != null)
            {
                _imageCursor.Visibility = Visibility.Visible;

                if (toolCursor.Rect.Height == 3)
                {
                    // big cursor like residential zone
                    newX = Math.Ceiling(mouseX/TILE_WIDTH - 2)*TILE_WIDTH;
                    newY = Math.Ceiling(mouseY/TILE_HEIGHT - 2)*TILE_HEIGHT;
                }
                else if (toolCursor.Rect.Height == 1)
                {
                    // small cursor like park
                    newX = Math.Ceiling(mouseX/TILE_WIDTH - 1)*TILE_WIDTH;
                    newY = Math.Ceiling(mouseY/TILE_HEIGHT - 1)*TILE_HEIGHT;
                }
                else if (toolCursor.Rect.Height == 4)
                {
                    // huge cursor like power plant
                    newX = Math.Ceiling(mouseX/TILE_WIDTH - 2)*TILE_WIDTH;
                    newY = Math.Ceiling(mouseY/TILE_HEIGHT - 2)*TILE_HEIGHT;
                }
                else if (toolCursor.Rect.Height == 6)
                {
                    // huger than huge cursor like airport
                    newX = Math.Ceiling(mouseX/TILE_WIDTH - 2)*TILE_WIDTH;
                    newY = Math.Ceiling(mouseY/TILE_HEIGHT - 2)*TILE_HEIGHT;
                }
                else if (toolCursor.Rect.Height == 0)
                {
                    _imageCursor.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _imageCursor.Visibility = Visibility.Collapsed;
            }

            bool positionNeedsToBeUpdated = exTr == null || (exTr.X != newX || exTr.Y != newY);
            if (positionNeedsToBeUpdated)
            {
                var transTrans = new TranslateTransform {X = newX, Y = newY};
                _imageCursor.RenderTransform = transTrans;
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
        internal void RepaintNow(bool paintTool = false)
        {
            PaintComponent(paintTool);
        }

        /// <summary>
        ///     Draws a sprite to the graphics.
        /// </summary>
        /// <param name="gr">The graphics to draw into.</param>
        /// <param name="sprite">The sprite to draw.</param>
        /// <returns>An awaitable task.</returns>
        private async Task DrawSprite(WriteableBitmap gr, Sprite sprite)
        {
            //assert sprite.isVisible();

            var p = new Point(
                (sprite.X + sprite.Offx)*TILE_WIDTH/16,
                (sprite.Y + sprite.Offy)*TILE_HEIGHT/16
                );

            WriteableBitmap img = tileImages.GetSpriteImage(sprite.Kind, sprite.Frame - 1);
            if (img != null)
            {
                gr.DrawInto(img, (int) p.X, (int) p.Y, WriteableBitmapExtensions.BlendMode.Alpha);
                _buffer.SetBuffer(p.X, p.Y, img.PixelWidth, img.PixelHeight, TILE_WIDTH, TILE_HEIGHT);
            }
            else
            {
                gr.FillRectangle((int) p.X, (int) p.Y, 16, 16, Colors.Red);

                await
                    gr.DrawString((sprite.Frame - 1).ToString(), (int) p.X, (int) p.Y, Colors.White, _textBlockToRender,
                        _sPToRender);
                _buffer.SetBuffer(p.X, p.Y, 100, 22, TILE_WIDTH, TILE_HEIGHT);
            }
        }

        /// <summary>
        ///     Clears the map so it gets redrawn.
        /// </summary>
        public void ClearMap()
        {
            _buffer.Reset();
            repaintNow = true;
            repaintCursorNow = true;
        }


        /// <summary>
        ///     Paints the map component (does the actual drawing)
        /// </summary>
        /// <param name="paintTool">whether to draw the tool</param>
        /// <param name="paintX">smallest X in map to draw to</param>
        /// <param name="paintY">smallest Y in map to draw to</param>
        /// <param name="paintWidth">width of the draw rectangle to draw to</param>
        /// <param name="paintHeight">height of the draw rectangle to draw to</param>
        private void PaintComponent(bool paintTool = false, int paintX = 0, int paintY = 0,
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
                cursor = new WriteableBitmap(10*TILE_WIDTH + 4, 10*TILE_HEIGHT + 4);
                Image = new WriteableBitmap(width*TILE_WIDTH, height*TILE_HEIGHT);
                _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _imageOutput.Height = height*TILE_HEIGHT;
                    _imageOutput.Width = width*TILE_WIDTH;

                    _imageCursor.Height = height*TILE_HEIGHT;
                    _imageCursor.Width = width*TILE_WIDTH;
                    _imageCursor.Stretch = Stretch.None;
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

            using (Image.GetBitmapContext())
            {
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

                        if (toolPreview != null && paintTool)
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

                            WriteableBitmap img = tileImages.GetTileImage(cell);
                            using (img.GetBitmapContext(ReadWriteMode.ReadOnly))
                            {
                                Image.DrawInto(img,
                                    x*TILE_WIDTH + (shakeStep != 0 ? GetShakeModifier(y) : 0),
                                    y*TILE_HEIGHT);
                            }
                        }
                    }
                }

                foreach (Sprite sprite in m.AllSprites())
                {
                    if (sprite.IsVisible())
                    {
                        DrawSprite(Image, sprite);
                    }
                }
            }

            Image.Invalidate();

            if (imageNotCreated || imageHasDifferentsize)
            {
                _imageOutput.Source = Image;
                _imageCursor.Source = cursor;
            }
        }

        /// <summary>
        ///     Queries cursor to be repainted next tick.
        /// </summary>
        public void RepaintCursor()
        {
            repaintCursorNow = true;
        }

        /// <summary>
        ///     Paints the cursor.
        /// </summary>
        private void PaintCursor()
        {
            if (toolPreview != null)
            {
                if (cursor.PixelWidth != (toolPreview.GetBounds().Width + 10)*TILE_WIDTH ||
                    cursor.PixelHeight != (toolPreview.GetBounds().Height + 10)*TILE_HEIGHT)
                {
                    cursor = cursor.Resize((toolPreview.GetBounds().Width + 10)*TILE_WIDTH,
                        (toolPreview.GetBounds().Height + 10)*TILE_HEIGHT,
                        WriteableBitmapExtensions.Interpolation.Bilinear);
                    _imageCursor.Source = cursor;
                }
            }

            using (cursor.GetBitmapContext())
            {
                if (toolCursor != null)
                {
                    double tileHeightCalc = TILE_HEIGHT;
                    double tileWidthCalc = TILE_WIDTH;

                    int width = m.GetWidth();
                    int height = m.GetHeight();

                    int x0 = 0;
                    double x1 = (0 + toolCursor.Rect.Width)*tileWidthCalc;
                    int y0 = 0;
                    double y1 = (0 + toolCursor.Rect.Height)*tileHeightCalc;

                    cursor.Clear();

                    if (!dragging)
                    {
                        cursor.DrawRectangle(x0, y0, (int) x1 + 4, (int) y1 + 4, toolCursor.BorderColor);
                        cursor.DrawRectangle(x0 + 1, y0 + 1, (int) x1 + 3, (int) y1 + 3, toolCursor.BorderColor);

                        if (toolCursor.FillColor != null && x0 >= 0 && y0 >= 0)
                        {
                            cursor.FillRectangle(x0 + 2, y0 + 2, (int) x1 + 2, (int) y1 + 2, toolCursor.FillColor);
                        }
                    }

                    if (toolPreview != null)
                    {
                        int cell = -1;

                        for (int row = 0; row < toolPreview.GetHeight(); row++)
                        {
                            for (int col = 0; col < toolPreview.GetWidth(); col++)
                            {
                                int x = col + toolPreview.GetBounds().X;
                                int y = row + toolPreview.GetBounds().Y;

                                cell = _buffer.Get(x, y);
                                int c = toolPreview.GetTile(x, y);
                                if (c != TileConstants.CLEAR)
                                {
                                    cell = c;
                                    _buffer.Set(x, y, -1);
                                }
                                bool needsUpdate = cell != _buffer.Get(x, y);
                                if (needsUpdate)
                                {
                                    _buffer.Set(x, y, cell);

                                    WriteableBitmap img = tileImages.GetTileImage(cell);
                                    using (img.GetBitmapContext(ReadWriteMode.ReadOnly))
                                    {
                                        int shiftedX = 0;
                                        int shiftedY = 0;
                                        if (_imageCursor.RenderTransform is TranslateTransform)
                                        {
                                            shiftedX = x
                                                       - (int)
                                                           Math.Ceiling(
                                                               (_imageCursor.RenderTransform as TranslateTransform).X
                                                               /TILE_WIDTH);
                                            shiftedY = y
                                                       - (int)
                                                           Math.Ceiling(
                                                               (_imageCursor.RenderTransform as TranslateTransform).Y
                                                               /TILE_HEIGHT);
                                        }
                                        cursor.DrawInto(img,
                                            shiftedX*TILE_WIDTH + (shakeStep != 0 ? GetShakeModifier(y) : 0),
                                            shiftedY*TILE_HEIGHT);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            cursor.Invalidate();
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
                RepaintCursor();
            }
            toolCursor = newCursor;
            if (toolCursor != null)
            {
                RepaintCursor();
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
            RepaintCursor();
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

            PaintComponent(false, xstart, ystart, xend, yend);

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