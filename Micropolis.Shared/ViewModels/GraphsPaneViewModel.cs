using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Core;
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
        private readonly GraphAreaViewModel _graphAreaViewModel;

        /// <summary>
        ///     Reference to game engine
        /// </summary>
        public Engine.Micropolis Engine;

        private string _dismissButtonText;

        /// <summary>
        ///     Reference to MainGamePage
        /// </summary>
        private MainGamePageViewModel _mainPageViewModel;

        private string _oneTwentyYearsButtonText;
        private bool _oneTwentyYearsIsChecked;
        private string _tenYearsButtonText;
        private bool _tenYearsIsChecked;

        public GraphsPaneViewModel(GraphAreaViewModel graphAreaViewModel)
        {
            Buttons = new ObservableCollection<GraphPaneToggleButtonViewModel>();
            _graphAreaViewModel = graphAreaViewModel;
            DismissCommand = new DelegateCommand(Dismiss);
            TenYearsCommand = new DelegateCommand(TenYearsButton_Click);
            OneTwentyYearsCommand = new DelegateCommand(OneTwentyYearsButton_Click);
        }

        public DelegateCommand DismissCommand { get; private set; }
        public DelegateCommand TenYearsCommand { get; private set; }
        public DelegateCommand OneTwentyYearsCommand { get; private set; }

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

        /// <summary>
        ///     Sets up after basic initialize.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="mainPage">The main page.</param>
        public void SetUpAfterBasicInit(Engine.Micropolis engine, MainGamePageViewModel mainPageViewModel)
        {
            _mainPageViewModel = mainPageViewModel;
            _graphAreaViewModel.SetUpAfterBasicInit(this);
            SetEngine(engine);

            DismissButtonText = Strings.GetString("dismiss_graph");
            TenYearsButtonText = Strings.GetString("ten_years");
            OneTwentyYearsButtonText = Strings.GetString("onetwenty_years");

            MakeDataBtn(GraphData.RESPOP);
            MakeDataBtn(GraphData.COMPOP);
            MakeDataBtn(GraphData.INDPOP);
            MakeDataBtn(GraphData.MONEY);
            MakeDataBtn(GraphData.CRIME);
            MakeDataBtn(GraphData.POLLUTION);

            SetTimePeriod(TimePeriod.TEN_YEARS);
        }

        internal Dictionary<GraphData, GraphPaneToggleButtonViewModel> DataBtns = new Dictionary<GraphData, GraphPaneToggleButtonViewModel>();
        public ObservableCollection<GraphPaneToggleButtonViewModel> Buttons { get; set; }

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
                _graphAreaViewModel.Repaint();
            }
        }

        /// <summary>
        ///     Called when user wants to close the window
        /// </summary>
        private void Dismiss()
        {
            _mainPageViewModel.HideGraphsPane();
        }

        /// <summary>
        ///     Makes the data buttons containing data types that can be displayed in the graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <returns></returns>
        private void MakeDataBtn(GraphData graph)
        {
            GraphPaneToggleButtonViewModel buttonViewModel = new GraphPaneToggleButtonViewModel();
            String icon1Name = "ms-appx:///resources/images/GameUI/" + Strings.GetString("graph_button." + graph);
            String icon2Name = "ms-appx:///resources/images/GameUI/" +
                               Strings.GetString("graph_button." + graph + ".selected");

            var uri1 = new Uri(icon1Name, UriKind.RelativeOrAbsolute);
            var uri2 = new Uri(icon2Name, UriKind.RelativeOrAbsolute);

            var icon1Brush = new ImageBrush {ImageSource = new BitmapImage(uri1)};

            var icon2Brush = new ImageBrush {ImageSource = new BitmapImage(uri2)};
            buttonViewModel.UncheckedStateImageBrush = icon1Brush;
            buttonViewModel.CheckedStateImageBrush = icon2Brush;
            buttonViewModel.Text = " ";
            buttonViewModel.ClickCommand = new DelegateCommand(
                () =>
                {
                    _graphAreaViewModel.Repaint();
                });

            if (graph == GraphData.MONEY || graph == GraphData.POLLUTION)
            {
                buttonViewModel.IsChecked = true;
            }

            DataBtns.Add(graph,buttonViewModel);
            Buttons.Add(buttonViewModel);

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
            if (_graphAreaViewModel != null)
            {
                _graphAreaViewModel.Repaint();
            }
        }

        public void Repaint()
        {
            _graphAreaViewModel.Repaint();
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
            _graphAreaViewModel.Repaint();
        }

        #endregion
    }
}