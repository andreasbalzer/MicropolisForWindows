using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
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
    public sealed partial class MainGamePage
    {
        private readonly MainGamePageViewModel _viewModel;

        public MainGamePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            App.MainPageReference = this;

            _viewModel = new MainGamePageViewModel(this, NotificationPanel.ViewModel, DrawingArea.ViewModel, SoundOutput,
                ToolsPanel.ViewModel, DrawingArea, ConfirmBar, NewBudgetDialog.ViewModel, GraphsPane.ViewModel,
                EvaluationPane.ViewModel, MiniMapPane, DrawingAreaScroll, MessagesScrollViewer, DemandInd.ViewModel,
                LevelMenu, SpeedMenu, NewGameDialogPaneInner);
            DataContext = _viewModel;
            Loaded += _viewModel.MainPage_Loaded;
        }

        public MainGamePageViewModel ViewModel
        {
            get { return _viewModel; }
        }

        /// <summary>
        ///     Wird unmittelbar aufgerufen, nachdem die Page entladen und nicht mehr die aktuelle Quelle eines übergeordneten
        ///     Frame ist.
        /// </summary>
        /// <param name="e">
        ///     Ereignisdaten, die vom überschreibenden Code überprüft werden können. Die Ereignisdaten repräsentieren
        ///     die Navigation, die die aktuelle Page hochgeladen hat.
        /// </param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.IsNavigatedAway = true;
            base.OnNavigatedFrom(e);
            _viewModel.OnWindowClosed();
        }

        /// <summary>
        ///     Wird unmittelbar aufgerufen, bevor die Page geladen und zur aktuellen Quelle eines übergeordneten Frame wird.
        /// </summary>
        /// <param name="e">
        ///     Ereignisdaten, die vom überschreibenden Code überprüft werden können. Die Ereignisdaten repräsentieren
        ///     die noch ausstehende Navigation, die die aktuelle Page hochladen wird. Normalerweise ist Parameter die wichtigste
        ///     zu überprüfende Eigenschaft.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            if (App.IsNavigatedAway) // had we ever been navigated away? Could be first run after all
            {
                _viewModel.OnWindowReopend();
            }
        }

        private void DrawingAreaScroll_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _viewModel.DrawingAreaScrollViewportHeight = e.NewSize.Height;
            _viewModel.DrawingAreaScrollViewportWidth = e.NewSize.Width;
        }

        private void DrawingAreaScroll_OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            _viewModel.DrawingAreaScrollVerticalOffset = DrawingAreaScroll.VerticalOffset;
            _viewModel.DrawingAreaScrollHorizontalOffset = DrawingAreaScroll.HorizontalOffset;
            _viewModel.DrawingAreaScrollZoomFactor = DrawingAreaScroll.ZoomFactor;
        }

        public void DrawingAreaScrollChangeView(double horizontalPos, double verticalPos, float zoomFactor)
        {
            DrawingAreaScroll.ChangeView(horizontalPos, verticalPos, zoomFactor);
        }
    }
}