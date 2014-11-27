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

    using System;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Engine;

    /// <summary>
    /// NotificationPane to inform user about important news and to give query tool.
    /// </summary>
    public sealed partial class NotificationPane
    {
        private static readonly Size VIEWPORT_SIZE = new Size(160, 160);
        private static readonly SolidColorBrush QUERY_COLOR = new SolidColorBrush(Color.FromArgb(255, 255, 165, 0));
        private MainGamePage _mainPage;

        /// <summary>
        /// Initiates a new instance of the NotificationPane control.
        /// </summary>
        public NotificationPane()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets up this instance after basic initialization.
        /// </summary>
        /// <param name="mainPage"></param>
        public void SetUpAfterBasicInit(MainGamePage mainPage)
        {
            _mainPage = mainPage;
            dismissBtn.Content = Strings.GetString("notification.dismiss");
            dismissBtn.Click += delegate { OnDismissClicked(); };
        }

        /// <summary>
        /// Called when user clicked the dismiss button to close the pane.
        /// </summary>
        private void OnDismissClicked()
        {
            _mainPage.HideNotificationPanel();
        }

        /// <summary>
        /// Sets the picture of the panel.
        /// </summary>
        /// <param name="xpos">xpos in map</param>
        /// <param name="ypos">ypos in map</param>
        private void SetPicture(int xpos, int ypos)
        {
            Size sz = VIEWPORT_SIZE;

            WriteableBitmap map = _mainPage.GetLandscapeFromDrawingArea(xpos, ypos, sz);

            ImageOutput.Source = map;
        }

        /// <summary>
        /// Shows the specified message for the specified map coordinates.
        /// </summary>
        /// <param name="msg">message to show</param>
        /// <param name="xpos">xpos in map</param>
        /// <param name="ypos">ypos in map</param>
        public void ShowMessage(MicropolisMessage msg, int xpos, int ypos)
        {
            SetPicture(xpos, ypos);

            if (infoPane.Visibility == Visibility.Visible)
            {
                infoPane.Visibility = Visibility.Collapsed;
            }

            headerLbl.Text = Strings.GetString(msg.Name + ".title");
            headerSP.Background = new SolidColorBrush(ColorParser.ParseColor(Strings.GetString(msg.Name + ".color")));

            var textBlock = new TextBlock {Text = Strings.GetString(msg.Name + ".detail")};
            detailPane.Children.Add(textBlock);
            detailPane.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Show zone status for specified coordinates and zonestatus
        /// </summary>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        /// <param name="zone"></param>
        public void ShowZoneStatus(int xpos, int ypos, ZoneStatus zone)
        {
            detailPane.Visibility = Visibility.Collapsed;

            headerLbl.Text = Strings.GetString("notification.query_hdr");
            headerSP.Background = QUERY_COLOR;

            String buildingStr = zone.Building != -1 ? Strings.GetString("zone." + zone.Building) : "";
            String popDensityStr = Strings.GetString("status." + zone.PopDensity);
            String landValueStr = Strings.GetString("status." + zone.LandValue);
            String crimeLevelStr = Strings.GetString("status." + zone.CrimeLevel);
            String pollutionStr = Strings.GetString("status." + zone.Pollution);
            String growthRateStr = Strings.GetString("status." + zone.GrowthRate);

            SetPicture(xpos, ypos);
            infoPane.Visibility = Visibility.Visible;

            t1textBlock.Text = Strings.GetString("notification.zone_lbl");
            buildStrTextBlock.Text = buildingStr;
            notificationDensityTextBlock.Text = Strings.GetString("notification.density_lbl");
            popDensityStrTextBlock.Text = popDensityStr;
            notificationDensityTextBlock.Text = Strings.GetString("notification.value_lbl");
            landValueStrTextBlock.Text = landValueStr;
            notificationCrimeTextBlock.Text = Strings.GetString("notification.crime_lbl");
            crimeLevelStrTextBlock.Text = crimeLevelStr;
            notificationPollutionTextBlock.Text = Strings.GetString("notification.pollution_lbl");
            pollutionStrTextBlock.Text = pollutionStr;
            notificationGrowthTextBlock.Text = Strings.GetString("notification.growth_lbl");
            growthRateStrTextBlock.Text = growthRateStr;
            _mainPage.ShowNotificationPanel();
        }
    }
}