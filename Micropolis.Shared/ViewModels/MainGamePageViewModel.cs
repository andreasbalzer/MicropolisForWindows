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
using System.Collections.ObjectModel;
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
#if WINDOWS_PHONE_APP
#else
using Windows.UI.ApplicationSettings;
#endif
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Engine;
using Micropolis.Common;
using Micropolis.Model.Entities;

namespace Micropolis.ViewModels
{
    public class MainGamePageViewModel : BindableBase, Engine.IListener, IEarthquakeListener
    {
        /// <summary>
        ///     The name of sound preferences whether to play sounds or not (does not include music).
        /// </summary>
        private static readonly string SOUNDS_PREF = "enable_sounds";

        /// <summary>
        ///     The file extension for Micropolis files.
        /// </summary>
        private static readonly string EXTENSION = "cty";

        private bool _autoBudgetCheckBoxIsChecked;
        private string _autoBudgetCheckBoxText;
        private bool _autoBulldozeCheckBoxIsChecked;
        private string _autoBulldozeCheckBoxText;
        private string _budgetButtonText;
        private bool _confirmBarIsVisible;

        /// <summary>
        ///     The current earthquake occuring.
        /// </summary>
        private EarthquakeStepper _currentEarthquake;

        private string _currentToolCostLblTextBlockText;
        private string _currentToolLblTextBlockText;
        private string _dateLblTextBlockText;
        private string _difficultyButtonContentText;

        /// <summary>
        ///     The difficulty menu items linking difficulty levels to their respective ToggleMenuFlyoutItems.
        /// </summary>
        private Dictionary<int, LevelButtonViewModel> _difficultyMenuItems;

        /// <summary>
        ///     Indicates if simulator took a step since last save.
        /// </summary>
        private bool _dirty2;

        private string _disasterButtonText;
        private bool _disastersCheckBoxIsChecked;
        private string _disastersCheckBoxText;
        private double _drawingAreaActualHeight;
        private double _drawingAreaActualWidth;
        private double _drawingAreaScrollHorizontalOffset;
        private double _drawingAreaScrollVerticalOffset;
        private double _drawingAreaScrollViewportHeight;
        private double _drawingAreaScrollViewportWidth;
        private float _drawingAreaScrollZoomFactor;
        private string _earthquakeButtonText;
        private string _evaluationButtonText;
        private bool _evaluationPaneIsVisible;
        private string _fireButtonText;
        private bool _firstRun = true;
        private string _floodButtonText;
        private string _fundsLblTextBlockText;
        private string _graphButtonText;
        private bool _graphsPaneIsVisible;
        private string _hideMapButtonText;
        private string _hideMessagesButtonText;
        private string _hideToolsButtonText;

        /// <summary>
        ///     Indicates if auto budget is pending.
        /// </summary>
        private bool _isAutoBudgetPending;

        /// <summary>
        ///     Indicates if a tool was successfully applied since last save.
        /// </summary>
        private bool _isDirty1;

        /// <summary>
        ///     Indicates if sounds should be played or not.
        /// </summary>
        private bool _isDoSounds = true;

        private bool _isMessagesVisible;
        private bool _isMiniMapVisible;

        /// <summary>
        ///     Indicates if the left mouse button is down (for dragging).
        /// </summary>
        private bool _isMouseDown;

        private bool _isSpeedFast;
        private bool _isSpeedNormal;
        private bool _isSpeedPause;
        private bool _isSpeedSlow;
        private bool _isSpeedSuperFast;
        private bool _isToolsVisible;

        /// <summary>
        ///     Real-time clock of when file was last saved
        /// </summary>
        private long _lastSavedTime;

        /// <summary>
        ///     Point where the tool was last applied during the current drag.
        /// </summary>
        private Point _lastToolUsage;

        private string _loadButtonText;
        private ImageSource _mapLegendLblImageSource;
        private double _mapLegendLblOpacity;
        private string _meltdownButtonText;
        private string _menuOverlaysHeaderButtonText;
        private string _menuZonesHeaderButtonText;

        /// <summary>
        ///     The messages pane used to display game messages.
        /// </summary>
        private MessagesPane _messagesPane;

        private string _monsterButtonText;
        private bool _newBudgetDialogPaneOuterIsVisible;
        private string _newButtonText;
        private bool _newGameDialogPaneOuterIsVisible;
        private bool _notificationPanelIsVisible;
        private string _popLblTextBlockText;
        private string _saveAsButtonText;
        private string _saveButtonText;
        private string _settingsButtonContentText;

        /// <summary>
        ///     The shake timer shakes the map during an earthquake.
        /// </summary>
        private DispatcherTimer _shakeTimer;

        private string _showMapButtonText;
        private string _showMessagesButtonText;
        private string _showToolsButtonText;

        /// <summary>
        ///     The sim timer progresses the game.
        /// </summary>
        private DispatcherTimer _simTimer;

        private bool _soundCheckBoxIsChecked;
        private string _soundCheckBoxText;
        private string _speedButtonText;
        private string _titleTextBlockText;

        /// <summary>
        ///     The tool stroke used when a tool is being pressed
        /// </summary>
        private ToolStroke _toolStroke;

        private string _tornadoButtonText;

        /// <summary>
        ///     The current file loaded in the game.
        /// </summary>
        public StorageFile CurrentFile;

        /// <summary>
        ///     The current tool selected.
        /// </summary>
        public MicropolisTool CurrentTool;

        /// <summary>
        ///     The touch pointer routed event arguments of current confirmation pending when user touched screen and confirmation
        ///     bar awaits user action.
        /// </summary>
        private PointerRoutedEventArgs touchPointerRoutedEventArgsOfCurrentConfirmationPending;

        private readonly ConfirmationBar _confirmBar;
        private readonly DemandIndicatorViewModel _demandIndViewModel;
        private readonly MicropolisDrawingArea _drawingArea;
        private readonly ScrollViewer _drawingAreaScroll;
        private readonly MicropolisDrawingAreaViewModel _drawingAreaViewModel;
        private readonly EvaluationPaneViewModel _evaluationPaneViewModel;
        private readonly GraphsPaneViewModel _graphsPaneViewModel;

        /// <summary>
        ///     The map state menu items contains map states linking to their respective ToggleMenuFlyoutItems.
        /// </summary>
        private readonly Dictionary<MapState, LevelButtonViewModel> _mapStateMenuItems =
            new Dictionary<MapState, LevelButtonViewModel>();

        private readonly OverlayMapViewModel _mapViewViewModel;
        private readonly ScrollViewer _messagesScrollViewer;
        private readonly BudgetDialogViewModel _newBudgetDialogViewModel;
        private readonly NewCityDialogViewModel _newCityDialogViewModel;
        private readonly NotificationPaneViewModel _notificationPanelViewModel;
        private readonly MediaElement _soundOutput;
        private readonly MenuFlyout _speedMenu;
        private readonly ToolbarViewModel _toolsPanelViewModel;

