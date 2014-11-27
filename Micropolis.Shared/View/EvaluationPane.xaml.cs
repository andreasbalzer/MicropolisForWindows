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
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Engine;

    /// <summary>
    ///     Evaluation pane showing inhabitants' amusement and agreement to mayors work.
    /// </summary>
    public partial class EvaluationPane : Engine.IListener
    {
        private Micropolis _engine;

        private MainGamePage _mainPage;
        private TextBlock[] _voterCountLbl;
        private TextBlock[] _voterProblemLbl;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EvaluationPane" /> class.
        /// </summary>
        public EvaluationPane()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Sets the engine.
        /// </summary>
        /// <param name="newEngine">The new engine.</param>
        public void SetEngine(Micropolis newEngine)
        {
            if (_engine != null)
            {
                //old engine
                _engine.RemoveListener(this);
            }
            _engine = newEngine;
            if (_engine != null)
            {
                //new engine
                _engine.AddListener(this);
                LoadEvaluation();
            }
        }

        /// <summary>
        ///     Called when user clicked the dismiss button to close the window.
        /// </summary>
        private void OnDismissClicked()
        {
            _mainPage.HideEvaluationPane();
        }

        /// <summary>
        ///     Makes the public opinion pane.
        /// </summary>
        /// <returns></returns>
        private void MakePublicOpinionPane()
        {
            headerPublicOpinionLbl.Text = Strings.GetString("public-opinion");
            pubOp.Text = Strings.GetString("public-opinion-1");
            pubOp2.Text = Strings.GetString("public-opinion-2");
            pubOp3.Text = Strings.GetString("public-opinion-yes");
            pubOp4.Text = Strings.GetString("public-opinion-no");

            const int numProbs = 4;
            _voterProblemLbl = new TextBlock[numProbs];
            _voterCountLbl = new TextBlock[numProbs];
            voterProblemSP.Children.Clear();
            for (int i = 0; i < numProbs; i++)
            {
                _voterProblemLbl[i] = new TextBlock();
                voterProblemSP.Children.Add(_voterProblemLbl[i]);

                _voterCountLbl[i] = new TextBlock();
                voterProblemSP.Children.Add(_voterCountLbl[i]);
            }
        }

        /// <summary>
        ///     Makes the statistics pane.
        /// </summary>
        /// <returns></returns>
        private void MakeStatisticsPane()
        {
            headerStatisticsLbl.Text = Strings.GetString("statistics-head");
            cityScoreHeader.Text = Strings.GetString("city-score-head");
            statPop.Text = Strings.GetString("stats-population");
            statMig.Text = Strings.GetString("stats-net-migration");
            deltaLbl = new TextBlock();
            statsLastYear.Text = Strings.GetString("stats-last-year");
            statsAssessedValue.Text = Strings.GetString("stats-assessed-value");
            statsCategory.Text = Strings.GetString("stats-category");
            statsGameLevel.Text = Strings.GetString("stats-game-level");
            cityScoreCurrent.Text = Strings.GetString("city-score-current");
            cityScoreChange.Text = Strings.GetString("city-score-change");
        }

        /// <summary>
        ///     Loads the evaluation.
        /// </summary>
        private void LoadEvaluation()
        {
            yesLbl.Text = ((0.01*_engine.Evaluation.CityYes).ToString());
            noLbl.Text = ((0.01*_engine.Evaluation.CityNo).ToString());

            for (int i = 0; i < _voterProblemLbl.Length; i++)
            {
                CityProblem p = i < _engine.Evaluation.ProblemOrder.Length
                    ? _engine.Evaluation.ProblemOrder[i]
                    : default(CityProblem);
                int numVotes = p != default(CityProblem) ? _engine.Evaluation.ProblemVotes[p] : 0;

                if (numVotes != 0)
                {
                    _voterProblemLbl[i].Text = ("problem." + p); //name
                    _voterCountLbl[i].Text = (0.01*numVotes).ToString();
                    _voterProblemLbl[i].Visibility = Visibility.Visible;
                    _voterCountLbl[i].Visibility = Visibility.Visible;
                }
                else
                {
                    _voterProblemLbl[i].Visibility = Visibility.Collapsed;
                    _voterCountLbl[i].Visibility = Visibility.Collapsed;
                }
            }


            popLbl.Text = (_engine.Evaluation.CityPop).ToString();
            deltaLbl.Text = (_engine.Evaluation.DeltaCityPop).ToString();
            assessLbl.Text = (_engine.Evaluation.CityAssValue).ToString();
            cityClassLbl.Text = (GetCityClassName(_engine.Evaluation.CityClass));
            gameLevelLbl.Text = (GetGameLevelName(_engine.GameLevel));
            scoreLbl.Text = (_engine.Evaluation.CityScore).ToString();
            scoreDeltaLbl.Text = (_engine.Evaluation.DeltaCityScore).ToString();
        }

        /// <summary>
        ///     Gets the name of the city class.
        /// </summary>
        /// <param name="cityClass">The city class.</param>
        /// <returns></returns>
        private static String GetCityClassName(int cityClass)
        {
            return Strings.GetString("class." + cityClass);
        }

        /// <summary>
        ///     Gets the name of the game level.
        /// </summary>
        /// <param name="gameLevel">The game level.</param>
        /// <returns></returns>
        private static String GetGameLevelName(int gameLevel)
        {
            return Strings.GetString("level." + gameLevel);
        }

        internal void SetupAfterBasicInit(MainGamePage mainPage, Micropolis engine)
        {
            _mainPage = mainPage;
            dismissBtn.Content = Strings.GetString("dismiss-evaluation");
            dismissBtn.Click += delegate { OnDismissClicked(); };

            MakePublicOpinionPane();

            MakeStatisticsPane();

            SetEngine(engine);
            LoadEvaluation();
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
        ///     Fired whenever the "census" is taken, and the various historical counters have been updated. (Once a month in
        ///     game.)
        /// </summary>
        public void CensusChanged()
        {
        }

        /// <summary>
        ///     Fired whenever resValve, comValve, or indValve changes. (Twice a month in game.)
        /// </summary>
        public void DemandChanged()
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

        #endregion

        #region implements Engine.IListener

        /// <summary>
        ///     Fired whenever the city evaluation is recalculated. (Once a year.)
        /// </summary>
        public void EvaluationChanged()
        {
            LoadEvaluation();
        }

        #endregion
    }
}