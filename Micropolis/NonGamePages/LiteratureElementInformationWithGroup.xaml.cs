using Windows.UI.Core;
using Micropolis.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Geteilte Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234234 dokumentiert.

namespace Micropolis.NonGamePages
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
    /// Eine Seite, auf der ein Gruppentitel, eine Liste mit Elementen innerhalb der Gruppe sowie Details für das
    /// derzeit ausgewählte Element angezeigt werden.
    /// </summary>
    public sealed partial class LiteratureElementInformationWithGroup : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Dies kann in ein stark typisiertes Anzeigemodell geändert werden.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper wird auf jeder Seite zur Unterstützung bei der Navigation verwendet und 
        /// Verwaltung der Prozesslebensdauer
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public LiteratureElementInformationWithGroup()
        {
            InitializeComponent();

            // Navigationshilfe einrichten
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;

            // Die Komponenten der logischen Seitennavigation einrichten, die
            // die Seite, nur einen Bereich gleichzeitig anzuzeigen.
            navigationHelper.GoBackCommand = new RelayCommand(() => GoBack(), () => CanGoBack());
            itemListView.SelectionChanged += itemListView_SelectionChanged;

            // Mit dem Lauschen auf Änderungen der Fenstergröße beginnen 
            // um von der Anzeige von zwei Bereichen zur Anzeige eines Bereichs zu wechseln
            Window.Current.SizeChanged += Window_SizeChanged;
            InvalidateVisualState();
        }

        void itemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsingLogicalPageNavigation())
            {
                navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird.  Gespeicherte Zustände werden ebenfalls
        /// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
        /// </summary>
        /// <param name="sender">
        /// Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Ereignisdaten, die die Navigationsparameter bereitstellen, die an
        /// <see cref="Frame.Navigate(Type, Object)"/> als diese Seite ursprünglich angefordert wurde und
        /// ein Wörterbuch des Zustands, der von dieser Seite während einer früheren
        /// beibehalten wurde.  Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Me.DefaultViewModel("Group") eine bindbare Gruppe zuweisen
            // TODO: Me.DefaultViewModel("Items") eine Auflistung von bindbaren Elementen zuweisen

            if (e.PageState == null)
            {
                // Wenn es sich hierbei um eine neue Seite handelt, das erste Element automatisch auswählen, außer wenn
                // logische Seitennavigation verwendet wird (weitere Informationen in der #Region zur logischen Seitennavigation unten).
                if (!UsingLogicalPageNavigation() && itemsViewSource.View != null)
                {
                    itemsViewSource.View.MoveCurrentToFirst();
                }
            }
            else
            {
                // Den zuvor gespeicherten Zustand wiederherstellen, der dieser Seite zugeordnet ist
                if (e.PageState.ContainsKey("SelectedItem") && itemsViewSource.View != null)
                {
                    // TODO: Me.itemsViewSource.View.MoveCurrentTo() mit dem ausgewählten
                    //       Element aufrufen, wie durch den Wert von pageState("SelectedItem") angegeben

                }
            }
        }

        /// <summary>
        /// Behält den dieser Seite zugeordneten Zustand bei, wenn die Anwendung angehalten oder
        /// die Seite im Navigationscache verworfen wird.  Die Werte müssen den Serialisierungsanforderungen
        /// von <see cref="SuspensionManager.SessionState"/> entsprechen.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/></param>
        /// <param name="e">Ereignisdaten, die ein leeres Wörterbuch zum Auffüllen bereitstellen
        /// serialisierbarer Zustand.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (itemsViewSource.View != null)
            {
                // TODO: Einen serialisierbaren Navigationsparameter ableiten und ihn
                //       pageState("SelectedItem")

            }
        }

        #region Logische Seitennavigation

        // Die geteilte Seite ist so entworfen, dass wenn Fenster nicht genug Raum hat, um sowohl
        // die Liste als auch die Details anzuzeigen, nur jeweils ein Bereich angezeigt wird.
        //
        // All dies wird mit einer einzigen physischen Seite implementiert, die zwei logische Seiten darstellen
        // kann.  Mit dem nachfolgenden Code wird dieses Ziel erreicht, ohne dass der Benutzer aufmerksam gemacht wird auf den
        // Unterschied.

        private const int MinimumWidthForSupportingTwoPanes = 768;

        /// <summary>
        /// Wird aufgerufen, um zu bestimmen, ob die Seite als eine logische Seite oder zwei agieren soll.
        /// </summary>
        /// <returns>True, wenn das Fenster als eine logische Seite fungieren soll, false
        /// in anderen Fällen.</returns>
        private bool UsingLogicalPageNavigation()
        {
            return Window.Current.Bounds.Width < MinimumWidthForSupportingTwoPanes;
        }

        /// <summary>
        /// Mit der Änderung der Fenstergröße aufgerufen
        /// </summary>
        /// <param name="sender">Das aktuelle Fenster</param>
        /// <param name="e">Ereignisdaten, die die neue Größe des Fensters beschreiben</param>
        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            InvalidateVisualState();
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein Element innerhalb der Liste ausgewählt wird.
        /// </summary>
        /// <param name="sender">Die GridView, die das ausgewählte Element anzeigt.</param>
        /// <param name="e">Ereignisdaten, die beschreiben, wie die Auswahl geändert wurde.</param>
        private void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Den Ansichtszustand ungültig machen, wenn die logische Seitennavigation wirksam ist, da eine Änderung an der
            // Auswahl zu einer entsprechenden Änderung an der aktuellen logischen Seite führen kann.  Wenn
            // ein Element ausgewählt wird, führt dies dazu, dass anstelle der Elementliste
            // die Details des ausgewählten Elements angezeigt werden.  Wenn die Auswahl aufgehoben wird, hat
            // dies den umgekehrten Effekt.
            if (UsingLogicalPageNavigation()) InvalidateVisualState();
        }

        private bool CanGoBack()
        {
            if (UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                return true;
            }
            else
            {
                return navigationHelper.CanGoBack();
            }
        }
        private void GoBack()
        {
            if (UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                // Wenn die logische Seitennavigation wirksam ist und ein ausgewähltes Element vorliegt, werden die
                // Details dieses Elements aktuell angezeigt.  Beim Aufheben der Auswahl wird die
                // Elementliste wieder aufgerufen.  Aus Sicht des Benutzers ist dies eine logische Rückwärtsnavigation
                // Rückwärtsnavigation.
                itemListView.SelectedItem = null;
            }
            else
            {
                navigationHelper.GoBack();
            }
        }

        private void InvalidateVisualState()
        {
            var visualState = DetermineVisualState();
            VisualStateManager.GoToState(this, visualState, false);
            navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Wird aufgerufen, um den Namen des visuellen Zustands zu bestimmen, der dem Ansichtszustand einer Anwendung
        /// entspricht.
        /// </summary>
        /// <returns>Der Name des gewünschten visuellen Zustands.  Dieser ist identisch mit dem Namen des
        /// Ansichtszustands, außer wenn ein ausgewähltes Element im Hochformat und in der angedockten Ansicht vorliegt, wobei
        /// diese zusätzliche logische Seite durch Hinzufügen des Suffix _Detail dargestellt wird.</returns>
        private string DetermineVisualState()
        {
            if (!UsingLogicalPageNavigation())
                return "PrimaryView";

            // Den Aktivierungszustand der Schaltfläche "Zurück" aktualisieren, wenn der Ansichtszustand geändert wird
            var logicalPageBack = UsingLogicalPageNavigation() && itemListView.SelectedItem != null;

            return logicalPageBack ? "SinglePane_Detail" : "SinglePane";
        }

        #endregion

        #region NavigationHelper-Registrierung

        /// Die in diesem Abschnitt bereitgestellten Methoden werden einfach verwendet, um
        /// damit NavigationHelper auf die Navigationsmethoden der Seite reagieren kann.
        /// 
        /// Platzieren Sie seitenspezifische Logik in Ereignishandlern für  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// und <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// Der Navigationsparameter ist in der LoadState-Methode verfügbar 
        /// zusätzlich zum Seitenzustand, der während einer früheren Sitzung beibehalten wurde.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