        public MainGamePageViewModel(NotificationPaneViewModel notificationPanelViewModel,
            MicropolisDrawingAreaViewModel drawingAreaViewModel,
            ToolbarViewModel toolsPanelViewModel, MicropolisDrawingArea drawingArea, ConfirmationBar confirmBar,
            BudgetDialogViewModel newBudgetDialogViewModel, GraphsPaneViewModel graphsPaneViewModel,
            EvaluationPaneViewModel evaluationPaneViewModel, ScrollViewer drawingAreaScroll,
            ScrollViewer messagesScrollViewer, DemandIndicatorViewModel demandIndViewModel,
            OverlayMapViewModel mapViewViewModel, NewCityDialogViewModel newCityDialogViewModel)
        {
            _newCityDialogViewModel = newCityDialogViewModel;
            _newCityDialogViewModel.MainPageViewModel = this;
            _mapViewViewModel = mapViewViewModel;

            ShowToolsButtonText = Strings.GetString("main.showTools");
            HideToolsButtonText = Strings.GetString("main.hideTools");

            ShowMessagesButtonText = Strings.GetString("main.showMessages");
            HideMessagesButtonText = Strings.GetString("main.hideMessages");

            ShowMapButtonText = Strings.GetString("main.showMap");
            HideMapButtonText = Strings.GetString("main.hideMap");

            MenuOverlaysHeaderButtonText = Strings.GetString("menu.overlays");
            MenuZonesHeaderFlyoutItems = new ObservableCollection<LevelButtonViewModel>();
            MenuOverlaysHeaderFlyoutItems = new ObservableCollection<LevelButtonViewModel>();
            MenuZonesHeaderButtonText = Strings.GetString("menu.zones");
            Levels = new ObservableCollection<LevelButtonViewModel>();
            BudgetCommand = new DelegateCommand(BudgetButton_Click);
            EvaluationCommand = new DelegateCommand(EvaluationButton_Click);
            GraphCommand = new DelegateCommand(GraphButton_Click);
            ToggleToolsCommand = new DelegateCommand(ToggleToolsButton_Click);

            MonsterCommand = new DelegateCommand(MonsterButton_Click);
            FireCommand = new DelegateCommand(FireButton_Click);
            FloodCommand = new DelegateCommand(FloodButton_Click);
            MeltdownCommand = new DelegateCommand(MeltdownButton_Click);
            TornadoCommand = new DelegateCommand(TornadoButton_Click);
            EarthquakeCommand = new DelegateCommand(EarthquakeButton_Click);

            AutoBudgetCommand = new DelegateCommand(AutoBudgetMenuItem_Click);
            AutoBulldozeCommand = new DelegateCommand(AutoBulldozeCheckBox_Click);
            DisastersCommand = new DelegateCommand(DisastersCheckBox_Click);
            SoundCommand = new DelegateCommand(SoundCheckBox_Click);

            NewCommand = new DelegateCommand(NewButton_Click);
            LoadCommand = new DelegateCommand(LoadButton_Click);
            SaveCommand = new DelegateCommand(SaveButton_Click);
            SaveAsCommand = new DelegateCommand(SaveAsButton_Click);


            _demandIndViewModel = demandIndViewModel;
            _messagesScrollViewer = messagesScrollViewer;
            _drawingAreaScroll = drawingAreaScroll;
            _newBudgetDialogViewModel = newBudgetDialogViewModel;

            _graphsPaneViewModel = graphsPaneViewModel;
            _evaluationPaneViewModel = evaluationPaneViewModel;
            _confirmBar = confirmBar;
            _drawingArea = drawingArea;
            _toolsPanelViewModel = toolsPanelViewModel;
            _notificationPanelViewModel = notificationPanelViewModel;
            _drawingAreaViewModel = drawingAreaViewModel;
            ToggleMiniMapCommand = new DelegateCommand(ToggleMiniMap);
            ToggleMessagesCommand = new DelegateCommand(ToggleMessages);
            SpeedPauseCommand = new DelegateCommand(SpeedPause);
            SpeedSlowCommand = new DelegateCommand(SpeedSlow);
            SpeedNormalCommand = new DelegateCommand(SpeedNormal);
            SpeedFastCommand = new DelegateCommand(SpeedFast);
            SpeedSuperFastCommand = new DelegateCommand(SpeedSuperFast);
        }

        public DelegateCommand AutoBudgetCommand { get; set; }
        public DelegateCommand AutoBulldozeCommand { get; set; }
        public DelegateCommand BudgetCommand { get; set; }
        public DelegateCommand DisastersCommand { get; set; }
        public DelegateCommand EarthquakeCommand { get; set; }
        public DelegateCommand EvaluationCommand { get; set; }
        public DelegateCommand FireCommand { get; set; }
        public DelegateCommand FloodCommand { get; set; }
        public DelegateCommand GraphCommand { get; set; }
        public DelegateCommand LoadCommand { get; set; }
        public DelegateCommand MeltdownCommand { get; set; }
        public DelegateCommand MonsterCommand { get; set; }
        public DelegateCommand NewCommand { get; set; }
        public DelegateCommand SaveAsCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand SoundCommand { get; set; }
        public DelegateCommand TornadoCommand { get; set; }
        public DelegateCommand ToggleToolsCommand { get; set; }

        public bool IsMessagesVisible
        {
            get { return _isMessagesVisible; }
            set { SetProperty(ref _isMessagesVisible, value); }
        }

        public double DrawingAreaScrollHorizontalOffset
        {
            get { return _drawingAreaScrollHorizontalOffset; }
            set { SetProperty(ref _drawingAreaScrollHorizontalOffset, value); }
        }

        public double DrawingAreaScrollVerticalOffset
        {
            get { return _drawingAreaScrollVerticalOffset; }
            set { SetProperty(ref _drawingAreaScrollVerticalOffset, value); }
        }

        public double DrawingAreaScrollViewportWidth
        {
            get { return _drawingAreaScrollViewportWidth; }
            set { SetProperty(ref _drawingAreaScrollViewportWidth, value); }
        }

        public string ShowMapButtonText
        {
            get { return _showMapButtonText; }
            set { SetProperty(ref _showMapButtonText, value); }
        }

        public string ShowToolsButtonText
        {
            get { return _showToolsButtonText; }
            set { SetProperty(ref _showToolsButtonText, value); }
        }

        public string ShowMessagesButtonText
        {
            get { return _showMessagesButtonText; }
            set { SetProperty(ref _showMessagesButtonText, value); }
        }

        public string HideMapButtonText
        {
            get { return _hideMapButtonText; }
            set { SetProperty(ref _hideMapButtonText, value); }
        }

        public string HideToolsButtonText
        {
            get { return _hideToolsButtonText; }
            set { SetProperty(ref _hideToolsButtonText, value); }
        }

        public string HideMessagesButtonText
        {
            get { return _hideMessagesButtonText; }
            set { SetProperty(ref _hideMessagesButtonText, value); }
        }

        public double DrawingAreaScrollViewportHeight
        {
            get { return _drawingAreaScrollViewportHeight; }
            set { SetProperty(ref _drawingAreaScrollViewportHeight, value); }
        }

        public bool IsMiniMapVisible
        {
            get { return _isMiniMapVisible; }

            set { SetProperty(ref _isMiniMapVisible, value); }
        }

        public bool IsSpeedPause
        {
            get { return _isSpeedPause; }
            set { SetProperty(ref _isSpeedPause, value); }
        }

        public bool IsSpeedSlow
        {
            get { return _isSpeedSlow; }
            set { SetProperty(ref _isSpeedSlow, value); }
        }

        public bool IsSpeedNormal
        {
            get { return _isSpeedNormal; }
            set { SetProperty(ref _isSpeedNormal, value); }
        }

        public bool IsSpeedFast
        {
            get { return _isSpeedFast; }
            set { SetProperty(ref _isSpeedFast, value); }
        }

        public bool IsSpeedSuperFast
        {
            get { return _isSpeedSuperFast; }
            set { SetProperty(ref _isSpeedSuperFast, value); }
        }

        public DelegateCommand ToggleMiniMapCommand { get; private set; }
        public DelegateCommand ToggleMessagesCommand { get; private set; }
        public DelegateCommand SpeedPauseCommand { get; private set; }
        public DelegateCommand SpeedSlowCommand { get; private set; }
        public DelegateCommand SpeedNormalCommand { get; private set; }
        public DelegateCommand SpeedFastCommand { get; private set; }
        public DelegateCommand SpeedSuperFastCommand { get; private set; }

        /// <summary>
        ///     Reference to the current game engine.
        /// </summary>
        public Engine.Micropolis Engine { get; private set; }

        public string CurrentToolLblTextBlockText
        {
            get { return _currentToolLblTextBlockText; }
            set { SetProperty(ref _currentToolLblTextBlockText, value); }
        }

        public string CurrentToolCostLblTextBlockText
        {
            get { return _currentToolCostLblTextBlockText; }
            set { SetProperty(ref _currentToolCostLblTextBlockText, value); }
        }

        public bool GraphsPaneIsVisible
        {
            get { return _graphsPaneIsVisible; }
            set { SetProperty(ref _graphsPaneIsVisible, value); }
        }

        public bool ConfirmBarIsVisible
        {
            get { return _confirmBarIsVisible; }
            set { SetProperty(ref _confirmBarIsVisible, value); }
        }

