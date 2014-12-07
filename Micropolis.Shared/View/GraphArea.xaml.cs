using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Micropolis.Model.Entities;
using Micropolis.ViewModels;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

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

    /// <summary>
    ///     GraphArea displaying graph with information on mayors work and city performance
    /// </summary>
    /// <remarks>ToDo: Paddings not yet implemented.</remarks>
    public sealed partial class GraphArea
    {
        private GraphAreaViewModel _viewModel;
        public GraphAreaViewModel ViewModel { get { return _viewModel; } }

        public GraphArea()
        {
            InitializeComponent();
            _viewModel=new GraphAreaViewModel(LineChart);
            this.DataContext = _viewModel;
        }
    }
}