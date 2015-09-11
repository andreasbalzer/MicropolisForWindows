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
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Engine;

    public sealed partial class MessagesPane
    {
        private MessagesPaneViewModel _viewModel;
        public MessagesPaneViewModel ViewModel { get { return _viewModel; } }

        /// <summary>
        /// Initiates a new instance of the MessagesPane control.
        /// </summary>
        public MessagesPane()
        {
            InitializeComponent();
            _viewModel=new MessagesPaneViewModel();
            this.DataContext = _viewModel;
        }
    }
}