using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;

using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Micropolis.Model.Entities;
using Micropolis.ViewModels;
using Microsoft.ApplicationInsights;

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
            try
            {
                _telemetry = new TelemetryClient();
                _telemetry.TrackPageView("MainMenuPage");
            }
            catch (Exception) { }

            _viewModel = new MainMenuViewModel();
            this.DataContext = _viewModel;
            App.MainMenuReference = this;

            Window.Current.SizeChanged += Window_SizeChanged;
            DetermineVisualState();

            // Register handler for CommandsRequested events from the settings pane
            //Bug:SettingsPane.GetForCurrentView().CommandsRequested += SettingsCharm.OnCommandsInMenuRequested;

            Loaded += MainMenuPage_Loaded;
        }

        void MainMenuPage_Loaded(object sender, RoutedEventArgs e)
        {
            bool informAboutTelemetry =
                ((ISupportsAppCommands)Application.Current).AppCommands.Any(
                    s => s.Instruction == AppCommands.UPDATEDVERSION && s.Value == "informAboutTelemetry");

            if (informAboutTelemetry)
            {
                var itemToRemove = ((ISupportsAppCommands)Application.Current).AppCommands.First(
                    s => s.Instruction == AppCommands.UPDATEDVERSION && s.Value == "informAboutTelemetry");
                ((ISupportsAppCommands)Application.Current).AppCommands.Remove(itemToRemove);
                MessageDialog dialog = new MessageDialog(Strings.GetString("InformAboutTelemetryContent"), Strings.GetString("InformAboutTelemetryTitle"));
                dialog.ShowAsync();
            }
        }

        private TelemetryClient _telemetry;

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            DetermineVisualState();
        }

        private void DetermineVisualState()
        {
            var size = Window.Current.Bounds;
            string state;

            if (size.Width <= 320)
            {
                state = "Snapped";
                try
                {
                    _telemetry.TrackEvent("MainMenuSnappedLayout");
                }
                catch (Exception) { }
            }
            else if (size.Width <= 500)
            {
                state = "Narrow";
                try
                {
                    _telemetry.TrackEvent("MainMenuNarrowLayout");
                }
                catch (Exception) { }
            }
            else
            {
                state = "DefaultLayout";
                try
                {
                    _telemetry.TrackEvent("MainMenuDefaultLayout");
                }
                catch (Exception) { }
            }


            VisualStateManager.GoToState(this, state, true);
        }

        private void MainMenuHub_OnLayoutUpdated(object sender, object e)
        {
            var relativePoint = GeneralHubSection.TransformToVisual(MainMenuHub).TransformPoint(new Point(0, 0));
            _viewModel.UpdateLogoColor(relativePoint);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var textBlock = (TextBlock)((StackPanel)((Grid)button.Content).Children[1]).Children[0];
            var title = textBlock.Text;

            try
            {
                _telemetry.TrackEvent("MainMenuLaunchMap" + title);
            }
            catch (Exception) { }

            _viewModel.LoadGameFile(title);
        }

        private void NewCityDialogPane_OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.RegisterNewCityDialogViewModel(((NewCityDialog)sender).ViewModel);
        }
    }
}