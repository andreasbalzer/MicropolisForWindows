using System;
using Windows.UI.Xaml.Controls;
using Engine;
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
    ///     The budget dialog appeas every year. It offers the user the possibility to inspect current budget, compare to last
    ///     budget and make tax changes as well as changes in maintenance funding.
    /// </summary>
    public sealed partial class BudgetDialog
    {
        private BudgetDialogViewModel _viewModel;

        public BudgetDialogViewModel ViewModel { get { return _viewModel; } }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BudgetDialog" /> class.
        /// </summary>
        public BudgetDialog()
        {
            InitializeComponent();
            _viewModel = new BudgetDialogViewModel();
            this.DataContext = _viewModel;
            taxRateEntry.ValueChanged += delegate { _viewModel.ApplyChange(); };
            roadFundEntry.ValueChanged += delegate { _viewModel.ApplyChange(); };
            fireFundEntry.ValueChanged += delegate { _viewModel.ApplyChange(); };
            policeFundEntry.ValueChanged += delegate { _viewModel.ApplyChange(); };

        }
    }
}