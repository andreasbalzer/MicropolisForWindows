using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;
using Micropolis.Screens;
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
    ///     Initiates settings charm
    /// </summary>
    public static class SettingsCharm
    {
        private static TelemetryClient _telemetry;

        static SettingsCharm()
        {
            _telemetry = new TelemetryClient();
        }
        /// <summary>
        ///     Called when commands are requested by settings charm. Adds an about, preferences, help and privacy command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="SettingsPaneCommandsRequestedEventArgs" /> instance containing the event data.</param>
        public static void OnCommandsInGameRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();

            // Add an About command
            var about = new SettingsCommand("about", Strings.GetString("settingsCharm.About"), handler =>
            {
                _telemetry.TrackEvent("SettingsCharmMainGameAboutClicked");

                var settings = new SettingsFlyout();
                settings.Content = new AboutUserControl();

                settings.Title = Strings.GetString("settingsCharm.About");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(about);

            
            var license = new SettingsCommand("license", Strings.GetString("settingsCharm.License"),
                handler =>
                {
                    _telemetry.TrackEvent("SettingsCharmMainGameLicenseClicked");

                    App.MainPageReference.Frame.Navigate(typeof (LicensePage));
                });

            args.Request.ApplicationCommands.Add(license);

            // Add a Preferences command
            var preferences = new SettingsCommand("preferences", Strings.GetString("settingsCharm.Preferences"), handler =>
            {
                _telemetry.TrackEvent("SettingsCharmMainGamePreferencesClicked");

                var settings = new SettingsFlyout();
                settings.Content = new PreferencesUserControl();
                settings.Title = Strings.GetString("settingsCharm.Preferences");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(preferences);

            // Add an Help command
            var help = new SettingsCommand("help", Strings.GetString("settingsCharm.Help"),
                handler => {
                    _telemetry.TrackEvent("SettingsCharmMainGameHelpClicked");
                    
                    App.MainPageReference.Frame.Navigate(typeof (HelpPage));
                });

            args.Request.ApplicationCommands.Add(help);

            // Add a Privacy command
            var privacy = new SettingsCommand("privacy", Strings.GetString("settingsCharm.Privacy"), handler =>
            {
                _telemetry.TrackEvent("SettingsCharmMainGamePrivacyClicked");

                var settings = new SettingsFlyout();
                settings.Content = new PrivacyUserControl();
                settings.Title = Strings.GetString("settingsCharm.Privacy");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(privacy);
        }

        /// <summary>
        ///     Called when commands are requested by settings charm. Adds an about, preferences, help and privacy command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="SettingsPaneCommandsRequestedEventArgs" /> instance containing the event data.</param>
        public static void OnCommandsInMenuRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();

            // Add an About command
            var about = new SettingsCommand("about", Strings.GetString("settingsCharm.About"), handler =>
            {
                _telemetry.TrackEvent("SettingsCharmMainMenuAboutClicked");

                var settings = new SettingsFlyout();
                settings.Content = new AboutUserControl();

                settings.Title = Strings.GetString("settingsCharm.About");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(about);

            var license = new SettingsCommand("license", Strings.GetString("settingsCharm.License"),
                handler => {
                    _telemetry.TrackEvent("SettingsCharmMainMenuLicenseClicked");

                    App.MainMenuReference.Frame.Navigate(typeof (LicensePage));
            });

            args.Request.ApplicationCommands.Add(license);

            // Add a Preferences command
            var preferences = new SettingsCommand("preferences", Strings.GetString("settingsCharm.Preferences"), handler =>
            {
                _telemetry.TrackEvent("SettingsCharmMainMenuPreferencesClicked");

                var settings = new SettingsFlyout();
                settings.Content = new PreferencesUserControl();
                settings.Title = Strings.GetString("settingsCharm.Preferences");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(preferences);

            // Add an Help command
            var help = new SettingsCommand("help", Strings.GetString("settingsCharm.Help"),
                handler =>
                {
                    _telemetry.TrackEvent("SettingsCharmMainMenuHelpClicked");

                    App.MainMenuReference.Frame.Navigate(typeof (HelpPage));
                });

            args.Request.ApplicationCommands.Add(help);

            // Add a Privacy command
            var privacy = new SettingsCommand("privacy", Strings.GetString("settingsCharm.Privacy"), handler =>
            {
                _telemetry.TrackEvent("SettingsCharmMainMenuPrivacyClicked");

                var settings = new SettingsFlyout();
                settings.Content = new PrivacyUserControl();
                settings.Title = Strings.GetString("settingsCharm.Privacy");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(privacy);
        }
    }
}