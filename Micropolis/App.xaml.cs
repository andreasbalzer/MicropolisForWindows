using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Micropolis.Model.Entities;
using Micropolis.Screens;
using Microsoft.ApplicationInsights;
using WinRTXamlToolkit.Controls;

// Die Vorlage "Leere Anwendung" ist unter http://go.microsoft.com/fwlink/?LinkId=234227 dokumentiert.

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
    ///     Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
    /// </summary>
    sealed partial class App : Application, ISupportsAppCommands
    {
        
        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public TelemetryClient _telemetry;

        private Frame rootFrame;

        /// <summary>
        ///     Initialisiert das Singletonanwendungsobjekt.  Dies ist die erste Zeile von erstelltem Code
        ///     und daher das logische Äquivalent von main() bzw. WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            try { 
                _telemetry=new TelemetryClient();
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
                if (Convert.ToDouble(ApplicationData.Current.RoamingSettings.Values["Version"]) == 1.00)
                {
                    AppCommands.Add(new AppCommand(Micropolis.Model.Entities.AppCommands.UPDATEDVERSION,"informAboutTelemetry"));
                    try { 
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
        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try { 
                _telemetry.TrackException(e.Exception);
            }
            catch (Exception) { }

            var x = ""; // Bug: Todo
            //e.Handled = true;
            var dialog =
                new MessageDialog(
                    "Unfortunately Micropolis experienced an issue it could not resolve.\n The game will now exit and you can manually restart it. Provided you participate in the Microsoft Customer Experience Improvement Program, we will then be notified of the issue and do our very best to fix it.",
                    "Your citizens riot.");

            dialog.ShowAsync();
        }

        /// <summary>
        ///     Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird.  Weitere Einstiegspunkte
        ///     werden z. B. verwendet, wenn die Anwendung gestartet wird, um eine bestimmte Datei zu öffnen.
        /// </summary>
        /// <param name="e">Details über Startanforderung und -prozess.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    this.DebugSettings.EnableFrameRateCounter = true;
            //}
#endif

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

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Zustand von zuvor angehaltener Anwendung laden
                    try { 
                        _telemetry.TrackEvent("AppPreviouslyTerminated");
                    }
                    catch (Exception) { }
                }

                // Den Rahmen im aktuellen Fenster platzieren
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Wenn der Navigationsstapel nicht wiederhergestellt wird, zur ersten Seite navigieren
                // und die neue Seite konfigurieren, indem die erforderlichen Informationen als Navigationsparameter
                // übergeben werden
                rootFrame.Navigate(typeof (LoadPage), e.Arguments);
            }

            // Sicherstellen, dass das aktuelle Fenster aktiv ist
            Window.Current.Activate();
        }

        /// <summary>
        ///     Wird aufgerufen, wenn die Navigation auf eine bestimmte Seite fehlschlägt
        /// </summary>
        /// <param name="sender">Der Rahmen, bei dem die Navigation fehlgeschlagen ist</param>
        /// <param name="e">Details über den Navigationsfehler</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
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
        ///     Wird aufgerufen, wenn die Ausführung der Anwendung angehalten wird.  Der Anwendungszustand wird gespeichert,
        ///     ohne zu wissen, ob die Anwendung beendet oder fortgesetzt wird und die Speicherinhalte dabei
        ///     unbeschädigt bleiben.
        /// </summary>
        /// <param name="sender">Die Quelle der Anhalteanforderung.</param>
        /// <param name="e">Details zur Anhalteanforderung.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden
            try { 
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
            try { 
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
                rootFrame.Navigate(typeof (LoadPage), null);
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
                    rootFrame.Navigate(typeof (LoadPage), "");
                }
                // Sicherstellen, dass das aktuelle Fenster aktiv ist
                Window.Current.Activate();
            }
        }
    }
}