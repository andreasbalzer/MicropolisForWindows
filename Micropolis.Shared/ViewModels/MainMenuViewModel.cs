using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Micropolis.Common;
using Micropolis.Model.Entities;

namespace Micropolis.ViewModels
{
    public class MainMenuViewModel : BindableBase
    {
        private readonly BitmapImage _blackHeader;
        private readonly BitmapImage _whiteHeader;
        private string _citiesHubSectionHeaderText;
        private string _citiesHubSectionNarrowHeaderText;
        private string _generalHubSectionHeaderText;
        private ImageSource _hubHeaderImageSource;
        private string _loadGameButtonText;
        private bool _loadUnsavedGameButtonIsVisible;
        private DelegateCommand _loadUnsavedGameCommand;
        private DelegateCommand _newGameCommand;
        private string _startNewGameButtonText;
        private IStorageItem _unsavedFileExists;
        private string _unsavedGameButtonText;
        private string _unsavedGameButtonWideText;
        private string _unsavedGameMessageText;
        private string _unsavedGameMessageWideText;

        public MainMenuViewModel()
        {
            Cities = new ObservableCollection<City>();

            UnsavedGameButtonText = Strings.GetString("UnsavedGameButton");
            UnsavedGameMessageText = Strings.GetString("UnsavedGameMessage");
            CitiesHubSectionHeaderText = Strings.GetString("CitiesHubSection");
            GeneralHubSectionHeaderText = Strings.GetString("GeneralHubSection");
            LoadGameButtonText = Strings.GetString("LoadGameButton");
            StartNewGameButtonText = Strings.GetString("StartNewGameButton");

            LoadUnsavedGameCommand = new DelegateCommand(LoadUnsavedGame);
            NewGameCommand = new DelegateCommand(NewGame);

            CheckForPreviousGame();
            LoadCities();

            var blackLogoUri = new Uri("ms-appx:///Assets/Logo/LogoBlack800.png", UriKind.RelativeOrAbsolute);
            _blackHeader = new BitmapImage(blackLogoUri);
            var whiteLogoUri = new Uri("ms-appx:///Assets/Logo/LogoWhite800.png", UriKind.RelativeOrAbsolute);
            _whiteHeader = new BitmapImage(whiteLogoUri);
        }

        public DelegateCommand LoadUnsavedGameCommand
        {
            get { return _loadUnsavedGameCommand; }
            set { SetProperty(ref _loadUnsavedGameCommand, value); }
        }

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

        private void MainMenuPage_Loaded(object sender, RoutedEventArgs e)
        {
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
                LoadUnsavedGameButtonIsVisible = true;
            }
        }

        /// <summary>
        ///     Handles the Click event of the NewGameButton control and loads the game page.
        /// </summary>
        private void NewGame()
        {
            App.MainMenuReference.Frame.Navigate(typeof (MainGamePage));
        }

        /// <summary>
        ///     Handles the OnClick event of the LoadUnsavedGameButton control, adds an app command to load the autosave file and
        ///     loads the main game page.
        /// </summary>
        private void LoadUnsavedGame()
        {
            ((ISupportsAppCommands) Application.Current).AppCommands.Add(new AppCommand(AppCommands.LOADFILE,
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

                    if (await cityThumbs.TryGetItemAsync(fileName) == null)
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