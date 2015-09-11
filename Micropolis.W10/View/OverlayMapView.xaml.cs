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
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Devices.Input;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Input;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Engine;
    using Micropolis.Lib.graphics;

    /// <summary>
    /// OverlayMapView displays a map with viewport rectangle and zone information.
    /// </summary>
    public sealed partial class OverlayMapView
    {
        private OverlayMapViewModel _viewModel;
        public OverlayMapViewModel ViewModel { get { return _viewModel; } }

        public OverlayMapView()
        {
            InitializeComponent();
            _viewModel=new OverlayMapViewModel(LayoutRoot);
            this.DataContext = _viewModel;

            PointerPressed += (sender, e) => _viewModel.OnMousePressed(e);
            PointerMoved += (sender, e) => _viewModel.OnMouseMoved(e);
            PointerReleased += (sender, e) => _viewModel.OnMouseReleased(e);

            CompositionTarget.Rendering += _viewModel.Render;

        }
    }
}