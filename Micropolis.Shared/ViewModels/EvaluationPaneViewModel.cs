using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Engine;
using Micropolis.Common;
using Microsoft.ApplicationInsights;

namespace Micropolis.ViewModels
{
    public class EvaluationPaneViewModel : BindableBase, Engine.IListener
    {
     private Engine.Micropolis _engine;

        private MainGamePageViewModel _mainPageViewModel;
        
        
        /// <summary>
        ///     Sets the engine.
        /// </summary>
        /// <param name="newEngine">The new engine.</param>
        public void SetEngine(Engine.Micropolis newEngine)
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

        public EvaluationPaneViewModel()
        {
            DismissCommand = new DelegateCommand(OnDismissClicked);

            try
            {
                _telemetry = new TelemetryClient();
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        ///     Called when user clicked the dismiss button to close the window.
        /// </summary>
        private void OnDismissClicked()
        {
            try { 
                _telemetry.TrackEvent("EvaluationPaneDismissClicked");
            }
            catch (Exception) { }

            _mainPageViewModel.HideEvaluationPane();
        }

        private string _headerPublicOpinionTextBlockText;

        public string HeaderPublicOpinionTextBlockText
        {
            get
            {
                return this._headerPublicOpinionTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._headerPublicOpinionTextBlockText, value);

            }
        }

        private string _pubOpTextBlockText;

        public string PubOpTextBlockText
        {
            get
            {
                return this._pubOpTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._pubOpTextBlockText, value);

            }
        }

        private string _pubOp2TextBlockText;

        public string PubOp2TextBlockText
        {
            get
            {
                return this._pubOp2TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._pubOp2TextBlockText, value);

            }
        }

        private string _pubOp3TextBlockText;

