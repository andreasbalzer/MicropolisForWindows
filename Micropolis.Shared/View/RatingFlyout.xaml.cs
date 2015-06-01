using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.ApplicationInsights;
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

    /// <summary>
    ///     PreferencesUserControl shows app preferences to be shown in a settings flyout in charms bar.
    /// </summary>
    public sealed partial class RatingFlyout : UserControl
    {
        public RatingFlyoutViewModel ViewModel { get; private set; }

        public RatingFlyout()
        {
            this.InitializeComponent();
            ViewModel = new RatingFlyoutViewModel();
            this.DataContext = ViewModel;
        }
    }
}