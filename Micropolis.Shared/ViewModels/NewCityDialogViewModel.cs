using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Engine;
using Micropolis.Common;

namespace Micropolis.ViewModels
{
    public class NewCityDialogViewModel : BindableBase
    {
         private readonly Dictionary<int, LevelButtonViewModel> _levelBtns = new Dictionary<int, LevelButtonViewModel>();
        private readonly MainGamePage _mainPage;
        private readonly Stack<Engine.Micropolis> _nextMaps = new Stack<Engine.Micropolis>();
        private readonly Stack<Engine.Micropolis> _previousMaps = new Stack<Engine.Micropolis>();
        private Engine.Micropolis _engine;
        private bool _firstTime;

        public ObservableCollection<LevelButtonViewModel> Levels { get; set; } 

        public NewCityDialogViewModel(MainGamePage mainPage, OverlayMapView mapPane)
        {
            _mapPane = mapPane;
            Levels=new ObservableCollection<LevelButtonViewModel>();
            TitleTextBlockText = Strings.GetString("welcome.caption");
            //mapPane.Destroy();

            _engine = new Engine.Micropolis();
            new MapGenerator(_engine).GenerateNewCity();
            //mapPane = new OverlayMapView(engine);
            _mapPane.SetUpAfterBasicInit(_engine);


            for (int lev = GameLevel.MIN_LEVEL; lev <= GameLevel.MAX_LEVEL; lev++)
            {
                int x = lev;
                var radioBtn = new LevelButtonViewModel {Text = Strings.GetString("menu.difficulty." + lev)};
                radioBtn.ClickCommand = new DelegateCommand(() => { SetGameLevel(x); });

                Levels.Add(radioBtn);
                _levelBtns.Add(x, radioBtn);
            }
            SetGameLevel(GameLevel.MIN_LEVEL);
            PreviousMapButtonText = Strings.GetString("welcome.previous_map");
            PreviousMapCommand = new DelegateCommand(() => { OnPreviousMapClicked(); });
            ThisMapButtonText = Strings.GetString("welcome.play_this_map");
            ThisMapCommand = new DelegateCommand(() =>
            {
                mainPage.HideNewGameDialogPanel();
                OnPlayClicked();
            });
            NextMapButtonText = Strings.GetString("welcome.next_map");
            NextMapCommand = new DelegateCommand(() => { OnNextMapClicked(); });
            CancelButtonText = Strings.GetString("welcome.cancel");
            CancelCommand = new DelegateCommand(() =>
            {
                mainPage.HideNewGameDialogPanel();
                OnCancelClicked();
            });

            LoadCityButtonText = Strings.GetString("welcome.load_city");

            LoadCityCommand = new DelegateCommand(() => { OnLoadCityClicked(); });


            _mainPage = mainPage;
        }

        private string _titleTextBlockText;
        public string TitleTextBlockText { get { return _titleTextBlockText; } set { SetProperty(ref _titleTextBlockText, value); } }

        private string _thisMapButtonText;
        public string ThisMapButtonText { get { return _thisMapButtonText; } set { SetProperty(ref _thisMapButtonText, value); } }

        private string _previousMapButtonText;
        public string PreviousMapButtonText { get { return _previousMapButtonText; } set { SetProperty(ref _previousMapButtonText, value); } }

        private bool _previousMapButtonIsEnabled;
        public bool PreviousMapButtonIsEnabled { get { return _previousMapButtonIsEnabled; } set { SetProperty(ref _previousMapButtonIsEnabled, value); } }

        private string _nextMapButtonText;
        public string NextMapButtonText { get { return _nextMapButtonText; } set { SetProperty(ref _nextMapButtonText, value); } }

        private string _cancelButtonText;
        public string CancelButtonText { get { return _cancelButtonText; } set { SetProperty(ref _cancelButtonText, value); } }

        private string _loadCityButtonText;
        public string LoadCityButtonText { get { return _loadCityButtonText; } set { SetProperty(ref _loadCityButtonText, value); } }

