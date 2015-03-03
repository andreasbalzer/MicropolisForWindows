using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.ApplicationInsights;

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
    public sealed partial class PreferencesUserControl
    {
        /// <summary>
        ///     Indicates if the PreferencesUserControl has been initialized.
        /// </summary>
        private readonly bool _isInit;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PreferencesUserControl" /> class.
        /// </summary>
        public PreferencesUserControl()
        {
            InitializeComponent();
            Music.Header = Strings.GetString("preferences.PlayMusic");
            Sounds.Header = Strings.GetString("preferences.PlaySounds");
            Language.Header = Strings.GetString("preferences.Language");
            automatic.Content = Strings.GetString("preferences.automatic");
            en.Content = Strings.GetString("preferences.en");
            de.Content = Strings.GetString("preferences.de");

            Music.IsOn = Prefs.GetBoolean("Music", true);
            Sounds.IsOn = Prefs.GetBoolean("enable_sounds", true);
            Language.SelectedItem =
                Language.Items.First(s => ((ComboBoxItem) s).Name == Prefs.GetString("Language", "automatic"));
            _isInit = true;

            try { 
                _telemetry = new TelemetryClient();
                _telemetry.TrackPageView("PreferencesUserControl");
            }
            catch (Exception) { }

        }
        private TelemetryClient _telemetry;


        /// <summary>
        ///     Handles the Toggled event of the Music control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Music_Toggled(object sender, RoutedEventArgs e)
        {
            Prefs.PutBoolean("Music", Music.IsOn);
        }

        /// <summary>
        ///     Handles the Toggled event of the Sounds control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Sounds_Toggled(object sender, RoutedEventArgs e)
        {
            Prefs.PutBoolean("enable_sounds", Sounds.IsOn);
        }

        /// <summary>
        ///     Handles the SelectionChanged event of the Language control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInit)
            {
                if (e.AddedItems.Any())
                {
                    var selectedItem = (ComboBoxItem) e.AddedItems[0];

                    Prefs.PutString("Language", selectedItem.Name);

                    string restartStringText = Strings.GetString("preferences.restartAppText");
                    string restartStringTitle = Strings.GetString("preferences.restartAppTitle");
                    var dialog = new MessageDialog(restartStringText,
                        restartStringTitle);
                    dialog.ShowAsync();
                }
            }
        }
    }
}