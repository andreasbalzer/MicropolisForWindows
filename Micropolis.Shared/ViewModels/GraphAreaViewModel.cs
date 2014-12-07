using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Micropolis.Common;
using Micropolis.Model.Entities;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace Micropolis.ViewModels
{
    public class GraphAreaViewModel : BindableBase
    {
        private readonly Chart _chart;
        private GraphsPaneViewModel _graphsPaneViewModel;

        private int _horizontalAxisInterval;
        private int _horizontalAxisMaximum;

        private int _horizontalAxisMinimum;

        public GraphAreaViewModel(Chart chart)
        {
            _chart = chart;
        }

        public int HorizontalAxisInterval
        {
            get { return _horizontalAxisInterval; }
            set { SetProperty(ref _horizontalAxisInterval, value); }
        }

        public int HorizontalAxisMinimum
        {
            get { return _horizontalAxisMinimum; }
            set { SetProperty(ref _horizontalAxisMinimum, value); }
        }

        public int HorizontalAxisMaximum
        {
            get { return _horizontalAxisMaximum; }
            set { SetProperty(ref _horizontalAxisMaximum, value); }
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
        public void SetUpAfterBasicInit(GraphsPaneViewModel pane)
        {
            _graphsPaneViewModel = pane;
            Repaint();
        }

        /// <summary>
        ///     Paints the component.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public void PaintComponent()
        {
            if (_graphsPaneViewModel.Engine == null)
            {
                return;
            }
            _chart.Series.Clear();

            bool isOneTwenty = _graphsPaneViewModel.OneTwentyYearsIsChecked;
            int unitPeriod = isOneTwenty ? 12*Engine.Micropolis.CENSUS_RATE : Engine.Micropolis.CENSUS_RATE;
            int hashPeriod = isOneTwenty ? 10*unitPeriod : 12*unitPeriod;
            int startTime = ((_graphsPaneViewModel.Engine.History.CityTime/unitPeriod) - 119)*unitPeriod;

            int h = isOneTwenty ? 239 : 119;

            int minimum = 0;
            int maximum = 0;

            foreach (GraphData gd in (GraphData[]) Enum.GetValues(typeof (GraphData)))
            {
                if (_graphsPaneViewModel.DataBtns.ContainsKey(gd) && _graphsPaneViewModel.DataBtns[gd].IsChecked)
                {
                    var graphData = new List<GraphDataPoint>();

                    for (int i = 0; i < 120; i++)
                    {
                        int t = startTime + i*unitPeriod; // t might be negative
                        if (t%hashPeriod == 0)
                        {
                            // year
                            int year = 1900 + (t/(12*Engine.Micropolis.CENSUS_RATE));

                            double yp = _graphsPaneViewModel.GetHistoryValue(gd, h - i);
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
                    var setter = new Setter(Control.BackgroundProperty, new SolidColorBrush(color));
                    lineStyle.Setters.Add(setter);
                    lineSeries.DataPointStyle = lineStyle;

                    _chart.Series.Add(lineSeries);
                }
            }
            if (maximum > minimum)
            {
                HorizontalAxisMaximum = maximum;
                HorizontalAxisMinimum = minimum;
                HorizontalAxisInterval = isOneTwenty ? 10 : 1;
            }
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
    }
}