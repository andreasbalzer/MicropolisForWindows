using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Micropolis.Model.Entities;
using Micropolis.Screens;
using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



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
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application, ISupportsAppCommands
    {
        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public TelemetryClient _telemetry;

        private Frame rootFrame;


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            _telemetry = new Microsoft.ApplicationInsights.TelemetryClient();

            this.InitializeComponent();
            try
            {
                _telemetry = new TelemetryClient();
            }
            catch (Exception) { }

            AppCommands = new List<AppCommand>();
            CheckVersion();
            Suspending += OnSuspending;
            UnhandledException += App_UnhandledException;
            Resuming += App_Resuming;

        }


        /// <summary>
        ///     Indicates if user navigated away from main page.
        /// </summary>
        public static bool IsNavigatedAway { get; set; }

        /// <summary>
        ///     Gets or sets the main page reference. It is used to signal MainGamePage of updates such as app suspension and helps
        ///     to get access to navigation frame.
        /// </summary>
        /// <value>
        ///     The main page reference.
        /// </value>
        public static MainGamePage MainPageReference { get; set; }

        /// <summary>
        ///     Gets or sets the main menu reference. It is used to signal MainGamePage of updates such as app suspension and helps
        ///     to get access to navigation frame.
        /// </summary>
        /// <value>
        ///     The main page reference.
        /// </value>
        public static MainMenuPage MainMenuReference { get; set; }

        /// <summary>
        ///     Gets or sets the load page reference. It is used to get a dispatcher during load of components.
        /// </summary>
        /// <value>
        ///     The load page reference.
        /// </value>
        public static LoadPage LoadPageReference { get; set; }

        /// <summary>
        ///     Gets the application commands used to signal game components for updates, e.g. to load a specific file or to skip
        ///     menu page and directly load maingamepage.
        /// </summary>
        /// <value>
        ///     The application commands.
        /// </value>
        public List<AppCommand> AppCommands { get; private set; }

        private void App_Resuming(object sender, object e)
        {
            if (MainPageReference != null)
            {
                try
                {
                    _telemetry.TrackEvent("AppResumed");
                }
                catch (Exception) { }
                MainPageReference.ViewModel.OnWindowReopend();
            }
        }

        /// <summary>
        ///     Checks for previously installed game versions and updates the game to the current version.
        /// </summary>
        private void CheckVersion()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Version"))
            {
                string versionStored = ApplicationData.Current.RoamingSettings.Values["Version"].ToString();
                bool isVersion100 = versionStored == "1.00";
                if (isVersion100)
                {
                    AppCommands.Add(new AppCommand(Micropolis.Model.Entities.AppCommands.UPDATEDVERSION, "informAboutTelemetry"));
                    try
                    {
                        _telemetry.TrackEvent("AppUpdatedFrom1.00");
                    }
                    catch (Exception) { }
                }
            }
            // ToDo: add version checks for new version

            Prefs.PutString("Version", "1.01");

        }


        /// <summary>
        ///     Handles the UnhandledException event of the App control to catch them during debug.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                _telemetry.TrackException(e.Exception);
            }
            catch (Exception) { }

            var x = ""; // Bug: Todo
            //e.Handled = true;
            var dialog =
                new MessageDialog(
                    "Unfortunately Micropolis experienced an issue it could not resolve.\n The game will now exit and you can manually restart it. Provided you participate in the Microsoft Customer Experience Improvement Program, we will then be notified of the issue and do our very best to fix it.",
                    "Your citizens riot.");

            await dialog.ShowAsync();
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(LoadPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            try
            {
                _telemetry.TrackEvent("AppNavigationFailed" + e.SourcePageType.FullName);
                _telemetry.TrackException(e.Exception);
            }
            catch (Exception) { }

            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden
            try
            {
                _telemetry.TrackEvent("AppSuspending");
            }
            catch (Exception) { }

            if (MainPageReference != null)
            {
                MainPageReference.ViewModel.OnAppClosed();
            }

            deferral.Complete();
        }

        /// <summary>
        ///     Wird aufgerufen, wenn die Anwendung durch den Vorgang "Datei öffnen" aktiviert wird.
        /// </summary>
        /// <param name="args">Die Ereignisdaten für das Ereignis.</param>
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            try
            {
                _telemetry.TrackEvent("AppLoadFileViaFileHandler");
            }
            catch (Exception) { }
            // TODO: check if there is any game running already.

            if (args.Files.Any())
            {
                AppCommands.Add(new AppCommand(Model.Entities.AppCommands.SKIPMENU));
                AppCommands.Add(new AppCommand(Model.Entities.AppCommands.LOADFILE, args.Files[0]));
            }
            if (rootFrame != null)
            {
                rootFrame.Navigate(typeof(LoadPage), null);
            }
            else
            {
                rootFrame = Window.Current.Content as Frame;
                // App-Initialisierung nicht wiederholen, wenn das Fenster bereits Inhalte enthält.
                // Nur sicherstellen, dass das Fenster aktiv ist.
                if (rootFrame == null)
                {
                    // Einen Rahmen erstellen, der als Navigationskontext fungiert und zum Parameter der ersten Seite navigieren
                    rootFrame = new Frame();
                    // Standardsprache festlegen
                    rootFrame.Language = ApplicationLanguages.Languages[0];

                    rootFrame.NavigationFailed += OnNavigationFailed;

                    // Den Rahmen im aktuellen Fenster platzieren
                    Window.Current.Content = rootFrame;
                }

                if (rootFrame.Content == null)
                {
                    // Wenn der Navigationsstapel nicht wiederhergestellt wird, zur ersten Seite navigieren
                    // und die neue Seite konfigurieren, indem die erforderlichen Informationen als Navigationsparameter
                    // übergeben werden
                    rootFrame.Navigate(typeof(LoadPage), "");
                }
                // Sicherstellen, dass das aktuelle Fenster aktiv ist
                Window.Current.Activate();
            }
        }

    }
}
