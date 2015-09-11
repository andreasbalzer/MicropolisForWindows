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
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Engine;

    /// <summary>
    /// Toolbar control that allows user to select tools. Supports multiple views.
    /// </summary>
    public sealed partial class Toolbar
    {
        private ToolbarViewModel _viewModel;
        public ToolbarViewModel ViewModel { get { return _viewModel; }}

        public Toolbar()
        {
            InitializeComponent();
            _viewModel=new ToolbarViewModel();
            this.DataContext = _viewModel;
        }
    }
}