        public string NewButtonText
        {
            get { return _newButtonText; }

            set { SetProperty(ref _newButtonText, value); }
        }

        public string LoadButtonText
        {
            get { return _loadButtonText; }

            set { SetProperty(ref _loadButtonText, value); }
        }

        public string SaveButtonText
        {
            get { return _saveButtonText; }

            set { SetProperty(ref _saveButtonText, value); }
        }

        public string SaveAsButtonText
        {
            get { return _saveAsButtonText; }

            set { SetProperty(ref _saveAsButtonText, value); }
        }

        public string SettingsButtonContentText
        {
            get { return _settingsButtonContentText; }

            set { SetProperty(ref _settingsButtonContentText, value); }
        }

        public string DifficultyButtonContentText
        {
            get { return _difficultyButtonContentText; }

            set { SetProperty(ref _difficultyButtonContentText, value); }
        }

        public string AutoBudgetCheckBoxText
        {
            get { return _autoBudgetCheckBoxText; }

            set { SetProperty(ref _autoBudgetCheckBoxText, value); }
        }

        public string AutoBulldozeCheckBoxText
        {
            get { return _autoBulldozeCheckBoxText; }

            set { SetProperty(ref _autoBulldozeCheckBoxText, value); }
        }

        public string DisastersCheckBoxText
        {
            get { return _disastersCheckBoxText; }

            set { SetProperty(ref _disastersCheckBoxText, value); }
        }

        public string SoundCheckBoxText
        {
            get { return _soundCheckBoxText; }

            set { SetProperty(ref _soundCheckBoxText, value); }
        }

        public string DisasterButtonText
        {
            get { return _disasterButtonText; }

            set { SetProperty(ref _disasterButtonText, value); }
        }

        public string MonsterButtonText
        {
            get { return _monsterButtonText; }

            set { SetProperty(ref _monsterButtonText, value); }
        }

        public string FireButtonText
        {
            get { return _fireButtonText; }

            set { SetProperty(ref _fireButtonText, value); }
        }

        public string FloodButtonText
        {
            get { return _floodButtonText; }

            set { SetProperty(ref _floodButtonText, value); }
        }

        public string MeltdownButtonText
        {
            get { return _meltdownButtonText; }

            set { SetProperty(ref _meltdownButtonText, value); }
        }

        public string TornadoButtonText
        {
            get { return _tornadoButtonText; }

            set { SetProperty(ref _tornadoButtonText, value); }
        }

        public string EarthquakeButtonText
        {
            get { return _earthquakeButtonText; }

            set { SetProperty(ref _earthquakeButtonText, value); }
        }

        public string SpeedButtonText
        {
            get { return _speedButtonText; }

            set { SetProperty(ref _speedButtonText, value); }
        }

        public string BudgetButtonText
        {
            get { return _budgetButtonText; }
            set { SetProperty(ref _budgetButtonText, value); }
        }

        public string EvaluationButtonText
        {
            get { return _evaluationButtonText; }
            set { SetProperty(ref _evaluationButtonText, value); }
        }

        public string GraphButtonText
        {
            get { return _graphButtonText; }
            set { SetProperty(ref _graphButtonText, value); }
        }

        public string TitleTextBlockText
        {
            get { return _titleTextBlockText; }
            set { SetProperty(ref _titleTextBlockText, value); }
        }

        public bool NewGameDialogPaneOuterIsVisible
        {
            get { return _newGameDialogPaneOuterIsVisible; }
            set { SetProperty(ref _newGameDialogPaneOuterIsVisible, value); }
        }

        public string PopLblTextBlockText
        {
            get { return _popLblTextBlockText; }
            set { SetProperty(ref _popLblTextBlockText, value); }
        }

        public string DateLblTextBlockText
        {
            get { return _dateLblTextBlockText; }
            set { SetProperty(ref _dateLblTextBlockText, value); }
        }

        public string FundsLblTextBlockText
        {
            get { return _fundsLblTextBlockText; }
            set { SetProperty(ref _fundsLblTextBlockText, value); }
        }

        public bool AutoBudgetCheckBoxIsChecked
        {
            get { return _autoBudgetCheckBoxIsChecked; }
            set { SetProperty(ref _autoBudgetCheckBoxIsChecked, value); }
        }

        public bool AutoBulldozeCheckBoxIsChecked
        {
            get { return _autoBulldozeCheckBoxIsChecked; }
            set { SetProperty(ref _autoBulldozeCheckBoxIsChecked, value); }
        }

        public bool DisastersCheckBoxIsChecked
        {
            get { return _disastersCheckBoxIsChecked; }
            set { SetProperty(ref _disastersCheckBoxIsChecked, value); }
        }

        public bool SoundCheckBoxIsChecked
        {
            get { return _soundCheckBoxIsChecked; }
            set { SetProperty(ref _soundCheckBoxIsChecked, value); }
        }

        public bool EvaluationPaneIsVisible
        {
            get { return _evaluationPaneIsVisible; }
            set { SetProperty(ref _evaluationPaneIsVisible, value); }
        }

        public bool NewBudgetDialogPaneOuterIsVisible
        {
            get { return _newBudgetDialogPaneOuterIsVisible; }
            set { SetProperty(ref _newBudgetDialogPaneOuterIsVisible, value); }
        }

        public ImageSource MapLegendLblImageSource
        {
            get { return _mapLegendLblImageSource; }
            set { SetProperty(ref _mapLegendLblImageSource, value); }
        }

        public double MapLegendLblOpacity
        {
            get { return _mapLegendLblOpacity; }
            set { SetProperty(ref _mapLegendLblOpacity, value); }
        }

        public bool NotificationPanelIsVisible
        {
            get { return _notificationPanelIsVisible; }
            set { SetProperty(ref _notificationPanelIsVisible, value); }
        }

        public float DrawingAreaScrollZoomFactor
        {
            get { return _drawingAreaScrollZoomFactor; }
            set { SetProperty(ref _drawingAreaScrollZoomFactor, value); }
        }

        public double DrawingAreaActualWidth
        {
            get { return _drawingAreaActualWidth; }
            set { SetProperty(ref _drawingAreaActualWidth, value); }
        }

        public double DrawingAreaActualHeight
        {
            get { return _drawingAreaActualHeight; }
            set { SetProperty(ref _drawingAreaActualHeight, value); }
        }

        public double HorizontalMapOffset
        {
            get { return DrawingAreaScrollHorizontalOffset; }
        }

        public double VerticalMapOffset
        {
            get { return DrawingAreaScrollVerticalOffset; }
        }

        public double ZoomFactor
        {
            get { return DrawingAreaScrollZoomFactor; }
        }

        public double MapWidth
        {
            get { return DrawingAreaActualWidth; }
        }

        public double MapHeight
        {
            get { return DrawingAreaActualHeight; }
        }

        public ObservableCollection<LevelButtonViewModel> MenuZonesHeaderFlyoutItems { get; set; }
        public ObservableCollection<LevelButtonViewModel> MenuOverlaysHeaderFlyoutItems { get; set; }

        public string MenuZonesHeaderButtonText
        {
            get { return _menuZonesHeaderButtonText; }
            set { SetProperty(ref _menuZonesHeaderButtonText, value); }
        }

        public string MenuOverlaysHeaderButtonText
        {
            get { return _menuOverlaysHeaderButtonText; }
            set { SetProperty(ref _menuOverlaysHeaderButtonText, value); }
        }

        public ObservableCollection<LevelButtonViewModel> Levels { get; set; }

