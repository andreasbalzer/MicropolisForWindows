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
    using System.Collections.Generic;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Engine;

    /// <summary>
    /// Toolbar control that allows user to select tools. Supports multiple views.
    /// </summary>
    public sealed partial class Toolbar
    {
        private MainGamePage _mainPage;
        private Dictionary<MicropolisTool, RadioButton> _toolBtns;

        private ToolBarMode _mode;

        /// <summary>
        /// Mode of the toolbar.
        /// </summary>
        public ToolBarMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                toolBar.Items.Clear();
                MakeToolbar();
            }
        }

        /// <summary>
        /// Initiates a new instance of this Toolbar control.
        /// </summary>
        public Toolbar()
        {
            InitializeComponent();
            _mode = ToolBarMode.NORMAL;
        }


        /// <summary>
        /// Sets up this instance after basic initalization.
        /// </summary>
        /// <param name="mainPage">Reference to main page</param>
        public void SetUpAfterBasicInit(MainGamePage mainPage)
        {
            _mainPage = mainPage;
            MakeToolbar();
        }

        /// <summary>
        ///     Makes the toolbar with tools to use on map.
        /// </summary>
        /// <returns>the toolbar as stackpanel</returns>
        private void MakeToolbar()
        {
            ToolRow.Height = new GridLength(1, GridUnitType.Star);
            if (_mode == ToolBarMode.NORMAL || _mode == ToolBarMode.WIDE)
            {
                ToolBarExpandButton.Visibility = Visibility.Collapsed;
                toolBar.Visibility = Visibility.Visible;
            }
            else if (_mode == ToolBarMode.FLYOUT)
            {
                ToolBarExpandButton.Visibility = Visibility.Visible;
                toolBar.Visibility = Visibility.Collapsed;
            }
            _toolBtns = new Dictionary<MicropolisTool, RadioButton>();

            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["EMPTY"]));

            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["BULLDOZER"]));
            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["WIRE"]));
            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["PARK"]));

            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["ROADS"]));
            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["RAIL"]));

            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["RESIDENTIAL"]));
            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["COMMERCIAL"]));
            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["INDUSTRIAL"]));

            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["FIRE"]));
            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["QUERY"]));
            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["POLICE"]));

            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["POWERPLANT"]));
            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["NUCLEAR"]));

            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["STADIUM"]));
            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["SEAPORT"]));

            toolBar.Items.Add(MakeToolBtn(MicropolisTools.MicropolisTool["AIRPORT"]));
        }

        /// <summary>
        ///     Makes a toolbar button for the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>the RadioButton</returns>
        private RadioButton MakeToolBtn(MicropolisTool tool)
        {
            String iconName = Strings.ContainsKey("tool." + tool.Name + ".icon")
                ? "ms-appx:///resources/images/GameUI" + Strings.GetString("tool." + tool.Name + ".icon")
                : "ms-appx:///graphics/tools/" + tool.Name.ToLower() + ".png";
            String iconSelectedName = Strings.ContainsKey("tool." + tool.Name + ".selected_icon")
                ? "ms-appx:///resources/images/GameUI" + Strings.GetString("tool." + tool.Name + ".selected_icon")
                : iconName;
            String tipText = Strings.ContainsKey("tool." + tool.Name + ".tip")
                ? Strings.GetString("tool." + tool.Name + ".tip")
                : tool.Name;


            RadioButton btn;

            if (_mode == ToolBarMode.NORMAL)
            {
                btn = new RadioButton {Width = 32, Height = 32, HorizontalAlignment = HorizontalAlignment.Left};
            }
            else if (_mode == ToolBarMode.FLYOUT || _mode == ToolBarMode.WIDE)
            {
                btn = new RadioButton { Width = 64, Height = 64, HorizontalAlignment = HorizontalAlignment.Left };
            }
            else
            {
                btn = new RadioButton { Width = 32, Height = 32, HorizontalAlignment = HorizontalAlignment.Left };
            }

            ResourceDictionary rd = Resources.MergedDictionaries[1];
            btn.Style = (Style)rd["ToolbarRadioButtonStyle"];

            var imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(iconName, UriKind.RelativeOrAbsolute))
            };

            btn.Background = imageBrush;

            var imageBrushSelected = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(iconSelectedName, UriKind.RelativeOrAbsolute))
            };

            btn.Checked += (a, b) => ToolButtonChecked(a, imageBrushSelected);
            btn.Unchecked += (a, b) => ToolButtonChecked(a, imageBrush);


            var tip = new ToolTip {Content = tipText};
            ToolTipService.SetToolTip(btn, tip);


            btn.Click += delegate { SelectTool(tool); };
            _toolBtns.Add(tool, btn);
            return btn;
        }

        /// <summary>
        ///     Called when the user clicks a tool button to select it. Replaces the background image of the tool button with the
        ///     image provided.
        /// </summary>
        /// <param name="radioButton">The RadioButton clicked.</param>
        /// <param name="imageBrush">The image brush to set as background of the RadioButton.</param>
        private void ToolButtonChecked(object radioButton, ImageBrush imageBrush)
        {
            var sender = ((RadioButton) radioButton);
            sender.Background = imageBrush;

            if (_mode == ToolBarMode.FLYOUT)
            {
                toolBar.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     Selects the tool specified.
        /// </summary>
        /// <param name="newTool">The new tool.</param>
        private void SelectTool(MicropolisTool newTool)
        {
            _toolBtns[newTool].IsChecked = true;
            if (newTool == _mainPage.CurrentTool)
            {
                return;
            }

            if (_mainPage.CurrentTool != null)
            {
                _toolBtns[_mainPage.CurrentTool].IsChecked = false;
            }

            _mainPage.SelectTool(newTool);
        }

        /// <summary>
        /// Fired when toolbar expand button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolBarExpandButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (toolBar.Visibility == Visibility.Visible)
            {
                toolBar.Visibility = Visibility.Collapsed;
                ToolBarExpandButton.Content = "▼";
                ToolRow.Height = new GridLength(0,GridUnitType.Auto);
            }
            else
            {
                toolBar.Visibility = Visibility.Visible;
                ToolBarExpandButton.Content = "▲";
                ToolRow.Height = new GridLength(1, GridUnitType.Star);

            }
             
        }
    }
}