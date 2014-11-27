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

    using System;
    using System.Collections.Generic;
    using System.IO;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI.Popups;
    using Windows.UI.Xaml.Controls;
    using Engine;

    /// <summary>
    /// NewCityDialog presents an interactive panel to start a new game, load a game, select difficulty level and chose a map.
    /// </summary>
    public sealed partial class NewCityDialog
    {
        private readonly Dictionary<int, RadioButton> _levelBtns = new Dictionary<int, RadioButton>();
        private readonly MainGamePage _mainPage;
        private readonly Stack<Engine.Micropolis> _nextMaps = new Stack<Engine.Micropolis>();
        private readonly Stack<Engine.Micropolis> _previousMaps = new Stack<Engine.Micropolis>();
        private Engine.Micropolis _engine;
        private bool _firstTime;

        /// <summary>
        /// Initiates a new instance of the NewCityDialog.
        /// </summary>
        public NewCityDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initiates a new instance of the NewCityDialog.
        /// </summary>
        /// <param name="mainPage">reference to mainPage</param>
        public NewCityDialog(MainGamePage mainPage)
            : this()
        {
            Title.Text = Strings.GetString("welcome.caption");
            //mapPane.Destroy();

            _engine = new Engine.Micropolis();
            new MapGenerator(_engine).GenerateNewCity();
            //mapPane = new OverlayMapView(engine);
            mapPane.SetUpAfterBasicInit(_engine);


            for (int lev = GameLevel.MIN_LEVEL; lev <= GameLevel.MAX_LEVEL; lev++)
            {
                int x = lev;
                var radioBtn = new RadioButton {Content = Strings.GetString("menu.difficulty." + lev)};
                radioBtn.Click += delegate { SetGameLevel(x); };

                levelBox.Children.Add(radioBtn);
                _levelBtns.Add(x, radioBtn);
            }
            SetGameLevel(GameLevel.MIN_LEVEL);
            previousMapBtn.Content = Strings.GetString("welcome.previous_map");
            previousMapBtn.Click += delegate { OnPreviousMapClicked(); };
            thisMapBtn.Content = Strings.GetString("welcome.play_this_map");
            thisMapBtn.Click += delegate
            {
                mainPage.HideNewGameDialogPanel();
                OnPlayClicked();
            };
            nextMapBtn.Content = Strings.GetString("welcome.next_map");
            nextMapBtn.Click += delegate { OnNextMapClicked(); };
            cancelBtn.Content = Strings.GetString("welcome.cancel");
            cancelBtn.Click += delegate
            {
                mainPage.HideNewGameDialogPanel();
                OnCancelClicked();
            };

            loadCityBtn.Content = Strings.GetString("welcome.load_city");
            loadCityBtn.Click += delegate { OnLoadCityClicked(); };


            _mainPage = mainPage;
        }

        /// <summary>
        /// Called when user clicked previous map button to go to the preview map.
        /// </summary>
        private void OnPreviousMapClicked()
        {
            if (_previousMaps.Count == 0)
                return;

            _nextMaps.Push(_engine);
            _engine = _previousMaps.Pop();
            mapPane.SetEngine(_engine);

            previousMapBtn.IsEnabled = _previousMaps.Count != 0;
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
            mapPane.SetEngine(_engine);

            previousMapBtn.IsEnabled = true;
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
                if (_levelBtns[lev].IsChecked.HasValue && _levelBtns[lev].IsChecked.Value)
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