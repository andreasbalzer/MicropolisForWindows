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
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Engine;

    /// <summary>
    ///     GraphsPane is a major UI component that can be expanded upon user request. It displays a graph with information on
    ///     the mayors work and the city performance.
    /// </summary>
    public sealed partial class GraphsPane 
    {
        private GraphsPaneViewModel _viewModel;
        public GraphsPaneViewModel ViewModel { get { return _viewModel; } }
        /// <summary>
        ///     Initializes a new instance of the <see cref="GraphsPane" /> class.
        /// </summary>
        public GraphsPane()
        {
            InitializeComponent();
            _viewModel=new GraphsPaneViewModel(graphArea);
            this.DataContext = _viewModel;
        }
    }
}