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
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Engine;

    /// <summary>
    ///     GraphsPane is a major UI component that can be expanded upon user request. It displays a graph with information on
    ///     the mayors work and the city performance.
    /// </summary>
    public sealed partial class GraphsPane : Engine.IListener
    {
        public static readonly int LEFT_MARGIN = 4;
        public static readonly int RIGHT_MARGIN = 4;
        public static readonly int TOP_MARGIN = 2;
        public static readonly int BOTTOM_MARGIN = 2;
        public static readonly int LEGEND_PADDING = 6;

        /// <summary>
        ///     The data buttons containing data types that can be included in the graph
        /// </summary>
        public Dictionary<GraphData, ToggleButton> DataBtns = new Dictionary<GraphData, ToggleButton>();

        /// <summary>
        /// Reference to game engine
        /// </summary>
        public Micropolis Engine;

        /// <summary>
        /// Reference to MainGamePage
        /// </summary>
        private MainGamePage _mainPage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GraphsPane" /> class.
        /// </summary>
        public GraphsPane()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets the one twenty years button public.
        /// </summary>
        /// <value>
        ///     The one twenty years button public.
        /// </value>
        public ToggleButton OneTwentyYearsButtonPublic
        {
            get { return OneTwentyYearsButton; }
        }

        /// <summary>
        ///     Sets up after basic initialize.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="mainPage">The main page.</param>
        public void SetUpAfterBasicInit(Micropolis engine, MainGamePage mainPage)
        {
            _mainPage = mainPage;
            graphArea.SetUpAfterBasicInit(this);
            SetEngine(engine);

            DismissButton.Content = Strings.GetString("dismiss_graph");
            TenYearsButton.Content = Strings.GetString("ten_years");
            OneTwentyYearsButton.Content = Strings.GetString("onetwenty_years");

            toolsPane.Children.Add(MakeDataBtn(GraphData.RESPOP));
            toolsPane.Children.Add(MakeDataBtn(GraphData.COMPOP));
            toolsPane.Children.Add(MakeDataBtn(GraphData.INDPOP));
            toolsPane.Children.Add(MakeDataBtn(GraphData.MONEY));
            toolsPane.Children.Add(MakeDataBtn(GraphData.CRIME));
            toolsPane.Children.Add(MakeDataBtn(GraphData.POLLUTION));

            DataBtns[GraphData.MONEY].IsChecked = true;
            DataBtns[GraphData.POLLUTION].IsChecked = true;

            SetTimePeriod(TimePeriod.TEN_YEARS);
        }

        /// <summary>
        ///     Handles the Click event of the DismissButton control when user wants to close the window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            OnDismissClicked();
        }

        /// <summary>
        ///     Handles the Click event of the TenYearsButton control when user wants graph to cover ten years.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void TenYearsButton_Click(object sender, RoutedEventArgs e)
        {
            SetTimePeriod(TimePeriod.TEN_YEARS);
        }

        /// <summary>
        ///     Handles the Click event of the TenYearsButton control when user wants graph to cover 120 years.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void OneTwentyYearsButton_Click(object sender, RoutedEventArgs e)
        {
            SetTimePeriod(TimePeriod.ONETWENTY_YEARS);
        }

        /// <summary>
        ///     Sets the engine.
        /// </summary>
        /// <param name="newEngine">The new engine.</param>
        public void SetEngine(Micropolis newEngine)
        {
            if (Engine != null)
            {
                //old engine
                Engine.RemoveListener(this);
            }
            Engine = newEngine;
            if (Engine != null)
            {
                //new engine
                Engine.AddListener(this);
                graphArea.Repaint();
            }
        }

        /// <summary>
        ///     Called when user wants to close the window
        /// </summary>
        private void OnDismissClicked()
        {
            _mainPage.HideGraphsPane();
        }

        /// <summary>
        ///     Makes the data buttons containing data types that can be displayed in the graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <returns></returns>
        private ToggleButton MakeDataBtn(GraphData graph)
        {
            String icon1Name = "ms-appx:///resources/images/GameUI/" + Strings.GetString("graph_button." + graph);
            String icon2Name = "ms-appx:///resources/images/GameUI/" + Strings.GetString("graph_button." + graph + ".selected");

            var uri1 = new Uri(icon1Name, UriKind.RelativeOrAbsolute);
            var uri2 = new Uri(icon2Name, UriKind.RelativeOrAbsolute);

            var icon1Brush = new ImageBrush {ImageSource = new BitmapImage(uri1)};

            var icon2Brush = new ImageBrush {ImageSource = new BitmapImage(uri2)};

            var btn = new ToggleButton
            {
                Style = (Style) Resources["ToolBarToggleButtonStyle"],
                Height = 32,
                Width = 32,
                Background = icon1Brush,
                Content = " "
            };

            btn.Click += (s, e) => graphArea.Repaint();
            btn.Checked += (o, e) => ToolbarButtonCheckChange(o, icon2Brush);
            btn.Unchecked += (o, e) => ToolbarButtonCheckChange(o, icon1Brush);

            DataBtns.Add(graph, btn);
            return btn;
        }

        /// <summary>
        ///     Toolbars the button check change.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="iconBrush">The icon brush.</param>
        private void ToolbarButtonCheckChange(object o, ImageBrush iconBrush)
        {
            ((ToggleButton) o).Background = iconBrush;
        }

        /// <summary>
        ///     Gets the history maximum.
        /// </summary>
        /// <returns></returns>
        public int GetHistoryMax()
        {
            int max = 0;
            foreach (GraphData g in (GraphData[]) Enum.GetValues(typeof (GraphData)))
            {
                for (int pos = 0; pos < 240; pos++)
                {
                    max = Math.Max(max, GetHistoryValue(g, pos));
                }
            }
            return max;
        }

        /// <summary>
        ///     Gets the history value.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="pos">The position.</param>
        /// <returns>history value</returns>
        /// <exception cref="Exception">unexpected</exception>
        public int GetHistoryValue(GraphData graph, int pos)
        {
            //assert pos >= 0 && pos < 240;
            switch (graph)
            {
                case GraphData.RESPOP:
                    return Engine.History.Res[pos];
                case GraphData.COMPOP:
                    return Engine.History.Com[pos];
                case GraphData.INDPOP:
                    return Engine.History.Ind[pos];
                case GraphData.MONEY:
                    return Engine.History.Money[pos];
                case GraphData.CRIME:
                    return Engine.History.Crime[pos];
                case GraphData.POLLUTION:
                    return Engine.History.Pollution[pos];
                default:
                    throw new Exception("unexpected");
            }
        }

        /// <summary>
        ///     Sets the time period.
        /// </summary>
        /// <param name="period">The period.</param>
        public void SetTimePeriod(TimePeriod period)
        {
            TenYearsButton.IsChecked = (period == TimePeriod.TEN_YEARS);
            OneTwentyYearsButton.IsChecked = (period == TimePeriod.ONETWENTY_YEARS);
            if (graphArea != null)
            {
                graphArea.Repaint();
            }
        }

        public void Repaint()
        {
            graphArea.Repaint();
        }

        #region implements Micropolis.IListener

        /// <summary>
        ///     Cities the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="loc">The loc.</param>
        public void CityMessage(MicropolisMessage message, CityLocation loc)
        {
        }

        /// <summary>
        ///     Cities the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="loc">The loc.</param>
        public void CitySound(Sound sound, CityLocation loc)
        {
        }

        /// <summary>
        ///     Fired whenever resValve, comValve, or indValve changes. (Twice a month in game.)
        /// </summary>
        public void DemandChanged()
        {
        }

        /// <summary>
        ///     Fired whenever the city evaluation is recalculated. (Once a year.)
        /// </summary>
        public void EvaluationChanged()
        {
        }

        /// <summary>
        ///     Fired whenever the mayor's money changes.
        /// </summary>
        public void FundsChanged()
        {
        }

        /// <summary>
        ///     Fired whenever autoBulldoze, autoBudget, noDisasters, or simSpeed change.
        /// </summary>
        public void OptionsChanged()
        {
        }

        /// <summary>
        ///     Fired whenever the "census" is taken, and the various historical counters have been updated. (Once a month in
        ///     game.)
        /// </summary>
        public void CensusChanged()
        {
            graphArea.Repaint();
        }

        #endregion
    }
}