using System;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Engine;
using Micropolis.Lib.graphics;
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
    
    public sealed partial class MicropolisDrawingArea
    {

        private MicropolisDrawingAreaViewModel _viewModel;
        public MicropolisDrawingAreaViewModel ViewModel { get { return _viewModel; } }
        /// <summary>
        ///     Initiates a new instance of the MicropolisDrawingArea control.
        /// </summary>
        public MicropolisDrawingArea()
        {
            InitializeComponent();

            _viewModel = new MicropolisDrawingAreaViewModel(this.LayoutRoot,ImageOutput,ImageCursor,SPToRender,TextBlockToRender,Dispatcher);
            this.DataContext = _viewModel;

            LayoutRoot.PointerPressed += _viewModel.LayoutRoot_PointerPressed;
            LayoutRoot.PointerReleased += _viewModel.LayoutRoot_PointerReleased;
            LayoutRoot.PointerMoved += _viewModel.LayoutRoot_PointerMoved;

            Loaded += (a, b) => _viewModel.Repaint();
            CompositionTarget.Rendering += _viewModel.Render;
        }
    }
}