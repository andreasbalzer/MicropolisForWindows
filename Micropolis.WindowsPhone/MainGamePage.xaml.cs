// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Engine;
using Micropolis.Model.Entities;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;

namespace Micropolis
{
    /// <summary>
    ///     Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden
    ///     kann.
    /// </summary>
    public sealed partial class MainGamePage : Page, Engine.IListener, IEarthquakeListener
    {


        private static readonly String PRODUCT_NAME = Strings.GetString("PRODUCT");
        private static readonly string SOUNDS_PREF = "enable_sounds";
        private static readonly string EXTENSION = "cty";

        private readonly Dictionary<MapState, ToggleMenuFlyoutItem> _mapStateMenuItems =
            new Dictionary<MapState, ToggleMenuFlyoutItem>();

        private bool _autoBudgetPending;
        private EarthquakeStepper _currentEarthquake;
        public StorageFile CurrentFile;
        public MicropolisTool CurrentTool;
        private Dictionary<int, ToggleMenuFlyoutItem> _difficultyMenuItems;
        private bool _dirty1; //indicates if a tool was successfully applied since last save
        private bool _dirty2; //indicates if simulator took a step since last save
        private bool _doSounds = true;
        private Engine.Micropolis _engine;
        private bool _isMouseDown;
        private long _lastSavedTime; //real-time clock of when file was last saved

        /// <summary>
        ///     Point where the tool was last applied during the current drag
        /// </summary>
        private Point _lastToolUsage;

        private OverlayMapView _mapView;
        private MessagesPane _messagesPane;

        private bool _navigatedAway;
        private Dictionary<Speed, ToggleMenuFlyoutItem> _priorityMenuItems;

        /// <summary>
        ///     The shake timer shakes the map during an earthquake.
        /// </summary>
        private DispatcherTimer _shakeTimer;

        /// <summary>
        ///     The sim timer progresses the game.
        /// </summary>
        private DispatcherTimer _simTimer;

        public void SelectTool(MicropolisTool newTool)
        {
            CurrentTool = newTool;

            currentToolLbl.Text =
                Strings.ContainsKey("tool." + CurrentTool.Name + ".name")
                    ? Strings.GetString("tool." + CurrentTool.Name + ".name")
                    : CurrentTool.Name;

            int cost = CurrentTool.GetToolCost();
            currentToolCostLbl.Text = cost != 0 ? FormatFunds(cost) : " ";

            if (newTool == MicropolisTools.MicropolisTool["EMPTY"])
            {
                OnEscapePressed();
                return;
            }
        }

        /// <summary>
        ///     The tool stroke used when a tool is being pressed
        /// </summary>
        private ToolStroke _toolStroke;

        public MainGamePage()
        {
            InitializeComponent();
            App.MainPageReference = this;
            Loaded += MainPage_Loaded;
            //this.Frame.Navigating += Frame_Navigating;

            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();
        }

        public MicropolisDrawingArea DrawingAreaPublic
        {
            get { return DrawingArea; }
        }

        /// <summary>
        ///     Called when an earthquake has started.
        /// </summary>
        public void EarthquakeStarted()
        {
            if (IsTimerActive())
            {
                StopTimer();
            }

            _currentEarthquake = new EarthquakeStepper(this);
            _currentEarthquake.OneStep();
            StartTimer();
        }

        /// <summary>
        ///     Cities the message.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="p">The p.</param>
        public void CityMessage(MicropolisMessage m, CityLocation p)
        {
            _messagesPane.AppendCityMessage(m);

            if (m.UseNotificationPane && p != null)
            {
                NotificationPanel.ShowMessage(m, p.X, p.Y);
            }
        }


        /// <summary>
        ///     Fired whenever the mayor's money changes.
        /// </summary>
        public void FundsChanged()
        {
            ReloadFunds();
        }

        /// <summary>
        ///     Fired whenever autoBulldoze, autoBudget, noDisasters, or simSpeed change.
        /// </summary>
        public void OptionsChanged()
        {
            reloadOptions();
        }

        /// <summary>
        ///     Plays the provided sound, if the provided location is currently on screen.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="loc">The location where the sound is located at.</param>
        public void CitySound(Sound sound, CityLocation loc)
        {
            if (!_doSounds)
                return;

            Uri afile = sound.GetAudioFile();
            if (afile == null)
                return;

            bool isOnScreen =
                RectangleContainsPoint(
                    new Rect(DrawingAreaScroll.HorizontalOffset, DrawingAreaScroll.VerticalOffset,
                        DrawingAreaScroll.ViewportWidth, DrawingAreaScroll.ViewportHeight),
                    DrawingArea.GetTileBoundsAsRect(loc.X, loc.Y));


            if (sound == Sounds.Sound["HONKHONK_LOW"] && !isOnScreen)
                return;

            try
            {
                SoundOutput.Source = afile;
                SoundOutput.Play();
            }
            catch (Exception e)
            {
                //e.printStackTrace(System.err);
            }
        }

        /// <summary>
        ///     Fired whenever the "census" is taken, and the various historical counters have been updated. (Once a month in
        ///     game.)
        /// </summary>
        public void CensusChanged()
        {
        }

        /// <summary>
        ///     Fired whenever resValve, comValve, or indValve changes. (Twice a month in game.)
        /// </summary>
        public void DemandChanged()
        {
        }

        /// <summary>
        ///     Fired whenever the city evaluation is recalculated. (Once a year.)
        /// </summary>
        public void EvaluationChanged()
        {
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigatedAway = true;
            base.OnNavigatedFrom(e);
            OnWindowClosed();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (_navigatedAway) // had we ever been navigated away? Could be first run after all
            {
                OnWindowReopend();
            }
        }

        public WriteableBitmap GetLandscapeFromDrawingArea(int xpos, int ypos, Size viewportSize)
        {
            return DrawingArea.GetLandscape(xpos, ypos, viewportSize);
        }

        public void HideGraphsPane()
        {
            graphsPane.Visibility = Visibility.Collapsed;
        }


        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            MainPageInit(new Engine.Micropolis());
            //TODO: Map-Creation als Teil des Hauptmenüs!
            OnNewCityClicked();
        }


