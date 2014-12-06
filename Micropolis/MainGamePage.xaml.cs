using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Engine;
using Micropolis.Model.Entities;
using Micropolis.ViewModels;

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

    /// <summary>
    ///     Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden
    ///     kann.
    /// </summary>
    public sealed partial class MainGamePage : Engine.IListener, IEarthquakeListener
    {
        /// <summary>
        /// The name of sound preferences whether to play sounds or not (does not include music).
        /// </summary>
        private static readonly string SOUNDS_PREF = "enable_sounds";
        /// <summary>
        /// The file extension for Micropolis files.
        /// </summary>
        private static readonly string EXTENSION = "cty";

        /// <summary>
        /// The map state menu items contains map states linking to their respective ToggleMenuFlyoutItems.
        /// </summary>
        private readonly Dictionary<MapState, ToggleMenuFlyoutItem> _mapStateMenuItems =
            new Dictionary<MapState, ToggleMenuFlyoutItem>();

        /// <summary>
        /// The current view model.
        /// </summary>
        private MainGamePageViewModel viewModel;

        /// <summary>
        /// The current file loaded in the game.
        /// </summary>
        public StorageFile CurrentFile;
        /// <summary>
        /// The current tool selected.
        /// </summary>
        public MicropolisTool CurrentTool;


        /// <summary>
        /// Indicates if auto budget is pending.
        /// </summary>
        private bool _isAutoBudgetPending;

        /// <summary>
        /// The current earthquake occuring.
        /// </summary>
        private EarthquakeStepper _currentEarthquake;

        /// <summary>
        /// The difficulty menu items linking difficulty levels to their respective ToggleMenuFlyoutItems.
        /// </summary>
        private Dictionary<int, ToggleMenuFlyoutItem> _difficultyMenuItems;

        /// <summary>
        /// Indicates if a tool was successfully applied since last save.
        /// </summary>
        private bool _isDirty1;
        
        /// <summary>
        /// Indicates if simulator took a step since last save.
        /// </summary>
        private bool _dirty2;

        /// <summary>
        /// Indicates if sounds should be played or not.
        /// </summary>
        private bool _isDoSounds = true;

        /// <summary>
        /// Reference to the current game engine.
        /// </summary>
        public Engine.Micropolis Engine { get; private set; }

        /// <summary>
        /// Indicates if the left mouse button is down (for dragging).
        /// </summary>
        private bool _isMouseDown;

        /// <summary>
        /// Real-time clock of when file was last saved
        /// </summary>
        private long _lastSavedTime; 

        /// <summary>
        ///     Point where the tool was last applied during the current drag.
        /// </summary>
        private Point _lastToolUsage;

        /// <summary>
        /// The map view used to display map states and viewport.
        /// </summary>
        private OverlayMapView _mapView;
        /// <summary>
        /// The messages pane used to display game messages.
        /// </summary>
        private MessagesPane _messagesPane;

        /// <summary>
        /// The priorityMenuItems links speeds to their respective ToggleMenuFlyoutItems.
        /// </summary>
        private Dictionary<Speed, ToggleMenuFlyoutItem> _priorityMenuItems;

        /// <summary>
        ///     The shake timer shakes the map during an earthquake.
        /// </summary>
        private DispatcherTimer _shakeTimer;

        /// <summary>
        ///     The sim timer progresses the game.
        /// </summary>
        private DispatcherTimer _simTimer;

        /// <summary>
        ///     The tool stroke used when a tool is being pressed
        /// </summary>
        private ToolStroke _toolStroke;

        /// <summary>
        /// The touch pointer routed event arguments of current confirmation pending when user touched screen and confirmation bar awaits user action.
        /// </summary>
        private PointerRoutedEventArgs touchPointerRoutedEventArgsOfCurrentConfirmationPending;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainGamePage"/> class.
        /// </summary>
        public MainGamePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            App.MainPageReference = this;
            Loaded += MainPage_Loaded;
            //this.Frame.Navigating += Frame_Navigating;
        }

        /// <summary>
        ///     Called when an earthquake has started. Pauses game simulation, does one earthquake step and restarts simulation.
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
        ///     Shows a city message and if required launches the NotificationPanel.
        /// </summary>
        /// <param name="m">The message to be displayed.</param>
        /// <param name="p">The location of the message. Can be null.</param>
        public void CityMessage(MicropolisMessage m, CityLocation p)
        {
            _messagesPane.AppendCityMessage(m);

            if (m.UseNotificationPane && p != null)
            {
                NotificationPanel.ShowMessage(m, p.X, p.Y);
                ShowNotificationPanel();
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
            ReloadOptions();
        }

        /// <summary>
        ///     Plays the provided sound, if the provided location is currently on screen.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="loc">The location where the sound is located at.</param>
        public void CitySound(Sound sound, CityLocation loc)
        {
            if (!_isDoSounds)
                return;

            Uri afile = sound.GetAudioFile();
            if (afile == null)
                return;

            bool isOnScreen =
                RectangleContainsPoint(
                    new Rect(DrawingAreaScroll.HorizontalOffset, DrawingAreaScroll.VerticalOffset,
                        DrawingAreaScroll.ViewportWidth, DrawingAreaScroll.ViewportHeight),
                    DrawingArea.ViewModel.GetTileBoundsAsRect(loc.X, loc.Y));


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


        /// <summary>
        /// Selects the tool specified.
        /// </summary>
        /// <param name="newTool">The new tool.</param>
        public void SelectTool(MicropolisTool newTool)
        {
            CurrentTool = newTool;

            CurrentToolLbl.Text =
                Strings.ContainsKey("tool." + CurrentTool.Name + ".name")
                    ? Strings.GetString("tool." + CurrentTool.Name + ".name")
                    : CurrentTool.Name;

            int cost = CurrentTool.GetToolCost();
            CurrentToolCostLbl.Text = cost != 0 ? FormatFunds(cost) : " ";

            if (newTool == MicropolisTools.MicropolisTool["EMPTY"])
            {
                OnEscapePressed();
            }
        }

        /// <summary>
        /// Wird unmittelbar aufgerufen, nachdem die Page entladen und nicht mehr die aktuelle Quelle eines übergeordneten Frame ist.
        /// </summary>
        /// <param name="e">Ereignisdaten, die vom überschreibenden Code überprüft werden können. Die Ereignisdaten repräsentieren die Navigation, die die aktuelle Page hochgeladen hat.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.IsNavigatedAway = true;
            base.OnNavigatedFrom(e);
            OnWindowClosed();
        }

        /// <summary>
        /// Wird unmittelbar aufgerufen, bevor die Page geladen und zur aktuellen Quelle eines übergeordneten Frame wird.
        /// </summary>
        /// <param name="e">Ereignisdaten, die vom überschreibenden Code überprüft werden können. Die Ereignisdaten repräsentieren die noch ausstehende Navigation, die die aktuelle Page hochladen wird. Normalerweise ist Parameter die wichtigste zu überprüfende Eigenschaft.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            

            if (App.IsNavigatedAway) // had we ever been navigated away? Could be first run after all
            {
                OnWindowReopend();
            }
        }

        /// <summary>
        /// Gets the landscape from drawing area for specified coordinates and viewport size.
        /// </summary>
        /// <param name="xpos">The xpos top left.</param>
        /// <param name="ypos">The ypos top left.</param>
        /// <param name="viewportSize">Size of the viewport.</param>
        /// <returns></returns>
        public WriteableBitmap GetLandscapeFromDrawingArea(int xpos, int ypos, Size viewportSize)
        {
            return DrawingArea.ViewModel.GetLandscape(xpos, ypos, viewportSize);
        }

        /// <summary>
        /// Hides the graphs pane.
        /// </summary>
        public void HideGraphsPane()
        {
            GraphsPane.Visibility = Visibility.Collapsed;
        }

        private bool _firstRun = true;

        /// <summary>
        /// Handles the Loaded event of the MainPage control, initializtes the settings charm, game and processes app commands.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_firstRun)
            {


                // Register handler for CommandsRequested events from the settings pane
                SettingsPane.GetForCurrentView().CommandsRequested += SettingsCharm.OnCommandsInGameRequested;

                bool engineExists = Engine != null;
                    // If we loaded another game via app commands, we already have an engine.
                if (!engineExists)
                {
                    Engine = new Engine.Micropolis();
                }
                MainGamePageInit(Engine);
            }
            ProcessAppCommands();
            
            _firstRun = false;

            VisualStateChanged += MainGamePage_VisualStateChanged;

            Window.Current.SizeChanged += Window_SizeChanged;
            DetermineVisualState();

            if (this.viewModel == null)
            {
                this.viewModel = new MainGamePageViewModel();
                this.DataContext = this.viewModel;
            }

            //TODO: Map-Creation als Teil des Hauptmenüs!
        }

        void MainGamePage_VisualStateChanged(object sender, EventArgs e)
        {
            ToolsPanel.Mode = (_state == "Snapped" || _state == "Narrow") ? ToolBarMode.FLYOUT : ToolBarMode.NORMAL;
        }

        /// <summary>
        /// Processes the application commands or shows the new game dialog.
        /// </summary>
        private void ProcessAppCommands()
        {
            var currentApp = (ISupportsAppCommands) Application.Current;
            AppCommand loadCityCommand =
                currentApp.AppCommands.FirstOrDefault(s => s.Instruction == AppCommands.LOADFILE);
            
            bool loadCity = loadCityCommand != null; // loadCityCommand present?

            AppCommand loadCityAsNewCommand =
                currentApp.AppCommands.FirstOrDefault(s => s.Instruction == AppCommands.LOADFILEASNEWCITY);

            bool loadCityAsNew = loadCityAsNewCommand != null; // loadCityCommand present?

            if (loadCity)
            {
                var file = (StorageFile) loadCityCommand.File;
                currentApp.AppCommands.Remove(loadCityCommand);

                LoadGameFile(file);
            }
            else if (loadCityAsNew)
            {
                var file = (StorageFile)loadCityAsNewCommand.File;
                currentApp.AppCommands.Remove(loadCityAsNewCommand);

                LoadGameFile(file,false);
            }
            else if (_firstRun)
            {
                OnNewCityClicked();
            }
        }

        /// <summary>
        /// Loads the game file.
        /// </summary>
        /// <param name="file">The file to be loaded.</param>
        private void LoadGameFile(StorageFile file, bool useFileForSave = true)
        {
            if (file != null)
            {
                var newEngine = new Engine.Micropolis();
                newEngine.Load(file).ContinueWith((a) =>
                {
                    App.MainPageReference.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        SetEngine(newEngine);
                        if (useFileForSave)
                        {
                            CurrentFile = file;
                        }
                        MakeClean();
                    });
                });
            }
        }


        /// <summary>
        /// Handles the KeyDown event of the MainPage control for zooming or canceling an action.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs"/> instance containing the event data.</param>
        public void DrawingArea_KeyDown(object sender, KeyRoutedEventArgs e)
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

        /// <summary>
        /// Handles the PointerPressed event of the DrawingArea control for when a tool is started to be used (e.g. for dragging).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        public void DrawingArea_PointerPressed(object sender, PointerRoutedEventArgs e)
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
                    ShowErrorMessage(exp);
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
                    ShowErrorMessage(exp);
                }
            }
        }


        /// <summary>
        /// Handles the PointerMoved event of the DrawingArea control for when a tool is dragged or hovered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        public void DrawingArea_PointerMoved(object sender, PointerRoutedEventArgs e)
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
                    ShowErrorMessage(exp);
                }
            }
        }

        /// <summary>
        /// Handles the PointerReleased event of the DrawingArea control for when a tool has been used.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        public void DrawingArea_PointerReleased(object sender, PointerRoutedEventArgs e)
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
                    ShowErrorMessage(exp);
                }
            }
            else if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch ||
                     e.Pointer.PointerDeviceType == PointerDeviceType.Pen)
            {
                onToolTouch(e);
                touchPointerRoutedEventArgsOfCurrentConfirmationPending = e;
            }
        }

        /// <summary>
        /// Handles the PointerRoutedEventArgs for when the finger or pen gets raised and the tool should show a confirmation bar.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void onToolTouch(PointerRoutedEventArgs e)
        {
            ConfirmBar.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles the PointerExited event of the DrawingArea control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void DrawingArea_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                try
                {
                    OnToolExited(e);
                }
                catch (Exception exp)
                {
                    ShowErrorMessage(exp);
                }
            }
        }

        /// <summary>
        /// Handles the PointerWheelChanged event of the DrawingArea control for zooming.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void DrawingArea_PointerWheelChanged(object sender, PointerRoutedEventArgs args)
        {
            int delta = args.GetCurrentPoint(DrawingArea).Properties.MouseWheelDelta;

            try
            {
                var posX = (int) (args.GetCurrentPoint(DrawingArea).Position.X + DrawingAreaScroll.HorizontalOffset);
                var posY = (int) (args.GetCurrentPoint(DrawingArea).Position.Y + DrawingAreaScroll.VerticalOffset);

                CityLocation loc = DrawingArea.ViewModel.GetCityLocation(posX, posY);
                onMouseWheelMoved(delta, new Point(posX, posY));
            }
            catch (Exception e)
            {
                ShowErrorMessage(e);
            }
        }

        /// <summary>
        /// Initializes the MainGamePage.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public void MainGamePageInit(Engine.Micropolis engine)
        {
            Engine = engine;

            ConfirmBar.Confirmed += ConfirmBar_Confirmed;
            ConfirmBar.Declined += ConfirmBar_Declined;
            ConfirmBar.Uped += ConfirmBar_Uped;
            ConfirmBar.Downed += ConfirmBar_Downed;
            ConfirmBar.Lefted += ConfirmBar_Lefted;
            ConfirmBar.Righted += ConfirmBar_Righted;

            DrawingArea.ViewModel.SetUpAfterBasicInit(engine, this);

            NewBudgetDialog.ViewModel.SetupAfterBasicInit(this, engine);

            MakeMenu();

            GraphsPane.SetUpAfterBasicInit(engine, this);

            EvaluationPane.SetupAfterBasicInit(this, engine);

            var mapViewContainer = new StackPanel();
            MiniMapPane.Children.Add(mapViewContainer);
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
            var mapMenuButtons = new StackPanel {Orientation = Orientation.Horizontal};

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
            _mapView.ConnectView(DrawingArea.ViewModel, DrawingAreaScroll);
            mapViewContainer.Children.Add(_mapView);

            SetMapState(MapState.ALL);

            _messagesPane = new MessagesPane();
            MessagesScrollViewer.Content = _messagesPane;

            NotificationPanel.SetUpAfterBasicInit(this);

            DrawingArea.KeyDown += DrawingArea_KeyDown;
            DrawingArea.PointerPressed += DrawingArea_PointerPressed;
            DrawingArea.PointerReleased += DrawingArea_PointerReleased;
            DrawingArea.PointerMoved += DrawingArea_PointerMoved;
            DrawingArea.PointerExited += DrawingArea_PointerExited;
            DrawingArea.PointerWheelChanged += DrawingArea_PointerWheelChanged;

            _isDoSounds = Prefs.GetBoolean("SOUNDS_PREF", true);

            // start things up
            _mapView.SetEngine(engine);

            engine.AddEarthquakeListener(this);
            ReloadFunds();
            ReloadOptions();
            UpdateDateLabel();
            StartTimer();
            MakeClean();

            DrawingAreaScroll.ViewChanging += _mapView.drawingAreaScroll_ViewChanging;

            ToolsPanel.SetUpAfterBasicInit(this);
        }

        /// <summary>
        /// Handles the Righted event of the ConfirmBar control to move sprite to the right.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ConfirmBar_Righted(object sender, EventArgs e)
        {
            if (_lastToolUsage.X + 1 >= Engine.Map[0].Length)
            {
                return;
            }

            MoveCurrentToolPosTo((int) _lastToolUsage.X + 1, (int) _lastToolUsage.Y);
        }

        /// <summary>
        /// Handles the Lefted event of the ConfirmBar control to move sprite to the left.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ConfirmBar_Lefted(object sender, EventArgs e)
        {
            if (_lastToolUsage.X - 1 < 0)
            {
                return;
            }
            MoveCurrentToolPosTo((int) _lastToolUsage.X - 1, (int) _lastToolUsage.Y);
        }

        /// <summary>
        /// Handles the Downed event of the ConfirmBar control to move sprite down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ConfirmBar_Downed(object sender, EventArgs e)
        {
            if (_lastToolUsage.Y + 1 >= Engine.Map.Length)
            {
                return;
            }
            MoveCurrentToolPosTo((int) _lastToolUsage.X, (int) _lastToolUsage.Y + 1);
        }

        /// <summary>
        /// Handles the Uped event of the ConfirmBar control to move sprite up.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ConfirmBar_Uped(object sender, EventArgs e)
        {
            if (_lastToolUsage.Y - 1 < 0)
            {
                return;
            }
            MoveCurrentToolPosTo((int) _lastToolUsage.X, (int) _lastToolUsage.Y - 1);
        }

        /// <summary>
        /// Handles the Declined event of the ConfirmBar control to hide sprite and conf bar.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ConfirmBar_Declined(object sender, EventArgs e)
        {
            ResetConfirmationBar();
        }

        /// <summary>
        /// Resets the confirmation bar and tool according to parameter.
        /// </summary>
        /// <param name="resetToolPreview">if set to <c>true</c> tool gets canceled.</param>
        private void ResetConfirmationBar(bool resetToolPreview = true)
        {
            ConfirmBar.Visibility = Visibility.Collapsed; // hide confirmation bar

            if (resetToolPreview)
            {
                // remove old preview
                _toolStroke = null;
                DrawingArea.ViewModel.SetToolPreview(null);
                DrawingArea.ViewModel.SetToolCursor(null);
            }
        }

        /// <summary>
        /// Handles the Confirmed event of the ConfirmBar control to place a tool at the previously defined position.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ConfirmBar_Confirmed(object sender, EventArgs e)
        {
            if (touchPointerRoutedEventArgsOfCurrentConfirmationPending != null)
            {
                OnToolUp(touchPointerRoutedEventArgsOfCurrentConfirmationPending,false); // actually build the stuff
            }
        }


        /// <summary>
        /// Sets the engine specified and updates the user interface.
        /// </summary>
        /// <param name="newEngine">The new engine.</param>
        public void SetEngine(Engine.Micropolis newEngine)
        {
            if (Engine != null)
            {
                // old engine
                Engine.RemoveListener(this);
                Engine.RemoveEarthquakeListener(this);
            }

            Engine = newEngine;

            if (Engine != null)
            {
                // new engine
                Engine.AddListener(this);
                Engine.AddEarthquakeListener(this);
            }

            bool timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }
            StopEarthquake();

            DrawingArea.ViewModel.SetEngine(Engine);
            _mapView.SetEngine(Engine); //must change mapView after DrawingArea
            EvaluationPane.SetEngine(Engine);
            DemandInd.SetEngine(Engine);
            GraphsPane.SetEngine(Engine);
            ReloadFunds();
            ReloadOptions();

            bool notPaused = Speeds.Speed.First(s => s.Value == Engine.SimSpeed).Key != "PAUSED";
            if (timerEnabled || notPaused)
            {
                StartTimer();
            }
        }

        /// <summary>
        /// Determines whether the game needs to be saved.
        /// </summary>
        /// <returns></returns>
        private bool NeedsSaved()
        {
            if (_isDirty1) //player has built something since last save
                return true;

            if (!_dirty2) //no simulator ticks since last save
                return false;

            // simulation time has passed since last save, but the player
            // hasn't done anything. Whether we need to prompt for save
            // will depend on how much real time has elapsed.
            // The threshold is 30 seconds.

            return (DateTime.Now.Millisecond - _lastSavedTime > 30000);
        }

        /// <summary>
        /// Handles the commands of save game question.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <remarks>ToDo: Depends on translation. Make it depend on ID.</remarks>
        private void CommandInvokedHandler(IUICommand command)
        {
            string yesString = Strings.GetString("main.saveDialogYes");
            if (command.Label == yesString) //ToDo: test this conversion of object to int
            {
                // Display message showing the label of the command that was invoked
                OnSaveCityClicked();
            }
        }

        /// <summary>
        /// Determines whether game needs to be saved and if so displays a message on screen for user confirmation.
        /// </summary>
        /// <returns>whether game needs to be saved and has not yet been saved by user (true), otherwise (false)</returns>
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
                    string yesString = Strings.GetString("main.saveDialogYes");
                    string noString = Strings.GetString("main.saveDialogNo");
                    var saveDialog = new MessageDialog(Strings.GetString("main.save_query"));
                    saveDialog.Commands.Add(new UICommand(
                        yesString,
                        CommandInvokedHandler));
                    saveDialog.Commands.Add(new UICommand(
                        noString,
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

        /// <summary>
        /// Displays a save message on game closing.
        /// </summary>
        private void CloseWindow()
        {
            MaybeSaveCity();
        }


        /// <summary>
        /// Handles the Click event of the NewButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void NewButton_Click(object sender, RoutedEventArgs e)
        {
            OnNewCityClicked();
        }

        /// <summary>
        /// Handles the Click event of the LoadButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OnLoadGameClicked();
        }

        /// <summary>
        /// Handles the Click event of the SaveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            OnSaveCityClicked();
        }

        /// <summary>
        /// Handles the Click event of the SaveAsButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            OnSaveCityAsClicked();
        }

        /// <summary>
        /// Handles the Click event of the ExitButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        /// <summary>
        /// Handles the Click event of the AutoBudgetMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void AutoBudgetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OnAutoBudgetClicked();
        }

        /// <summary>
        /// Handles the Click event of the AutoBulldozeCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void AutoBulldozeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnAutoBulldozeClicked();
        }

        /// <summary>
        /// Handles the Click event of the DisastersCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void DisastersCheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnDisastersClicked();
        }

        /// <summary>
        /// Handles the Click event of the SoundCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SoundCheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnSoundClicked();
        }

        /// <summary>
        /// Handles the Click event of the ZoomInButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            DoZoom(1);
        }

        /// <summary>
        /// Handles the Click event of the ZoomOutButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            DoZoom(-1);
        }

        /// <summary>
        /// Handles the Click event of the MonsterButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void MonsterButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.MONSTER);
        }

        /// <summary>
        /// Handles the Click event of the FireButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void FireButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.FIRE);
        }

        /// <summary>
        /// Handles the Click event of the FloodButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void FloodButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.FLOOD);
        }

        /// <summary>
        /// Handles the Click event of the MeltdownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void MeltdownButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.MELTDOWN);
        }

        /// <summary>
        /// Handles the Click event of the TornadoButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void TornadoButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.TORNADO);
        }

        /// <summary>
        /// Handles the Click event of the EarthquakeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void EarthquakeButton_Click(object sender, RoutedEventArgs e)
        {
            OnInvokeDisasterClicked(Disaster.EARTHQUAKE);
        }

        /// <summary>
        /// Handles the Click event of the BudgetButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void BudgetButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewBudgetClicked();
        }

        /// <summary>
        /// Handles the Click event of the EvaluationButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void EvaluationButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewEvaluationClicked();
        }

        /// <summary>
        /// Handles the Click event of the GraphButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void GraphButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewGraphClicked();
        }

        /// <summary>
        /// Handles the Click event of the AboutButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            OnAboutClicked();
        }

        /// <summary>
        /// Makes the menu and displays translated strings.
        /// </summary>
        private void MakeMenu()
        {
            NewButton.Text = Strings.GetString("menu.game.new");
            LoadButton.Text = Strings.GetString("menu.game.load");
            SaveButton.Text = Strings.GetString("menu.game.save");
            SaveAsButton.Text = Strings.GetString("menu.game.save_as");

            SettingsButtonContent.Text = Strings.GetString("menu.options");
            DifficultyButtonContent.Text = Strings.GetString("menu.difficulty");
            AutoBudgetCheckBox.Text = Strings.GetString("menu.options.auto_budget");
            AutoBulldozeCheckBox.Text = Strings.GetString("menu.options.auto_bulldoze");
            DisastersCheckBox.Text = Strings.GetString("menu.options.disasters");
            SoundCheckBox.Text = Strings.GetString("menu.options.sound");

            DisasterButtonContent.Text = Strings.GetString("menu.disasters");
            MonsterButton.Text = Strings.GetString("menu.disasters.MONSTER");
            FireButton.Text = Strings.GetString("menu.disasters.FIRE");
            FloodButton.Text = Strings.GetString("menu.disasters.FLOOD");
            MeltdownButton.Text = Strings.GetString("menu.disasters.MELTDOWN");
            TornadoButton.Text = Strings.GetString("menu.disasters.TORNADO");
            EarthquakeButton.Text = Strings.GetString("menu.disasters.EARTHQUAKE");

            SpeedButtonContent.Text = Strings.GetString("menu.speed");
            
            _difficultyMenuItems = new Dictionary<int, ToggleMenuFlyoutItem>();
            for (int i = GameLevel.MIN_LEVEL; i <= GameLevel.MAX_LEVEL; i++)
            {
                int level = i;
                var menuItemc = new ToggleMenuFlyoutItem {Text = Strings.GetString("menu.difficulty." + level)};
                menuItemc.Click += delegate { OnDifficultyClicked(level); };

                LevelMenu.Items.Add(menuItemc);
                _difficultyMenuItems.Add(level, menuItemc);
            }
            
            _priorityMenuItems = new Dictionary<Speed, ToggleMenuFlyoutItem>();

            var menuItemb = new ToggleMenuFlyoutItem {Text = Strings.GetString("menu.speed.SUPER_FAST")};
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["SUPER_FAST"], (ToggleMenuFlyoutItem) o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["SUPER_FAST"], menuItemb);

            menuItemb = new ToggleMenuFlyoutItem {Text = Strings.GetString("menu.speed.FAST")};
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["FAST"], (ToggleMenuFlyoutItem) o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["FAST"], menuItemb);

            menuItemb = new ToggleMenuFlyoutItem {Text = Strings.GetString("menu.speed.NORMAL"), IsChecked = true};
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["NORMAL"], (ToggleMenuFlyoutItem) o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["NORMAL"], menuItemb);

            menuItemb = new ToggleMenuFlyoutItem {Text = Strings.GetString("menu.speed.SLOW")};
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["SLOW"], (ToggleMenuFlyoutItem) o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["SLOW"], menuItemb);

            menuItemb = new ToggleMenuFlyoutItem {Text = Strings.GetString("menu.speed.PAUSED")};
            menuItemb.Click +=
                (o, e) => OnPriorityClicked(Speeds.Speed["PAUSED"], (ToggleMenuFlyoutItem) o);
            SpeedMenu.Items.Add(menuItemb);
            _priorityMenuItems.Add(Speeds.Speed["PAUSED"], menuItemb);
            
            BudgetButton.Content = Strings.GetString("menu.windows.budget");
            EvaluationButton.Content = Strings.GetString("menu.windows.evaluation");
            GraphButton.Content = Strings.GetString("menu.windows.graph");
        }


        /// <summary>
        /// Called when automatic budget button clicked.
        /// </summary>
        private void OnAutoBudgetClicked()
        {
            _isDirty1 = true;
            Engine.ToggleAutoBudget();
        }

        /// <summary>
        /// Called when automatic bulldoze button clicked.
        /// </summary>
        private void OnAutoBulldozeClicked()
        {
            _isDirty1 = true;
            Engine.ToggleAutoBulldoze();
        }

        /// <summary>
        /// Called when disasters button clicked.
        /// </summary>
        private void OnDisastersClicked()
        {
            _isDirty1 = true;
            Engine.ToggleDisasters();
        }

        /// <summary>
        /// Called when sound button clicked.
        /// </summary>
        private void OnSoundClicked()
        {
            _isDoSounds = !_isDoSounds;
            Prefs.PutBoolean(SOUNDS_PREF, _isDoSounds);
            ReloadOptions();
        }

        /// <summary>
        /// Makes the game clean so new game can be played.
        /// </summary>
        public void MakeClean()
        {
            _isDirty1 = false;
            _dirty2 = false;
            _lastSavedTime = DateTime.Now.Millisecond;
            if (CurrentFile != null)
            {
                String fileName = CurrentFile.Name;
                if (fileName.EndsWith("." + EXTENSION))
                {
                    fileName = fileName.Substring(0, fileName.Length - 1 - EXTENSION.Length);
                }
                Title.Text = Strings.GetString("main.caption_named_city") + fileName;
            }
            else
            {
                Title.Text = Strings.GetString("main.caption_unnamed_city");
            }
        }

        /// <summary>
        /// Called when save city button clicked.
        /// </summary>
        /// <returns>true, if game has been saved, otherwise false</returns>
        private async Task<bool> OnSaveCityClicked()
        {
            if (CurrentFile == null)
            {
                return await OnSaveCityAsClicked();
            }

            try
            {
                await Engine.Save(CurrentFile);
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

        /// <summary>
        /// Called when save city as button clicked. Stops the timers, launches file packer for saving and after saving restarts the timers.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> OnSaveCityAsClicked()
        {
            bool timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }
            try
            {
                var picker = new FileSavePicker {DefaultFileExtension = ".cty"};
                picker.FileTypeChoices.Add(".cty Micropolis city", new List<string> {".cty"});
                picker.FileTypeChoices.Add(".cty_file Micropolis city", new List<string> {".cty_file"});
                StorageFile fileToSave = await picker.PickSaveFileAsync();
                if (fileToSave != null)
                {
                    await Engine.Save(fileToSave);
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
        ///     Called when user clicked the load button. Stops the timers, shows file picker, loads the game and restarts the timers.
        /// </summary>
        private async void OnLoadGameClicked()
        {
            // check if user wants to save their current city
            bool saveNeeded = await MaybeSaveCity();

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
            DoNewCity(false);
            //}
        }

        /// <summary>
        ///     Shows a new city dialog.
        /// </summary>
        /// <param name="firstTime">if set to <c>true</c> [first time].</param>
        public void DoNewCity(bool firstTime)
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
        }

        /// <summary>
        ///     Hides the new game dialog panel.
        /// </summary>
        public void HideNewGameDialogPanel()
        {
            NewGameDialogPaneOuter.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     Queries the map at the specified coordinates and shows the results in the notification panel.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        private void DoQueryTool(int xpos, int ypos)
        {
            if (!Engine.TestBounds(xpos, ypos))
                return;

            ZoneStatus z = Engine.QueryZoneStatus(xpos, ypos);
            NotificationPanel.ShowZoneStatus(xpos, ypos, z);
        }

        /// <summary>
        ///     Zooms the map at provided screen coordinates of drawing area.
        /// </summary>
        /// <param name="dir">The direction. +1 is zoom in, -1 is zoom out.</param>
        /// <param name="mousePt">The mouse position to center at.</param>
        private void DoZoom(int dir, Point mousePt)
        {
            int oldZoom = DrawingArea.ViewModel.GetTileSize();
            int newZoom = dir < 0 ? (oldZoom/2) : (oldZoom*2);
            if (newZoom <= 8)
            {
                newZoom = 8;
            }
            else if (newZoom >= 32)
            {
                newZoom = 32;
            }

            if (oldZoom != newZoom)
            {
                // preserve effective mouse position in viewport when changing zoom level
                double zoomFactor = newZoom/(double) oldZoom;
                var pos = new Point(DrawingAreaScroll.HorizontalOffset, DrawingAreaScroll.VerticalOffset);
                var newX = (int) Math.Round(mousePt.X*zoomFactor - (mousePt.X - pos.X));
                var newY = (int) Math.Round(mousePt.Y*zoomFactor - (mousePt.Y - pos.Y));
                DrawingArea.ViewModel.SelectTileSize(newZoom);
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
                new Point(DrawingAreaScroll.HorizontalOffset + DrawingAreaScroll.ViewportWidth/2,
                    DrawingAreaScroll.VerticalOffset + DrawingAreaScroll.ViewportHeight/2));
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
                    (int) ((ev.GetCurrentPoint(target).Position.X + DrawingAreaScroll.HorizontalOffset)/zoomFactor);
                var posY = (int) ((ev.GetCurrentPoint(target).Position.Y + DrawingAreaScroll.VerticalOffset)/zoomFactor);

                CityLocation loc = DrawingArea.ViewModel.GetCityLocation(posX, posY);

                DoQueryTool(loc.X, loc.Y);
                return;
            }

            if (!ev.GetCurrentPoint(target).Properties.IsLeftButtonPressed)
                return;

            if (CurrentTool == null)
                return;

            var posXb = (int) ((ev.GetCurrentPoint(target).Position.X + DrawingAreaScroll.HorizontalOffset)/zoomFactor);
            var posYb = (int) ((ev.GetCurrentPoint(target).Position.Y + DrawingAreaScroll.VerticalOffset)/zoomFactor);

            CityLocation locb = DrawingArea.ViewModel.GetCityLocation(posXb, posYb);
            int x = locb.X;
            int y = locb.Y;

            if (CurrentTool == MicropolisTools.MicropolisTool["QUERY"])
            {
                DoQueryTool(x, y);
                _toolStroke = null;
            }
            else
            {
                _toolStroke = CurrentTool.BeginStroke(Engine, x, y);
                PreviewTool();
            }

            MoveCurrentToolPosTo(x, y);
        }

        /// <summary>
        /// Moves the current tool position to.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        private void MoveCurrentToolPosTo(int x, int y)
        {
            _lastToolUsage.X = x;
            _lastToolUsage.Y = y;

            bool isQueryTool = CurrentTool == MicropolisTools.MicropolisTool["QUERY"];
            if (isQueryTool)
            {
                DoQueryTool(x, y);
                _toolStroke = null;
            }
            else if(CurrentTool != null)
            {
                DrawingArea.ViewModel.PositionToolCursor((x+1)*DrawingArea.ViewModel.TILE_WIDTH, (y+1)*DrawingArea.ViewModel.TILE_HEIGHT);
                _toolStroke = CurrentTool.BeginStroke(Engine, x, y);
                PreviewTool();
            }
        }

        /// <summary>
        ///     Called when the Escape key is pressed to release tool or hide notification panel.
        /// </summary>
        private void OnEscapePressed()
        {
            // if currently dragging a tool...
            if (_toolStroke != null)
            {
                // cancel the current mouse operation
                _toolStroke = null;
                DrawingArea.ViewModel.SetToolPreview(null);
                DrawingArea.ViewModel.SetToolCursor(null);
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
            OnToolUp(ev,true);
        }

        private void OnToolUp(PointerRoutedEventArgs ev, bool resetConfirmationBar)
        {
            if (_toolStroke != null)
            {
                CityLocation loc = _toolStroke.GetLocation();
                ToolResult tr = _toolStroke.Apply();
                ShowToolResult(loc, tr);

                DrawingArea.ViewModel.RepaintNow(true);
                DrawingArea.ViewModel.SetToolPreview(null);
                _toolStroke = null;
            }

            OnToolHover(ev,resetConfirmationBar);

            if (_isAutoBudgetPending)
            {
                _isAutoBudgetPending = false;
                ShowBudgetWindow(true);
            }
        }

        /// <summary>
        ///     Shows a preview of the tool.
        /// </summary>
        private void PreviewTool()
        {
            DrawingArea.ViewModel.SetToolCursor(
                _toolStroke.GetBounds(),
                CurrentTool
                );
            DrawingArea.ViewModel.SetToolPreview(
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

            var posX = (int) ((ev.GetCurrentPoint(target).Position.X/zoomFactor + DrawingAreaScroll.HorizontalOffset));
            var posY = (int) ((ev.GetCurrentPoint(target).Position.Y/zoomFactor + DrawingAreaScroll.VerticalOffset));

            CityLocation loc = DrawingArea.ViewModel.GetCityLocation(posX, posY);

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
            OnToolHover(ev, true);
        }

        private void OnToolHover(PointerRoutedEventArgs ev, bool resetConfirmationBar)
        {
            if (resetConfirmationBar)
            {
                ResetConfirmationBar(!_isMouseDown);
            }

            // if confirmation bar is shown and we hover but do not drag we send true, otherwise when we drag we send false

            if (ev.Pointer.PointerDeviceType != PointerDeviceType.Mouse)
            {
                return;
            }

            double zoomFactor = DrawingAreaScroll.ZoomFactor;
            ScrollViewer target = DrawingAreaScroll;
            if (CurrentTool == null || CurrentTool == MicropolisTools.MicropolisTool["QUERY"])
            {
                DrawingArea.ViewModel.SetToolCursor(null);
                return;
            }

            var posX = (int)((ev.GetCurrentPoint(target).Position.X + DrawingAreaScroll.HorizontalOffset) / zoomFactor);
            var posY = (int)((ev.GetCurrentPoint(target).Position.Y + DrawingAreaScroll.VerticalOffset) / zoomFactor);

            CityLocation loc = DrawingArea.ViewModel.GetCityLocation(posX, posY);
            int x = loc.X;
            int y = loc.Y;
            int w = CurrentTool.GetWidth();
            int h = CurrentTool.GetHeight();

            if (w >= 3)
                x--;
            if (h >= 3)
                y--;

            DrawingArea.ViewModel.SetToolCursor(new CityRect(x, y, w, h), CurrentTool);
        }

        /// <summary>
        ///     Called when the cursor leaves the drawing area
        /// </summary>
        /// <param name="ev">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void OnToolExited(PointerRoutedEventArgs ev)
        {
            DrawingArea.ViewModel.SetToolCursor(null);
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
                    _isDirty1 = true;
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
                c = c.AddYears(1900 + cityTime/48);
                c = c.AddMonths((cityTime%48)/4);
                c = c.AddDays((cityTime%4)*7 + 1);

                return c.ToString("MMM yyyy"); //Strings.getString("citytime")
        }

        /// <summary>
        ///     Updates the user interface to reflect the current date.
        /// </summary>
        private void UpdateDateLabel()
        {
            DateLbl.Text = FormatGameDate(Engine.CityTime);

            PopLbl.Text = Engine.GetCityPopulation().ToString();
        }

        /// <summary>
        ///     Starts the timers.
        /// </summary>
        internal void StartTimer()
        {
            Engine.Micropolis engine = Engine;
            int count = engine.SimSpeed.SimStepsPerUpdate;

            if (engine.SimSpeed == Speeds.Speed["PAUSED"])
                return;

            if (_currentEarthquake != null)
            {
                int interval = 3000/MicropolisDrawingAreaViewModel.SHAKE_STEPS;
                _shakeTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, interval)};
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
            _simTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, engine.SimSpeed.AnimationDelay)};
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


        /// <summary>
        /// Shows the error message.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>empty task to await</returns>
        private async Task ShowErrorMessage(Exception e)
        {
            var d = new MessageDialog(Strings.GetString("main.error_unexpected") + e);
            await d.ShowAsync();
            Debug.WriteLine(Strings.GetString("main.error_unexpected") + e);
        }

        /// <summary>
        ///     Called when an earthquake has stopped.
        /// </summary>
        private void StopEarthquake()
        {
            DrawingArea.ViewModel.Shake(0);
            _currentEarthquake = null;
        }

        /// <summary>
        ///     Stops the simTimer.
        /// </summary>
        internal void StopTimer()
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
        internal bool IsTimerActive()
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
            DrawingArea.ViewModel.StopRendering();
        }

        /// <summary>
        ///     Called when the page had been closed and is opened again in the same instance of the game.
        /// </summary>
        public void OnWindowReopend()
        {
            //DrawingArea.ClearMap();
            if (!IsTimerActive())
            {
                StartTimer();
            }

            DrawingArea.ViewModel.StartRendering();
        }

        /// <summary>
        ///     Called when the user clicked on a difficulty button.
        /// </summary>
        /// <param name="newDifficulty">The new difficulty.</param>
        public void OnDifficultyClicked(int newDifficulty)
        {
            Engine.SetGameLevel(newDifficulty);

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

            Engine.SetSpeed(newSpeed);
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
            _isDirty1 = true;
            switch (disaster)
            {
                case Disaster.FIRE:
                    Engine.MakeFire();
                    break;
                case Disaster.FLOOD:
                    Engine.MakeFlood();
                    break;
                case Disaster.MONSTER:
                    Engine.MakeMonster();
                    break;
                case Disaster.MELTDOWN:
                    if (!Engine.MakeMeltdown())
                    {
                        _messagesPane.AppendCityMessage(MicropolisMessages.NO_NUCLEAR_PLANTS);
                    }
                    break;
                case Disaster.TORNADO:
                    Engine.MakeTornado();
                    break;
                case Disaster.EARTHQUAKE:
                    Engine.MakeEarthquake();
                    break;
                    //assert false; //unknown disaster
            }
        }

        /// <summary>
        ///     Updates the user interface to reflect current funds.
        /// </summary>
        private void ReloadFunds()
        {
            FundsLbl.Text = FormatFunds(Engine.Budget.TotalFunds);
        }


        /// <summary>
        ///     Updates the user interface to reflect current options.
        /// </summary>
        private void ReloadOptions()
        {
            AutoBudgetCheckBox.IsChecked = (Engine.AutoBudget);
            AutoBulldozeCheckBox.IsChecked = (Engine.AutoBulldoze);
            DisastersCheckBox.IsChecked = (!Engine.NoDisasters);
            SoundCheckBox.IsChecked = (_isDoSounds);
            foreach (Speed spd in _priorityMenuItems.Keys)
            {
                _priorityMenuItems[spd].IsChecked = (Engine.SimSpeed == spd);
            }
            for (int i = GameLevel.MIN_LEVEL; i <= GameLevel.MAX_LEVEL; i++)
            {
                _difficultyMenuItems[i].IsChecked = (Engine.GameLevel == i);
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
            _isDirty1 = true;
            ShowBudgetWindow(false);
        }

        /// <summary>
        ///     Called when the user clicked the evaluation button. Shows the evaluation panel.
        /// </summary>
        private void OnViewEvaluationClicked()
        {
            EvaluationPane.Visibility = EvaluationPane.Visibility == Visibility.Collapsed
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <summary>
        ///     Called when the user clicked the graph button. Shows the graph panel.
        /// </summary>
        private void OnViewGraphClicked()
        {
            GraphsPane.Visibility = GraphsPane.Visibility == Visibility.Collapsed
                ? Visibility.Visible
                : Visibility.Collapsed;
            GraphsPane.Repaint();
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
                _isAutoBudgetPending = true;
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

            ShowBudgetDialog(timerEnabled);
        }

        /// <summary>
        ///     Shows the budget dialog.
        /// </summary>
        private void ShowBudgetDialog(bool EnableTimerWhenClosing)
        {
            NewBudgetDialog.ViewModel.SetEngine(Engine);
            NewBudgetDialogPaneOuter.Visibility = Visibility.Visible;
            NewBudgetDialog.ViewModel.EnableTimerWhenClosing = EnableTimerWhenClosing;
        }

        /// <summary>
        ///     Hides the budget dialog.
        /// </summary>
        public void HideBudgetDialog()
        {
            NewBudgetDialogPaneOuter.Visibility = Visibility.Collapsed;
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
            var menuItem = new ToggleMenuFlyoutItem {Text = caption};
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
                IStorageItem file =
                    await
                        folder.TryGetItemAsync(
                            iconUrl.AbsoluteUri.Substring(
                                iconUrl.AbsoluteUri.LastIndexOf("/", StringComparison.Ordinal) + 1));

                if (file != null)
                {
                    ImageSource iSource = new BitmapImage(iconUrl);

                    MapLegendLbl.Source = iSource;
                    MapLegendLbl.Opacity = 1;
                    return;
                }
            }
            MapLegendLbl.Opacity = 0;
        }

        /// <summary>
        ///     Called when someone clicks the about button and displays the about message
        /// </summary>
        private void OnAboutClicked()
        {
            String version = "0.5";
            String versionStr = Strings.GetString("main.about_caption") + ": " +
                                Strings.GetString("main.version_string") + " " + version;

            var d = new MessageDialog(Strings.GetString("main.about_text") + versionStr);
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
            EvaluationPane.Visibility = Visibility.Collapsed;
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
                        await Engine.Save(fileToSave);
                        //makeClean();
                    }
                }
                catch (Exception e)
                {
                    var d = new MessageDialog(Strings.GetString("main.error_caption") + ": " + e);
                }
            }
        }

        /// <summary>
        /// Gets the zoom factor.
        /// </summary>
        /// <returns></returns>
        internal float GetZoomFactor()
        {
            return DrawingAreaScroll.ZoomFactor;
        }

        public double HorizontalMapOffset { get { return DrawingAreaScroll.HorizontalOffset; }}
        public double VerticalMapOffset { get { return DrawingAreaScroll.VerticalOffset; } }

        public double ZoomFactor { get { return DrawingAreaScroll.ZoomFactor; } }

        public double MapWidth { get { return DrawingArea.ActualWidth; } }

        public double MapHeight { get { return DrawingArea.ActualHeight; } }

        
        /// <summary>
        /// http://marcominerva.wordpress.com/2013/10/16/handling-visualstate-in-windows-8-1-store-apps/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cleanup(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Window_SizeChanged;
        }

        /// <summary>
        /// http://marcominerva.wordpress.com/2013/10/16/handling-visualstate-in-windows-8-1-store-apps/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            DetermineVisualState();
        }

        string _state = string.Empty;

        /// <summary>
        /// http://marcominerva.wordpress.com/2013/10/16/handling-visualstate-in-windows-8-1-store-apps/
        /// </summary>
        private void DetermineVisualState()
        {
            
            var applicationView = ApplicationView.GetForCurrentView();
            var size = Window.Current.Bounds;

            if (applicationView.IsFullScreen)
            {
                if (applicationView.Orientation == ApplicationViewOrientation.Landscape)
                    _state = "FullScreenLandscape";
                else
                    _state = "FullScreenPortrait";
            }
            else
            {
                if (size.Width == 320)
                    _state = "Snapped";
                else if (size.Width <= 500)
                    _state = "Narrow";
                else
                    _state = "Filled";
            }

            VisualStateManager.GoToState(this, _state, true);
            OnVisualStateChanged();
        }

        private void OnVisualStateChanged()
        {
            if (VisualStateChanged != null)
            {
                VisualStateChanged(this,new EventArgs());
            }
        }

        public event EventHandler VisualStateChanged;
    }
}