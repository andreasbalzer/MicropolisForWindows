using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Engine;
using Micropolis.Common;

namespace Micropolis.ViewModels
{
    public class GraphsPaneViewModel : BindableBase, Engine.IListener
    {
        public static readonly int LEFT_MARGIN = 4;
        public static readonly int RIGHT_MARGIN = 4;
        public static readonly int TOP_MARGIN = 2;
        public static readonly int BOTTOM_MARGIN = 2;
        public static readonly int LEGEND_PADDING = 6;
        private readonly GraphArea _graphArea;
        private readonly ResourceDictionary _resources;
        private readonly StackPanel _toolsPane;


        /// <summary>
        ///     Reference to game engine
        /// </summary>
        public Engine.Micropolis Engine;

        private bool _dataBtnsComPopIsChecked;
        private bool _dataBtnsCrimeIsChecked;
        private bool _dataBtnsIndPopIsChecked;
        private bool _dataBtnsMoneyIsChecked;
        private bool _dataBtnsPollutionIsChecked;

        private bool _dataBtnsResPopIsChecked;
        private string _dismissButtonText;

        /// <summary>
        ///     Reference to MainGamePage
        /// </summary>
        private MainGamePage _mainPage;

        private string _oneTwentyYearsButtonText;
        private bool _oneTwentyYearsIsChecked;
        private string _tenYearsButtonText;
        private bool _tenYearsIsChecked;

        public GraphsPaneViewModel(StackPanel toolsPane, GraphArea graphArea, ResourceDictionary resources)
        {
            _toolsPane = toolsPane;
            _graphArea = graphArea;
            DismissCommand = new DelegateCommand(Dismiss);
            TenYearsCommand = new DelegateCommand(TenYearsButton_Click);
            OneTwentyYearsCommand = new DelegateCommand(OneTwentyYearsButton_Click);
            _resources = resources;
        }

        public DelegateCommand DismissCommand { get; private set; }
        public DelegateCommand TenYearsCommand { get; private set; }
        public DelegateCommand OneTwentyYearsCommand { get; private set; }

        public bool DataBtnsResPopIsChecked
        {
            get { return _dataBtnsResPopIsChecked; }
            set { SetProperty(ref _dataBtnsResPopIsChecked, value); }
        }

        public bool DataBtnsComPopIsChecked
        {
            get { return _dataBtnsComPopIsChecked; }
            set { SetProperty(ref _dataBtnsComPopIsChecked, value); }
        }

        public bool DataBtnsIndPopIsChecked
        {
            get { return _dataBtnsIndPopIsChecked; }
            set { SetProperty(ref _dataBtnsIndPopIsChecked, value); }
        }

        public bool DataBtnsCrimeIsChecked
        {
            get { return _dataBtnsCrimeIsChecked; }
            set { SetProperty(ref _dataBtnsCrimeIsChecked, value); }
        }

        public bool OneTwentyYearsIsChecked
        {
            get { return _oneTwentyYearsIsChecked; }
            set { SetProperty(ref _oneTwentyYearsIsChecked, value); }
        }

        public bool TenYearsIsChecked
        {
            get { return _tenYearsIsChecked; }
            set { SetProperty(ref _tenYearsIsChecked, value); }
        }


        public string DismissButtonText
        {
            get { return _dismissButtonText; }
            set { SetProperty(ref _dismissButtonText, value); }
        }

        public string TenYearsButtonText
        {
            get { return _tenYearsButtonText; }
            set { SetProperty(ref _tenYearsButtonText, value); }
        }

        public string OneTwentyYearsButtonText
        {
            get { return _oneTwentyYearsButtonText; }
            set { SetProperty(ref _oneTwentyYearsButtonText, value); }
        }

        public bool DataBtnsMoneyIsChecked
        {
            get { return _dataBtnsMoneyIsChecked; }
            set { SetProperty(ref _dataBtnsMoneyIsChecked, value); }
        }

        public bool DataBtnsPollutionIsChecked
        {
            get { return _dataBtnsPollutionIsChecked; }
            set { SetProperty(ref _dataBtnsPollutionIsChecked, value); }
        }

