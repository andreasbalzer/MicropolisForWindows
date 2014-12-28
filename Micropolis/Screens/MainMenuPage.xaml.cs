using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Micropolis.Model.Entities;
using Micropolis.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Micropolis.Screens
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
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenuPage
    {

        private MainMenuViewModel _viewModel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainMenuPage" /> class.
        /// </summary>
        public MainMenuPage()
        {
           InitializeComponent();
           _viewModel = new MainMenuViewModel();
           this.DataContext = _viewModel;
           App.MainMenuReference = this;
                     
            Window.Current.SizeChanged += Window_SizeChanged;
            DetermineVisualState();

            // Register handler for CommandsRequested events from the settings pane
            SettingsPane.GetForCurrentView().CommandsRequested += SettingsCharm.OnCommandsInMenuRequested;
        }
        
        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            DetermineVisualState();
        }

        private void DetermineVisualState()
        {
            var size = Window.Current.Bounds;
            string state;

            if (size.Width <= 320)
                state = "Snapped";
            else if (size.Width <= 500)
                state = "Narrow";
            else
                state = "DefaultLayout";


            VisualStateManager.GoToState(this, state, true);
        }
        
        private void MainMenuHub_OnLayoutUpdated(object sender, object e)
        {
            var relativePoint = GeneralHubSection.TransformToVisual(MainMenuHub).TransformPoint(new Point(0, 0));
            _viewModel.UpdateLogoColor(relativePoint);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var textBlock = (TextBlock) ((StackPanel) ((Grid) button.Content).Children[1]).Children[0];
            var title = textBlock.Text;
            _viewModel.LoadGameFile(title);
        }
    }
}