        private DelegateCommand _thisMapCommand;
        public DelegateCommand ThisMapCommand { get { return _thisMapCommand; } set { SetProperty(ref _thisMapCommand, value); } }

        private DelegateCommand _previousMapCommand;
        public DelegateCommand PreviousMapCommand { get { return _previousMapCommand; } set { SetProperty(ref _previousMapCommand, value); } }

        private DelegateCommand _nextMapCommand;
        public DelegateCommand NextMapCommand { get { return _nextMapCommand; } set { SetProperty(ref _nextMapCommand, value); } }

        private DelegateCommand _loadCityCommand;
        public DelegateCommand LoadCityCommand { get { return _loadCityCommand; } set { SetProperty(ref _loadCityCommand, value); } }

        private DelegateCommand _cancelCommand;
        private OverlayMapView _mapPane;
        public DelegateCommand CancelCommand { get { return _cancelCommand; } set { SetProperty(ref _cancelCommand, value); } }

        /// <summary>
        /// Called when user clicked previous map button to go to the preview map.
        /// </summary>
        private void OnPreviousMapClicked()
        {
            if (_previousMaps.Count == 0)
                return;

            _nextMaps.Push(_engine);
            _engine = _previousMaps.Pop();
            _mapPane.SetEngine(_engine);

            PreviousMapButtonIsEnabled = _previousMaps.Count != 0;
        }
        
        /// <summary>
        /// Called when user clicked next map button to go to next map.
        /// </summary>
        private void OnNextMapClicked()
        {
            if (_nextMaps.Count == 0)
            {
                var m = new Engine.Micropolis();
                new MapGenerator(m).GenerateNewCity();
                _nextMaps.Push(m);
            }

            _previousMaps.Push(_engine);
            _engine = _nextMaps.Pop();
            _mapPane.SetEngine(_engine);

            PreviousMapButtonIsEnabled = true;
        }

        /// <summary>
        /// Called when user clicked load button to load a city from disk.
        /// </summary>
        private async void OnLoadCityClicked()
        {
            try
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".cty");
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    var newEngine = new Engine.Micropolis();
                    Stream stream = await file.OpenStreamForReadAsync();
                    await newEngine.LoadFile(stream);
                    StartPlaying(newEngine, file);
                    _mainPage.HideNewGameDialogPanel();
                }
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(Strings.GetString("main.error_caption") + e);
                dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Start to play the specified file with the specified engine.
        /// </summary>
        /// <param name="newEngine">engine to play with</param>
        /// <param name="file">file to load and play</param>
        private void StartPlaying(Engine.Micropolis newEngine, StorageFile file)
        {
            MainGamePage win = _mainPage;
            win.SetEngine(newEngine);
            win.CurrentFile = file;
            win.MakeClean();
        }

        /// <summary>
        /// Called when user clicked the play button to play the currently visible map.
        /// </summary>
        private void OnPlayClicked()
        {
            _engine.SetGameLevel(GetSelectedGameLevel());
            _engine.SetFunds(GameLevel.GetStartingFunds(_engine.GameLevel));
            StartPlaying(_engine, null);
            MainGamePage win = _mainPage;
            win.OnDifficultyClicked(GetSelectedGameLevel());
        }

        /// <summary>
        /// Called when user clicked cancel button to abort the new game dialog.
        /// </summary>
        private void OnCancelClicked()
        {
            MainGamePage win = _mainPage;
            win.HideNewGameDialogPanel();
        }

        /// <summary>
        /// Gets the selected level
        /// </summary>
        /// <returns>selected level</returns>
        private int GetSelectedGameLevel()
        {
            foreach (int lev in _levelBtns.Keys)
            {
                if (_levelBtns[lev].IsChecked)
                {
                    return lev;
                }
            }
            return GameLevel.MIN_LEVEL;
        }

        /// <summary>
        /// Sets game level
        /// </summary>
        /// <param name="level">level to set engine to</param>
        private void SetGameLevel(int level)
        {
            foreach (int lev in _levelBtns.Keys)
            {
                _levelBtns[lev].IsChecked = (lev == level);
            }
        }
    }
}
