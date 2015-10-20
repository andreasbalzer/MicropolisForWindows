using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Micropolis.Common;
using Micropolis.Model.Entities;
using Micropolis.Screens;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;
using Micropolis.Controller;
using Windows.Storage.Pickers;
using Engine.Model.Enums;
using Engine.Libs;

#if WINDOWS_PHONE_APP
using Windows.Media.SpeechRecognition;
#endif

namespace Micropolis.ViewModels
{
    public class MainMenuViewModel : BindableBase
    {


        private string _hamburgerHomeText;
        private string _hamburgerNewText;
        private string _hamburgerLoadText;
        private string _hamburgerSaveText;
        private string _hamburgerSaveAsText;
        private string _hamburgerPrivacyText;
        private string _hamburgerHelpText;
        private string _hamburgerAboutText;
        private string _hamburgerSettingsText;
        private string _hamburgerLicenseText;
        private string _hamburgerRatingText;
        private readonly BitmapImage _blackHeader;
        private readonly TelemetryClient _telemetry;
        private readonly BitmapImage _whiteHeader;
        private string _citiesHubSectionHeaderText;
        private string _citiesHubSectionNarrowHeaderText;
        private string _scenariosHubSectionHeaderText;
        private string _generalHubSectionHeaderText;
        private ImageSource _hubHeaderImageSource;
        private string _loadGameButtonText;
        private bool _loadUnsavedGameButtonIsVisible;
        private DelegateCommand _loadUnsavedGameCommand;
        private string _newCityDialogHeaderText;
        private DelegateCommand _newGameCommand;
        private string _startNewGameButtonText;
        private IStorageItem _unsavedFileExists;
        private string _unsavedGameButtonText;
        private string _unsavedGameButtonWideText;
        private string _unsavedGameMessageText;
        private string _unsavedGameMessageWideText;
        private DelegateCommand _speechCommand;
        private bool _splitViewIsOpen;
        private DelegateCommand _toggleSplitViewCommand;
        private DelegateCommand _helpCommand;
        private DelegateCommand _privacyCommand;
        private DelegateCommand _settingsCommand;
        private DelegateCommand _licenseCommand;
        private DelegateCommand _aboutCommand;

        public string HamburgerHomeText { get { return _hamburgerHomeText; } set { SetProperty(ref _hamburgerHomeText, value); } }
        public string HamburgerNewText { get { return _hamburgerNewText; } set { SetProperty(ref _hamburgerNewText, value); } }
        public string HamburgerLoadText { get { return _hamburgerLoadText; } set { SetProperty(ref _hamburgerLoadText, value); } }
        public string HamburgerSaveText { get { return _hamburgerSaveText; } set { SetProperty(ref _hamburgerSaveText, value); } }
        public string HamburgerSaveAsText { get { return _hamburgerSaveAsText; } set { SetProperty(ref _hamburgerSaveAsText, value); } }
        public string HamburgerPrivacyText { get { return _hamburgerPrivacyText; } set { SetProperty(ref _hamburgerPrivacyText, value); } }
        public string HamburgerHelpText { get { return _hamburgerHelpText; } set { SetProperty(ref _hamburgerHelpText, value); } }
        public string HamburgerAboutText { get { return _hamburgerAboutText; } set { SetProperty(ref _hamburgerAboutText, value); } }
        public string HamburgerSettingsText { get { return _hamburgerSettingsText; } set { SetProperty(ref _hamburgerSettingsText, value); } }
        public string HamburgerLicenseText { get { return _hamburgerLicenseText; } set { SetProperty(ref _hamburgerLicenseText, value); } }
        public string HamburgerRatingText { get { return _hamburgerRatingText; } set { SetProperty(ref _hamburgerRatingText, value); } }

        public DelegateCommand HelpCommand
        {
            get { return _helpCommand; }
            set { SetProperty(ref _helpCommand, value); }
        }