        /// <summary>
        ///     Sets up after basic initialize.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="mainPage">The main page.</param>
        public void SetUpAfterBasicInit(Engine.Micropolis engine, MainGamePage mainPage)
        {
            _mainPage = mainPage;
            _graphArea.SetUpAfterBasicInit(this);
            SetEngine(engine);

            DismissButtonText = Strings.GetString("dismiss_graph");
            TenYearsButtonText = Strings.GetString("ten_years");
            OneTwentyYearsButtonText = Strings.GetString("onetwenty_years");

            _toolsPane.Children.Add(MakeDataBtn(GraphData.RESPOP));
            _toolsPane.Children.Add(MakeDataBtn(GraphData.COMPOP));
            _toolsPane.Children.Add(MakeDataBtn(GraphData.INDPOP));
            _toolsPane.Children.Add(MakeDataBtn(GraphData.MONEY));
            _toolsPane.Children.Add(MakeDataBtn(GraphData.CRIME));
            _toolsPane.Children.Add(MakeDataBtn(GraphData.POLLUTION));

            DataBtnsResPopIsChecked = false;
            DataBtnsComPopIsChecked = false;
            DataBtnsIndPopIsChecked = false;
            DataBtnsMoneyIsChecked = true;
            DataBtnsCrimeIsChecked = false;
            DataBtnsPollutionIsChecked = true;

            SetTimePeriod(TimePeriod.TEN_YEARS);
        }


        /// <summary>
        ///     Handles the Click event of the TenYearsButton control when user wants graph to cover ten years.
        /// </summary>
        public void TenYearsButton_Click()
        {
            SetTimePeriod(TimePeriod.TEN_YEARS);
        }

        /// <summary>
        ///     Handles the Click event of the TenYearsButton control when user wants graph to cover 120 years.
        /// </summary>
        public void OneTwentyYearsButton_Click()
        {
            SetTimePeriod(TimePeriod.ONETWENTY_YEARS);
        }

        /// <summary>
        ///     Sets the engine.
        /// </summary>
        /// <param name="newEngine">The new engine.</param>
        public void SetEngine(Engine.Micropolis newEngine)
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
                _graphArea.Repaint();
            }
        }

        /// <summary>
        ///     Called when user wants to close the window
        /// </summary>
        private void Dismiss()
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
            String icon2Name = "ms-appx:///resources/images/GameUI/" +
                               Strings.GetString("graph_button." + graph + ".selected");

            var uri1 = new Uri(icon1Name, UriKind.RelativeOrAbsolute);
            var uri2 = new Uri(icon2Name, UriKind.RelativeOrAbsolute);

            var icon1Brush = new ImageBrush {ImageSource = new BitmapImage(uri1)};

            var icon2Brush = new ImageBrush {ImageSource = new BitmapImage(uri2)};

            var btn = new ToggleButton
            {
                Style = (Style) _resources["ToolBarToggleButtonStyle"],
                Height = 32,
                Width = 32,
                Background = icon1Brush,
                Content = " "
            };

            btn.Click += (s, e) => _graphArea.Repaint();
            btn.Checked += (o, e) => ToolbarButtonCheckChange(o, icon2Brush);
            btn.Unchecked += (o, e) => ToolbarButtonCheckChange(o, icon1Brush);


            var myBinding = new Binding();
            myBinding.Source = this;

            myBinding.Mode = BindingMode.TwoWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;


            if (graph == GraphData.RESPOP)
            {
                myBinding.Path = new PropertyPath("DataBtnsResPopIsChecked");
            }
            if (graph == GraphData.COMPOP)
            {
                myBinding.Path = new PropertyPath("DataBtnsComPopIsChecked");
            }
            if (graph == GraphData.INDPOP)
            {
                myBinding.Path = new PropertyPath("DataBtnsIndPopIsChecked");
            }
            if (graph == GraphData.CRIME)
            {
                myBinding.Path = new PropertyPath("DataBtnsCrimeIsChecked");
            }
            if (graph == GraphData.MONEY)
            {
                myBinding.Path = new PropertyPath("DataBtnsMoneyIsChecked");
            }
            if (graph == GraphData.POLLUTION)
            {
                myBinding.Path = new PropertyPath("DataBtnsPollutionIsChecked");
            }
            BindingOperations.SetBinding(btn, ToggleButton.IsCheckedProperty, myBinding);

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
            TenYearsIsChecked = (period == TimePeriod.TEN_YEARS);
            OneTwentyYearsIsChecked = (period == TimePeriod.ONETWENTY_YEARS);
            if (_graphArea != null)
            {
                _graphArea.Repaint();
            }
        }

        public void Repaint()
        {
            _graphArea.Repaint();
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
            _graphArea.Repaint();
        }

        #endregion
    }
}