        public string PubOp3TextBlockText
        {
            get
            {
                return this._pubOp3TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._pubOp3TextBlockText, value);

            }
        }

        private string _pubOp4TextBlockText;

        public string PubOp4TextBlockText
        {
            get
            {
                return this._pubOp4TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._pubOp4TextBlockText, value);

            }
        }

        /// <summary>
        ///     Makes the public opinion pane.
        /// </summary>
        /// <returns></returns>
        private void MakePublicOpinionPane()
        {
            HeaderPublicOpinionTextBlockText = Strings.GetString("public-opinion");
            PubOpTextBlockText = Strings.GetString("public-opinion-1");
            PubOp2TextBlockText = Strings.GetString("public-opinion-2");
            PubOp3TextBlockText = Strings.GetString("public-opinion-yes");
            PubOp4TextBlockText = Strings.GetString("public-opinion-no");

            VoterProblem1TextBlockText = "";
            VoterCount1TextBlockText = "";
            VoterProblem2TextBlockText = "";
            VoterCount2TextBlockText = "";
            VoterProblem3TextBlockText = "";
            VoterCount3TextBlockText = "";
            VoterProblem4TextBlockText = "";
            VoterCount4TextBlockText = "";
        }

        private string _voterProblem4TextBlockText;

        public string VoterProblem4TextBlockText
        {
            get
            {
                return this._voterProblem4TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._voterProblem4TextBlockText, value);

            }
        }

        private string _voterProblem3TextBlockText;

        public string VoterProblem3TextBlockText
        {
            get
            {
                return this._voterProblem3TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._voterProblem3TextBlockText, value);

            }
        }

        private string _voterProblem2TextBlockText;

        public string VoterProblem2TextBlockText
        {
            get
            {
                return this._voterProblem2TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._voterProblem2TextBlockText, value);

            }
        }

        private string _voterProblem1TextBlockText;

        public string VoterProblem1TextBlockText
        {
            get
            {
                return this._voterProblem1TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._voterProblem1TextBlockText, value);

            }
        }

        private string _voterCount1TextBlockText;

        public string VoterCount1TextBlockText
        {
            get
            {
                return this._voterCount1TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._voterCount1TextBlockText, value);

            }
        }

        private string _voterCount2TextBlockText;

        public string VoterCount2TextBlockText
        {
            get
            {
                return this._voterCount2TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._voterCount2TextBlockText, value);

            }
        }

        private string _voterCount3TextBlockText;

        public string VoterCount3TextBlockText
        {
            get
            {
                return this._voterCount3TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._voterCount3TextBlockText, value);

            }
        }

        private string _voterCount4TextBlockText;

        public string VoterCount4TextBlockText
        {
            get
            {
                return this._voterCount4TextBlockText;
            }
            set
            {
                this.SetProperty(ref this._voterCount4TextBlockText, value);

            }
        }


        /// <summary>
        ///     Makes the statistics pane.
        /// </summary>
        /// <returns></returns>
        private void MakeStatisticsPane()
        {
            HeaderStatisticsTextBlockText = Strings.GetString("statistics-head");
            CityScoreHeaderTextBlockText = Strings.GetString("city-score-head");
            StatPopTextBlockText = Strings.GetString("stats-population");
            StatMigTextBlockText = Strings.GetString("stats-net-migration");

            StatsLastYearTextBlockText = Strings.GetString("stats-last-year");
            StatsAssessedValueTextBlockText = Strings.GetString("stats-assessed-value");
            StatsCategoryTextBlockText = Strings.GetString("stats-category");
            StatsGameLevelTextBlockText = Strings.GetString("stats-game-level");
            CityScoreCurrentTextBlockText = Strings.GetString("city-score-current");
            CityScoreChangeTextBlockText = Strings.GetString("city-score-change");
        }

        private string _headerStatisticsTextBlockText;

        public string HeaderStatisticsTextBlockText
        {
            get
            {
                return this._headerStatisticsTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._headerStatisticsTextBlockText, value);

            }
        }

        private string _cityScoreHeaderTextBlockText;

        public string CityScoreHeaderTextBlockText
        {
            get
            {
                return this._cityScoreHeaderTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._cityScoreHeaderTextBlockText, value);

            }
        }

        private string _statPopTextBlockText;

        public string StatPopTextBlockText
        {
            get
            {
                return this._statPopTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._statPopTextBlockText, value);

            }
        }


        private string _statMigTextBlockText;

        public string StatMigTextBlockText
        {
            get
            {
                return this._statMigTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._statMigTextBlockText, value);

            }
        }

        private string _statsLastYearTextBlockText;

        public string StatsLastYearTextBlockText
        {
            get
            {
                return this._statsLastYearTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._statsLastYearTextBlockText, value);

            }
        }

        private string _statsAssessedValueTextBlockText;

        public string StatsAssessedValueTextBlockText
        {
            get
            {
                return this._statsAssessedValueTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._statsAssessedValueTextBlockText, value);

            }
        }

        private string _statsCategoryTextBlockText;

        public string StatsCategoryTextBlockText
        {
            get
            {
                return this._statsCategoryTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._statsCategoryTextBlockText, value);

            }
        }

        private string _statsGameLevelTextBlockText;

        public string StatsGameLevelTextBlockText
        {
            get
            {
                return this._statsGameLevelTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._statsGameLevelTextBlockText, value);

            }
        }

        private string _cityScoreCurrentTextBlockText;

        public string CityScoreCurrentTextBlockText
        {
            get
            {
                return this._cityScoreCurrentTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._cityScoreCurrentTextBlockText, value);

            }
        }

        private string _cityScoreChangeTextBlockText;

        public string CityScoreChangeTextBlockText
        {
            get
            {
                return this._cityScoreChangeTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._cityScoreChangeTextBlockText, value);

            }
        }


        private string _yesTextBlockText;

        public string YesTextBlockText
        {
            get
            {
                return this._yesTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._yesTextBlockText, value);

            }
        }

        private string _noTextBlockText;

        public string NoTextBlockText
        {
            get
            {
                return this._noTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._noTextBlockText, value);

            }
        }

        /// <summary>
        ///     Loads the evaluation.
        /// </summary>
        private void LoadEvaluation()
        {
            YesTextBlockText = ((_engine.Evaluation.CityYes).ToString()+"%");
            NoTextBlockText = ((_engine.Evaluation.CityNo).ToString()+"%");

            
                CityProblem p;
            int numVotes;

            p = 0 < _engine.Evaluation.ProblemOrder.Length
                    ? _engine.Evaluation.ProblemOrder[0]
                    : default(CityProblem);
                numVotes = p != default(CityProblem) ? _engine.Evaluation.ProblemVotes[p] : 0;

                if (numVotes != 0)
                {
                    VoterProblem1TextBlockText = ("problem." + p); //name
                    VoterCount1TextBlockText = (0.01*numVotes).ToString();
                    VoterProblem1IsVisible = true;
                    VoterCount1IsVisible = true;
                }
                else
                {
                    VoterProblem1IsVisible = false;
                    VoterCount1IsVisible = false;
                }

                p = 1 < _engine.Evaluation.ProblemOrder.Length
                       ? _engine.Evaluation.ProblemOrder[1]
                       : default(CityProblem);
                numVotes = p != default(CityProblem) ? _engine.Evaluation.ProblemVotes[p] : 0;

                if (numVotes != 0)
                {
                    VoterProblem2TextBlockText = ("problem." + p); //name
                    VoterCount2TextBlockText = (0.01 * numVotes).ToString();
                    VoterProblem2IsVisible = true;
                    VoterCount2IsVisible = true;
                }
                else
                {
                    VoterProblem2IsVisible = false;
                    VoterCount2IsVisible = false;
                }

                p = 2 < _engine.Evaluation.ProblemOrder.Length
                       ? _engine.Evaluation.ProblemOrder[2]
                       : default(CityProblem);
                numVotes = p != default(CityProblem) ? _engine.Evaluation.ProblemVotes[p] : 0;

                if (numVotes != 0)
                {
                    VoterProblem3TextBlockText = ("problem." + p); //name
                    VoterCount3TextBlockText = (0.01 * numVotes).ToString();
                    VoterProblem3IsVisible = true;
                    VoterCount3IsVisible = true;
                }
                else
                {
                    VoterProblem3IsVisible = false;
                    VoterCount3IsVisible = false;
                }

                p = 3 < _engine.Evaluation.ProblemOrder.Length
                       ? _engine.Evaluation.ProblemOrder[3]
                       : default(CityProblem);
                numVotes = p != default(CityProblem) ? _engine.Evaluation.ProblemVotes[p] : 0;

                if (numVotes != 0)
                {
                    VoterProblem4TextBlockText = ("problem." + p); //name
                    VoterCount4TextBlockText = (0.01 * numVotes).ToString();
                    VoterProblem4IsVisible = true;
                    VoterCount4IsVisible = true;
                }
                else
                {
                    VoterProblem4IsVisible = false;
                    VoterCount4IsVisible = false;
                }


            PopTextBlockText = (_engine.Evaluation.CityPop).ToString();
            DeltaTextBlockText = (_engine.Evaluation.DeltaCityPop).ToString();
            AssessTextBlockText = (_engine.Evaluation.CityAssValue).ToString();
            CityClassTextBlockText = (GetCityClassName(_engine.Evaluation.CityClass));
            GameLevelTextBlockText = (GetGameLevelName(_engine.GameLevel));
            ScoreTextBlockText = (_engine.Evaluation.CityScore).ToString();
            ScoreDeltaTextBlockText = (_engine.Evaluation.DeltaCityScore).ToString();
        }

        private bool _voterCount4IsVisible;

        public bool VoterCount4IsVisible
        {
            get
            {
                return this._voterCount4IsVisible;
            }
            set
            {
                this.SetProperty(ref this._voterCount4IsVisible, value);

            }
        }

        private bool _voterCount3IsVisible;

        public bool VoterCount3IsVisible
        {
            get
            {
                return this._voterCount3IsVisible;
            }
            set
            {
                this.SetProperty(ref this._voterCount3IsVisible, value);

            }
        }

        private bool _voterCount2IsVisible;

        public bool VoterCount2IsVisible
        {
            get
            {
                return this._voterCount2IsVisible;
            }
            set
            {
                this.SetProperty(ref this._voterCount2IsVisible, value);

            }
        }

        private bool _voterCount1IsVisible;

        public bool VoterCount1IsVisible
        {
            get
            {
                return this._voterCount1IsVisible;
            }
            set
            {
                this.SetProperty(ref this._voterCount1IsVisible, value);

            }
        }

        private bool _voterProblem4IsVisible;

        public bool VoterProblem4IsVisible
        {
            get
            {
                return this._voterProblem4IsVisible;
            }
            set
            {
                this.SetProperty(ref this._voterProblem4IsVisible, value);

            }
        }

        private bool _voterProblem3IsVisible;

        public bool VoterProblem3IsVisible
        {
            get
            {
                return this._voterProblem3IsVisible;
            }
            set
            {
                this.SetProperty(ref this._voterProblem3IsVisible, value);

            }
        }

        private bool _voterProblem2IsVisible;

        public bool VoterProblem2IsVisible
        {
            get
            {
                return this._voterProblem2IsVisible;
            }
            set
            {
                this.SetProperty(ref this._voterProblem2IsVisible, value);

            }
        }

        private bool _voterProblem1IsVisible;

        public bool VoterProblem1IsVisible
        {
            get
            {
                return this._voterProblem1IsVisible;
            }
            set
            {
                this.SetProperty(ref this._voterProblem1IsVisible, value);

            }
        }

        private string _scoreDeltaTextBlockText;

        public string ScoreDeltaTextBlockText
        {
            get
            {
                return this._scoreDeltaTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._scoreDeltaTextBlockText, value);

            }
        }

        private string _scoreTextBlockText;

        public string ScoreTextBlockText
        {
            get
            {
                return this._scoreTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._scoreTextBlockText, value);

            }
        }

        private string _gameLevelTextBlockText;

        public string GameLevelTextBlockText
        {
            get
            {
                return this._gameLevelTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._gameLevelTextBlockText, value);

            }
        }

        private string _cityClassTextBlockText;

        public string CityClassTextBlockText
        {
            get
            {
                return this._cityClassTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._cityClassTextBlockText, value);

            }
        }

        private string _assessTextBlockText;

        public string AssessTextBlockText
        {
            get
            {
                return this._assessTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._assessTextBlockText, value);

            }
        }

        private string _deltaTextBlockText;

        public string DeltaTextBlockText
        {
            get
            {
                return this._deltaTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._deltaTextBlockText, value);

            }
        }

        private string _popTextBlockText;

        public string PopTextBlockText
        {
            get
            {
                return this._popTextBlockText;
            }
            set
            {
                this.SetProperty(ref this._popTextBlockText, value);

            }
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

        public DelegateCommand DismissCommand { get; private set; }

        private string _dismissButtonText;
        private TelemetryClient _telemetry;

        public string DismissButtonText
        {
            get
            {
                return this._dismissButtonText;
            }
            set
            {
                this.SetProperty(ref this._dismissButtonText, value);

            }
        }

        internal void SetupAfterBasicInit(MainGamePageViewModel mainPageViewModel, Engine.Micropolis engine)
        {
            _mainPageViewModel = mainPageViewModel;
            DismissButtonText = Strings.GetString("dismiss-evaluation");

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
