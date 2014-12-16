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
        private BitmapImage _blackHeader;
        private IStorageItem _unsavedFileExists;
        private BitmapImage _whiteHeader;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainMenuPage" /> class.
        /// </summary>
        public MainMenuPage()
        {
            Cities = new ObservableCollection<City>();
            InitializeComponent();
            App.MainMenuReference = this;

            UnsavedGameButtonWide.Content = Strings.GetString("UnsavedGameButton");
            UnsavedGameMessageWide.Text = Strings.GetString("UnsavedGameMessage");
            UnsavedGameButtonNarrow.Content = Strings.GetString("UnsavedGameButton");
            UnsavedGameMessageNarrow.Text = Strings.GetString("UnsavedGameMessage");
            CitiesHubSectionWideHeader.Text = Strings.GetString("CitiesHubSection");
            CitiesHubSectionNarrowHeader.Text = Strings.GetString("CitiesHubSection");
            GeneralHubSectionHeader.Text = Strings.GetString("GeneralHubSection");

            CheckForPreviousGame();
            LoadCities();
            Loaded += MainMenuPage_Loaded;

            Window.Current.SizeChanged += Window_SizeChanged;
            DetermineVisualState();
        }

        public ObservableCollection<City> Cities { get; set; }

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

        private void MainMenuPage_Loaded(object sender, RoutedEventArgs e)
        {
            var blackLogoUri = new Uri("ms-appx:///Assets/Logo/LogoBlack800.png", UriKind.RelativeOrAbsolute);
            _blackHeader = new BitmapImage(blackLogoUri);
            var whiteLogoUri = new Uri("ms-appx:///Assets/Logo/LogoWhite800.png", UriKind.RelativeOrAbsolute);
            _whiteHeader = new BitmapImage(whiteLogoUri);
            // Register handler for CommandsRequested events from the settings pane
            SettingsPane.GetForCurrentView().CommandsRequested += SettingsCharm.OnCommandsInMenuRequested;
        }

        /// <summary>
        ///     Checks for previous autosave game in local folder. If it exists, a button to load that autosave is displayed.
        /// </summary>
        private async void CheckForPreviousGame()
        {
            var folder = ApplicationData.Current.LocalFolder;

            _unsavedFileExists = await folder.TryGetItemAsync("autosave.cty");
            if (_unsavedFileExists != null)
            {
                LoadUnsavedGameButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     Handles the Click event of the NewGameButton control and loads the game page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (MainGamePage));
        }

        /// <summary>
        ///     Handles the OnClick event of the LoadUnsavedGameButton control, adds an app command to load the autosave file and
        ///     loads the main game page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void LoadUnsavedGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            ((ISupportsAppCommands) Application.Current).AppCommands.Add(new AppCommand(AppCommands.LOADFILE,
                _unsavedFileExists));
            Frame.Navigate(typeof (MainGamePage));
        }

        private async Task LoadCities()
        {
            var installFolder = Package.Current.InstalledLocation;
            var cityFolder = await installFolder.GetFolderAsync("resources");
            cityFolder = await cityFolder.GetFolderAsync("cities");

            var localFolder = ApplicationData.Current.LocalFolder;
            var cityThumbs = await localFolder.GetFolderAsync("cityThumbs");

            foreach (var file in await cityFolder.GetFilesAsync())
            {
                if (file.FileType == ".cty")
                {
                    var newCity = new City();
                    newCity.FilePath = file.Path;
                    newCity.Title = file.Name;

                    var fileName = file.Name + ".png";

                    var iconUri = new Uri(cityThumbs.Path + "/" + fileName, UriKind.Absolute);

                    if (await cityThumbs.TryGetItemAsync(fileName) == null)
                    {
                        iconUri = new Uri(cityFolder.Path + "/unknown.png", UriKind.Absolute);
                    }
                    newCity.ImageSource = new BitmapImage(iconUri);
                    Cities.Add(newCity);
                }
            }
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var textBlock = (TextBlock) ((StackPanel) ((Grid) button.Content).Children[1]).Children[0];
            var title = textBlock.Text;
            var path = new Uri("ms-appx:///resources/cities/" + title, UriKind.Absolute);
            var file = await StorageFile.GetFileFromApplicationUriAsync(path);


            ((ISupportsAppCommands) Application.Current).AppCommands.Add(new AppCommand(AppCommands.LOADFILEASNEWCITY,
                file));
            Frame.Navigate(typeof (MainGamePage));
        }

        private void LoadGameButtonTB_OnLoaded(object sender, RoutedEventArgs e)
        {
            var sendObj = (TextBlock) sender;
            sendObj.Text = Strings.GetString("LoadGameButton");
        }

        private void StartNewGameButtonTB_OnLoaded(object sender, RoutedEventArgs e)
        {
            var sendObj = (TextBlock) sender;
            sendObj.Text = Strings.GetString("StartNewGameButton");
        }

        private void MainMenuHub_OnLayoutUpdated(object sender, object e)
        {
            if (_blackHeader != null && _whiteHeader != null)
            {
                var relativePoint = GeneralHubSection.TransformToVisual(MainMenuHub).TransformPoint(new Point(0, 0));

                var xScrollOffset = relativePoint.X;
                if (xScrollOffset < 150)
                {
                    if (HubHeaderImage.Source != _blackHeader)
                    {
                        HubHeaderImage.Source = _blackHeader;
                    }
                }
                else
                {
                    if (HubHeaderImage.Source != _whiteHeader)
                    {
                        HubHeaderImage.Source = _whiteHeader;
                    }
                }
            }
        }
    }
}