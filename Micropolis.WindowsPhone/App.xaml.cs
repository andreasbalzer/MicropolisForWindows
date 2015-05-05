using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Micropolis.Model.Entities;
using Micropolis.Screens;
using Microsoft.ApplicationInsights;

// Die Vorlage "Leere Anwendung" ist unter http://go.microsoft.com/fwlink/?LinkId=391641 dokumentiert.

namespace Micropolis
{
    /// <summary>
    ///     Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
    /// </summary>
    public sealed partial class App : Application, ISupportsAppCommands
    {
        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public TelemetryClient TelemetryClient = new TelemetryClient();

        private Frame rootFrame;
        private TransitionCollection transitions;

        /// <summary>
        ///     Initialisiert das Singletonanwendungsobjekt.  Dies ist die erste Zeile von erstelltem Code
        ///     und daher das logische Äquivalent von main() bzw. WinMain().
        /// </summary>
        public App()
        {
            TelemetryClient = new TelemetryClient();

            InitializeComponent();
            AppCommands = new List<AppCommand>();
            CheckVersion();
            Suspending += OnSuspending;
            UnhandledException += App_UnhandledException;
            Resuming += App_Resuming;
        }

        private async Task HideSystemTray()
        {
            StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

            // Hide the status bar
            await statusBar.HideAsync();
        }

        /// <summary>
        ///     Indicates if user navigated away from main page.
        /// </summary>
        public static bool IsNavigatedAway { get; set; }

        public static MainGamePage MainPageReference { get; set; }
        public static LoadPage LoadPageReference { get; set; }

        /// <summary>
        ///     Gets or sets the main menu reference. It is used to signal MainGamePage of updates such as app suspension and helps
        ///     to get access to navigation frame.
        /// </summary>
        /// <value>
        ///     The main page reference.
        /// </value>
        public static MainMenuPage MainMenuReference { get; set; }

        /// <summary>
        ///     App commands currently active.
        /// </summary>
        public List<AppCommand> AppCommands { get; set; }

        private void App_Resuming(object sender, object e)
        {
            if (MainPageReference != null)
            {
                MainPageReference.ViewModel.OnWindowReopend();
            }
        }

        /// <summary>
        ///     Checks for previously installed game versions and updates the game to the current version.
        /// </summary>
        private void CheckVersion()
        {
            /*if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Version"))
            {
                if (Convert.ToDouble(ApplicationData.Current.RoamingSettings.Values["Version"]) <= 2)
                {
                    AppCommands.Add(new AppCommand(AppCommands.UpdatedVersion));    
                }
            }
             // ToDo: add version checks for new version
             */
            Prefs.PutString("Version", "1.00");
        }

        /// <summary>
        ///     Handles the UnhandledException event of the App control to catch them during debug.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
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
        ///     werden verwendet, wenn die Anwendung zum Öffnen einer bestimmten Datei, zum Anzeigen
        ///     von Suchergebnissen usw. gestartet wird.
        /// </summary>
        /// <param name="e">Details über Startanforderung und -prozess.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            HideSystemTray();

            rootFrame = Window.Current.Content as Frame;

            // App-Initialisierung nicht wiederholen, wenn das Fenster bereits Inhalte enthält.
            // Nur sicherstellen, dass das Fenster aktiv ist.
            if (rootFrame == null)
            {
                // Frame erstellen, der als Navigationskontext fungiert und zum Parameter der ersten Seite navigieren
                rootFrame = new Frame();

                // TODO: diesen Wert auf eine Cachegröße ändern, die für Ihre Anwendung geeignet ist
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Zustand von zuvor angehaltener Anwendung laden
                }

                // Den Rahmen im aktuellen Fenster platzieren
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Entfernt die Drehkreuznavigation für den Start.
                if (rootFrame.ContentTransitions != null)
                {
                    transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrame_FirstNavigated;

                // Wenn der Navigationsstapel nicht wiederhergestellt wird, zur ersten Seite navigieren
                // und die neue Seite konfigurieren, indem die erforderlichen Informationen als Navigationsparameter
                // übergeben werden
                if (!rootFrame.Navigate(typeof (LoadPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Sicherstellen, dass das aktuelle Fenster aktiv ist
            Window.Current.Activate();
        }

        /// <summary>
        ///     Stellt die Inhaltsübergänge nach dem Start der App wieder her.
        /// </summary>
        /// <param name="sender">Das Objekt, an das der Handler angefügt wird.</param>
        /// <param name="e">Details zum Navigationsereignis.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = transitions ?? new TransitionCollection {new NavigationThemeTransition()};
            rootFrame.Navigated -= RootFrame_FirstNavigated;
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

        /// <summary>
        ///     Wird aufgerufen, wenn die Navigation auf eine bestimmte Seite fehlschlägt
        /// </summary>
        /// <param name="sender">Der Rahmen, bei dem die Navigation fehlgeschlagen ist</param>
        /// <param name="e">Details über den Navigationsfehler</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}