        /// <summary>
        ///     Called when an earthquake has started. Pauses game simulation, does one earthquake step and restarts simulation.
        /// </summary>
        public void EarthquakeStarted()
        {
            if (IsTimerActive())
            {
                StopTimer();
            }

            _currentEarthquake = new EarthquakeStepper(_drawingAreaViewModel);
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
            _messagesPane.ViewModel.AppendCityMessage(m);

            if (m.UseNotificationPane && p != null)
            {
                _notificationPanelViewModel.ShowMessage(m, p.X, p.Y);
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

            var afile = sound.GetAudioFile();
            if (afile == null)
                return;

            var isOnScreen =
                RectangleContainsPoint(
                    new Rect(DrawingAreaScrollHorizontalOffset, DrawingAreaScrollVerticalOffset,
                        DrawingAreaScrollViewportWidth, DrawingAreaScrollViewportHeight),
                    _drawingAreaViewModel.GetTileBoundsAsRect(loc.X, loc.Y));


            if (sound == Sounds.Sound["HONKHONK_LOW"] && !isOnScreen)
                return;

            try
            {
                OnPlaySound(afile);
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

        private void ToggleToolsButton_Click()
        {
            _toolsPanelViewModel.ToolBarIsVisible = !_toolsPanelViewModel.ToolBarIsVisible;
        }

        public event EventHandler<Uri> PlaySound;

        private void OnPlaySound(Uri file)
        {
            if (PlaySound != null)
            {
                PlaySound(this, file);
            }
        }

        private void ToggleMiniMap()
        {
            IsMiniMapVisible = !IsMiniMapVisible;
        }

        private void ToggleMessages()
        {
            IsMessagesVisible = !IsMessagesVisible;
        }

        private void SpeedPause()
        {
            IsSpeedSlow = false;
            IsSpeedNormal = false;
            IsSpeedFast = false;
            IsSpeedSuperFast = false;
            IsSpeedPause = true;
            SetSpeed(Speeds.Speed["PAUSED"]);
        }

        private void SpeedSlow()
        {
            IsSpeedPause = false;
            IsSpeedNormal = false;
            IsSpeedFast = false;
            IsSpeedSuperFast = false;
            IsSpeedSlow = true;
            SetSpeed(Speeds.Speed["SLOW"]);
        }

        private void SpeedNormal()
        {
            IsSpeedPause = false;
            IsSpeedSlow = false;
            IsSpeedFast = false;
            IsSpeedSuperFast = false;
            IsSpeedNormal = true;
            SetSpeed(Speeds.Speed["NORMAL"]);
        }

        private void SpeedFast()
        {
            IsSpeedPause = false;
            IsSpeedSlow = false;
            IsSpeedNormal = false;
            IsSpeedSuperFast = false;
            IsSpeedFast = true;
            SetSpeed(Speeds.Speed["FAST"]);
        }

        private void SpeedSuperFast()
        {
            IsSpeedPause = false;
            IsSpeedSlow = false;
            IsSpeedNormal = false;
            IsSpeedFast = false;
            IsSpeedSuperFast = true;
            SetSpeed(Speeds.Speed["SUPER_FAST"]);
        }

        /// <summary>
        ///     Selects the tool specified.
        /// </summary>
        /// <param name="newTool">The new tool.</param>
        public void SelectTool(MicropolisTool newTool)
        {
            CurrentTool = newTool;

            CurrentToolLblTextBlockText =
                Strings.ContainsKey("tool." + CurrentTool.Name + ".name")
                    ? Strings.GetString("tool." + CurrentTool.Name + ".name")
                    : CurrentTool.Name;

            var cost = CurrentTool.GetToolCost();
            CurrentToolCostLblTextBlockText = cost != 0 ? FormatFunds(cost) : " ";

            if (newTool == MicropolisTools.MicropolisTool["EMPTY"])
            {
                OnEscapePressed();
            }
        }

        /// <summary>
        ///     Gets the landscape from drawing area for specified coordinates and viewport size.
        /// </summary>
        /// <param name="xpos">The xpos top left.</param>
        /// <param name="ypos">The ypos top left.</param>
        /// <param name="viewportSize">Size of the viewport.</param>
        /// <returns></returns>
        public WriteableBitmap GetLandscapeFromDrawingArea(int xpos, int ypos, Size viewportSize)
        {
            return _drawingAreaViewModel.GetLandscape(xpos, ypos, viewportSize);
        }

        /// <summary>
        ///     Hides the graphs pane.
        /// </summary>
        public void HideGraphsPane()
        {
            GraphsPaneIsVisible = false;
        }

        /// <summary>
        ///     Handles the Loaded event of the MainPage control, initializtes the settings charm, game and processes app commands.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        internal void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_firstRun)
            {
#if WINDOWS_PHONE_APP
                //Bug: Add settings
#else
                // Register handler for CommandsRequested events from the settings pane
                SettingsPane.GetForCurrentView().CommandsRequested += SettingsCharm.OnCommandsInGameRequested;
#endif
                var engineExists = Engine != null;
                // If we loaded another game via app commands, we already have an engine.
                if (!engineExists)
                {
                    Engine = new Engine.Micropolis();
                }
                MainGamePageInit(Engine);
            }
            ProcessAppCommands();

            _firstRun = false;


            //TODO: Map-Creation als Teil des Hauptmenüs!
        }

        public void MainGamePage_VisualStateChanged(object sender, VisualStateEventInformation e)
        {
            _toolsPanelViewModel.Mode = (e.State == "Snapped" || e.State == "Narrow")
                ? ToolBarMode.FLYOUT
                : ToolBarMode.NORMAL;
        }

        /// <summary>
        ///     Processes the application commands or shows the new game dialog.
        /// </summary>
        private void ProcessAppCommands()
        {
            var currentApp = (ISupportsAppCommands) Application.Current;
            var loadCityCommand =
                currentApp.AppCommands.FirstOrDefault(s => s.Instruction == AppCommands.LOADFILE);

            var loadCity = loadCityCommand != null; // loadCityCommand present?

            var loadCityAsNewCommand =
                currentApp.AppCommands.FirstOrDefault(s => s.Instruction == AppCommands.LOADFILEASNEWCITY);

            var loadCityAsNew = loadCityAsNewCommand != null; // loadCityCommand present?

            if (loadCity)
            {
                var file = (StorageFile) loadCityCommand.File;
                currentApp.AppCommands.Remove(loadCityCommand);

                LoadGameFile(file);
            }
            else if (loadCityAsNew)
            {
                var file = (StorageFile) loadCityAsNewCommand.File;
                currentApp.AppCommands.Remove(loadCityAsNewCommand);

                LoadGameFile(file, false);
            }
            else if (_firstRun)
            {
                OnNewCityClicked();
            }
        }

        /// <summary>
        ///     Loads the game file.
        /// </summary>
        /// <param name="file">The file to be loaded.</param>
        private void LoadGameFile(StorageFile file, bool useFileForSave = true)
        {
            if (file != null)
            {
                var newEngine = new Engine.Micropolis();
                newEngine.Load(file).ContinueWith(a =>
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
        ///     Handles the KeyDown event of the MainPage control for zooming or canceling an action.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs" /> instance containing the event data.</param>
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
        ///     Handles the PointerPressed event of the DrawingArea control for when a tool is started to be used (e.g. for
        ///     dragging).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
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
        ///     Handles the PointerMoved event of the DrawingArea control for when a tool is dragged or hovered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
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
        ///     Handles the PointerReleased event of the DrawingArea control for when a tool has been used.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
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
        ///     Handles the PointerRoutedEventArgs for when the finger or pen gets raised and the tool should show a confirmation
        ///     bar.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void onToolTouch(PointerRoutedEventArgs e)
        {
            ConfirmBarIsVisible = true;
        }

        /// <summary>
        ///     Handles the PointerExited event of the DrawingArea control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
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
        ///     Handles the PointerWheelChanged event of the DrawingArea control for zooming.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void DrawingArea_PointerWheelChanged(object sender, PointerRoutedEventArgs args)
        {
            var delta = args.GetCurrentPoint(_drawingArea).Properties.MouseWheelDelta;

            try
            {
                var posX = (int) (args.GetCurrentPoint(_drawingArea).Position.X + DrawingAreaScrollHorizontalOffset);
                var posY = (int) (args.GetCurrentPoint(_drawingArea).Position.Y + DrawingAreaScrollVerticalOffset);

                var loc = _drawingAreaViewModel.GetCityLocation(posX, posY);
                onMouseWheelMoved(delta, new Point(posX, posY));
            }
            catch (Exception e)
            {
                ShowErrorMessage(e);
            }
        }

        /// <summary>
        ///     Initializes the MainGamePage.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public void MainGamePageInit(Engine.Micropolis engine)
        {
            Engine = engine;

            _confirmBar.Confirmed += ConfirmBar_Confirmed;
            _confirmBar.Declined += ConfirmBar_Declined;
            _confirmBar.Uped += ConfirmBar_Uped;
            _confirmBar.Downed += ConfirmBar_Downed;
            _confirmBar.Lefted += ConfirmBar_Lefted;
            _confirmBar.Righted += ConfirmBar_Righted;

            _drawingAreaViewModel.SetUpAfterBasicInit(engine, this);

            _newBudgetDialogViewModel.SetupAfterBasicInit(this, engine);

            MakeMenu();

            _graphsPaneViewModel.SetUpAfterBasicInit(engine, this);

            _evaluationPaneViewModel.SetupAfterBasicInit(this, engine);

            MenuZonesHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.zones.ALL", MapState.ALL));
            MenuZonesHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.zones.RESIDENTIAL", MapState.RESIDENTIAL));
            MenuZonesHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.zones.COMMERCIAL", MapState.COMMERCIAL));
            MenuZonesHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.zones.INDUSTRIAL", MapState.INDUSTRIAL));
            MenuZonesHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.zones.TRANSPORT", MapState.TRANSPORT));


            MenuOverlaysHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.overlays.POPDEN_OVERLAY",
                MapState.POPDEN_OVERLAY));
            MenuOverlaysHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.overlays.GROWTHRATE_OVERLAY",
                MapState.GROWTHRATE_OVERLAY));
            MenuOverlaysHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.overlays.LANDVALUE_OVERLAY",
                MapState.LANDVALUE_OVERLAY));
            MenuOverlaysHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.overlays.CRIME_OVERLAY", MapState.CRIME_OVERLAY));
            MenuOverlaysHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.overlays.POLLUTE_OVERLAY",
                MapState.POLLUTE_OVERLAY));
            MenuOverlaysHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.overlays.TRAFFIC_OVERLAY",
                MapState.TRAFFIC_OVERLAY));
            MenuOverlaysHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.overlays.POWER_OVERLAY", MapState.POWER_OVERLAY));
            MenuOverlaysHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.overlays.FIRE_OVERLAY", MapState.FIRE_OVERLAY));
            MenuOverlaysHeaderFlyoutItems.Add(MakeMapStateMenuItem("menu.overlays.POLICE_OVERLAY",
                MapState.POLICE_OVERLAY));

            _mapViewViewModel.SetUpAfterBasicInit(engine);
            _mapViewViewModel.ConnectView(_drawingAreaViewModel, _drawingAreaScroll);


            SetMapState(MapState.ALL);

            _messagesPane = new MessagesPane();
            _messagesScrollViewer.Content = _messagesPane;

            _notificationPanelViewModel.SetUpAfterBasicInit(this);

            _drawingArea.KeyDown += DrawingArea_KeyDown;
            _drawingArea.PointerPressed += DrawingArea_PointerPressed;
            _drawingArea.PointerReleased += DrawingArea_PointerReleased;
            _drawingArea.PointerMoved += DrawingArea_PointerMoved;
            _drawingArea.PointerExited += DrawingArea_PointerExited;
            _drawingArea.PointerWheelChanged += DrawingArea_PointerWheelChanged;

            _isDoSounds = Prefs.GetBoolean("SOUNDS_PREF", true);

            // start things up
            _mapViewViewModel.SetEngine(engine);

            engine.AddEarthquakeListener(this);
            ReloadFunds();
            ReloadOptions();
            UpdateDateLabel();
            StartTimer();
            MakeClean();

            _drawingAreaScroll.ViewChanging += _mapViewViewModel.drawingAreaScroll_ViewChanging;

            _toolsPanelViewModel.SetUpAfterBasicInit(this);
        }

        /// <summary>
        ///     Handles the Righted event of the ConfirmBar control to move sprite to the right.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ConfirmBar_Righted(object sender, EventArgs e)
        {
            if (_lastToolUsage.X + 1 >= Engine.Map[0].Length)
            {
                return;
            }

            MoveCurrentToolPosTo((int) _lastToolUsage.X + 1, (int) _lastToolUsage.Y);
        }

        /// <summary>
        ///     Handles the Lefted event of the ConfirmBar control to move sprite to the left.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ConfirmBar_Lefted(object sender, EventArgs e)
        {
            if (_lastToolUsage.X - 1 < 0)
            {
                return;
            }
            MoveCurrentToolPosTo((int) _lastToolUsage.X - 1, (int) _lastToolUsage.Y);
        }

        /// <summary>
        ///     Handles the Downed event of the ConfirmBar control to move sprite down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ConfirmBar_Downed(object sender, EventArgs e)
        {
            if (_lastToolUsage.Y + 1 >= Engine.Map.Length)
            {
                return;
            }
            MoveCurrentToolPosTo((int) _lastToolUsage.X, (int) _lastToolUsage.Y + 1);
        }

        /// <summary>
        ///     Handles the Uped event of the ConfirmBar control to move sprite up.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ConfirmBar_Uped(object sender, EventArgs e)
        {
            if (_lastToolUsage.Y - 1 < 0)
            {
                return;
            }
            MoveCurrentToolPosTo((int) _lastToolUsage.X, (int) _lastToolUsage.Y - 1);
        }

        /// <summary>
        ///     Handles the Declined event of the ConfirmBar control to hide sprite and conf bar.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ConfirmBar_Declined(object sender, EventArgs e)
        {
            ResetConfirmationBar();
        }

        /// <summary>
        ///     Resets the confirmation bar and tool according to parameter.
        /// </summary>
        /// <param name="resetToolPreview">if set to <c>true</c> tool gets canceled.</param>
        private void ResetConfirmationBar(bool resetToolPreview = true)
        {
            ConfirmBarIsVisible = false; // hide confirmation bar

            if (resetToolPreview)
            {
                // remove old preview
                _toolStroke = null;
                _drawingAreaViewModel.SetToolPreview(null);
                _drawingAreaViewModel.SetToolCursor(null);
            }
        }

        /// <summary>
        ///     Handles the Confirmed event of the ConfirmBar control to place a tool at the previously defined position.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ConfirmBar_Confirmed(object sender, EventArgs e)
        {
            if (touchPointerRoutedEventArgsOfCurrentConfirmationPending != null)
            {
                OnToolUp(touchPointerRoutedEventArgsOfCurrentConfirmationPending, false); // actually build the stuff
            }
        }

        /// <summary>
        ///     Sets the engine specified and updates the user interface.
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

            var timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }
            StopEarthquake();

            _drawingAreaViewModel.SetEngine(Engine);
            _mapViewViewModel.SetEngine(Engine); //must change mapView after DrawingArea
            _evaluationPaneViewModel.SetEngine(Engine);
            _demandIndViewModel.SetEngine(Engine);
            _graphsPaneViewModel.SetEngine(Engine);
            ReloadFunds();
            ReloadOptions();

            var notPaused = Speeds.Speed.First(s => s.Value == Engine.SimSpeed).Key != "PAUSED";
            if (timerEnabled || notPaused)
            {
                StartTimer();
            }
        }

        /// <summary>
        ///     DisastersCheckBoxText
        ///     Determines whether the game needs to be saved.
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
        ///     Handles the commands of save game question.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <remarks>ToDo: Depends on translation. Make it depend on ID.</remarks>
        private void CommandInvokedHandler(IUICommand command)
        {
            var yesString = Strings.GetString("main.saveDialogYes");
            if (command.Label == yesString) //ToDo: test this conversion of object to int
            {
                // Display message showing the label of the command that was invoked
                OnSaveCityClicked();
            }
        }

        /// <summary>
        ///     Determines whether game needs to be saved and if so displays a message on screen for user confirmation.
        /// </summary>
        /// <returns>whether game needs to be saved and has not yet been saved by user (true), otherwise (false)</returns>
        private async Task<bool> MaybeSaveCity()
        {
            if (NeedsSaved())
            {
                var timerEnabled = IsTimerActive();
                if (timerEnabled)
                {
                    StopTimer();
                }

                try
                {
                    var yesString = Strings.GetString("main.saveDialogYes");
                    var noString = Strings.GetString("main.saveDialogNo");
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
        ///     Displays a save message on game closing.
        /// </summary>
        private void CloseWindow()
        {
            MaybeSaveCity();
        }

        /// <summary>
        ///     Handles the Click event of the NewButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void NewButton_Click()
        {
            OnNewCityClicked();
        }

        /// <summary>
        ///     Handles the Click event of the LoadButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void LoadButton_Click()
        {
            OnLoadGameClicked();
        }

        /// <summary>
        ///     Handles the Click event of the SaveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void SaveButton_Click()
        {
            OnSaveCityClicked();
        }

        /// <summary>
        ///     Handles the Click event of the SaveAsButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void SaveAsButton_Click()
        {
            OnSaveCityAsClicked();
        }

        /// <summary>
        ///     Handles the Click event of the ExitButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void ExitButton_Click()
        {
            CloseWindow();
        }

        /// <summary>
        ///     Handles the Click event of the AutoBudgetMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void AutoBudgetMenuItem_Click()
        {
            OnAutoBudgetClicked();
        }

        /// <summary>
        ///     Handles the Click event of the AutoBulldozeCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void AutoBulldozeCheckBox_Click()
        {
            OnAutoBulldozeClicked();
        }

        /// <summary>
        ///     Handles the Click event of the DisastersCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void DisastersCheckBox_Click()
        {
            OnDisastersClicked();
        }

        /// <summary>
        ///     Handles the Click event of the SoundCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void SoundCheckBox_Click()
        {
            OnSoundClicked();
        }

        /// <summary>
        ///     Handles the Click event of the ZoomInButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void ZoomInButton_Click()
        {
            DoZoom(1);
        }

        /// <summary>
        ///     Handles the Click event of the ZoomOutButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void ZoomOutButton_Click()
        {
            DoZoom(-1);
        }

        /// <summary>
        ///     Handles the Click event of the MonsterButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void MonsterButton_Click()
        {
            OnInvokeDisasterClicked(Disaster.MONSTER);
        }

        /// <summary>
        ///     Handles the Click event of the FireButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void FireButton_Click()
        {
            OnInvokeDisasterClicked(Disaster.FIRE);
        }

        /// <summary>
        ///     Handles the Click event of the FloodButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void FloodButton_Click()
        {
            OnInvokeDisasterClicked(Disaster.FLOOD);
        }

        /// <summary>
        ///     Handles the Click event of the MeltdownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void MeltdownButton_Click()
        {
            OnInvokeDisasterClicked(Disaster.MELTDOWN);
        }

        /// <summary>
        ///     Handles the Click event of the TornadoButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void TornadoButton_Click()
        {
            OnInvokeDisasterClicked(Disaster.TORNADO);
        }

        /// <summary>
        ///     Handles the Click event of the EarthquakeButton control.
        /// </summary>
        public void EarthquakeButton_Click()
        {
            OnInvokeDisasterClicked(Disaster.EARTHQUAKE);
        }

        /// <summary>
        ///     Handles the Click event of the BudgetButton control.
        /// </summary>
        public void BudgetButton_Click()
        {
            OnViewBudgetClicked();
        }

        /// <summary>
        ///     Handles the Click event of the EvaluationButton control.
        /// </summary>
        public void EvaluationButton_Click()
        {
            OnViewEvaluationClicked();
        }

        /// <summary>
        ///     Handles the Click event of the GraphButton control.
        /// </summary>
        public void GraphButton_Click()
        {
            OnViewGraphClicked();
        }

        /// <summary>
        ///     Handles the Click event of the AboutButton control.
        /// </summary>
        public void AboutButton_Click()
        {
            OnAboutClicked();
        }

        /// <summary>
        ///     Makes the menu and displays translated strings.
        /// </summary>
        private void MakeMenu()
        {
            NewButtonText = Strings.GetString("menu.game.new");
            LoadButtonText = Strings.GetString("menu.game.load");
            SaveButtonText = Strings.GetString("menu.game.save");
            SaveAsButtonText = Strings.GetString("menu.game.save_as");

            SettingsButtonContentText = Strings.GetString("menu.options");
            DifficultyButtonContentText = Strings.GetString("menu.difficulty");
            AutoBudgetCheckBoxText = Strings.GetString("menu.options.auto_budget");
            AutoBulldozeCheckBoxText = Strings.GetString("menu.options.auto_bulldoze");
            DisastersCheckBoxText = Strings.GetString("menu.options.disasters");
            SoundCheckBoxText = Strings.GetString("menu.options.sound");

            DisasterButtonText = Strings.GetString("menu.disasters");
            MonsterButtonText = Strings.GetString("menu.disasters.MONSTER");
            FireButtonText = Strings.GetString("menu.disasters.FIRE");
            FloodButtonText = Strings.GetString("menu.disasters.FLOOD");
            MeltdownButtonText = Strings.GetString("menu.disasters.MELTDOWN");
            TornadoButtonText = Strings.GetString("menu.disasters.TORNADO");
            EarthquakeButtonText = Strings.GetString("menu.disasters.EARTHQUAKE");

            SpeedButtonText = Strings.GetString("menu.speed");

            _difficultyMenuItems = new Dictionary<int, LevelButtonViewModel>();
            for (var i = GameLevel.MIN_LEVEL; i <= GameLevel.MAX_LEVEL; i++)
            {
                var level = i;
                var menuItemc = new LevelButtonViewModel {Text = Strings.GetString("menu.difficulty." + level)};
                menuItemc.ClickCommand = new DelegateCommand(() => { OnDifficultyClicked(level); });

                _difficultyMenuItems.Add(level, menuItemc);
                Levels.Add(menuItemc);
            }

            BudgetButtonText = Strings.GetString("menu.windows.budget");
            EvaluationButtonText = Strings.GetString("menu.windows.evaluation");
            GraphButtonText = Strings.GetString("menu.windows.graph");
        }

        /// <summary>
        ///     Called when automatic budget button clicked.
        /// </summary>
        private void OnAutoBudgetClicked()
        {
            _isDirty1 = true;
            Engine.ToggleAutoBudget();
        }

        /// <summary>
        ///     Called when automatic bulldoze button clicked.
        /// </summary>
        private void OnAutoBulldozeClicked()
        {
            _isDirty1 = true;
            Engine.ToggleAutoBulldoze();
        }

        /// <summary>
        ///     Called when disasters button clicked.
        /// </summary>
        private void OnDisastersClicked()
        {
            _isDirty1 = true;
            Engine.ToggleDisasters();
        }

        /// <summary>
        ///     Called when sound button clicked.
        /// </summary>
        private void OnSoundClicked()
        {
            _isDoSounds = !_isDoSounds;
            Prefs.PutBoolean(SOUNDS_PREF, _isDoSounds);
            ReloadOptions();
        }

        /// <summary>
        ///     Makes the game clean so new game can be played.
        /// </summary>
        public void MakeClean()
        {
            _isDirty1 = false;
            _dirty2 = false;
            _lastSavedTime = DateTime.Now.Millisecond;
            if (CurrentFile != null)
            {
                var fileName = CurrentFile.Name;
                if (fileName.EndsWith("." + EXTENSION))
                {
                    fileName = fileName.Substring(0, fileName.Length - 1 - EXTENSION.Length);
                }
                TitleTextBlockText = Strings.GetString("main.caption_named_city") + fileName;
            }
            else
            {
                TitleTextBlockText = Strings.GetString("main.caption_unnamed_city");
            }
        }

        /// <summary>
        ///     Called when save city button clicked.
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
        ///     Called when save city as button clicked. Stops the timers, launches file packer for saving and after saving
        ///     restarts the timers.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> OnSaveCityAsClicked()
        {
            var timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }
            try
            {
                var picker = new FileSavePicker {DefaultFileExtension = ".cty"};
                picker.FileTypeChoices.Add(".cty Micropolis city", new List<string> {".cty"});
                picker.FileTypeChoices.Add(".cty_file Micropolis city", new List<string> {".cty_file"});
                var fileToSave = await picker.PickSaveFileAsync();
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
        ///     Called when user clicked the load button. Stops the timers, shows file picker, loads the game and restarts the
        ///     timers.
        /// </summary>
        private async void OnLoadGameClicked()
        {
            // check if user wants to save their current city
            var saveNeeded = await MaybeSaveCity();

            var timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }

            try
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".cty");
                picker.FileTypeFilter.Add(".cty_file");
                var file = await picker.PickSingleFileAsync();

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
            var saveNeeded = await MaybeSaveCity();
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
            var timerEnabled = IsTimerActive();
            if (timerEnabled)
            {
                StopTimer();
            }

            ShowNewGameDialogPanel();

            if (timerEnabled) //Bug:not a thread stopping dialog
            {
                StartTimer();
            }
        }

        /// <summary>
        ///     Shows the new game dialog panel.
        /// </summary>
        public void ShowNewGameDialogPanel()
        {
            NewGameDialogPaneOuterIsVisible = true;
        }

        /// <summary>
        ///     Hides the new game dialog panel.
        /// </summary>
        public void HideNewGameDialogPanel()
        {
            NewGameDialogPaneOuterIsVisible = false;
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

            var z = Engine.QueryZoneStatus(xpos, ypos);
            _notificationPanelViewModel.ShowZoneStatus(xpos, ypos, z);
        }

        /// <summary>
        ///     Zooms the map at provided screen coordinates of drawing area.
        /// </summary>
        /// <param name="dir">The direction. +1 is zoom in, -1 is zoom out.</param>
        /// <param name="mousePt">The mouse position to center at.</param>
        private void DoZoom(int dir, Point mousePt)
        {
            var oldZoom = _drawingAreaViewModel.GetTileSize();
            var newZoom = dir < 0 ? (oldZoom/2) : (oldZoom*2);
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
                var zoomFactor = newZoom/(double) oldZoom;
                var pos = new Point(DrawingAreaScrollHorizontalOffset, DrawingAreaScrollVerticalOffset);
                var newX = (int) Math.Round(mousePt.X*zoomFactor - (mousePt.X - pos.X));
                var newY = (int) Math.Round(mousePt.Y*zoomFactor - (mousePt.Y - pos.Y));
                _drawingAreaViewModel.SelectTileSize(newZoom);
                OnDrawingAreaScrollChangeView(newX, newY, DrawingAreaScrollZoomFactor);
            }
        }

        /// <summary>
        ///     Zooms the map.
        /// </summary>
        /// <param name="dir">The direction. +1 is zoom in, -1 is zoom out.</param>
        private void DoZoom(int dir)
        {
            DoZoom(dir,
                new Point(DrawingAreaScrollHorizontalOffset + DrawingAreaScrollViewportWidth/2,
                    DrawingAreaScrollVerticalOffset + DrawingAreaScrollViewportHeight/2));
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
            double zoomFactor = DrawingAreaScrollZoomFactor;
            var target = _drawingAreaScroll;
            if (ev.GetCurrentPoint(target).Properties.IsRightButtonPressed)
            {
                var posX =
                    (int) ((ev.GetCurrentPoint(target).Position.X + DrawingAreaScrollHorizontalOffset)/zoomFactor);
                var posY = (int) ((ev.GetCurrentPoint(target).Position.Y + DrawingAreaScrollVerticalOffset)/zoomFactor);

                var loc = _drawingAreaViewModel.GetCityLocation(posX, posY);

                DoQueryTool(loc.X, loc.Y);
                return;
            }

            if (!ev.GetCurrentPoint(target).Properties.IsLeftButtonPressed)
                return;

            if (CurrentTool == null)
                return;

            var posXb = (int) ((ev.GetCurrentPoint(target).Position.X + DrawingAreaScrollHorizontalOffset)/zoomFactor);
            var posYb = (int) ((ev.GetCurrentPoint(target).Position.Y + DrawingAreaScrollVerticalOffset)/zoomFactor);

            var locb = _drawingAreaViewModel.GetCityLocation(posXb, posYb);
            var x = locb.X;
            var y = locb.Y;

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
        ///     Moves the current tool position to.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        private void MoveCurrentToolPosTo(int x, int y)
        {
            _lastToolUsage.X = x;
            _lastToolUsage.Y = y;

            var isQueryTool = CurrentTool == MicropolisTools.MicropolisTool["QUERY"];
            if (isQueryTool)
            {
                DoQueryTool(x, y);
                _toolStroke = null;
            }
            else if (CurrentTool != null)
            {
                _drawingAreaViewModel.PositionToolCursor((x + 1)*_drawingAreaViewModel.TILE_WIDTH,
                    (y + 1)*_drawingAreaViewModel.TILE_HEIGHT);
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
                _drawingAreaViewModel.SetToolPreview(null);
                _drawingAreaViewModel.SetToolCursor(null);
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
            OnToolUp(ev, true);
        }

        private void OnToolUp(PointerRoutedEventArgs ev, bool resetConfirmationBar)
        {
            if (_toolStroke != null)
            {
                var loc = _toolStroke.GetLocation();
                var tr = _toolStroke.Apply();
                ShowToolResult(loc, tr);

                _drawingAreaViewModel.RepaintNow(true);
                _drawingAreaViewModel.SetToolPreview(null);
                _toolStroke = null;
            }

            OnToolHover(ev, resetConfirmationBar);

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
            _drawingAreaViewModel.SetToolCursor(
                _toolStroke.GetBounds(),
                CurrentTool
                );
            _drawingAreaViewModel.SetToolPreview(
                _toolStroke.GetPreview()
                );
        }

        /// <summary>
        ///     Called when the mouse is dragged above the drawing area.
        /// </summary>
        /// <param name="ev">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void OnToolDrag(PointerRoutedEventArgs ev)
        {
            double zoomFactor = DrawingAreaScrollZoomFactor;
            var target = _drawingAreaScroll;
            if (CurrentTool == null)
                return;
            /*if ((ev.getModifiersEx() & MouseEvent.BUTTON1_DOWN_MASK) == 0)
			return;*/
            // Bug: do

            var posX = (int) ((ev.GetCurrentPoint(target).Position.X/zoomFactor + DrawingAreaScrollHorizontalOffset));
            var posY = (int) ((ev.GetCurrentPoint(target).Position.Y/zoomFactor + DrawingAreaScrollVerticalOffset));

            var loc = _drawingAreaViewModel.GetCityLocation(posX, posY);

            var x = loc.X;
            var y = loc.Y;
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

            double zoomFactor = DrawingAreaScrollZoomFactor;
            var target = _drawingAreaScroll;
            if (CurrentTool == null || CurrentTool == MicropolisTools.MicropolisTool["QUERY"])
            {
                _drawingAreaViewModel.SetToolCursor(null);
                return;
            }

            var posX = (int) ((ev.GetCurrentPoint(target).Position.X + DrawingAreaScrollHorizontalOffset)/zoomFactor);
            var posY = (int) ((ev.GetCurrentPoint(target).Position.Y + DrawingAreaScrollVerticalOffset)/zoomFactor);

            var loc = _drawingAreaViewModel.GetCityLocation(posX, posY);
            var x = loc.X;
            var y = loc.Y;
            var w = CurrentTool.GetWidth();
            var h = CurrentTool.GetHeight();

            if (w >= 3)
                x--;
            if (h >= 3)
                y--;

            _drawingAreaViewModel.SetToolCursor(new CityRect(x, y, w, h), CurrentTool);
        }

        /// <summary>
        ///     Called when the cursor leaves the drawing area
        /// </summary>
        /// <param name="ev">The <see cref="PointerRoutedEventArgs" /> instance containing the event data.</param>
        private void OnToolExited(PointerRoutedEventArgs ev)
        {
            _drawingAreaViewModel.SetToolCursor(null);
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
                    _messagesPane.ViewModel.AppendCityMessage(MicropolisMessages.BULLDOZE_FIRST);
                    CitySound(Sounds.Sound["UHUH"], loc);
                    break;

                case ToolResult.INSUFFICIENT_FUNDS:
                    _messagesPane.ViewModel.AppendCityMessage(MicropolisMessages.INSUFFICIENT_FUNDS);
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
            DateLblTextBlockText = FormatGameDate(Engine.CityTime);

            PopLblTextBlockText = Engine.GetCityPopulation().ToString();
        }

        /// <summary>
        ///     Starts the timers.
        /// </summary>
        internal void StartTimer()
        {
            var engine = Engine;
            var count = engine.SimSpeed.SimStepsPerUpdate;

            if (engine.SimSpeed == Speeds.Speed["PAUSED"])
                return;

            if (_currentEarthquake != null)
            {
                var interval = 3000/MicropolisDrawingAreaViewModel.SHAKE_STEPS;
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
                for (var i = 0; i < count; i++)
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
        ///     Shows the error message.
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
            _drawingAreaViewModel.Shake(0);
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
        internal void OnWindowClosed()
        {
            if (IsTimerActive())
            {
                StopTimer();
            }
            _drawingAreaViewModel.StopRendering();
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

            _drawingAreaViewModel.StartRendering();
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

        private void SetSpeed(Speed newSpeed)
        {
            if (IsTimerActive())
            {
                StopTimer();
            }

            Engine.SetSpeed(newSpeed);
            StartTimer();
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
                        _messagesPane.ViewModel.AppendCityMessage(MicropolisMessages.NO_NUCLEAR_PLANTS);
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
            FundsLblTextBlockText = FormatFunds(Engine.Budget.TotalFunds);
        }

        /// <summary>
        ///     Updates the user interface to reflect current options.
        /// </summary>
        private void ReloadOptions()
        {
            AutoBudgetCheckBoxIsChecked = (Engine.AutoBudget);
            AutoBulldozeCheckBoxIsChecked = (Engine.AutoBulldoze);
            DisastersCheckBoxIsChecked = (!Engine.NoDisasters);
            SoundCheckBoxIsChecked = (_isDoSounds);

            IsSpeedFast = Engine.SimSpeed == Speeds.Speed["FAST"];
            IsSpeedSuperFast = Engine.SimSpeed == Speeds.Speed["SUPER_FAST"];
            IsSpeedNormal = Engine.SimSpeed == Speeds.Speed["NORMAL"];
            IsSpeedSlow = Engine.SimSpeed == Speeds.Speed["SLOW"];
            IsSpeedPause = Engine.SimSpeed == Speeds.Speed["PAUSED"];

            for (var i = GameLevel.MIN_LEVEL; i <= GameLevel.MAX_LEVEL; i++)
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
            EvaluationPaneIsVisible = EvaluationPaneIsVisible == false
                ? true
                : false;
        }

        /// <summary>
        ///     Called when the user clicked the graph button. Shows the graph panel.
        /// </summary>
        private void OnViewGraphClicked()
        {
            GraphsPaneIsVisible = GraphsPaneIsVisible == false
                ? true
                : false;
            _graphsPaneViewModel.Repaint();
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
            var timerEnabled = IsTimerActive();
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
            _newBudgetDialogViewModel.SetEngine(Engine);
            NewBudgetDialogPaneOuterIsVisible = true;
            _newBudgetDialogViewModel.EnableTimerWhenClosing = EnableTimerWhenClosing;
        }

        /// <summary>
        ///     Hides the budget dialog.
        /// </summary>
        public void HideBudgetDialog()
        {
            NewBudgetDialogPaneOuterIsVisible = false;
        }

        /// <summary>
        ///     Makes an item for the mapstatemenu from the state provided with the caption identified by stringPrefix.
        /// </summary>
        /// <param name="stringPrefix">The string prefix identifying the caption for the map state menu item.</param>
        /// <param name="state">The state, the map state menu item represents.</param>
        /// <returns></returns>
        private LevelButtonViewModel MakeMapStateMenuItem(String stringPrefix, MapState state)
        {
            var caption = Strings.GetString(stringPrefix);
            var menuItem = new LevelButtonViewModel {Text = caption};
            menuItem.ClickCommand = new DelegateCommand(() => { SetMapState(state); });
            _mapStateMenuItems.Add(state, menuItem);
            return menuItem;
        }

        /// <summary>
        ///     Sets the map state to the state provided and clears the old state.
        /// </summary>
        /// <param name="state">The state.</param>
        private void SetMapState(MapState state)
        {
            _mapStateMenuItems[(_mapViewViewModel.GetMapState())].IsChecked = false;
            _mapStateMenuItems[(state)].IsChecked = true;
            _mapViewViewModel.SetMapState(state);
            SetMapLegend(state);
        }

        /// <summary>
        ///     Sets the map legend for the provided map state by loading and displaying image. If an image cannot be loaded for
        ///     the state provided, the image on screen will be hidden.
        /// </summary>
        /// <param name="state">The state.</param>
        private async void SetMapLegend(MapState state)
        {
            var k = "legend_image." + state;
            Uri iconUrl = null;
            if (Strings.ContainsKey(k))
            {
                var iconName = "ms-appx:///resources/" + Strings.GetString(k);
                iconUrl = new Uri(iconName, UriKind.RelativeOrAbsolute);

                var folder = Package.Current.InstalledLocation;
                folder = await folder.GetFolderAsync("resources");
#if WINDOWS_PHONE_APP
                try
                {
                    var file = await
                        folder.GetFileAsync(
                            iconUrl.AbsoluteUri.Substring(
                                iconUrl.AbsoluteUri.LastIndexOf("/", StringComparison.Ordinal) + 1));
                }
                catch
#else
                var file =
                    await
                        folder.TryGetItemAsync(
                            iconUrl.AbsoluteUri.Substring(
                                iconUrl.AbsoluteUri.LastIndexOf("/", StringComparison.Ordinal) + 1));

                if (file != null)
#endif

                {
                    ImageSource iSource = new BitmapImage(iconUrl);

                    MapLegendLblImageSource = iSource;
                    MapLegendLblOpacity = 1;
                    return;
                }
            }
            MapLegendLblOpacity = 0;
        }

        /// <summary>
        ///     Called when someone clicks the about button and displays the about message
        /// </summary>
        private void OnAboutClicked()
        {
            var version = "0.5";
            var versionStr = Strings.GetString("main.about_caption") + ": " +
                             Strings.GetString("main.version_string") + " " + version;

            var d = new MessageDialog(Strings.GetString("main.about_text") + versionStr);
        }

        /// <summary>
        ///     Hides the notification panel.
        /// </summary>
        internal void HideNotificationPanel()
        {
            NotificationPanelIsVisible = false;
        }

        /// <summary>
        ///     Hides the evaluation pane.
        /// </summary>
        internal void HideEvaluationPane()
        {
            EvaluationPaneIsVisible = false;
        }

        /// <summary>
        ///     Shows the notification panel.
        /// </summary>
        internal void ShowNotificationPanel()
        {
            NotificationPanelIsVisible = true;
        }

        /// <summary>
        ///     Scrolls the game field by delta.
        /// </summary>
        /// <param name="dx">The horizontal delta.</param>
        /// <param name="dy">The vertical delta.</param>
        internal void ScrollGameFieldBy(int dx, int dy)
        {
            var horizontalPos = DrawingAreaScrollHorizontalOffset - dx;
            var verticalPos = DrawingAreaScrollVerticalOffset - dy;
            OnDrawingAreaScrollChangeView(horizontalPos, verticalPos, 1);
        }

        public event EventHandler<DrawingAreaScrollChangeCoordinates> DrawingAreaScrollChangeView;

        public void OnDrawingAreaScrollChangeView(double x, double y, float zoomFactor)
        {
            if (DrawingAreaScrollChangeView != null)
            {
                DrawingAreaScrollChangeView(this, new DrawingAreaScrollChangeCoordinates(x, y, zoomFactor));
            }
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
                var timerEnabled = IsTimerActive();
                if (timerEnabled)
                {
                    StopTimer();
                }

                try
                {
                    var folderToSave = ApplicationData.Current.LocalFolder;
                    var fileToSave =
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
        ///     Gets the zoom factor.
        /// </summary>
        /// <returns></returns>
        internal float GetZoomFactor()
        {
            return DrawingAreaScrollZoomFactor;
        }

        public bool GoBack()
        {
            if (NewBudgetDialogPaneOuterIsVisible)
            {
                StartTimer();
                HideBudgetDialog();
                
                return false;
            }
            
            if (GraphsPaneIsVisible)
            {
                HideGraphsPane();

                return false;
            }
            
            return true; // Hardware button should trigger page go back
        }
    }
}