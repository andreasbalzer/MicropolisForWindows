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

#if WINDOWS_PHONE_APP
using Windows.Media.SpeechRecognition;
#endif

namespace Micropolis.ViewModels
{
    public class MainMenuViewModel : BindableBase
    {
        private readonly BitmapImage _blackHeader;
        private readonly TelemetryClient _telemetry;
        private readonly BitmapImage _whiteHeader;
        private string _citiesHubSectionHeaderText;
        private string _citiesHubSectionNarrowHeaderText;
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

        public DelegateCommand HelpCommand
        {
            get { return _helpCommand; }
            set { SetProperty(ref _helpCommand, value); }
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

            UnsavedGameButtonText = Strings.GetString("UnsavedGameButton");
            UnsavedGameMessageText = Strings.GetString("UnsavedGameMessage");
            CitiesHubSectionHeaderText = Strings.GetString("CitiesHubSection");
            GeneralHubSectionHeaderText = Strings.GetString("GeneralHubSection");
            LoadGameButtonText = Strings.GetString("LoadGameButton");
            StartNewGameButtonText = Strings.GetString("StartNewGameButton");
            NewCityDialogHeaderText = Strings.GetString("NewCityDialogHeaderText");

            LoadUnsavedGameCommand = new DelegateCommand(LoadUnsavedGame);
            NewGameCommand = new DelegateCommand(NewGame);
            ToggleSplitViewCommand = new DelegateCommand(ToggleSplitView);

            SettingsCommand = new DelegateCommand(OpenSettings);
            AboutCommand = new DelegateCommand(OpenAbout);
            LicenseCommand = new DelegateCommand(OpenLicense);
            PrivacyCommand = new DelegateCommand(OpenPrivacy);
            HelpCommand = new DelegateCommand(OpenHelp);

            CheckForPreviousGame();
            LoadCities();

            var blackLogoUri = new Uri("ms-appx:///Assets/Logo/LogoBlack800.png", UriKind.RelativeOrAbsolute);
            _blackHeader = new BitmapImage(blackLogoUri);
            var whiteLogoUri = new Uri("ms-appx:///Assets/Logo/LogoWhite800.png", UriKind.RelativeOrAbsolute);
            _whiteHeader = new BitmapImage(whiteLogoUri);

#if WINDOWS_PHONE_APP
            SpeechCommand = new DelegateCommand(RunSpeechRecognition);
            FeedbackCommand = new DelegateCommand(RunFeedback);
            
            NotifierHelper.RegisterNotifier();
#endif
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

        #if WINDOWS_PHONE_APP
        private void RunSpeechRecognition()
        {
            RunSpeechRecognitionAsync();
        }

        private void RunFeedback()
        {
            var feedbackTitle = Strings.GetString("feedback.title");
            var feedbackBody = Strings.GetString("feedback.body");
            Launcher.LaunchUriAsync(
                new Uri("mailto:micropolis@andreas-balzer.de?subject=" + feedbackTitle + "&body=" + feedbackBody,
                    UriKind.Absolute));
        }

        private async Task RunSpeechRecognitionAsync()
        {
            List<string> speechCommands = new List<string>();
            speechCommands.Add("create new city");
            speechCommands.Add("load city");
            speechCommands.Add("start about");
            speechCommands.Add("start badnews");
            speechCommands.Add("start bruce");
            speechCommands.Add("start deadwood");
            speechCommands.Add("start finnigan");
            speechCommands.Add("start freds");
            speechCommands.Add("start haight");
            speechCommands.Add("start happisle");
            speechCommands.Add("start joffburg");
            speechCommands.Add("start kamakura");
            speechCommands.Add("start kobe");
            speechCommands.Add("start kowloon");
            speechCommands.Add("start kyoto");
            speechCommands.Add("start linecity");
            speechCommands.Add("start med_isle");
            speechCommands.Add("start ndulls");
            speechCommands.Add("start neatmap");
            speechCommands.Add("start radial");
            speechCommands.Add("start senri");
            speechCommands.Add("start southpac");
            speechCommands.Add("start splats");
            speechCommands.Add("start wetcity");
            speechCommands.Add("start yokohama");

            SpeechRecognizer sr = new SpeechRecognizer();

            //add dication grammar to the recognizer
            SpeechRecognitionListConstraint topicConstraint =
                new SpeechRecognitionListConstraint(speechCommands);

            sr.Constraints.Add(topicConstraint);
            await sr.CompileConstraintsAsync();

            sr.UIOptions.AudiblePrompt = Strings.GetString("SpeechWhatCanIDo");
            sr.UIOptions.ExampleText = Strings.GetString("SpeechCreateNewCity") + "\n"
                                       + Strings.GetString("SpeechLoadCity") + "\n"
                                       + Strings.GetString("SpeechStart") + " linecity"; 

            SpeechRecognitionResult result = await sr.RecognizeWithUIAsync();
            if (result.Confidence == SpeechRecognitionConfidence.High ||
                result.Confidence == SpeechRecognitionConfidence.Medium)
            {
                var lower = result.Text.ToLower();
                if (lower == Strings.GetString("SpeechCreateNewCity").ToLower())
                {
                    NewGame();

                }
                else if (lower == Strings.GetString("SpeechLoadCity").ToLower())
                {
                    NewGame();
                }
                else if (lower.StartsWith(Strings.GetString("SpeechStart").ToLower()))
                {
                    var map = lower.Substring(Strings.GetString("SpeechStart").Length+1).Trim();
                    foreach (var city in Cities)
                    {
                        var cityname = city.Title.Substring(0, city.Title.IndexOf(".cty")).ToLower().Trim();
                        if (cityname == map)
                        {
                            LoadGameFile(city.Title);
                            return;
                        }
                    }
                }
            }
        }
#endif

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

#if WINDOWS_PHONE_APP
            try
            {
                _unsavedFileExists = await folder.GetFileAsync("autosave.cty");
            }
            catch
            {
                return;
            }
#else
            _unsavedFileExists = await folder.TryGetItemAsync("autosave.cty");
            if (_unsavedFileExists != null)
#endif
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

#if WINDOWS_PHONE_APP
                    try
                    {
                        var voidi = await cityThumbs.GetFileAsync(fileName);
                    }
                    catch
#else
                    if (await cityThumbs.TryGetItemAsync(fileName) == null)
#endif
                    {
                        iconUri = new Uri(cityFolder.Path + "/unknown.png", UriKind.Absolute);
                    }
                    newCity.ImageSource = new BitmapImage(iconUri);
                    Cities.Add(newCity);
                }
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

            var path = new Uri("ms-appx:///resources/cities/" + title, UriKind.Absolute);
            var file = await StorageFile.GetFileFromApplicationUriAsync(path);

            ((ISupportsAppCommands) Application.Current).AppCommands.Add(new AppCommand(AppCommands.LOADFILEASNEWCITY,
                file));
            App.MainMenuReference.Frame.Navigate(typeof (MainGamePage));
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