        public DelegateCommand LoadGameCommand
        {
            get { return _loadGameCommand; }
            set { SetProperty(ref _loadGameCommand, value); }
        }

        public DelegateCommand PrivacyCommand
        {
            get { return _privacyCommand; }
            set { SetProperty(ref _privacyCommand, value); }
        }

        public DelegateCommand SettingsCommand
        {
            get { return _settingsCommand; }
            set { SetProperty(ref _settingsCommand, value); }
        }

        public DelegateCommand LicenseCommand
        {
            get { return _licenseCommand; }
            set { SetProperty(ref _licenseCommand, value); }
        }

        public DelegateCommand AboutCommand
        {
            get { return _aboutCommand; }
            set { SetProperty(ref _aboutCommand, value); }
        }


        public bool SplitViewIsOpen
        {
            get { return _splitViewIsOpen; }
            set { SetProperty(ref _splitViewIsOpen, value); }
        }

        public MainMenuViewModel()
        {
            try
            {
                _telemetry = new TelemetryClient();
            }
            catch (Exception)
            {
            }

            Cities = new ObservableCollection<City>();
            Scenarios = new ObservableCollection<City>();

            UnsavedGameButtonText = Strings.GetString("UnsavedGameButton");
            UnsavedGameMessageText = Strings.GetString("UnsavedGameMessage");
            CitiesHubSectionHeaderText = Strings.GetString("CitiesHubSection");
            ScenariosHubSectionHeaderText = Strings.GetString("ScenariosHubSection");
            GeneralHubSectionHeaderText = Strings.GetString("GeneralHubSection");
            LoadGameButtonText = Strings.GetString("LoadGameButton");
            StartNewGameButtonText = Strings.GetString("StartNewGameButton");
            NewCityDialogHeaderText = Strings.GetString("NewCityDialogHeaderText");

            HamburgerHomeText = Strings.GetString("hamburgerMenu.Home");
            HamburgerNewText = Strings.GetString("hamburgerMenu.New");
            HamburgerLoadText = Strings.GetString("hamburgerMenu.Load");
            HamburgerSaveText = Strings.GetString("hamburgerMenu.Save");
            HamburgerSaveAsText = Strings.GetString("hamburgerMenu.SaveAs");
            HamburgerPrivacyText = Strings.GetString("hamburgerMenu.Privacy");
            HamburgerHelpText = Strings.GetString("hamburgerMenu.Help");
            HamburgerAboutText = Strings.GetString("hamburgerMenu.About");
            HamburgerSettingsText = Strings.GetString("hamburgerMenu.Settings");
            HamburgerLicenseText = Strings.GetString("hamburgerMenu.License");
            HamburgerRatingText = Strings.GetString("hamburgerMenu.Rating");

            LoadUnsavedGameCommand = new DelegateCommand(LoadUnsavedGame);
            NewGameCommand = new DelegateCommand(NewGame);
            LoadGameCommand = new DelegateCommand(LoadGame);
            ToggleSplitViewCommand = new DelegateCommand(ToggleSplitView);

            SettingsCommand = new DelegateCommand(OpenSettings);
            AboutCommand = new DelegateCommand(OpenAbout);
            LicenseCommand = new DelegateCommand(OpenLicense);
            PrivacyCommand = new DelegateCommand(OpenPrivacy);
            HelpCommand = new DelegateCommand(OpenHelp);
            RateCommand = new DelegateCommand(OpenRatingAndFeedback);

            CheckForPreviousGame();
            LoadCities();
            LoadScenarios();

            var blackLogoUri = new Uri("ms-appx:///Assets/Logo/LogoBlack800.png", UriKind.RelativeOrAbsolute);
            _blackHeader = new BitmapImage(blackLogoUri);
            var whiteLogoUri = new Uri("ms-appx:///Assets/Logo/LogoWhite800.png", UriKind.RelativeOrAbsolute);
            _whiteHeader = new BitmapImage(whiteLogoUri);

#if WINDOWS_PHONE_APP
            SpeechCommand = new DelegateCommand(RunSpeechRecognition);
            FeedbackCommand = new DelegateCommand(RunFeedback);
            
            NotifierHelper.RegisterNotifier();
#endif
            NotifierHelper.RegisterNotifier();
        }

