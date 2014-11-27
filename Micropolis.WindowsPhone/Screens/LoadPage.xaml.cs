using Windows.UI.Core;
using Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Standardseite" ist unter http://go.microsoft.com/fwlink/?LinkId=234237 dokumentiert.

namespace Micropolis.Screens
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

    using System.Threading;

    using Micropolis.Model.Entities;

    /// <summary>
    /// Eine Standardseite mit Eigenschaften, die die meisten Anwendungen aufweisen.
    /// </summary>
    public sealed partial class LoadPage : Page
    {

        private bool _loaded;

        private CancellationToken _cancelToken;

        private CancellationTokenSource _tokenSource;


        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPage"/> class.
        /// </summary>
        public LoadPage()
        {
            InitializeComponent();
            App.LoadPageReference = this;
            App.Current.Suspending += Current_Suspending;
        }

        void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            this.CancelLoading();
        }

        /// <summary>
        /// Handles the ImageOpened event of the Image control, progresses the bar by one and starts the overall loading process.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            this._tokenSource = new CancellationTokenSource();
            this._cancelToken = this._tokenSource.Token;
            LoadData();
            ProgressIn.Value = 1;
        }

        public void CancelLoading()
        {
            this._tokenSource.Cancel();
        }

        /// <summary>
        /// Loads the game data and displays progress in progress indicator. At end of loading phase app commands get processed and action taken accordingly.
        /// </summary>
        /// <returns>void task</returns>
        /// <remarks>Calls into UI classes to load images. At the end of every load execution, the progress bar gets incremented by 1.
        /// As soon as everything is loaded, the page is navigated to the MainGamePage.</remarks>
        private async Task LoadData()
        {
            if (_loaded)
            {
                this.ProcessAppCommands();
                return;
            }

            try
            {
                Task t1 = Strings.Initialize(this._cancelToken).ContinueWith(
                    (e) =>
                    {
                        App.LoadPageReference.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            () => { ProgressIn.Value += 1; });
                    }); // async load language files
                Task t2 = TileImages.Initialize(this._cancelToken).ContinueWith(
                    (e) =>
                    {
                        App.LoadPageReference.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            () => { ProgressIn.Value += 1; });
                    });
                Task t3 = Tiles.Initialize().ContinueWith(
                    (e) =>
                    {
                        App.LoadPageReference.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            () => { ProgressIn.Value += 1; });
                    });
                Task t5 = OverlayMapView.Initialize(this._cancelToken).ContinueWith(
                    (e) =>
                    {
                        App.LoadPageReference.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            () => { ProgressIn.Value += 1; });
                    });

                Task.WhenAll(new List<Task> { t1, t2, t3, t5 }).ContinueWith(
                    (e) =>
                    {
                        App.LoadPageReference.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            () =>
                            {
                                _loaded = true;
                                ProcessAppCommands();
                            });
                    });
            }
            catch (AggregateException)
            {
                //ToDo: we do nothing here; in case of ThreadCanceledException we are fine, other cases are mostly checked but here's something for testing & QA.
            }
        }

        /// <summary>
        /// Processes the application commands. Depending on commands either the main menu or the game page gets loaded. The command gets removed from the pool.
        /// </summary>
        private void ProcessAppCommands()
        {
            ISupportsAppCommands current = (ISupportsAppCommands)App.Current;
            AppCommand skipCommand = current.AppCommands.FirstOrDefault(s => s.Instruction == AppCommands.SKIPMENU);

            bool skipMenu = skipCommand != null;
            if (skipMenu)
            {
                current.AppCommands.Remove(skipCommand);
                Frame.Navigate(typeof(MainGamePage));
            }
            else
            {
                Frame.Navigate(typeof(MainGamePage));
            }
        }



    }
}
