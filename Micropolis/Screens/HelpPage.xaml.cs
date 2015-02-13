using Windows.UI.Xaml.Documents;
using Micropolis.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.ApplicationInsights;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

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

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class HelpPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public HelpPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            HelpTextTB.Inlines.Clear();
           
            HelpTextTB.Inlines.Add(new Run(){Text = Strings.GetString("help.Text")});
           
            helpTitle.Inlines.Clear();
            helpP1.Inlines.Clear();
            helpP2.Inlines.Clear();
            helpP3.Inlines.Clear();
            helpP4.Inlines.Clear();
            helpP5.Inlines.Clear();
            helpP6.Inlines.Clear();
            helpP7.Inlines.Clear();
            helpP8.Inlines.Clear();
            helpP9.Inlines.Clear();
            helpP10.Inlines.Clear();
            helpP11.Inlines.Clear();
            helpP12.Inlines.Clear();
            helpP13.Inlines.Clear();
            helpP14.Inlines.Clear();
            helpP15.Inlines.Clear();
            helpP16.Inlines.Clear();
            helpP17.Inlines.Clear();
            helpP18.Inlines.Clear();
            helpP19.Inlines.Clear();
            helpP20.Inlines.Clear();
            helpP21.Inlines.Clear();
            helpP22.Inlines.Clear();
            helpP23.Inlines.Clear();
            helpP24.Inlines.Clear();
            helpP25.Inlines.Clear();
            helpP26.Inlines.Clear();
            helpP27.Inlines.Clear();
            helpP28.Inlines.Clear();
            helpP29.Inlines.Clear();
            helpP30.Inlines.Clear();
            helpP31.Inlines.Clear();
            helpP32.Inlines.Clear();
            helpP33.Inlines.Clear();
            helpP34.Inlines.Clear();
            helpP35.Inlines.Clear();
            helpP36.Inlines.Clear();
            helpP37.Inlines.Clear();
            helpP38.Inlines.Clear();
            helpP39.Inlines.Clear();
            helpP40.Inlines.Clear();
            helpP41.Inlines.Clear();
            helpP42.Inlines.Clear();
            helpP43.Inlines.Clear();
            helpP44.Inlines.Clear();
            helpP45.Inlines.Clear();
            helpP46.Inlines.Clear();
            helpP47.Inlines.Clear();
            helpP48.Inlines.Clear();
            helpP49.Inlines.Clear();
            helpP50.Inlines.Clear();
            helpP51.Inlines.Clear();
            helpP52.Inlines.Clear();
            helpP53.Inlines.Clear();
            helpP54.Inlines.Clear();
            helpP55.Inlines.Clear();
            helpP56.Inlines.Clear();
            helpP57.Inlines.Clear();
            helpP58.Inlines.Clear();
            helpP59.Inlines.Clear();
            helpP60.Inlines.Clear();
            helpP61.Inlines.Clear();
            helpP62.Inlines.Clear();
            helpP63.Inlines.Clear();
            helpP64.Inlines.Clear();
            helpP65.Inlines.Clear();
            helpP66.Inlines.Clear();
            helpP67.Inlines.Clear();
            helpP68.Inlines.Clear();
            helpP69.Inlines.Clear();
            helpP70.Inlines.Clear();
            helpP71.Inlines.Clear();
            helpP72.Inlines.Clear();
            helpP73.Inlines.Clear();
            helpP74.Inlines.Clear();
            helpP75.Inlines.Clear();
            helpP76.Inlines.Clear();
            helpP77.Inlines.Clear();
            helpP78.Inlines.Clear();
            helpP79.Inlines.Clear();
            helpP80.Inlines.Clear();
            helpP81.Inlines.Clear();
            helpP82.Inlines.Clear();
            helpP83.Inlines.Clear();
            helpP84.Inlines.Clear();
            helpP85.Inlines.Clear();
            helpP86.Inlines.Clear();
            helpP87.Inlines.Clear();
            helpP88.Inlines.Clear();
            helpP89.Inlines.Clear();
            helpP90.Inlines.Clear();
            helpP91.Inlines.Clear();
            helpP92.Inlines.Clear();
            helpP93.Inlines.Clear();
            helpP94.Inlines.Clear();
            helpP95.Inlines.Clear();
            helpP96.Inlines.Clear();
            helpP97.Inlines.Clear();
            helpP98.Inlines.Clear();
            helpP99.Inlines.Clear();
            helpP100.Inlines.Clear();
            helpP101.Inlines.Clear();
            helpP102.Inlines.Clear();
            helpP103.Inlines.Clear();
            helpP104.Inlines.Clear();
            helpP105.Inlines.Clear();
            helpP106.Inlines.Clear();
            helpP107.Inlines.Clear();
            helpP108.Inlines.Clear();
            helpP109.Inlines.Clear();
            helpP110.Inlines.Clear();
            helpP111.Inlines.Clear();
            helpP112.Inlines.Clear();
            helpP113.Inlines.Clear();
            helpP114.Inlines.Clear();
            helpP115.Inlines.Clear();
            helpP116.Inlines.Clear();
            helpP117.Inlines.Clear();
            helpP118.Inlines.Clear();
            helpP119.Inlines.Clear();
            helpP120.Inlines.Clear();
            helpP121.Inlines.Clear();
            helpP122.Inlines.Clear();
            helpP123.Inlines.Clear();
            helpP124.Inlines.Clear();
            helpP125.Inlines.Clear();
            helpP126.Inlines.Clear();
            helpP127.Inlines.Clear();
            helpP128.Inlines.Clear();
            helpP129.Inlines.Clear();
            helpP130.Inlines.Clear();
            helpP131.Inlines.Clear();
            helpP132.Inlines.Clear();
            helpP133.Inlines.Clear();
            helpP134.Inlines.Clear();
            helpP135.Inlines.Clear();
            helpP136.Inlines.Clear();
            helpP137.Inlines.Clear();
            helpP138.Inlines.Clear();
            helpP139.Inlines.Clear();
            helpP140.Inlines.Clear();
            helpP141.Inlines.Clear();
            helpP142.Inlines.Clear();
            helpP143.Inlines.Clear();
            helpP144.Inlines.Clear();
            helpP145.Inlines.Clear();


            helpTitle.Inlines.Add(new Run() { Text = Strings.GetString("help.Title") });
            helpP1.Inlines.Add(new Run() {Text = Strings.GetString("help.P1")});
            helpP2.Inlines.Add(new Run() {Text = Strings.GetString("help.P2")});
            helpP3.Inlines.Add(new Run() {Text = Strings.GetString("help.P3")});
            helpP4.Inlines.Add(new Run() {Text = Strings.GetString("help.P4")});
            helpP5.Inlines.Add(new Run() {Text = Strings.GetString("help.P5")});
            helpP6.Inlines.Add(new Run() {Text = Strings.GetString("help.P6")});
            helpP7.Inlines.Add(new Run() {Text = Strings.GetString("help.P7")});
            helpP8.Inlines.Add(new Run() {Text = Strings.GetString("help.P8")});
            helpP9.Inlines.Add(new Run() {Text = Strings.GetString("help.P9")});
            helpP10.Inlines.Add(new Run() {Text = Strings.GetString("help.P10")});
            helpP11.Inlines.Add(new Run() {Text = Strings.GetString("help.P11")});
            helpP12.Inlines.Add(new Run() {Text = Strings.GetString("help.P12")});
            helpP13.Inlines.Add(new Run() {Text = Strings.GetString("help.P13")});
            helpP14.Inlines.Add(new Run() {Text = Strings.GetString("help.P14")});
            helpP15.Inlines.Add(new Run() {Text = Strings.GetString("help.P15")});
            helpP16.Inlines.Add(new Run() {Text = Strings.GetString("help.P16")});
            helpP17.Inlines.Add(new Run() {Text = Strings.GetString("help.P17")});
            helpP18.Inlines.Add(new Run() {Text = Strings.GetString("help.P18")});
            helpP19.Inlines.Add(new Run() {Text = Strings.GetString("help.P19")});
            helpP20.Inlines.Add(new Run() {Text = Strings.GetString("help.P20")});
            helpP21.Inlines.Add(new Run() {Text = Strings.GetString("help.P21")});
            helpP22.Inlines.Add(new Run() {Text = Strings.GetString("help.P22")});
            helpP23.Inlines.Add(new Run() {Text = Strings.GetString("help.P23")});
            helpP24.Inlines.Add(new Run() {Text = Strings.GetString("help.P24")});
            helpP25.Inlines.Add(new Run() {Text = Strings.GetString("help.P25")});
            helpP26.Inlines.Add(new Run() {Text = Strings.GetString("help.P26")});
            helpP27.Inlines.Add(new Run() {Text = Strings.GetString("help.P27")});
            helpP28.Inlines.Add(new Run() {Text = Strings.GetString("help.P28")});
            helpP29.Inlines.Add(new Run() {Text = Strings.GetString("help.P29")});
            helpP30.Inlines.Add(new Run() {Text = Strings.GetString("help.P30")});
            helpP31.Inlines.Add(new Run() {Text = Strings.GetString("help.P31")});
            helpP32.Inlines.Add(new Run() {Text = Strings.GetString("help.P32")});
            helpP33.Inlines.Add(new Run() {Text = Strings.GetString("help.P33")});
            helpP34.Inlines.Add(new Run() {Text = Strings.GetString("help.P34")});
            helpP35.Inlines.Add(new Run() {Text = Strings.GetString("help.P35")});
            helpP36.Inlines.Add(new Run() {Text = Strings.GetString("help.P36")});
            helpP37.Inlines.Add(new Run() {Text = Strings.GetString("help.P37")});
            helpP38.Inlines.Add(new Run() {Text = Strings.GetString("help.P38")});
            helpP39.Inlines.Add(new Run() {Text = Strings.GetString("help.P39")});
            helpP40.Inlines.Add(new Run() {Text = Strings.GetString("help.P40")});
            helpP41.Inlines.Add(new Run() {Text = Strings.GetString("help.P41")});
            helpP42.Inlines.Add(new Run() {Text = Strings.GetString("help.P42")});
            helpP43.Inlines.Add(new Run() {Text = Strings.GetString("help.P43")});
            helpP44.Inlines.Add(new Run() {Text = Strings.GetString("help.P44")});
            helpP45.Inlines.Add(new Run() {Text = Strings.GetString("help.P45")});
            helpP46.Inlines.Add(new Run() {Text = Strings.GetString("help.P46")});
            helpP47.Inlines.Add(new Run() {Text = Strings.GetString("help.P47")});
            helpP48.Inlines.Add(new Run() {Text = Strings.GetString("help.P48")});
            helpP49.Inlines.Add(new Run() {Text = Strings.GetString("help.P49")});
            helpP50.Inlines.Add(new Run() {Text = Strings.GetString("help.P50")});
            helpP51.Inlines.Add(new Run() {Text = Strings.GetString("help.P51")});
            helpP52.Inlines.Add(new Run() {Text = Strings.GetString("help.P52")});
            helpP53.Inlines.Add(new Run() {Text = Strings.GetString("help.P53")});
            helpP54.Inlines.Add(new Run() {Text = Strings.GetString("help.P54")});
            helpP55.Inlines.Add(new Run() {Text = Strings.GetString("help.P55")});
            helpP56.Inlines.Add(new Run() {Text = Strings.GetString("help.P56")});
            helpP57.Inlines.Add(new Run() {Text = Strings.GetString("help.P57")});
            helpP58.Inlines.Add(new Run() {Text = Strings.GetString("help.P58")});
            helpP59.Inlines.Add(new Run() {Text = Strings.GetString("help.P59")});
            helpP60.Inlines.Add(new Run() {Text = Strings.GetString("help.P60")});
            helpP61.Inlines.Add(new Run() {Text = Strings.GetString("help.P61")});
            helpP62.Inlines.Add(new Run() {Text = Strings.GetString("help.P62")});
            helpP63.Inlines.Add(new Run() {Text = Strings.GetString("help.P63")});
            helpP64.Inlines.Add(new Run() {Text = Strings.GetString("help.P64")});
            helpP65.Inlines.Add(new Run() {Text = Strings.GetString("help.P65")});
            helpP66.Inlines.Add(new Run() {Text = Strings.GetString("help.P66")});
            helpP67.Inlines.Add(new Run() {Text = Strings.GetString("help.P67")});
            helpP68.Inlines.Add(new Run() {Text = Strings.GetString("help.P68")});
            helpP69.Inlines.Add(new Run() {Text = Strings.GetString("help.P69")});
            helpP70.Inlines.Add(new Run() {Text = Strings.GetString("help.P70")});
            helpP71.Inlines.Add(new Run() {Text = Strings.GetString("help.P71")});
            helpP72.Inlines.Add(new Run() {Text = Strings.GetString("help.P72")});
            helpP73.Inlines.Add(new Run() {Text = Strings.GetString("help.P73")});
            helpP74.Inlines.Add(new Run() {Text = Strings.GetString("help.P74")});
            helpP75.Inlines.Add(new Run() {Text = Strings.GetString("help.P75")});
            helpP76.Inlines.Add(new Run() {Text = Strings.GetString("help.P76")});
            helpP77.Inlines.Add(new Run() {Text = Strings.GetString("help.P77")});
            helpP78.Inlines.Add(new Run() {Text = Strings.GetString("help.P78")});
            helpP79.Inlines.Add(new Run() {Text = Strings.GetString("help.P79")});
            helpP80.Inlines.Add(new Run() {Text = Strings.GetString("help.P80")});
            helpP81.Inlines.Add(new Run() {Text = Strings.GetString("help.P81")});
            helpP82.Inlines.Add(new Run() {Text = Strings.GetString("help.P82")});
            helpP83.Inlines.Add(new Run() {Text = Strings.GetString("help.P83")});
            helpP84.Inlines.Add(new Run() {Text = Strings.GetString("help.P84")});
            helpP85.Inlines.Add(new Run() {Text = Strings.GetString("help.P85")});
            helpP86.Inlines.Add(new Run() {Text = Strings.GetString("help.P86")});
            helpP87.Inlines.Add(new Run() {Text = Strings.GetString("help.P87")});
            helpP88.Inlines.Add(new Run() {Text = Strings.GetString("help.P88")});
            helpP89.Inlines.Add(new Run() {Text = Strings.GetString("help.P89")});
            helpP90.Inlines.Add(new Run() {Text = Strings.GetString("help.P90")});
            helpP91.Inlines.Add(new Run() {Text = Strings.GetString("help.P91")});
            helpP92.Inlines.Add(new Run() {Text = Strings.GetString("help.P92")});
            helpP93.Inlines.Add(new Run() {Text = Strings.GetString("help.P93")});
            helpP94.Inlines.Add(new Run() {Text = Strings.GetString("help.P94")});
            helpP95.Inlines.Add(new Run() {Text = Strings.GetString("help.P95")});
            helpP96.Inlines.Add(new Run() {Text = Strings.GetString("help.P96")});
            helpP97.Inlines.Add(new Run() {Text = Strings.GetString("help.P97")});
            helpP98.Inlines.Add(new Run() {Text = Strings.GetString("help.P98")});
            helpP99.Inlines.Add(new Run() {Text = Strings.GetString("help.P99")});
            helpP100.Inlines.Add(new Run() {Text = Strings.GetString("help.P100")});
            helpP101.Inlines.Add(new Run() {Text = Strings.GetString("help.P101")});
            helpP102.Inlines.Add(new Run() {Text = Strings.GetString("help.P102")});
            helpP103.Inlines.Add(new Run() {Text = Strings.GetString("help.P103")});
            helpP104.Inlines.Add(new Run() {Text = Strings.GetString("help.P104")});
            helpP105.Inlines.Add(new Run() {Text = Strings.GetString("help.P105")});
            helpP106.Inlines.Add(new Run() {Text = Strings.GetString("help.P106")});
            helpP107.Inlines.Add(new Run() {Text = Strings.GetString("help.P107")});
            helpP108.Inlines.Add(new Run() {Text = Strings.GetString("help.P108")});
            helpP109.Inlines.Add(new Run() {Text = Strings.GetString("help.P109")});
            helpP110.Inlines.Add(new Run() {Text = Strings.GetString("help.P110")});
            helpP111.Inlines.Add(new Run() {Text = Strings.GetString("help.P111")});
            helpP112.Inlines.Add(new Run() {Text = Strings.GetString("help.P112")});
            helpP113.Inlines.Add(new Run() {Text = Strings.GetString("help.P113")});
            helpP114.Inlines.Add(new Run() {Text = Strings.GetString("help.P114")});
            helpP115.Inlines.Add(new Run() {Text = Strings.GetString("help.P115")});
            helpP116.Inlines.Add(new Run() {Text = Strings.GetString("help.P116")});
            helpP117.Inlines.Add(new Run() {Text = Strings.GetString("help.P117")});
            helpP118.Inlines.Add(new Run() {Text = Strings.GetString("help.P118")});
            helpP119.Inlines.Add(new Run() {Text = Strings.GetString("help.P119")});
            helpP120.Inlines.Add(new Run() {Text = Strings.GetString("help.P120")});
            helpP121.Inlines.Add(new Run() {Text = Strings.GetString("help.P121")});
            helpP122.Inlines.Add(new Run() {Text = Strings.GetString("help.P122")});
            helpP123.Inlines.Add(new Run() {Text = Strings.GetString("help.P123")});
            helpP124.Inlines.Add(new Run() {Text = Strings.GetString("help.P124")});
            helpP125.Inlines.Add(new Run() {Text = Strings.GetString("help.P125")});
            helpP126.Inlines.Add(new Run() {Text = Strings.GetString("help.P126")});
            helpP127.Inlines.Add(new Run() {Text = Strings.GetString("help.P127")});
            helpP128.Inlines.Add(new Run() {Text = Strings.GetString("help.P128")});
            helpP129.Inlines.Add(new Run() {Text = Strings.GetString("help.P129")});
            helpP130.Inlines.Add(new Run() {Text = Strings.GetString("help.P130")});
            helpP131.Inlines.Add(new Run() {Text = Strings.GetString("help.P131")});
            helpP132.Inlines.Add(new Run() {Text = Strings.GetString("help.P132")});
            helpP133.Inlines.Add(new Run() {Text = Strings.GetString("help.P133")});
            helpP134.Inlines.Add(new Run() {Text = Strings.GetString("help.P134")});
            helpP135.Inlines.Add(new Run() {Text = Strings.GetString("help.P135")});
            helpP136.Inlines.Add(new Run() {Text = Strings.GetString("help.P136")});
            helpP137.Inlines.Add(new Run() {Text = Strings.GetString("help.P137")});
            helpP138.Inlines.Add(new Run() {Text = Strings.GetString("help.P138")});
            helpP139.Inlines.Add(new Run() {Text = Strings.GetString("help.P139")});
            helpP140.Inlines.Add(new Run() {Text = Strings.GetString("help.P140")});
            helpP141.Inlines.Add(new Run() {Text = Strings.GetString("help.P141")});
            helpP142.Inlines.Add(new Run() {Text = Strings.GetString("help.P142")});
            helpP143.Inlines.Add(new Run() {Text = Strings.GetString("help.P143")});
            helpP144.Inlines.Add(new Run() {Text = Strings.GetString("help.P144")});
            helpP145.Inlines.Add(new Run() {Text = Strings.GetString("help.P145")});

            _telemetry = new TelemetryClient();
            _telemetry.TrackPageView("HelpPage");

        }

        private TelemetryClient _telemetry;
        /*
        void Output_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.scrollViewer.MaxWidth = this.HelpContainer.ActualWidth;
            this.scrollViewer.MaxHeight = this.HelpContainer.ActualHeight;
            this.richTextColumns.MaxHeight = this.HelpContainer.ActualHeight;
            // this.richTextColumns.Height = this.Output.ActualHeight;
            this.scrollViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }*/

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

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