        public void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Add)
            {
                DoZoom(1);
            }
            if (e.Key == VirtualKey.Subtract)
            {
                DoZoom(-1);
            }
            if (e.Key == VirtualKey.Escape)
            {
                OnEscapePressed();
            }
        }

        public void MainPage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                _isMouseDown = true;
                try
                {
                    OnToolDown(e);
                }
                catch (Exception exp)
                {
                    showErrorMessage(exp);
                }
            }
            else if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch ||
                                e.Pointer.PointerDeviceType == PointerDeviceType.Pen)
            {
                try
                {
                    ResetConfirmationBar(true);
                    OnToolDown(e);
                }
                catch (Exception exp)
                {
                    showErrorMessage(exp);
                }
            }
        }


        public void MainPage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                try
                {
                    if (_isMouseDown)
                    {
                        OnToolDrag(e);
                    }

                    OnToolHover(e);
                }
                catch (Exception exp)
                {
                    showErrorMessage(exp);
                }
            }
        }

        public void MainPage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                _isMouseDown = false;
                try
                {
                    OnToolUp(e);
                    touchPointerRoutedEventArgsOfCurrentConfirmationPending = null;
                }
                catch (Exception exp)
                {
                    showErrorMessage(exp);
                }
            }
            else if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch ||
                                e.Pointer.PointerDeviceType == PointerDeviceType.Pen)
            {
                onToolTouch(e);
                touchPointerRoutedEventArgsOfCurrentConfirmationPending = e;
            }
        }

        private void onToolTouch(PointerRoutedEventArgs e)
        {
            var mouseX = e.GetCurrentPoint(DrawingArea).Position.X;
            var mouseY = e.GetCurrentPoint(DrawingArea).Position.Y;

            float zoomFactor = DrawingAreaScroll.ZoomFactor;

            if (DrawingArea.ConfirmBar.RenderTransform is TranslateTransform)
            { //check whether click occured within confirmbar in which case we don't move anything
                var confPos = (TranslateTransform)DrawingArea.ConfirmBar.RenderTransform;
                if (mouseX >= confPos.X / zoomFactor && mouseX <= DrawingArea.ConfirmBar.Width + confPos.X / zoomFactor && mouseY >= confPos.Y / zoomFactor && mouseY <= DrawingArea.ConfirmBar.Height +

confPos.Y / zoomFactor)
                {
                    return;
                }
            }

            TranslateTransform translate = new TranslateTransform();
            var toolCursorSize = DrawingArea.GetToolCursorSizeInPixel();

            var posX = Math.Ceiling(mouseX / DrawingArea.TILE_WIDTH - Math.Ceiling(toolCursorSize.Width / DrawingArea.TILE_WIDTH / 2)) * DrawingArea.TILE_WIDTH;
            posX += toolCursorSize.Width;
            posX *= zoomFactor;
            if (posX + DrawingArea.ConfirmBar.ActualWidth - DrawingAreaScroll.HorizontalOffset >= DrawingAreaScroll.ActualWidth) // in case the bar is out of view, we'll move it to the opposite side of the toolCursor
            {
                posX = Math.Ceiling(mouseX / DrawingArea.TILE_WIDTH - Math.Ceiling(toolCursorSize.Width / DrawingArea.TILE_WIDTH / 2)) * DrawingArea.TILE_WIDTH - DrawingArea.ConfirmBar.ActualWidth;
                posX *= zoomFactor;
            }
            translate.X = posX;


            var posY = Math.Ceiling(mouseY / DrawingArea.TILE_HEIGHT - Math.Ceiling(toolCursorSize.Height / DrawingArea.TILE_HEIGHT / 2)) * DrawingArea.TILE_HEIGHT;
            posY += toolCursorSize.Height;
            posY *= zoomFactor;
            if (posY + DrawingArea.ConfirmBar.ActualHeight - DrawingAreaScroll.VerticalOffset >= DrawingAreaScroll.ActualHeight)
            {
                posY = Math.Ceiling(mouseY / DrawingArea.TILE_HEIGHT - Math.Ceiling(toolCursorSize.Height / DrawingArea.TILE_HEIGHT / 2)) * DrawingArea.TILE_HEIGHT - DrawingArea.ConfirmBar.ActualHeight;
                posY *= zoomFactor;
            }
            translate.Y = posY;

            DrawingArea.ConfirmBar.RenderTransform = translate;
            DrawingArea.ConfirmBar.Visibility = Visibility.Visible;
        }

        private void MainPage_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                try
                {
                    OnToolExited(e);
                }
                catch (Exception exp)
                {
                    showErrorMessage(exp);
                }
            }
        }

        /*private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            closeWindow();

            onWindowClosed();

        }*/

        private void DrawingArea_PointerWheelChanged(object sender, PointerRoutedEventArgs args)
        {
            int delta = args.GetCurrentPoint(DrawingArea).Properties.MouseWheelDelta;

            try
            {
                var posX = (int)(args.GetCurrentPoint(DrawingArea).Position.X + DrawingAreaScroll.HorizontalOffset);
                var posY = (int)(args.GetCurrentPoint(DrawingArea).Position.Y + DrawingAreaScroll.VerticalOffset);

                CityLocation loc = DrawingArea.GetCityLocation(posX, posY);
                onMouseWheelMoved(delta, new Point(posX, posY));
            }
            catch (Exception e)
            {
                showErrorMessage(e);
            }
        }

        public void MainPageInit(Engine.Micropolis engine)
        {
            _engine = engine;


            DrawingArea.ConfirmBar.Confirmed += ConfirmBar_Confirmed;
            DrawingArea.ConfirmBar.Declined += ConfirmBar_Declined;
            DrawingArea.ConfirmBar.Uped += ConfirmBar_Uped;
            DrawingArea.ConfirmBar.Downed += ConfirmBar_Downed;
            DrawingArea.ConfirmBar.Lefted += ConfirmBar_Lefted;
            DrawingArea.ConfirmBar.Righted += ConfirmBar_Righted;

            DrawingArea.SetUpAfterBasicInit(engine, this);

            MakeMenu();

            graphsPane.SetUpAfterBasicInit(engine, this);

            evaluationPane.SetupAfterBasicInit(this, engine);

            //makeDateFunds();

            var mapViewContainer = new StackPanel();
            minimapPane.Children.Add(mapViewContainer);
            var mapMenu = new StackPanel();
            mapViewContainer.Children.Add(mapMenu);

            var zonesMenu = new StackPanel();
            var menuZonesHeader = new Button
            {
                Content = Strings.GetString("menu.zones"),
                Style = Application.Current.Resources["WhiteButtonStyle"] as Style
            };
            zonesMenu.Children.Add(menuZonesHeader);

            //setupKeys(zonesMenu, "menu.zones");
            var mapMenuButtons = new StackPanel { Orientation = Orientation.Horizontal };

            mapMenu.Children.Add(mapMenuButtons);
            mapMenuButtons.Children.Add(zonesMenu);

            var zoneflyout = new MenuFlyout();
            menuZonesHeader.Flyout = zoneflyout;
            zoneflyout.Items.Add(MakeMapStateMenuItem("menu.zones.ALL", MapState.ALL));
            zoneflyout.Items.Add(MakeMapStateMenuItem("menu.zones.RESIDENTIAL", MapState.RESIDENTIAL));
            zoneflyout.Items.Add(MakeMapStateMenuItem("menu.zones.COMMERCIAL", MapState.COMMERCIAL));
            zoneflyout.Items.Add(MakeMapStateMenuItem("menu.zones.INDUSTRIAL", MapState.INDUSTRIAL));
            zoneflyout.Items.Add(MakeMapStateMenuItem("menu.zones.TRANSPORT", MapState.TRANSPORT));

            var overlaysMenu = new StackPanel();
            mapMenuButtons.Children.Add(overlaysMenu);

            var overlaysMenuHeader = new Button();
            overlaysMenu.Children.Add(overlaysMenuHeader);
            overlaysMenuHeader.Content = Strings.GetString("menu.overlays");
            overlaysMenuHeader.Style = Application.Current.Resources["WhiteButtonStyle"] as Style;
            var menuflyout = new MenuFlyout();
            overlaysMenuHeader.Flyout = menuflyout;
            //setupKeys(overlaysMenu, "menu.overlays");

            menuflyout.Items.Add(MakeMapStateMenuItem("menu.overlays.POPDEN_OVERLAY", MapState.POPDEN_OVERLAY));
            menuflyout.Items.Add(MakeMapStateMenuItem("menu.overlays.GROWTHRATE_OVERLAY", MapState.GROWTHRATE_OVERLAY));
            menuflyout.Items.Add(MakeMapStateMenuItem("menu.overlays.LANDVALUE_OVERLAY", MapState.LANDVALUE_OVERLAY));
            menuflyout.Items.Add(MakeMapStateMenuItem("menu.overlays.CRIME_OVERLAY", MapState.CRIME_OVERLAY));
            menuflyout.Items.Add(MakeMapStateMenuItem("menu.overlays.POLLUTE_OVERLAY", MapState.POLLUTE_OVERLAY));
            menuflyout.Items.Add(MakeMapStateMenuItem("menu.overlays.TRAFFIC_OVERLAY", MapState.TRAFFIC_OVERLAY));
            menuflyout.Items.Add(MakeMapStateMenuItem("menu.overlays.POWER_OVERLAY", MapState.POWER_OVERLAY));
            menuflyout.Items.Add(MakeMapStateMenuItem("menu.overlays.FIRE_OVERLAY", MapState.FIRE_OVERLAY));
            menuflyout.Items.Add(MakeMapStateMenuItem("menu.overlays.POLICE_OVERLAY", MapState.POLICE_OVERLAY));

            _mapView = new OverlayMapView();
            _mapView.SetUpAfterBasicInit(engine);
            _mapView.ConnectView(DrawingArea, DrawingAreaScroll);
            mapViewContainer.Children.Add(_mapView);


            SetMapState(MapState.ALL);

            _messagesPane = new MessagesPane();
            messagesScrollViewer.Content = _messagesPane;


            NotificationPanel.SetUpAfterBasicInit(this);

            DrawingArea.KeyDown += MainPage_KeyDown;
            DrawingArea.PointerPressed += MainPage_PointerPressed;
            DrawingArea.PointerReleased += MainPage_PointerReleased;
            DrawingArea.PointerMoved += MainPage_PointerMoved;
            DrawingArea.PointerExited += MainPage_PointerExited;
            DrawingArea.PointerWheelChanged += DrawingArea_PointerWheelChanged;

            _doSounds = Prefs.GetBoolean("SOUNDS_PREF", true);

            // start things up
            _mapView.SetEngine(engine);

            engine.AddEarthquakeListener(this);
            ReloadFunds();
            reloadOptions();
            StartTimer();
            MakeClean();

            DrawingAreaScroll.ViewChanging += _mapView.drawingAreaScroll_ViewChanging;

            toolsPanel.SetUpAfterBasicInit(this);
            toolsPanel.Mode = ToolBarMode.FLYOUT;
        }

        void ConfirmBar_Righted(object sender, EventArgs e)
        {
            if (_lastToolUsage.X + 1 >= _engine.Map[0].Length)
            {
                return;
            }

            moveCurrentToolPosTo((int)_lastToolUsage.X + 1, (int)_lastToolUsage.Y);
        }

        void ConfirmBar_Lefted(object sender, EventArgs e)
        {
            if (_lastToolUsage.X - 1 < 0)
            {
                return;
            }
            moveCurrentToolPosTo((int)_lastToolUsage.X - 1, (int)_lastToolUsage.Y);
        }

        void ConfirmBar_Downed(object sender, EventArgs e)
        {
            if (_lastToolUsage.Y + 1 >= _engine.Map.Length)
            {
                return;
            }
            moveCurrentToolPosTo((int)_lastToolUsage.X, (int)_lastToolUsage.Y + 1);
        }

        void ConfirmBar_Uped(object sender, EventArgs e)
        {
            if (_lastToolUsage.Y - 1 < 0)
            {
                return;
            }
            moveCurrentToolPosTo((int)_lastToolUsage.X, (int)_lastToolUsage.Y - 1);
        }

        void ConfirmBar_Declined(object sender, EventArgs e)
        {
            ResetConfirmationBar();
        }

        private void ResetConfirmationBar(bool resetToolPreview = true)
        {
            DrawingArea.ConfirmBar.Visibility = Visibility.Collapsed; // hide confirmation bar

            if (resetToolPreview)
            {
                // remove old preview
                _toolStroke = null;
                DrawingArea.SetToolPreview(null);
                DrawingArea.SetToolCursor(null);
            }
        }

        private PointerRoutedEventArgs touchPointerRoutedEventArgsOfCurrentConfirmationPending;
        void ConfirmBar_Confirmed(object sender, EventArgs e)
        {
            if (touchPointerRoutedEventArgsOfCurrentConfirmationPending != null)
            {
                OnToolUp(touchPointerRoutedEventArgsOfCurrentConfirmationPending); // actually build the stuff
            }
            DrawingArea.ConfirmBar.Visibility = Visibility.Collapsed; // hide confirmation bar
        }


        public void SetEngine(Engine.Micropolis newEngine)
        {
            if (_engine != null)
            {
                // old engine
                _engine.RemoveListener(this);
                _engine.RemoveEarthquakeListener(this);
            }

            _engine = newEngine;

            if (_engine != null)
            {
                // new engine
                _engine.AddListener(this);
                _engine.AddEarthquakeListener(this);
            }

            bool timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }
            StopEarthquake();

            DrawingArea.SetEngine(_engine);
            _mapView.SetEngine(_engine); //must change mapView after DrawingArea
            evaluationPane.SetEngine(_engine);
            demandInd.SetEngine(_engine);
            graphsPane.SetEngine(_engine);
            ReloadFunds();
            reloadOptions();

            if (timerEnabled)
            {
                StartTimer();
            }
        }

        private bool NeedsSaved()
        {
            if (_dirty1) //player has built something since last save
                return true;

            if (!_dirty2) //no simulator ticks since last save
                return false;

            // simulation time has passed since last save, but the player
            // hasn't done anything. Whether we need to prompt for save
            // will depend on how much real time has elapsed.
            // The threshold is 30 seconds.

            return (DateTime.Now.Millisecond - _lastSavedTime > 30000);
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Yes") //ToDo: test this conversion of object to int
            {
                // Display message showing the label of the command that was invoked
                OnSaveCityClicked();
            }
        }

        private async Task<bool> MaybeSaveCity()
        {
            if (NeedsSaved())
            {
                bool timerEnabled = IsTimerActive();
                if (timerEnabled)
                {
                    StopTimer();
                }

                try
                {
                    var saveDialog = new MessageDialog(Strings.GetString("main.save_query"));
                    saveDialog.Commands.Add(new UICommand(
                        "Yes",
                        CommandInvokedHandler));
                    saveDialog.Commands.Add(new UICommand(
                        "No",
                        CommandInvokedHandler));

                    saveDialog.DefaultCommandIndex = 0;

                    // Set the command to be invoked when escape is pressed
                    saveDialog.CancelCommandIndex = 1;
                    await saveDialog.ShowAsync();

                    return false;
                }
                finally
                {
                    if (timerEnabled)
                    {
                        StartTimer();
                    }
                }
            }
            return true;
        }

        private void CloseWindow()
        {
            MaybeSaveCity();
        }


        //private void makeDateFunds()
        //{
        //    //dateLabel.Text = Strings.getString("main.date_label");

        //    //fundsLabel.Text = Strings.getString("main.funds_label");

        //    //populationLabel.Text = Strings.getString("main.population_label");

        //}

        private void setupKeys(StackPanel menu, String prefix)
        {
            /*if (Strings.containsKey(prefix+".key")) {
			String mnemonic = Strings.getString(prefix+".key");
			menu.setMnemonic(
				KeyStroke.getKeyStroke(mnemonic).getKeyCode()
				);
		}*/
        }

        /*private void setupKeys(StackPanel menuItem, String prefix)
	{
		if (strings.containsKey(prefix+".key")) {
			String mnemonic = strings.getString(prefix+".key");
			menuItem.setMnemonic(
				KeyStroke.getKeyStroke(mnemonic).getKeyCode()
				);
		}
		if (strings.containsKey(prefix+".shortcut")) {
			String shortcut = strings.getString(prefix+".shortcut");
			menuItem.setAccelerator(
				KeyStroke.getKeyStroke(shortcut)
				);
		}
	}*/


        public void NewButton_Click(object sender, RoutedEventArgs e)
        {
            OnNewCityClicked();
        }

        public void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OnLoadGameClicked();
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            OnSaveCityClicked();
        }

        public void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            OnSaveCityAsClicked();
        }

        public void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        public void AutoBudgetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OnAutoBudgetClicked();
        }

        public void AutoBulldozeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnAutoBulldozeClicked();
        }

        public void DisastersCheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnDisastersClicked();
        }

        public void SoundCheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnSoundClicked();
        }

        public void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            DoZoom(1);
        }

        public void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            DoZoom(-1);
        }

        public void MonsterButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.MONSTER);
        }

        public void FireButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.FIRE);
        }

        public void FloodButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.FLOOD);
        }

        public void MeltdownButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.MELTDOWN);
        }

        public void TornadoButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.TORNADO);
        }

        public void EarthquakeButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.EARTHQUAKE);
        }

        public void BudgetButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewBudgetClicked();
        }

        public void EvaluationButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewEvaluationClicked();
        }

        public void GraphButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewGraphClicked();
        }

        public void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            onAboutClicked();
        }

        private void MakeMenu()
        {
            //NewButton.Label = Strings.getString("menu.game.new");
            //LoadButton.Text = Strings.GetString("menu.game.load");
            //SaveButton.Text = Strings.GetString("menu.game.save");
            //SaveAsButton.Text = Strings.GetString("menu.game.save_as");
            //ExitButton.Label = Strings.getString("menu.game.exit");
            SettingsButton.Label = Strings.GetString("menu.options");
            DifficultyButton.Label = Strings.GetString("menu.difficulty");
            AutoBudgetCheckBox.Text = Strings.GetString("menu.options.auto_budget");
            AutoBulldozeCheckBox.Text = Strings.GetString("menu.options.auto_bulldoze");
            DisastersCheckBox.Text = Strings.GetString("menu.options.disasters");
            SoundCheckBox.Text = Strings.GetString("menu.options.sound");

            //ZoomInButton.Label = Strings.getString("menu.options.zoom_in");
            //ZoomOutButton.Label = Strings.getString("menu.options.zoom_out");
            DisasterButton.Label = Strings.GetString("menu.disasters");
            MonsterButton.Text = Strings.GetString("menu.disasters.MONSTER");
            FireButton.Text = Strings.GetString("menu.disasters.FIRE");
            FloodButton.Text = Strings.GetString("menu.disasters.FLOOD");
            MeltdownButton.Text = Strings.GetString("menu.disasters.MELTDOWN");
            TornadoButton.Text = Strings.GetString("menu.disasters.TORNADO");
            EarthquakeButton.Text = Strings.GetString("menu.disasters.EARTHQUAKE");

            SpeedButton.Label = Strings.GetString("menu.speed");


            _difficultyMenuItems = new Dictionary<int, ToggleMenuFlyoutItem>();
            for (int i = GameLevel.MIN_LEVEL; i <= GameLevel.MAX_LEVEL; i++)
            {
                int level = i;
                var menuItemc = new ToggleMenuFlyoutItem { Text = Strings.GetString("menu.difficulty." + level) };
                //setupKeys(menuItem, "menu.difficulty."+level);
                menuItemc.Click += delegate { OnDifficultyClicked(level); };

                levelMenu.Items.Add(menuItemc);
                _difficultyMenuItems.Add(level, menuItemc);
            }


            _priorityMenuItems = new Dictionary<Speed, ToggleMenuFlyoutItem>();
            var menuItemb = new ToggleMenuFlyoutItem { Text = Strings.GetString("menu.speed.SUPER_FAST") };
            //setupKeys(menuItem, "menu.speed.SUPER_FAST");
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["SUPER_FAST"], (ToggleMenuFlyoutItem)o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["SUPER_FAST"], menuItemb);

            menuItemb = new ToggleMenuFlyoutItem { Text = Strings.GetString("menu.speed.FAST") };
            //setupKeys(menuItem, "menu.speed.FAST");
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["FAST"], (ToggleMenuFlyoutItem)o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["FAST"], menuItemb);

            menuItemb = new ToggleMenuFlyoutItem { Text = Strings.GetString("menu.speed.NORMAL"), IsChecked = true };
            //setupKeys(menuItem, "menu.speed.NORMAL");
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["NORMAL"], (ToggleMenuFlyoutItem)o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["NORMAL"], menuItemb);

            menuItemb = new ToggleMenuFlyoutItem { Text = Strings.GetString("menu.speed.SLOW") };
            //setupKeys(menuItem, "menu.speed.SLOW");
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["SLOW"], (ToggleMenuFlyoutItem)o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["SLOW"], menuItemb);

            menuItemb = new ToggleMenuFlyoutItem { Text = Strings.GetString("menu.speed.PAUSED") };
            //setupKeys(menuItem, "menu.speed.PAUSED");
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["PAUSED"], (ToggleMenuFlyoutItem)o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["PAUSED"], menuItemb);


            var windowsMenu = new StackPanel();
            var windowsHeader = new TextBlock { Text = Strings.GetString("menu.windows") };
            //setupKeys(windowsMenu, "menu.windows");
            //menuBar.Children.Add(windowsMenu);


            BudgetButton.Label = Strings.GetString("menu.windows.budget");
            EvaluationButton.Label = Strings.GetString("menu.windows.evaluation");
            GraphButton.Label = Strings.GetString("menu.windows.graph");
            //AboutButton.Label = Strings.getString("menu.help.about");
        }

        private Engine.Micropolis GetEngine()
        {
            return _engine;
        }

        private void OnAutoBudgetClicked()
        {
            _dirty1 = true;
            GetEngine().ToggleAutoBudget();
        }

        private void OnAutoBulldozeClicked()
        {
            _dirty1 = true;
            GetEngine().ToggleAutoBulldoze();
        }

        private void OnDisastersClicked()
        {
            _dirty1 = true;
            GetEngine().ToggleDisasters();
        }

        private void OnSoundClicked()
        {
            _doSounds = !_doSounds;
            Prefs.PutBoolean(SOUNDS_PREF, _doSounds);
            reloadOptions();
        }

        public void MakeClean()
        {
            _dirty1 = false;
            _dirty2 = false;
            _lastSavedTime = DateTime.Now.Millisecond;
            if (CurrentFile != null)
            {
                String fileName = CurrentFile.Name;
                if (fileName.EndsWith("." + EXTENSION))
                {
                    fileName = fileName.Substring(0, fileName.Length - 1 - EXTENSION.Length);
                }
                title.Text = Strings.GetString("main.caption_named_city") + fileName;
            }
            else
            {
                title.Text = Strings.GetString("main.caption_unnamed_city");
            }
        }

        private async Task<bool> OnSaveCityClicked()
        {
            if (CurrentFile == null)
            {
                return await OnSaveCityAsClicked();
            }

            try
            {
                await GetEngine().Save(CurrentFile);
                MakeClean();
                return true;
            }
            catch (IOException e)
            {
                //e.printStackTrace(System.err);
                /*JOptionPane.showMessageDialog(this, e, strings.getString("main.error_caption"),
				JOptionPane.ERROR_MESSAGE);*/
                var d = new MessageDialog(Strings.GetString("main.error_caption") + ": " + e);
                d.ShowAsync();
                return false;
            }
        }

        private async Task<bool> OnSaveCityAsClicked()
        {
            bool timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }
            try
            {
                var picker = new FileSavePicker { DefaultFileExtension = "cty" };
                picker.FileTypeChoices.Add(".cty Micropolis city", new List<string> { ".cty" });
                picker.FileTypeChoices.Add(".cty_file Micropolis city", new List<string> { ".cty_file" });
                StorageFile fileToSave = await picker.PickSaveFileAsync();
                if (fileToSave != null)
                {
                    await GetEngine().Save(fileToSave);
                    MakeClean();
                    return true;
                }
            }
            catch (Exception e)
            {
                var d = new MessageDialog(Strings.GetString("main.error_caption") + ": " + e);
            }
            finally
            {
                if (timerEnabled)
                {
                    StartTimer();
                }
            }
            return false;
        }

        /// <summary>
        ///     Called when user clicked the load button.
        /// </summary>
        private async void OnLoadGameClicked()
        {
            // check if user wants to save their current city
            bool saveNeeded = await MaybeSaveCity();
            /*if (!saveNeeded)
            {
                return;
            }*/

            bool timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }

            try
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".cty");
                picker.FileTypeFilter.Add(".cty_file");
                StorageFile file = await picker.PickSingleFileAsync();

                //assert !isTimerActive();

                if (file != null)
                {
                    var newEngine = new Engine.Micropolis();
                    newEngine.Load(file);
                    SetEngine(newEngine);
                    CurrentFile = file;
                    MakeClean();
                }
            }
            catch (Exception e)
            {
                var d = new MessageDialog(Strings.GetString("main.error_caption") + ": " + e);
            }
            finally
            {
                if (timerEnabled)
                {
                    StartTimer();
                }
            }
        }







        /// <summary>
        ///     Called when the user clicked the new city button to open the dialog.
        /// </summary>
        private async void OnNewCityClicked()
        {
            bool saveNeeded = await MaybeSaveCity();
            //if (saveNeeded)
            //{
            DoNewCity();
            //}
        }

        /// <summary>
        ///     Shows a new city dialog.
        /// </summary>
        public void DoNewCity()
        {
            bool timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }

            var dialog = new NewCityDialog(this);
            NewGameDialogPaneInner.Children.Clear();
            NewGameDialogPaneInner.Children.Add(dialog);
            ShowNewGameDialogPanel();

            if (timerEnabled)
            {
                StartTimer();
            }
        }

        /// <summary>
        ///     Shows the new game dialog panel.
        /// </summary>
        public void ShowNewGameDialogPanel()
        {
            NewGameDialogPaneOuter.Visibility = Visibility.Visible;
            HideAppBars();
        }

        /// <summary>
        ///     Hides the application bars.
        /// </summary>
        private void HideAppBars()
        {
            //TopAppBar.IsOpen = false;
            BottomAppBar.IsOpen = false;
        }

        /// <summary>
        ///     Hides the new game dialog panel.
        /// </summary>
        public void HideNewGameDialogPanel()
        {
            NewGameDialogPaneOuter.Visibility = Visibility.Collapsed;
            ShowAppBars();
        }

        /// <summary>
        ///     Shows the application bars.
        /// </summary>
        private void ShowAppBars()
        {
            //TopAppBar.IsOpen = true;
            BottomAppBar.IsOpen = true;
        }

        /// <summary>
        ///     Queries the map at the specified coordinates and shows the results in the notification panel.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        private void DoQueryTool(int xpos, int ypos)
        {
            if (!_engine.TestBounds(xpos, ypos))
                return;

            ZoneStatus z = _engine.QueryZoneStatus(xpos, ypos);
            NotificationPanel.ShowZoneStatus(xpos, ypos, z);
        }

        /// <summary>
        ///     Zooms the map at provided screen coordinates of drawing area.
        /// </summary>
        /// <param name="dir">The direction. +1 is zoom in, -1 is zoom out.</param>
        /// <param name="mousePt">The mouse position to center at.</param>
        private void DoZoom(int dir, Point mousePt)
        {
            int oldZoom = DrawingArea.GetTileSize();
            int newZoom = dir < 0 ? (oldZoom / 2) : (oldZoom * 2);
            if (newZoom <= 8)
            {
                newZoom = 8;
                //ZoomOutButton.IsEnabled = false;
            }
            else if (newZoom >= 32)
            {
                newZoom = 32;
                //ZoomInButton.IsEnabled = false;
            }

            if (oldZoom != newZoom)
            {
                // preserve effective mouse position in viewport when changing zoom level
                double f = newZoom / (double)oldZoom;
                var pos = new Point(DrawingAreaScroll.HorizontalOffset, DrawingAreaScroll.VerticalOffset);
                var newX = (int)Math.Round(mousePt.X * f - (mousePt.X - pos.X));
                var newY = (int)Math.Round(mousePt.Y * f - (mousePt.Y - pos.Y));
                DrawingArea.SelectTileSize(newZoom);
                DrawingAreaScroll.ScrollToHorizontalOffset(newX);
                DrawingAreaScroll.ScrollToVerticalOffset(newY);
            }
        }

        /// <summary>
        ///     Zooms the map.
        /// </summary>
        /// <param name="dir">The direction. +1 is zoom in, -1 is zoom out.</param>
        private void DoZoom(int dir)
        {
            DoZoom(dir,
                new Point(DrawingAreaScroll.HorizontalOffset + DrawingAreaScroll.ViewportWidth / 2,
                    DrawingAreaScroll.VerticalOffset + DrawingAreaScroll.ViewportHeight / 2));
        }

        /// <summary>
        ///     Called when the mouse wheel is moved
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="point">The point.</param>
        private void onMouseWheelMoved(int delta, Point point)
        {
            //TODO: Temporärer auskommentiert, um ungewünschtes Zoom-Verhalten zu unterdrücken. Evtl. mit Tastenmodifier verknüpfen?
            /*if (delta < 0)
            {
                doZoom(1, point);
            }
            else
            {
                doZoom(-1, point);
            }*/
        }


        /// <summary>
        ///     Called when the user wants to use a tool and presses the mouse button for it. Could be a single tool use or a drag
        ///     at this point.
        /// </summary>
        /// <param name="ev">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void OnToolDown(PointerRoutedEventArgs ev)
        {
            double zoomFactor = DrawingAreaScroll.ZoomFactor;
            ScrollViewer target = DrawingAreaScroll;
            if (ev.GetCurrentPoint(target).Properties.IsRightButtonPressed)
            {
                var posX =
                    (int)((ev.GetCurrentPoint(target).Position.X + DrawingAreaScroll.HorizontalOffset) / zoomFactor);
                var posY = (int)((ev.GetCurrentPoint(target).Position.Y + DrawingAreaScroll.VerticalOffset) / zoomFactor);

                CityLocation loc = DrawingArea.GetCityLocation(posX, posY);

                DoQueryTool(loc.X, loc.Y);
                return;
            }

            if (!ev.GetCurrentPoint(target).Properties.IsLeftButtonPressed)
                return;

            if (CurrentTool == null)
                return;

            var posXb = (int)((ev.GetCurrentPoint(target).Position.X + DrawingAreaScroll.HorizontalOffset) / zoomFactor);
            var posYb = (int)((ev.GetCurrentPoint(target).Position.Y + DrawingAreaScroll.VerticalOffset) / zoomFactor);

            CityLocation locb = DrawingArea.GetCityLocation(posXb, posYb);
            int x = locb.X;
            int y = locb.Y;

            if (CurrentTool == MicropolisTools.MicropolisTool["QUERY"])
            {
                DoQueryTool(x, y);
                _toolStroke = null;
            }
            else
            {
                _toolStroke = CurrentTool.BeginStroke(_engine, x, y);
                PreviewTool();
            }

            _lastToolUsage.X = x;
            _lastToolUsage.Y = y;
        }

        private void moveCurrentToolPosTo(int x, int y)
        {
            _lastToolUsage.X = x;
            _lastToolUsage.Y = y;

            if (CurrentTool == MicropolisTools.MicropolisTool["QUERY"])
            {
                DoQueryTool(x, y);
                _toolStroke = null;
            }
            else
            {
                _toolStroke = CurrentTool.BeginStroke(_engine, x, y);
                PreviewTool();
            }

        }

        /// <summary>
        ///     Called when the Escape key is pressed.
        /// </summary>
        private void OnEscapePressed()
        {
            // if currently dragging a tool...
            if (_toolStroke != null)
            {
                // cancel the current mouse operation
                _toolStroke = null;
                DrawingArea.SetToolPreview(null);
                DrawingArea.SetToolCursor(null);
            }
            else
            {
                // dismiss any alerts currently visible
                HideNotificationPanel();
            }
        }

        /// <summary>
        ///     Called when the tool should be applied.
        /// </summary>
        /// <param name="ev">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void OnToolUp(PointerRoutedEventArgs ev)
        {
            if (_toolStroke != null)
            {
                DrawingArea.SetToolPreview(null);

                CityLocation loc = _toolStroke.GetLocation();
                ToolResult tr = _toolStroke.Apply();
                ShowToolResult(loc, tr);
                _toolStroke = null;
            }

            OnToolHover(ev);

            if (_autoBudgetPending)
            {
                _autoBudgetPending = false;
                ShowBudgetWindow(true);
            }
        }

        /// <summary>
        ///     Shows a preview of the tool.
        /// </summary>
        private void PreviewTool()
        {
            //assert this.toolStroke != null;
            //assert this.currentTool != null;

            DrawingArea.SetToolCursor(
                _toolStroke.GetBounds(),
                CurrentTool
                );
            DrawingArea.SetToolPreview(
                _toolStroke.GetPreview()
                );
        }

        /// <summary>
        ///     Called when the mouse is dragged above the drawing area.
        /// </summary>
        /// <param name="ev">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void OnToolDrag(PointerRoutedEventArgs ev)
        {
            double zoomFactor = DrawingAreaScroll.ZoomFactor;
            ScrollViewer target = DrawingAreaScroll;
            if (CurrentTool == null)
                return;
            /*if ((ev.getModifiersEx() & MouseEvent.BUTTON1_DOWN_MASK) == 0)
			return;*/
            // Bug: do

            var posX = (int)((ev.GetCurrentPoint(target).Position.X / zoomFactor + DrawingAreaScroll.HorizontalOffset));
            var posY = (int)((ev.GetCurrentPoint(target).Position.Y / zoomFactor + DrawingAreaScroll.VerticalOffset));

            //int posX = (int) (Math.Ceiling(ev.GetCurrentPoint(this).Position.X / DrawingArea.getTileSize() - 1) * DrawingArea.getTileSize());
            //int posY = (int) (Math.Ceiling(ev.GetCurrentPoint(this).Position.Y / DrawingArea.getTileSize() - 1) * DrawingArea.getTileSize());

            CityLocation loc = DrawingArea.GetCityLocation(posX, posY);

            int x = loc.X;
            int y = loc.Y;
            if (x == _lastToolUsage.X && y == _lastToolUsage.Y)
                return;

            if (_toolStroke != null)
            {
                _toolStroke.DragTo(x, y);
                PreviewTool();
            }
            else if (CurrentTool == MicropolisTools.MicropolisTool["QUERY"])
            {
                DoQueryTool(x, y);
            }

            _lastToolUsage.X = x;
            _lastToolUsage.Y = y;
        }

        /// <summary>
        ///     Called when the mouse hovers the drawing area.
        /// </summary>
        /// <param name="ev">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void OnToolHover(PointerRoutedEventArgs ev)
        {

            ResetConfirmationBar(!_isMouseDown); // if confirmation bar is shown and we hover but do not drag we send true, otherwise when we drag we send false

            if (ev.Pointer.PointerDeviceType != PointerDeviceType.Mouse)
            {
                return;
            }

            double zoomFactor = DrawingAreaScroll.ZoomFactor;
            ScrollViewer target = DrawingAreaScroll;
            if (CurrentTool == null || CurrentTool == MicropolisTools.MicropolisTool["QUERY"])
            {
                DrawingArea.SetToolCursor(null);
                return;
            }

            var posX = (int)((ev.GetCurrentPoint(target).Position.X + DrawingAreaScroll.HorizontalOffset) / zoomFactor);
            var posY = (int)((ev.GetCurrentPoint(target).Position.Y + DrawingAreaScroll.VerticalOffset) / zoomFactor);

            CityLocation loc = DrawingArea.GetCityLocation(posX, posY);
            int x = loc.X;
            int y = loc.Y;
            int w = CurrentTool.GetWidth();
            int h = CurrentTool.GetHeight();

            if (w >= 3)
                x--;
            if (h >= 3)
                y--;

            DrawingArea.SetToolCursor(new CityRect(x, y, w, h), CurrentTool);
        }

        /// <summary>
        ///     Called when the cursor leaves the drawing area
        /// </summary>
        /// <param name="ev">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void OnToolExited(PointerRoutedEventArgs ev)
        {
            DrawingArea.SetToolCursor(null);
        }

        /// <summary>
        ///     Shows the tool result, ie. message and sound.
        /// </summary>
        /// <param name="loc">The location of the tool.</param>
        /// <param name="result">The result.</param>
        private void ShowToolResult(CityLocation loc, ToolResult result)
        {
            switch (result)
            {
                case ToolResult.SUCCESS:
                    CitySound(
                        CurrentTool == MicropolisTools.MicropolisTool["BULLDOZER"]
                            ? Sounds.Sound["BULLDOZE"]
                            : Sounds.Sound["BUILD"], loc);
                    _dirty1 = true;
                    break;

                case ToolResult.NONE:
                    break;
                case ToolResult.UH_OH:
                    _messagesPane.AppendCityMessage(MicropolisMessages.BULLDOZE_FIRST);
                    CitySound(Sounds.Sound["UHUH"], loc);
                    break;

                case ToolResult.INSUFFICIENT_FUNDS:
                    _messagesPane.AppendCityMessage(MicropolisMessages.INSUFFICIENT_FUNDS);
                    CitySound(Sounds.Sound["SORRY"], loc);
                    break;

                //assert false;
            }
        }

        /// <summary>
        ///     Formats the funds as Euros.
        /// </summary>
        /// <param name="funds">The funds as euros.</param>
        /// <returns></returns>
        public static String FormatFunds(int funds)
        {
            return funds + " €"; //Strings.getString("funds")
        }

        /// <summary>
        ///     Formats the game date.
        /// </summary>
        /// <param name="cityTime">The city time.</param>
        /// <returns>The city date as MMM yyyy</returns>
        public static String FormatGameDate(int cityTime)
        {
            var c = new DateTime();
            c = c.AddYears(1900 + cityTime / 48);
            c = c.AddMonths((cityTime % 48) / 4);
            c = c.AddDays((cityTime % 4) * 7 + 1);

            return c.ToString("MMM yyyy"); //Strings.getString("citytime")
        }

        /// <summary>
        ///     Updates the user interface to reflect the current date.
        /// </summary>
        private void UpdateDateLabel()
        {
            dateLbl.Text = FormatGameDate(_engine.CityTime);

            popLbl.Text = GetEngine().GetCityPopulation().ToString();
        }

        /// <summary>
        ///     Starts the timers.
        /// </summary>
        private void StartTimer()
        {
            Engine.Micropolis engine = GetEngine();
            int count = engine.SimSpeed.SimStepsPerUpdate;

            //assert !isTimerActive();

            if (engine.SimSpeed == Speeds.Speed["PAUSED"])
                return;

            if (_currentEarthquake != null)
            {
                int interval = 3000 / MicropolisDrawingArea.SHAKE_STEPS;
                _shakeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, interval) };
                _shakeTimer.Tick += delegate
                {
                    _currentEarthquake.OneStep();
                    if (_currentEarthquake.Count == 0)
                    {
                        StopTimer();
                        _currentEarthquake = null;
                        StartTimer();
                    }
                };

                _shakeTimer.Start();
                return;
            }

            //assert simTimer == null;
            _simTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, engine.SimSpeed.AnimationDelay) };
            _simTimer.Tick += delegate
            {
                for (int i = 0; i < count; i++)
                {
                    engine.Animate();
                    if (!engine.AutoBudget && engine.IsBudgetTime())
                    {
                        ShowAutoBudget();
                        return;
                    }
                }
                UpdateDateLabel();
                _dirty2 = true;
            };

            _simTimer.Start();
        }


        private void showErrorMessage(Exception e)
        {
            var w = new StringWriter();
            var d = new MessageDialog(Strings.GetString("main.error_unexpected") + e);
            Debug.WriteLine(Strings.GetString("main.error_unexpected") + e);

            /*JTextPane stackTracePane = new JTextPane();
		stackTracePane.setEditable(false);
		stackTracePane.setText(w.ToString());

		final JScrollPane detailsPane = new JScrollPane(stackTracePane);
		detailsPane.setVerticalScrollBarPolicy(JScrollPane.VERTICAL_SCROLLBAR_ALWAYS);
		detailsPane.setPreferredSize(new Dimension(480,240));
		detailsPane.setMinimumSize(new Dimension(0,0));

		int rv = JOptionPane.showOptionDialog(this, e,
			strings.getString("main.error_unexpected"),
			JOptionPane.DEFAULT_OPTION,
			JOptionPane.ERROR_MESSAGE,
			null,
			new String[] {
				strings.getString("main.error_show_stacktrace"),
				strings.getString("main.error_close"),
				strings.getString("main.error_shutdown")
				},
			1
			);
		if (rv == 0)
		{
			JOptionPane.showMessageDialog(this, detailsPane,
				strings.getString("main.error_unexpected"),
				JOptionPane.ERROR_MESSAGE);
		}
		if (rv == 2)
		{
			rv = JOptionPane.showConfirmDialog(
				this,
				strings.getString("error.shutdown_query"),
				strings.getString("main.error_unexpected"),
				JOptionPane.OK_CANCEL_OPTION,
				JOptionPane.WARNING_MESSAGE);
			if (rv == JOptionPane.OK_OPTION) {
				System.exit(1);
			}
		}*/
        }

        /// <summary>
        ///     Called when an earthquake has stopped.
        /// </summary>
        private void StopEarthquake()
        {
            DrawingArea.Shake(0);
            _currentEarthquake = null;
        }

        /// <summary>
        ///     Stops the simTimer.
        /// </summary>
        private void StopTimer()
        {
            //assert isTimerActive();

            if (_simTimer != null)
            {
                _simTimer.Stop();
                _simTimer = null;
            }
            if (_shakeTimer != null)
            {
                _shakeTimer.Stop();
                _shakeTimer = null;
            }
        }

        /// <summary>
        ///     Determines whether the simTimer or shakeTimer is active.
        /// </summary>
        /// <returns>true, if any is active, otherwise false</returns>
        private bool IsTimerActive()
        {
            return _simTimer != null || _shakeTimer != null;
        }

        /// <summary>
        ///     Called when the window is closed.
        /// </summary>
        private void OnWindowClosed()
        {
            if (IsTimerActive())
            {
                StopTimer();
            }
        }

        /// <summary>
        ///     Called when the page had been closed and is opened again in the same instance of the game.
        /// </summary>
        public void OnWindowReopend()
        {
            if (!IsTimerActive())
            {
                StartTimer();
            }
        }

        /// <summary>
        ///     Called when the user clicked on a difficulty button.
        /// </summary>
        /// <param name="newDifficulty">The new difficulty.</param>
        public void OnDifficultyClicked(int newDifficulty)
        {
            GetEngine().SetGameLevel(newDifficulty);

            foreach (var item in _difficultyMenuItems)
                item.Value.IsChecked = (item.Key.Equals(newDifficulty));
        }

        /// <summary>
        ///     Called when the user clicked on a speed button.
        /// </summary>
        /// <param name="newSpeed">The new speed.</param>
        private void OnPriorityClicked(Speed newSpeed, ToggleMenuFlyoutItem sender)
        {
            if (IsTimerActive())
            {
                StopTimer();
            }

            GetEngine().SetSpeed(newSpeed);
            StartTimer();

            foreach (ToggleMenuFlyoutItem item in SpeedMenu.Items)
                item.IsChecked = (item.Equals(sender)) ? true : false;
        }

        /// <summary>
        ///     Called when the user clicked one of the disaster buttons.
        /// </summary>
        /// <param name="disaster">The disaster.</param>
        private void OnInvokeDisasterClicked(Disaster disaster)
        {
            _dirty1 = true;
            switch (disaster)
            {
                case Disaster.FIRE:
                    GetEngine().MakeFire();
                    break;
                case Disaster.FLOOD:
                    GetEngine().MakeFlood();
                    break;
                case Disaster.MONSTER:
                    GetEngine().MakeMonster();
                    break;
                case Disaster.MELTDOWN:
                    if (!GetEngine().MakeMeltdown())
                    {
                        _messagesPane.AppendCityMessage(MicropolisMessages.NO_NUCLEAR_PLANTS);
                    }
                    break;
                case Disaster.TORNADO:
                    GetEngine().MakeTornado();
                    break;
                case Disaster.EARTHQUAKE:
                    GetEngine().MakeEarthquake();
                    break;
                //assert false; //unknown disaster
            }
        }

        /// <summary>
        ///     Updates the user interface to reflect current funds.
        /// </summary>
        private void ReloadFunds()
        {
            fundsLbl.Text = FormatFunds(GetEngine().Budget.TotalFunds);
        }


        /// <summary>
        ///     Updates the user interface to reflect current options.
        /// </summary>
        private void reloadOptions()
        {
            AutoBudgetCheckBox.IsChecked = (_engine.AutoBudget);
            AutoBulldozeCheckBox.IsChecked = (_engine.AutoBulldoze);
            DisastersCheckBox.IsChecked = (!_engine.NoDisasters);
            SoundCheckBox.IsChecked = (_doSounds);
            foreach (Speed spd in _priorityMenuItems.Keys)
            {
                _priorityMenuItems[spd].IsChecked = (_engine.SimSpeed == spd);
            }
            for (int i = GameLevel.MIN_LEVEL; i <= GameLevel.MAX_LEVEL; i++)
            {
                _difficultyMenuItems[i].IsChecked = (_engine.GameLevel == i);
            }
        }


        /// <summary>
        ///     Checks whether the rectangle contains the point.
        /// </summary>
        /// <param name="outer">The outer rectangle.</param>
        /// <param name="inner">The inner point.</param>
        /// <returns></returns>
        private bool RectangleContainsPoint(Rect outer, Rect inner)
        {
            if (inner.Height > outer.Height || inner.Width > outer.Width || inner.X + inner.Width < outer.X ||
                inner.Y + inner.Height < outer.Y || inner.X > outer.Width || inner.Y > outer.Height)
            {
                //ToDo: check
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Called when the user clicks the budget button. Shows the budget window.
        /// </summary>
        private void OnViewBudgetClicked()
        {
            _dirty1 = true;
            ShowBudgetWindow(false);
        }

        /// <summary>
        ///     Called when the user clicked the evaluation button. Shows the evaluation panel.
        /// </summary>
        private void OnViewEvaluationClicked()
        {
            evaluationPane.Visibility = evaluationPane.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        ///     Called when the user clicked the graph button. Shows the graph panel.
        /// </summary>
        private void OnViewGraphClicked()
        {
            graphsPane.Visibility = graphsPane.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            graphsPane.Repaint();
        }

        /// <summary>
        ///     Shows the automatic budget.
        /// </summary>
        private void ShowAutoBudget()
        {
            if (_toolStroke == null)
            {
                ShowBudgetWindow(true);
            }
            else
            {
                _autoBudgetPending = true;
            }
        }

        /// <summary>
        ///     Shows the budget window.
        /// </summary>
        /// <param name="isEndOfYear">if set to <c>true</c> [is end of year].</param>
        private void ShowBudgetWindow(bool isEndOfYear)
        {
            bool timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }

            NewBudgetDialog.SetupAfterBasicInit(this, GetEngine());

            ShowBudgetDialog();

            if (timerEnabled)
            {
                StartTimer();
            }
        }

        /// <summary>
        ///     Shows the budget dialog.
        /// </summary>
        private void ShowBudgetDialog()
        {
            NewBudgetDialogPaneOuter.Visibility = Visibility.Visible;
            //TopAppBar.IsOpen = false;
            //BottomAppBar.IsOpen = false;
            BottomAppBar.IsOpen = false;
        }

        /// <summary>
        ///     Hides the budget dialog.
        /// </summary>
        public void HideBudgetDialog()
        {
            NewBudgetDialogPaneOuter.Visibility = Visibility.Collapsed;
            //TopAppBar.IsOpen = true;
            //BottomAppBar.IsOpen = true;
            BottomAppBar.IsOpen = true;
        }

        /// <summary>
        ///     Makes an item for the mapstatemenu from the state provided with the caption identified by stringPrefix.
        /// </summary>
        /// <param name="stringPrefix">The string prefix identifying the caption for the map state menu item.</param>
        /// <param name="state">The state, the map state menu item represents.</param>
        /// <returns></returns>
        private ToggleMenuFlyoutItem MakeMapStateMenuItem(String stringPrefix, MapState state)
        {
            String caption = Strings.GetString(stringPrefix);
            var menuItem = new ToggleMenuFlyoutItem { Text = caption };
            //setupKeys(menuItem, stringPrefix);
            menuItem.Click += delegate { SetMapState(state); };
            _mapStateMenuItems.Add(state, menuItem);
            return menuItem;
        }

        /// <summary>
        ///     Sets the map state to the state provided and clears the old state.
        /// </summary>
        /// <param name="state">The state.</param>
        private void SetMapState(MapState state)
        {
            _mapStateMenuItems[(_mapView.GetMapState())].IsChecked = false;
            _mapStateMenuItems[(state)].IsChecked = true;
            _mapView.SetMapState(state);
            SetMapLegend(state);
        }

        /// <summary>
        ///     Sets the map legend for the provided map state by loading and displaying image. If an image cannot be loaded for
        ///     the state provided, the image on screen will be hidden.
        /// </summary>
        /// <param name="state">The state.</param>
        private async void SetMapLegend(MapState state)
        {
            String k = "legend_image." + state;
            Uri iconUrl = null;
            if (Strings.ContainsKey(k))
            {
                String iconName = "ms-appx:///resources/" + Strings.GetString(k);
                iconUrl = new Uri(iconName, UriKind.RelativeOrAbsolute);

                StorageFolder folder = Package.Current.InstalledLocation;
                folder = await folder.GetFolderAsync("resources");
#if WINDOWS_APP
                IStorageItem file;
                file = 
                    await
                        folder.TryGetItemAsync(iconUrl.AbsoluteUri.Substring(iconUrl.AbsoluteUri.LastIndexOf("/", StringComparison.Ordinal) + 1));
#endif
#if WINDOWS_PHONE_APP
                IStorageItem file;
                try
                {
                    file =
                        await
                            folder.GetFileAsync(
                                iconUrl.AbsoluteUri.Substring(
                                    iconUrl.AbsoluteUri.LastIndexOf("/", StringComparison.Ordinal) + 1));
                }
                catch (FileNotFoundException)
                {
                    file = null;
                }
#endif

                if (file != null)
                {
                    ImageSource iSource = new BitmapImage(iconUrl);

                    mapLegendLbl.Source = iSource;
                    mapLegendLbl.Opacity = 1;
                    return;
                }
            }
            mapLegendLbl.Opacity = 0;
        }

        /// <summary>
        ///     Called when someone clicks the about button and displays the about message
        /// </summary>
        private void onAboutClicked()
        {
            String version = "0.5";
            String versionStr = Strings.GetString("main.about_caption") + ": " +
                                Strings.GetString("main.version_string") + " " + version;

            var d = new MessageDialog(Strings.GetString("main.about_text"));
        }

        /// <summary>
        ///     Hides the notification panel.
        /// </summary>
        internal void HideNotificationPanel()
        {
            NotificationPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     Hides the evaluation pane.
        /// </summary>
        internal void HideEvaluationPane()
        {
            evaluationPane.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     Shows the notification panel.
        /// </summary>
        internal void ShowNotificationPanel()
        {
            NotificationPanel.Visibility = Visibility.Visible;
        }


        /// <summary>
        ///     Scrolls the game field by delta.
        /// </summary>
        /// <param name="dx">The horizontal delta.</param>
        /// <param name="dy">The vertical delta.</param>
        internal void ScrollGameFieldBy(int dx, int dy)
        {
            double horizontalPos = DrawingAreaScroll.HorizontalOffset - dx;
            double verticalPos = DrawingAreaScroll.VerticalOffset - dy;
            DrawingAreaScroll.ChangeView(horizontalPos, verticalPos, 1);
        }


        /// <summary>
        ///     Called when app gets suspended. Saves current game if necessary and initiates app shutdown.
        /// </summary>
        internal void OnAppClosed()
        {
            SaveToInternalStorage();
            OnWindowClosed();
        }

        /// <summary>
        ///     Saves to internal storage file autosave.cty.
        /// </summary>
        private async void SaveToInternalStorage()
        {
            if (NeedsSaved())
            {
                bool timerEnabled = IsTimerActive();
                if (timerEnabled)
                {
                    StopTimer();
                }

                try
                {
                    StorageFolder folderToSave = ApplicationData.Current.LocalFolder;
                    StorageFile fileToSave =
                        await folderToSave.CreateFileAsync("autosave.cty", CreationCollisionOption.ReplaceExisting);
                    if (fileToSave != null)
                    {
                        await GetEngine().Save(fileToSave);
                        //makeClean();
                    }
                }
                catch (Exception e)
                {
                    var d = new MessageDialog(Strings.GetString("main.error_caption") + ": " + e);
                }
            }
        }

        internal float GetZoomFactor()
        {
            return DrawingAreaScroll.ZoomFactor;
        }

        private void UIVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (demandInd.Visibility == Visibility.Visible)
            {
                demandInd.Visibility = Visibility.Collapsed;
                minimapPane.Visibility = Visibility.Collapsed;
                InformationBar.Visibility = Visibility.Collapsed;
                messagesScrollViewerPane.Visibility = Visibility.Collapsed;
            }
            else
            {
                demandInd.Visibility = Visibility.Visible;
                minimapPane.Visibility = Visibility.Visible;
                InformationBar.Visibility = Visibility.Visible;
                messagesScrollViewerPane.Visibility = Visibility.Visible;
            }
        }
    }
}