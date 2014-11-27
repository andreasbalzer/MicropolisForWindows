using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Micropolis.Model.Entities;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

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
    ///     GraphArea displaying graph with information on mayors work and city performance
    /// </summary>
    /// <remarks>ToDo: Paddings not yet implemented.</remarks>
    public sealed partial class GraphArea
    {
        private GraphsPane _graphsPane;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GraphArea" /> class.
        /// </summary>
        public GraphArea()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Repaints this instance.
        /// </summary>
        public void Repaint()
        {
            PaintComponent();
        }

        /// <summary>
        ///     Sets up after basic initialize.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void SetUpAfterBasicInit(GraphsPane pane)
        {
            _graphsPane = pane;
            Repaint();
        }

        /// <summary>
        ///     Paints the component.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public async Task PaintComponent()
        {
            if (_graphsPane.Engine == null || IsNotVisible())
            {
                return;
            }
            LineChart.Series.Clear();

            bool isOneTwenty = _graphsPane.OneTwentyYearsButtonPublic.IsChecked.HasValue &&
                               _graphsPane.OneTwentyYearsButtonPublic.IsChecked.Value;
            int unitPeriod = isOneTwenty ? 12*Engine.Micropolis.CENSUS_RATE : Engine.Micropolis.CENSUS_RATE;
            int hashPeriod = isOneTwenty ? 10*unitPeriod : 12*unitPeriod;
            int startTime = ((_graphsPane.Engine.History.CityTime/unitPeriod) - 119)*unitPeriod;

            int h = isOneTwenty ? 239 : 119;

            int minimum = 0;
            int maximum = 0;

            foreach (GraphData gd in (GraphData[]) Enum.GetValues(typeof (GraphData)))
            {
                if (_graphsPane.DataBtns[gd].IsChecked.HasValue && _graphsPane.DataBtns[gd].IsChecked.Value)
                {
                    var graphData = new List<GraphDataPoint>();

                    for (int i = 0; i < 120; i++)
                    {
                        int t = startTime + i*unitPeriod; // t might be negative
                        if (t%hashPeriod == 0)
                        {
                            // year
                            int year = 1900 + (t/(12*Engine.Micropolis.CENSUS_RATE));

                            double yp = _graphsPane.GetHistoryValue(gd, h - i);
                            var dataPoint = new GraphDataPoint {Year = year, Data = yp};
                            graphData.Add(dataPoint);
                        }
                    }

                    minimum = graphData[0].Year;
                    maximum = graphData[graphData.Count - 1].Year;
                    String colStr = Strings.GetString("graph_color." + gd);
                    Color color = ColorParser.ParseColor(colStr);

                    var lineSeries = new LineSeries();
                    lineSeries.Title = Strings.GetString("graph_label." + gd);
                    lineSeries.IndependentValueBinding = CreateBinding("Year");
                    lineSeries.DependentValueBinding = CreateBinding("Data");
                    lineSeries.ItemsSource = graphData;
                    lineSeries.IsSelectionEnabled = false;

                    var lineStyle = new Style(typeof (Control));
                    var setter = new Setter(BackgroundProperty, new SolidColorBrush(color));
                    lineStyle.Setters.Add(setter);
                    lineSeries.DataPointStyle = lineStyle;

                    LineChart.Series.Add(lineSeries);
                }
            }
            HorizontalAxis.Minimum = minimum;
            HorizontalAxis.Maximum = maximum;
            HorizontalAxis.Interval = isOneTwenty ? 10 : 1;
        }

        /// <summary>
        ///     Creates a Binding to element with Name equal to PropName.
        /// </summary>
        /// <param name="PropName">Name of Element</param>
        /// <returns>Binding to Element with Name PropName</returns>
        private Binding CreateBinding(string PropName)
        {
            var bind = new Binding();
            bind.Path = new PropertyPath(PropName);

            return bind;
        }

        /// <summary>
        ///     Determins whether this GraphsArea instance is not visible in Visual Tree.
        /// </summary>
        /// <returns>Not visible (true), otherwise false.</returns>
        private bool IsNotVisible()
        {
            var parent = (FrameworkElement) Parent;
            while (parent != null)
            {
                if (parent.Visibility == Visibility.Collapsed)
                {
                    return true;
                }
                parent = (FrameworkElement) parent.Parent;
            }
            return false;
        }
    }
}