namespace Micropolis
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Devices.Input;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Input;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Engine;
    using Micropolis.Lib.graphics;

    /// <summary>
    /// OverlayMapView displays a map with viewport rectangle and zone information.
    /// </summary>
    public sealed partial class OverlayMapView : IMapListener
    {
        private readonly int FRAMES_PER_SECOND = 3;
        private static WriteableBitmap _tileArrayImage;
        private static readonly int TILE_WIDTH = 3;
        private static readonly int TILE_HEIGHT = 3;
        private static readonly int TILE_OFFSET_Y = 3;
        private static readonly Color VAL_LOW = Color.FromArgb(255, 0xbf, 0xbf, 0xbf);
        private static readonly Color VAL_MEDIUM = Color.FromArgb(255, 0xff, 0xff, 0x00);
        private static readonly Color VAL_HIGH = Color.FromArgb(255, 0xff, 0x7f, 0x00);
        private static readonly Color VAL_VERYHIGH = Color.FromArgb(255, 0xff, 0x00, 0x00);
        private static readonly Color VAL_PLUS = Color.FromArgb(255, 0x00, 0x7f, 0x00);
        private static readonly Color VAL_VERYPLUS = Color.FromArgb(255, 0x00, 0xe6, 0x00);
        private static readonly Color VAL_MINUS = Color.FromArgb(255, 0xff, 0x7f, 0x00);
        private static readonly Color VAL_VERYMINUS = Color.FromArgb(255, 0xff, 0xff, 0x00);
        private static readonly int UNPOWERED = 0x6666e6; //lightblue
        private static readonly int POWERED = 0xff0000; //red
        private static readonly int CONDUCTIVE = 0xbfbfbf; //lightgray
        private readonly List<ConnectedView> _views = new List<ConnectedView>();
        private WriteableBitmap _gr;
        private WriteableBitmap _img;
        private bool _isMouseDown;

        private long _lastRepaintTicks;
        private MapState _mapState = MapState.ALL;
        private bool _repaintNow;
        private int[][] _tileBuffer;

        /// <summary>
        /// Initiates a new instance of this OverlayMapView control.
        /// </summary>
        public OverlayMapView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Reference to the engine.
        /// </summary>
        public Engine.Micropolis Engine { get; set; }

        /// <summary>
        /// Fired when map overlay data changed.
        /// </summary>
        /// <param name="overlayDataType">data changed</param>
        public void MapOverlayDataChanged(MapState overlayDataType)
        {
            Repaint();
        }

        /// <summary>
        /// Fired when sprite moved
        /// </summary>
        /// <param name="sprite">sprite moved</param>
        public void SpriteMoved(Sprite sprite)
        {
        }

        /// <summary>
        /// Fired when Tile changed to repaint map
        /// </summary>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        public void TileChanged(int xpos, int ypos)
        {
            //Rect r = new Rect(xpos*TILE_WIDTH, ypos * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT);
            //repaint(r); //Bug: ToDo
            Repaint();
        }

        /// <summary>
        /// Fired when whole map changed to repaint.
        /// </summary>
        public void WholeMapChanged()
        {
            Repaint();
            Engine.CalculateCenterMass();
            DragViewToCityCenter();
        }

        /// <summary>
        /// Fired when mouse, touch or pen moved.
        /// </summary>
        /// <param name="e">pointer event data</param>
        private void OnMouseMoved(PointerRoutedEventArgs e)
        {
            if (_isMouseDown)
            {
                OnMouseDragged(e);
            }
        }

        /// <summary>
        /// Sets up this instance after basic initialization.
        /// </summary>
        /// <param name="engine">Engine to be used</param>
        public void SetUpAfterBasicInit(Engine.Micropolis engine)
        {
            PointerPressed += (sender, e) => OnMousePressed(e);
            PointerMoved += (sender, e) => OnMouseMoved(e);
            PointerReleased += (sender, e) => OnMouseReleased(e);

            SetEngine(engine);

            CompositionTarget.Rendering += Render;

        }

        /// <summary>
        /// Renders this map in loop every now and then.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Render(object sender, object e)
        {
            var x = new TimeSpan(0, 0, 0, 0, (int) (1.0/FRAMES_PER_SECOND*1000));
            long diff = DateTime.Now.Ticks - _lastRepaintTicks;
            if (diff > x.Ticks)
            {
                if (_repaintNow)
                {
                    _repaintNow = false;
                    PaintComponent();
                    UpdateLayout();
                }
                _lastRepaintTicks = DateTime.Now.Ticks;
            }
        }

        /// <summary>
        /// Fired when pointer released.
        /// </summary>
        /// <param name="e"></param>
        private void OnMouseReleased(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == PointerDeviceType.Pen || e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                _isMouseDown = false;
            }
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        public void Destroy()
        {
            if (Engine != null)
            {
                Engine.RemoveMapListener(this);
            }
        }

        /// <summary>
        /// Sets new engine to be used.
        /// </summary>
        /// <param name="newEngine"></param>
        public void SetEngine(Engine.Micropolis newEngine)
        {
            if (Engine != null)
            {
                //old engine //was newEngine
                Engine.RemoveMapListener(this);
            }

            Engine = newEngine;
            if (Engine != null)
            {
                //new engine
                Engine.AddMapListener(this);
            }

            Repaint();
            // paintComponent();
            if (Engine != null)
            {
                Engine.CalculateCenterMass();
            }
            DragViewToCityCenter();
        }

        /// <summary>
        /// Gets map state
        /// </summary>
        /// <returns></returns>
        public MapState GetMapState()
        {
            return _mapState;
        }

        /// <summary>
        /// Gets preferred size
        /// </summary>
        /// <returns></returns>
        public Size GetPreferredSize() //override
        {
            /*return new Point(
			getInsets().left + getInsets().right + TILE_WIDTH*engine.getWidth(),
			getInsets().top + getInsets().bottom + TILE_HEIGHT*engine.getHeight()
			);*/
            return new Size(TILE_WIDTH*Engine.GetWidth(), TILE_HEIGHT*Engine.GetHeight());
        }

        /// <summary>
        /// Sets map state and repaints the map on next tick
        /// </summary>
        /// <param name="newState"></param>
        public void SetMapState(MapState newState)
        {
            if (_mapState == newState)
                return;

            _mapState = newState;
            Repaint();
        }

        /// <summary>
        /// Initializes graphics to be used
        /// </summary>
        /// <returns></returns>
        public static async Task Initialize(CancellationToken cancelToken)
        {
            Utils.ThreadCancellation.CheckCancellation(cancelToken);
            _tileArrayImage = await LoadImage(@"ms-appx:///resources/images/Game/tiles/tilessm.png");
        }


        /// <summary>
        /// Loads an image from resources
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private static async Task<WriteableBitmap> LoadImage(String resourceName)
        {
            var iconUrl = new Uri(resourceName, UriKind.RelativeOrAbsolute);
            var refImage = new WriteableBitmap(1, 1);
            refImage = await refImage.FromContent(iconUrl);

            return refImage;
        }

        /// <summary>
        /// Gets color for Commercial zones.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private Color? getCI(int x)
        {
            if (x < 50)
                return null;
            if (x < 100)
                return VAL_LOW;
            if (x < 150)
                return VAL_MEDIUM;
            if (x < 200)
                return VAL_HIGH;
            return VAL_VERYHIGH;
        }

        /// <summary>
        /// Gets colors for commercial  zones
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private Color? GetCI_rog(int x)
        {
            if (x > 100)
                return VAL_VERYPLUS;
            if (x > 20)
                return VAL_PLUS;
            if (x < -100)
                return VAL_VERYMINUS;
            if (x < -20)
                return VAL_MINUS;
            return null;
        }

        /// <summary>
        /// Draws pollution map into image
        /// </summary>
        /// <param name="gr">image to be drawn into</param>
        private void DrawPollutionMap(WriteableBitmap gr)
        {
            int[][] a = Engine.PollutionMem;

            for (int y = 0; y < a.Length; y++)
            {
                for (int x = 0; x < a[y].Length; x++)
                {
                    Color? color = getCI(10 + a[y][x]);
                    if (color.HasValue)
                    {
                        maybeDrawRect(gr, color.Value, x*6, y*6, 6, 6);
                    }
                }
            }
        }

        /// <summary>
        /// Draws crime map into image
        /// </summary>
        /// <param name="gr">Image to be drawn into</param>
        private void DrawCrimeMap(WriteableBitmap gr)
        {
            int[][] a = Engine.CrimeMem;

            for (int y = 0; y < a.Length; y++)
            {
                for (int x = 0; x < a[y].Length; x++)
                {
                    Color? color = getCI(a[y][x]);
                    if (color.HasValue)
                    {
                        maybeDrawRect(gr, color.Value, x*6, y*6, 6, 6);
                    }
                }
            }
        }

        /// <summary>
        /// Draws pop density map into image
        /// </summary>
        /// <param name="gr">Image to be drawn into</param>
        private void DrawPopDensity(WriteableBitmap gr)
        {
            int[][] a = Engine.PopDensity;

            for (int y = 0; y < a.Length; y++)
            {
                for (int x = 0; x < a[y].Length; x++)
                {
                    Color? color = getCI(a[y][x]);
                    if (color.HasValue)
                    {
                        maybeDrawRect(gr, color.Value, x*6, y*6, 6, 6);
                    }
                }
            }
        }

        /// <summary>
        /// Draws rate of growth into image
        /// </summary>
        /// <param name="gr">Image to be drawn into</param>
        private void DrawRateOfGrowth(WriteableBitmap gr)
        {
            int[][] a = Engine.RateOgMem;

            for (int y = 0; y < a.Length; y++)
            {
                for (int x = 0; x < a[y].Length; x++)
                {
                    Color? color = getCI(a[y][x]);
                    if (color.HasValue)
                    {
                        maybeDrawRect(gr, color.Value, x*24, y*24, 24, 24);
                    }
                }
            }
        }

        /// <summary>
        /// Draws fire into image
        /// </summary>
        /// <param name="gr">Image to be drawn into</param>
        private void DrawFireRadius(WriteableBitmap gr)
        {
            int[][] a = Engine.FireRate;

            for (int y = 0; y < a.Length; y++)
            {
                for (int x = 0; x < a[y].Length; x++)
                {
                    Color? color = getCI(a[y][x]);
                    if (color.HasValue)
                    {
                        maybeDrawRect(gr, color.Value, x*24, y*24, 24, 24);
                    }
                }
            }
        }

        /// <summary>
        /// Draws police radius into image
        /// </summary>
        /// <param name="gr">Image to be drawn into</param>
        private void DrawPoliceRadius(WriteableBitmap gr)
        {
            int[][] a = Engine.PoliceMapEffect;

            for (int y = 0; y < a.Length; y++)
            {
                for (int x = 0; x < a[y].Length; x++)
                {
                    Color? color = getCI(a[y][x]);
                    if (color.HasValue)
                    {
                        maybeDrawRect(gr, color.Value, x*24, y*24, 24, 24);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a filled rectangle into image
        /// </summary>
        /// <param name="gr">Image to be drawn into</param>
        /// <param name="col">Color</param>
        /// <param name="x">xpos</param>
        /// <param name="y">ypos</param>
        /// <param name="width">width of rectangle</param>
        /// <param name="height">height of rectangle</param>
        private void maybeDrawRect(WriteableBitmap gr, Color col, int x, int y, int width, int height)
        {
                //gr.DrawRectangle(x, y, x+width, y+height, col);
                gr.FillRectangle(x, y, x + width, y + height, col);
        }

        /// <summary>
        /// Checks power and draws pixel into image
        /// </summary>
        /// <param name="img">Image to be drawn into</param>
        /// <param name="x">xpos</param>
        /// <param name="y">ypos</param>
        /// <param name="rawTile">raw tile</param>
        /// <returns></returns>
        private int checkPower(WriteableBitmap img, int x, int y, int rawTile)
        {
            int pix;

            if ((rawTile & TileConstants.LOMASK) <= 63)
            {
                return rawTile & TileConstants.LOMASK;
            }
            if (TileConstants.IsZoneCenter(rawTile))
            {
                // zone
                pix = ((rawTile & TileConstants.PWRBIT) != 0) ? POWERED : UNPOWERED;
            }
            else if (TileConstants.IsConductive(rawTile))
            {
                pix = CONDUCTIVE;
            }
            else
            {
                return TileConstants.DIRT;
            }

            for (int yy = 0; yy < TILE_HEIGHT; yy++)
            {
                for (int xx = 0; xx < TILE_WIDTH; xx++)
                {
                    img.SetPixel(x*TILE_WIDTH + xx, y*TILE_HEIGHT + yy, pix);
                }
            }
            return -1; //this special value tells caller to skip the tile bitblt,
            //since it was performed here
        }

        /// <summary>
        /// Draws land value into image
        /// </summary>
        /// <param name="img">Image to be drawn into</param>
        /// <param name="xpos">xpos</param>
        /// <param name="ypos">ypos</param>
        /// <param name="tile">tile</param>
        /// <returns></returns>
        private int CheckLandValueOverlay(WriteableBitmap img, int xpos, int ypos, int tile)
        {
            int v = Engine.GetLandValue(xpos, ypos);
            Color? c = getCI(v);
            if (c == null)
            {
                return tile;
            }

            for (int yy = 0; yy < TILE_HEIGHT; yy++)
            {
                for (int xx = 0; xx < TILE_WIDTH; xx++)
                {
                    img.SetPixel(xpos*TILE_WIDTH + xx, ypos*TILE_HEIGHT + yy, c.Value);
                }
            }
            return TileConstants.CLEAR;
        }

        /// <summary>
        /// Draws traffix into image
        /// </summary>
        /// <param name="img">Image to be drawn into</param>
        /// <param name="xpos">xpos</param>
        /// <param name="ypos">ypos</param>
        /// <param name="tile">tile</param>
        /// <returns></returns>
        private int CheckTrafficOverlay(WriteableBitmap img, int xpos, int ypos, int tile)
        {
            int d = Engine.GetTrafficDensity(xpos, ypos);
            Color? c = getCI(d);
            if (c == null | !c.HasValue)
            {
                return tile;
            }

            for (int yy = 0; yy < TILE_HEIGHT; yy++)
            {
                for (int xx = 0; xx < TILE_WIDTH; xx++)
                {
                    img.FillRectangle(xpos*TILE_WIDTH + xx, ypos*TILE_HEIGHT + yy, xpos*TILE_WIDTH + xx,
                        ypos*TILE_HEIGHT + yy, c.Value);
                }
            }
            return TileConstants.CLEAR;
        }

        /// <summary>
        /// Queries map to be redrawn next tick.
        /// </summary>
        private void Repaint()
        {
            _repaintNow = true;
        }

        /// <summary>
        /// Paints the map component.
        /// </summary>
        public void PaintComponent() //override
        {
            int width = Engine.GetWidth();
            int height = Engine.GetHeight();

            
            int minX = 0;
            int minY = 0;
            int maxX = width;
            int maxY = height;


            if (_tileBuffer == null)
            {
                _tileBuffer = new int[maxY][];
                for (int i = 0; i < maxY; i++)
                {
                    _tileBuffer[i] = new int[maxX];
                    for (int o = 0; o < maxX; o++)
                    {
                        _tileBuffer[i][o] = -1;
                    }
                }
            }

            CheckImageNotCreated(width, height);

            using (_tileArrayImage.GetBitmapContext())
            {
                using (_img.GetBitmapContext())
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        for (int x = minX; x < maxX; x++)
                        {
                            int tile = Engine.GetTile(x, y);
                            switch (_mapState)
                            {
                                case MapState.RESIDENTIAL:
                                    if (TileConstants.IsZoneAny(tile) &&
                                        !TileConstants.IsResidentialZoneAny(tile))
                                    {
                                        tile = TileConstants.DIRT;
                                    }
                                    break;
                                case MapState.COMMERCIAL:
                                    if (TileConstants.IsZoneAny(tile) &&
                                        !TileConstants.IsCommercialZone(tile))
                                    {
                                        tile = TileConstants.DIRT;
                                    }
                                    break;
                                case MapState.INDUSTRIAL:
                                    if (TileConstants.IsZoneAny(tile) &&
                                        !TileConstants.IsIndustrialZone(tile))
                                    {
                                        tile = TileConstants.DIRT;
                                    }
                                    break;
                                case MapState.POWER_OVERLAY:
                                    tile = checkPower(_img, x, y, Engine.GetTile(x, y));
                                    break;
                                case MapState.TRANSPORT:
                                case MapState.TRAFFIC_OVERLAY:
                                    if (TileConstants.IsConstructed(tile)
                                        && !TileConstants.IsRoadAny(tile)
                                        && !TileConstants.IsRailAny(tile))
                                    {
                                        tile = TileConstants.DIRT;
                                    }
                                    if (_mapState == MapState.TRAFFIC_OVERLAY)
                                    {
                                        tile = CheckTrafficOverlay(_img, x, y, tile);
                                    }
                                    break;

                                case MapState.LANDVALUE_OVERLAY:
                                    tile = CheckLandValueOverlay(_img, x, y, tile);
                                    break;
                            }

                            // tile == -1 means it's already been drawn
                            // in the checkPower function
                            bool needsUpdate = tile != _tileBuffer[y][x];
                            if (needsUpdate)
                            {
                                _tileBuffer[y][x] = tile;

                                if (tile != -1)
                                {
                                    paintTile(_img, x, y, tile);
                                }
                            }
                        }
                    }
                }
            }

            _gr.DrawInto(_img, 0, 0);
            using (_gr.GetBitmapContext())
            {
                switch (_mapState)
                {
                    case MapState.POLICE_OVERLAY:
                        DrawPoliceRadius(_gr);
                        break;
                    case MapState.FIRE_OVERLAY:
                        DrawFireRadius(_gr);
                        break;
                    case MapState.CRIME_OVERLAY:
                        DrawCrimeMap(_gr);
                        break;
                    case MapState.POLLUTE_OVERLAY:
                        DrawPollutionMap(_gr);
                        break;
                    case MapState.GROWTHRATE_OVERLAY:
                        DrawRateOfGrowth(_gr);
                        break;
                    case MapState.POPDEN_OVERLAY:
                        DrawPopDensity(_gr);
                        break;
                }

                //paintViewPort();
            }
        }

        /// <summary>
        /// Fired when drawingAreaScroll changed to update viewport visualization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void drawingAreaScroll_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            PaintViewPort();
        }

        /// <summary>
        /// Paints viewport
        /// </summary>
        private void PaintViewPort()
        {
            /*foreach (ConnectedView cv in views)
        {*/
            if (_views.Any())
            {
                Rect rect = getViewRect(_views[0]);

                var translateTransform = new TranslateTransform {X = (int) rect.X, Y = (int) rect.Y};

                ViewPortRectangle.RenderTransform = translateTransform;
                ViewPortRectangle.Height = ((int) (rect.Height));
                ViewPortRectangle.Width = ((int) (rect.Width));
                //}
            }
        }

        /// <summary>
        /// Checks whether image has been created. If not it gets created.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void CheckImageNotCreated(int width, int height)
        {
            bool imageNotCreated = _gr == null;
            bool imageHasDifferentsize = !imageNotCreated &&
                                         (_gr.PixelWidth != width*TILE_WIDTH || _gr.PixelHeight != height*TILE_HEIGHT);

            if (imageNotCreated || imageHasDifferentsize)
            {
                _gr = new WriteableBitmap(width*TILE_WIDTH, height*TILE_HEIGHT);
                _img = new WriteableBitmap(width*TILE_WIDTH, height*TILE_HEIGHT);

                ImageOutput.Height = height*TILE_HEIGHT;
                ImageOutput.Width = width*TILE_WIDTH;

                if (imageNotCreated)
                {
                    ImageOutput.Source = _gr;
                }
            }
        }

        /// <summary>
        /// Debug output of map to debug console
        /// </summary>
        /// <param name="gr">Image to be analyzed</param>
        public static void ToDebug(WriteableBitmap gr)
        {
            using (gr.GetBitmapContext())
            {
                for (int i = 0; i < gr.PixelHeight; i++)
                {
                    string line = "";
                    Color lastColor = gr.GetPixel(0, i);
                    for (int o = 0; o < gr.PixelWidth; o++)
                    {
                        Color color = gr.GetPixel(o, i);
                        if (color != lastColor)
                        {
                            line += ".";
                        }
                        else
                        {
                            line += " ";
                        }
                        lastColor = color;
                    }
                    Debug.WriteLine(line);
                }
            }
            Debug.WriteLine("-----------------------");
            Debug.WriteLine("-----------------------");
            Debug.WriteLine("-----------------------");
            Debug.WriteLine("-----------------------");
        }

        /// <summary>
        /// Paints a tile of the map to the image
        /// </summary>
        /// <param name="img">Image to be drawn into</param>
        /// <param name="x">xpos</param>
        /// <param name="y">ypos</param>
        /// <param name="tile">tile to be drawn</param>
        private void paintTile(WriteableBitmap img, int x, int y, int tile)
        {
            img.DrawInto(_tileArrayImage, x*TILE_WIDTH, y*TILE_HEIGHT, 0, tile*TILE_OFFSET_Y, TILE_WIDTH,
                tile*TILE_OFFSET_Y + TILE_HEIGHT);
        }

        /// <summary>
        /// Gets a view rectangle
        /// </summary>
        /// <param name="cv">connected view to get rectangle for</param>
        /// <returns>View rectangle</returns>
        private Rect getViewRect(ConnectedView cv)
        {
            double zoomFactor = cv.ScrollPane.ZoomFactor;
            var rawRect = new Rect(cv.ScrollPane.HorizontalOffset/zoomFactor, cv.ScrollPane.VerticalOffset/zoomFactor,
                cv.ScrollPane.ViewportWidth/zoomFactor, cv.ScrollPane.ViewportHeight/zoomFactor);
            return new Rect(
                rawRect.X*3/cv.View.GetTileSize(),
                rawRect.Y*3/cv.View.GetTileSize(),
                rawRect.Width*3/cv.View.GetTileSize(),
                rawRect.Height*3/cv.View.GetTileSize()
                );
        }

        /// <summary>
        /// Drags view to specified coordinates
        /// </summary>
        /// <param name="p">coordinates</param>
        private void DragViewTo(Point p)
        {
            if (_views.Count == 0)
                return;

            ConnectedView cv = _views[0];

            var d = new Size(cv.ScrollPane.ViewportWidth, cv.ScrollPane.ViewportHeight);
                //Size d = cv.scrollPane.getViewport().getExtentSize();
            var mapSize = new Size(cv.ScrollPane.ScrollableWidth + cv.ScrollPane.ViewportWidth,
                cv.ScrollPane.ScrollableHeight + cv.ScrollPane.ViewportHeight);
                //Size mapSize = cv.scrollPane.getViewport().getViewSize();

            var np = new Point(
                p.X*cv.View.GetTileSize()/3*cv.ScrollPane.ZoomFactor - d.Width/2,
                p.Y*cv.View.GetTileSize()/3*cv.ScrollPane.ZoomFactor - d.Height/2
                );
            np.X = Math.Max(0, Math.Min(np.X, mapSize.Width - d.Width));
            np.Y = Math.Max(0, Math.Min(np.Y, mapSize.Height - d.Height));

            cv.ScrollPane.ChangeView(np.X, np.Y, cv.ScrollPane.ZoomFactor);

            PaintViewPort();
        }

        /// <summary>
        /// Gets preferred size
        /// </summary>
        /// <returns></returns>
        public Size GetPreferredScrollableViewportSize()
        {
            return new Size(120, 120);
        }

        /// <summary>
        /// Drags View to City Center
        /// </summary>
        public void DragViewToCityCenter()
        {
            DragViewTo(new Point(TILE_WIDTH*Engine.CenterMassX + 1,
                TILE_HEIGHT*Engine.CenterMassY + 1));
        }

        /// <summary>
        /// Connects to view
        /// </summary>
        /// <param name="view">view to connect to</param>
        /// <param name="scrollPane">connected scrollpane</param>
        public void ConnectView(MicropolisDrawingArea view, ScrollViewer scrollPane)
        {
            var cv = new ConnectedView(view, scrollPane);
            _views.Add(cv);
            view.Repaint();
        }

        /// <summary>
        /// Fired when pointer pressed
        /// </summary>
        /// <param name="e"></param>
        private void OnMousePressed(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == PointerDeviceType.Pen || e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                _isMouseDown = true;
                PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
                if (properties.IsLeftButtonPressed)
                {
                    DragViewTo(e.GetCurrentPoint(this).Position);
                }
            }
        }

        /// <summary>
        /// Fired when pointer dragged
        /// </summary>
        /// <param name="e"></param>
        private void OnMouseDragged(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == PointerDeviceType.Pen || e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
                if (e.Pointer.PointerDeviceType==PointerDeviceType.Mouse && !properties.IsLeftButtonPressed)
                {
                    return;
                }

                DragViewTo(e.GetCurrentPoint(this).Position);
            }
            //if ((ev.getModifiersEx() & MouseEvent.BUTTON1_DOWN_MASK) == 0)
            //	return; //Bug: ToDo
        }
    }
}