using System;
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
            try { 
                _telemetry = new TelemetryClient();
            }
            catch (Exception) { }
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
                try { 
                    _telemetry.TrackEvent("SettingsCharmMainGameAboutClicked");
                }
                catch (Exception) { }

                var settings = new SettingsFlyout();
                settings.Content = new AboutUserControl();

                settings.Title = Strings.GetString("settingsCharm.About");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(about);

            
            var license = new SettingsCommand("license", Strings.GetString("settingsCharm.License"),
                handler =>
                {
                    try { 
                        _telemetry.TrackEvent("SettingsCharmMainGameLicenseClicked");
                    }
                    catch (Exception) { }

                    App.MainPageReference.Frame.Navigate(typeof (LicensePage));
                });

            args.Request.ApplicationCommands.Add(license);

            // Add a Preferences command
            var preferences = new SettingsCommand("preferences", Strings.GetString("settingsCharm.Preferences"), handler =>
            {
                try { 
                    _telemetry.TrackEvent("SettingsCharmMainGamePreferencesClicked");
                }
                catch (Exception) { }

                var settings = new SettingsFlyout();
                settings.Content = new PreferencesUserControl();
                settings.Title = Strings.GetString("settingsCharm.Preferences");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(preferences);

            // Add an Help command
            var help = new SettingsCommand("help", Strings.GetString("settingsCharm.Help"),
                handler => {
                    try { 
                        _telemetry.TrackEvent("SettingsCharmMainGameHelpClicked");
                    }
                    catch (Exception) { }

                    App.MainPageReference.Frame.Navigate(typeof (HelpPage));
                });

            args.Request.ApplicationCommands.Add(help);

            // Add a Privacy command
            var privacy = new SettingsCommand("privacy", Strings.GetString("settingsCharm.Privacy"), handler =>
            {
                try
                {
                    _telemetry.TrackEvent("SettingsCharmMainGamePrivacyClicked");
                }
                catch (Exception) { }

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
                try { 
                    _telemetry.TrackEvent("SettingsCharmMainMenuAboutClicked");
                }
                catch (Exception) { }

                var settings = new SettingsFlyout();
                settings.Content = new AboutUserControl();

                settings.Title = Strings.GetString("settingsCharm.About");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(about);

            var license = new SettingsCommand("license", Strings.GetString("settingsCharm.License"),
                handler => {
                    try { 
                        _telemetry.TrackEvent("SettingsCharmMainMenuLicenseClicked");
                    }
                    catch (Exception) { }

                    App.MainMenuReference.Frame.Navigate(typeof (LicensePage));
            });

            args.Request.ApplicationCommands.Add(license);

            // Add a Preferences command
            var preferences = new SettingsCommand("preferences", Strings.GetString("settingsCharm.Preferences"), handler =>
            {
                try { 
                    _telemetry.TrackEvent("SettingsCharmMainMenuPreferencesClicked");
                }
                catch (Exception) { }

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
                    try { 
                    _telemetry.TrackEvent("SettingsCharmMainMenuHelpClicked");
                    }
                    catch (Exception) { }

                    App.MainMenuReference.Frame.Navigate(typeof (HelpPage));
                });

            args.Request.ApplicationCommands.Add(help);

            // Add a Privacy command
            var privacy = new SettingsCommand("privacy", Strings.GetString("settingsCharm.Privacy"), handler =>
            {
                try { 
                _telemetry.TrackEvent("SettingsCharmMainMenuPrivacyClicked");
                }
                catch (Exception) { }

                var settings = new SettingsFlyout();
                settings.Content = new PrivacyUserControl();
                settings.Title = Strings.GetString("settingsCharm.Privacy");
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(privacy);
        }
    }
}