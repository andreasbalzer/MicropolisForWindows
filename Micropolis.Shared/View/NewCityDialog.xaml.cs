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
        private NewCityDialogViewModel _viewModel;

        public NewCityDialogViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public NewCityDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initiates a new instance of the NewCityDialog.
        /// </summary>
        /// <param name="mainPage">reference to mainPage</param>
        public NewCityDialog(MainGamePageViewModel mainPageViewModel)
            : this()
        {
            _viewModel=new NewCityDialogViewModel(mainPageViewModel,mapPane.ViewModel);
            this.DataContext = _viewModel;
        }

    }
}