        private async void LoadGame()
        {
            if (App.MainPageReference != null && App.MainPageReference.ViewModel != null)
            {
                var saveNeeded = await App.MainPageReference.ViewModel.MaybeSaveCity();
            }
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".cty");
            picker.FileTypeFilter.Add(".cty_file");

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var loadCommand = new AppCommand(AppCommands.LOADFILE, file);
                ((ISupportsAppCommands)App.Current).AppCommands.Add(loadCommand);
            }
        }

        private void OpenSettings()
        {
            try
            {
                _telemetry.TrackEvent("MainMenuPreferencesClicked");
        }
            catch (Exception)
            {
            }

            var settings = new SettingsFlyout();
            settings.Content = new PreferencesUserControl();
            settings.Title = Strings.GetString("settingsCharm.Preferences");
            settings.ShowIndependent();
        }

        private void OpenLicense()
        {
            try
            {
                _telemetry.TrackEvent("MainMenuLicenseClicked");
            }
            catch (Exception)
            {
            }

            App.MainMenuReference.Frame.Navigate(typeof (LicensePage));
        }

        private void OpenAbout()
        {
            try
            {
                _telemetry.TrackEvent("MainMenuAboutClicked");
            }
            catch (Exception)
            {
            }

            var settings = new SettingsFlyout();
            settings.Content = new AboutUserControl();

            settings.Title = Strings.GetString("settingsCharm.About");
            settings.ShowIndependent();
        }

        private void OpenPrivacy()
        {
            try
            {
                _telemetry.TrackEvent("MainMenuPrivacyClicked");
            }
            catch (Exception)
            {
            }

            var settings = new SettingsFlyout();
            settings.Content = new PrivacyUserControl();
            settings.Title = Strings.GetString("settingsCharm.Privacy");
            settings.ShowIndependent();
        }

        private void OpenHelp()
        {
            try
            {
                _telemetry.TrackEvent("MainMenuHelpClicked");
            }
            catch (Exception)
            {
            }

            App.MainMenuReference.Frame.Navigate(typeof (HelpPage));
        }

        private void OpenRatingAndFeedback()
        {
            try
            {
                _telemetry.TrackEvent("MainMenuRatingClicked");
            }
            catch (Exception)
            {
            }

            var settings = new SettingsFlyout();
            settings.Content = new RatingFlyout();
            settings.Title = Strings.GetString("settingsCharm.Rating");
            settings.ShowIndependent();
        }

        public string NewCityDialogHeaderText
        {
            get { return _newCityDialogHeaderText; }
            set { SetProperty(ref _newCityDialogHeaderText, value); }
        }

        public DelegateCommand LoadUnsavedGameCommand
        {
            get { return _loadUnsavedGameCommand; }
            set { SetProperty(ref _loadUnsavedGameCommand, value); }
        }

        public DelegateCommand RateCommand
        {
            get { return _openRatingAndFeedbackCommand; }
            set { SetProperty(ref _openRatingAndFeedbackCommand, value); }
        }

        public DelegateCommand ToggleSplitViewCommand
        {
            get { return _toggleSplitViewCommand; }
            set { SetProperty(ref _toggleSplitViewCommand, value); }
        }

        private void ToggleSplitView()
        {
            if (!_splitViewIsOpen)
            {
                _telemetry.TrackEvent("MainMenu_OpenSplitView");
            }
            else
            {
                _telemetry.TrackEvent("MainMenu_CloseSplitView");
            }
            SplitViewIsOpen = !SplitViewIsOpen;
        }

        public DelegateCommand SpeechCommand
        {
            get { return _speechCommand; }
            set { SetProperty(ref _speechCommand, value); }
        }

        public DelegateCommand FeedbackCommand
        {
            get { return _feedbackCommand; }
            set { SetProperty(ref _feedbackCommand, value); }
        }

        private DelegateCommand _feedbackCommand;
        private DelegateCommand _openRatingAndFeedbackCommand;
        private DelegateCommand _loadGameCommand;

        public DelegateCommand NewGameCommand
        {
            get { return _newGameCommand; }
            set { SetProperty(ref _newGameCommand, value); }
        }

        public ImageSource HubHeaderImageSource
        {
            get { return _hubHeaderImageSource; }
            set { SetProperty(ref _hubHeaderImageSource, value); }
        }

        public string LoadGameButtonText
        {
            get { return _loadGameButtonText; }
            set { SetProperty(ref _loadGameButtonText, value); }
        }

        public string StartNewGameButtonText
        {
            get { return _startNewGameButtonText; }
            set { SetProperty(ref _startNewGameButtonText, value); }
        }

        public string UnsavedGameButtonText
        {
            get { return _unsavedGameButtonText; }
            set { SetProperty(ref _unsavedGameButtonText, value); }
        }

        public string UnsavedGameMessageText
        {
            get { return _unsavedGameMessageText; }
            set { SetProperty(ref _unsavedGameMessageText, value); }
        }

        public string CitiesHubSectionHeaderText
        {
            get { return _citiesHubSectionHeaderText; }
            set { SetProperty(ref _citiesHubSectionHeaderText, value); }
        }


        public string ScenariosHubSectionHeaderText
        {
            get { return _scenariosHubSectionHeaderText; }
            set { SetProperty(ref _scenariosHubSectionHeaderText, value); }
        }

        public string GeneralHubSectionHeaderText
        {
            get { return _generalHubSectionHeaderText; }
            set { SetProperty(ref _generalHubSectionHeaderText, value); }
        }

        public bool LoadUnsavedGameButtonIsVisible
        {
            get { return _loadUnsavedGameButtonIsVisible; }
            set { SetProperty(ref _loadUnsavedGameButtonIsVisible, value); }
        }

        public ObservableCollection<City> Cities { get; set; }
        public ObservableCollection<City> Scenarios { get; set; }

        public void RegisterNewCityDialogViewModel(NewCityDialogViewModel newCityDialogViewModel)
        {
            newCityDialogViewModel.PlayClicked += newCityDialogViewModel_PlayClicked;
        }

        private void newCityDialogViewModel_PlayClicked(object sender, Tuple<Engine.Micropolis, IStorageFile, int> e)
        {
            try
            {
                _telemetry.TrackEvent("MainMenuThisMapClicked");
            }
            catch (Exception)
            {
            }

            ((ISupportsAppCommands) Application.Current).AppCommands.Add(
                new AppCommand(AppCommands.LOADFILEASNEWCITYANDDELETE,
                    e));
            App.MainMenuReference.Frame.Navigate(typeof (MainGamePage));
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
                try
                {
                    _telemetry.TrackEvent("MainMenuUnsavedGameDetected");
                }
                catch (Exception)
                {
                }

                LoadUnsavedGameButtonIsVisible = true;
            }
        }

        /// <summary>
        ///     Handles the Click event of the NewGameButton control and loads the game page.
        /// </summary>
        private void NewGame()
        {
            try
            {
                _telemetry.TrackEvent("MainMenuLoadNewGameClicked");
            }
            catch (Exception)
            {
            }

            App.MainMenuReference.Frame.Navigate(typeof (MainGamePage));
        }

        /// <summary>
        ///     Handles the OnClick event of the LoadUnsavedGameButton control, adds an app command to load the autosave file and
        ///     loads the main game page.
        /// </summary>
        private void LoadUnsavedGame()
        {
            try
            {
                _telemetry.TrackEvent("MainMenuLoadUnsavedGameClicked");
            }
            catch (Exception)
            {
            }

            ((ISupportsAppCommands) Application.Current).AppCommands.Add(
                new AppCommand(AppCommands.LOADFILEASNEWCITYANDDELETE,
                    _unsavedFileExists));
            App.MainMenuReference.Frame.Navigate(typeof (MainGamePage));
        }

        private async Task LoadCities()
        {
            var installFolder = Package.Current.InstalledLocation;
            var cityFolder = await installFolder.GetFolderAsync("Assets");
            cityFolder = await cityFolder.GetFolderAsync("resources");
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


        private async Task LoadScenarios()
        {
            var installFolder = Package.Current.InstalledLocation;
            var cityFolder = await installFolder.GetFolderAsync("Assets");
            cityFolder = await cityFolder.GetFolderAsync("resources");
            cityFolder = await cityFolder.GetFolderAsync("scenarios");

            var localFolder = ApplicationData.Current.LocalFolder;
            var cityThumbs = await localFolder.GetFolderAsync("cityThumbs");

            foreach (Scenario scenario in Engine.Model.Enums.Scenarios.Items.Values)
            {
                if (scenario == Engine.Model.Enums.Scenarios.Items[ScenarioENUM.SC_NONE])
                {
                    continue;
                }
                StorageFile file = await LoadFiles.GetPackagedFile("Assets/resources/scenarios", scenario.FileName);
                var newCity = new City();
                newCity.FilePath = file.Path;
                newCity.Title = Strings.GetString("scenario." + scenario.Name);

                var fileName = file.Name + ".png";

                var iconUri = new Uri(cityThumbs.Path + "/" + fileName, UriKind.Absolute);

                if (await cityThumbs.TryGetItemAsync(fileName) == null)
                {
                    iconUri = new Uri(cityFolder.Path + "/unknown.png", UriKind.Absolute);
                }
                newCity.ImageSource = new BitmapImage(iconUri);
                Scenarios.Add(newCity);
            }
        }

        public async Task LoadGameFile(string title)
        {
            try
            {
                _telemetry.TrackEvent("MainMenuLoadGameFile" + title);
            }
            catch (Exception)
            {
            }

            var path = new Uri("ms-appx:///Assets/resources/cities/" + title, UriKind.Absolute);
            var file = await StorageFile.GetFileFromApplicationUriAsync(path);

            ((ISupportsAppCommands) Application.Current).AppCommands.Add(new AppCommand(AppCommands.LOADFILEASNEWCITY,
                file));
            App.MainMenuReference.Frame.Navigate(typeof (MainGamePage));
        }

        public async Task LoadScenarioGameFile(string title)
        {
            try
            {
                _telemetry.TrackEvent("MainMenuLoadScenarioGameFile" + title);
            }
            catch (Exception)
            {
            }

            ScenarioENUM scenario = ScenarioENUM.SC_NONE;
            foreach (Scenario currentScenario in Engine.Model.Enums.Scenarios.Items.Values)
            {
                if (Strings.GetString("scenario." + currentScenario.Name) == title)
                {
                    scenario = currentScenario.Type;
                    break;
                }
            }

                ((ISupportsAppCommands)Application.Current).AppCommands.Add(new AppCommand(AppCommands.LOADSCENARIOASNEWCITY,
                scenario));
            App.MainMenuReference.Frame.Navigate(typeof(MainGamePage));
        }

        public void UpdateLogoColor(Point position)
        {
            var xScrollOffset = position.X;
            if (xScrollOffset < 150)
            {
                if (HubHeaderImageSource != _blackHeader)
                {
                    HubHeaderImageSource = _blackHeader;
                }
            }
            else
            {
                if (HubHeaderImageSource != _whiteHeader)
                {
                    HubHeaderImageSource = _whiteHeader;
                }
            }
        }
    }
}