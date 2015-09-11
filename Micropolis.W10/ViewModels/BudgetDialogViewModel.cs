using System;
using Engine;
using Micropolis.Common;
using Microsoft.ApplicationInsights;

namespace Micropolis.ViewModels
{
    public class BudgetDialogViewModel : BindableBase
    {
        private readonly TelemetryClient _telemetry;
        private string _autoBudgetButtonText;
        private string _capExpenses1TextBlockText;
        private string _capExpenses2TextBlockText;
        private string[] _capExpensesLbl;
        private string _continueButtonText;
        private string _costsTextBlockText;
        private Engine.Micropolis _engine;
        private string _financingTextBlockText;
        private string _fireExpensesTextBlockText;
        private string _fireFundAlloc;
        private int _fireFundEntry;
        private string _fireFundRequest;
        private string _investionsTextBlockText;
        private bool _isAutoBudget;
        private bool _isPause;
        private MainGamePageViewModel _mainPageViewModel;
        private string _moneyAtBeginningOfYearTextBlockText;
        private string _moneyAtEndOfYearTextBlockText;
        private string _newBalance1TextBlockText;
        private string _newBalance2TextBlockText;
        private string[] _newBalanceLbl;
        private string _opExpenses1TextBlockText;
        private string _opExpenses2TextBlockText;
        private string[] _opExpensesLbl;
        private double _origFirePct;
        private double _origPolicePct;
        private double _origRoadPct;
        private int _origTaxRate;
        private string _pauseButtonText;
        private string _payedTextBlockText;
        private string _policeExpensesTextBlockText;
        private string _policeFundAlloc;
        private int _policeFundEntry;
        private string _policeFundRequest;
        private string _previousBalance1TextBlockText;
        private string _previousBalance2TextBlockText;
        private string[] _previousBalanceLbl;
        private string _requestedTextBlockText;
        private string _resetButtonText;
        private string _roadFundAlloc;
        private int _roadFundEntry;
        private string _roadFundRequest;
        private string _taxesMadeTextBlockText;
        private string _taxIncome1TextBlockText;
        private string _taxIncome2TextBlockText;
        private string[] _taxIncomeLbl;
        private string _taxIncomeTextBlockText;
        private int _taxRateEntry;
        private string _taxRevenue;
        private string _tayTextBlockText;
        private string _th1TextBlockText;
        private string _th2TextBlockText;
        private string[] _thLbl;
        private string _titleTextBlockText;
        private string _trafficExpensesTextBlockText;
        private string _yearlyIncomeTextBlockText;
        private string _yearlyReportTextBlockText;

        public BudgetDialogViewModel()
        {
            ContinueCommand = new DelegateCommand(Continue);
            ResetCommand = new DelegateCommand(Reset);

            try { 
            _telemetry = new TelemetryClient();
            }
            catch (Exception) { }
        }

        public DelegateCommand ContinueCommand { get; private set; }
        public DelegateCommand ResetCommand { get; private set; }
        public bool EnableTimerWhenClosing { get; set; }

        public int TaxRateEntry
        {
            get { return _taxRateEntry; }
            set { SetProperty(ref _taxRateEntry, value); }
        }

        public int RoadFundEntry
        {
            get { return _roadFundEntry; }
            set { SetProperty(ref _roadFundEntry, value); }
        }

        public int PoliceFundEntry
        {
            get { return _policeFundEntry; }
            set { SetProperty(ref _policeFundEntry, value); }
        }

        public int FireFundEntry
        {
            get { return _fireFundEntry; }
            set { SetProperty(ref _fireFundEntry, value); }
        }

        public string RoadFundRequest
        {
            get { return _roadFundRequest; }
            set { SetProperty(ref _roadFundRequest, value); }
        }

        public string PoliceFundRequest
        {
            get { return _policeFundRequest; }
            set { SetProperty(ref _policeFundRequest, value); }
        }

        public string FireFundRequest
        {
            get { return _fireFundRequest; }
            set { SetProperty(ref _fireFundRequest, value); }
        }

        public string RoadFundAlloc
        {
            get { return _roadFundAlloc; }
            set { SetProperty(ref _roadFundAlloc, value); }
        }

        public string PoliceFundAlloc
        {
            get { return _policeFundAlloc; }
            set { SetProperty(ref _policeFundAlloc, value); }
        }

        public string FireFundAlloc
        {
            get { return _fireFundAlloc; }
            set { SetProperty(ref _fireFundAlloc, value); }
        }

        public string TaxRevenue
        {
            get { return _taxRevenue; }
            set { SetProperty(ref _taxRevenue, value); }
        }

        public string AutoBudgetButtonText
        {
            get { return _autoBudgetButtonText; }
            set { SetProperty(ref _autoBudgetButtonText, value); }
        }

        public string PauseButtonText
        {
            get { return _pauseButtonText; }
            set { SetProperty(ref _pauseButtonText, value); }
        }

        public string TitleTextBlockText
        {
            get { return _titleTextBlockText; }
            set { SetProperty(ref _titleTextBlockText, value); }
        }

        public string ContinueButtonText
        {
            get { return _continueButtonText; }
            set { SetProperty(ref _continueButtonText, value); }
        }

        public string ResetButtonText
        {
            get { return _resetButtonText; }
            set { SetProperty(ref _resetButtonText, value); }
        }

        public string FireExpensesTextBlockText
        {
            get { return _fireExpensesTextBlockText; }
            set { SetProperty(ref _fireExpensesTextBlockText, value); }
        }

        public string PoliceExpensesTextBlockText
        {
            get { return _policeExpensesTextBlockText; }
            set { SetProperty(ref _policeExpensesTextBlockText, value); }
        }

        public string TrafficExpensesTextBlockText
        {
            get { return _trafficExpensesTextBlockText; }
            set { SetProperty(ref _trafficExpensesTextBlockText, value); }
        }

        public string PayedTextBlockText
        {
            get { return _payedTextBlockText; }
            set { SetProperty(ref _payedTextBlockText, value); }
        }

        public string RequestedTextBlockText
        {
            get { return _requestedTextBlockText; }
            set { SetProperty(ref _requestedTextBlockText, value); }
        }

        public string FinancingTextBlockText
        {
            get { return _financingTextBlockText; }
            set { SetProperty(ref _financingTextBlockText, value); }
        }

        public bool IsAutoBudget
        {
            get { return _isAutoBudget; }
            set
            {
                SetProperty(ref _isAutoBudget, value);

                try
                {
                    if (value)
                    {
                        _telemetry.TrackEvent("BudgetDialogAutoBudgetEnabled");
                    }
                    else
                    {
                        _telemetry.TrackEvent("BudgetDialogAutoBudgetDisabled");
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public bool IsPause
        {
            get { return _isPause; }
            set
            {
                SetProperty(ref _isPause, value);
                try
                {
                    if (value)
                    {
                        _telemetry.TrackEvent("BudgetDialogPauseEnabled");
                    }
                    else
                    {
                        _telemetry.TrackEvent("BudgetDialogPauseDisabled");
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public string TayTextBlockText
        {
            get { return _tayTextBlockText; }
            set { SetProperty(ref _tayTextBlockText, value); }
        }

        public string YearlyIncomeTextBlockText
        {
            get { return _yearlyIncomeTextBlockText; }
            set { SetProperty(ref _yearlyIncomeTextBlockText, value); }
        }

        public string TaxIncomeTextBlockText
        {
            get { return _taxIncomeTextBlockText; }
            set { SetProperty(ref _taxIncomeTextBlockText, value); }
        }

        public string YearlyReportTextBlockText
        {
            get { return _yearlyReportTextBlockText; }
            set { SetProperty(ref _yearlyReportTextBlockText, value); }
        }

        public string MoneyAtBeginningOfYearTextBlockText
        {
            get { return _moneyAtBeginningOfYearTextBlockText; }
            set { SetProperty(ref _moneyAtBeginningOfYearTextBlockText, value); }
        }

        public string TaxesMadeTextBlockText
        {
            get { return _taxesMadeTextBlockText; }
            set { SetProperty(ref _taxesMadeTextBlockText, value); }
        }

        public string InvestionsTextBlockText
        {
            get { return _investionsTextBlockText; }
            set { SetProperty(ref _investionsTextBlockText, value); }
        }

        public string CostsTextBlockText
        {
            get { return _costsTextBlockText; }
            set { SetProperty(ref _costsTextBlockText, value); }
        }

        public string MoneyAtEndOfYearTextBlockText
        {
            get { return _moneyAtEndOfYearTextBlockText; }
            set { SetProperty(ref _moneyAtEndOfYearTextBlockText, value); }
        }

        public string Th1TextBlockText
        {
            get { return _th1TextBlockText; }
            set { SetProperty(ref _th1TextBlockText, value); }
        }

        public string Th2TextBlockText
        {
            get { return _th2TextBlockText; }
            set { SetProperty(ref _th2TextBlockText, value); }
        }

        public string PreviousBalance1TextBlockText
        {
            get { return _previousBalance1TextBlockText; }
            set { SetProperty(ref _previousBalance1TextBlockText, value); }
        }

        public string PreviousBalance2TextBlockText
        {
            get { return _previousBalance2TextBlockText; }
            set { SetProperty(ref _previousBalance2TextBlockText, value); }
        }

        public string TaxIncome1TextBlockText
        {
            get { return _taxIncome1TextBlockText; }
            set { SetProperty(ref _taxIncome1TextBlockText, value); }
        }

        public string TaxIncome2TextBlockText
        {
            get { return _taxIncome2TextBlockText; }
            set { SetProperty(ref _taxIncome2TextBlockText, value); }
        }

        public string CapExpenses1TextBlockText
        {
            get { return _capExpenses1TextBlockText; }
            set { SetProperty(ref _capExpenses1TextBlockText, value); }
        }

        public string CapExpenses2TextBlockText
        {
            get { return _capExpenses2TextBlockText; }
            set { SetProperty(ref _capExpenses2TextBlockText, value); }
        }

        public string OpExpenses1TextBlockText
        {
            get { return _opExpenses1TextBlockText; }
            set { SetProperty(ref _opExpenses1TextBlockText, value); }
        }

        public string OpExpenses2TextBlockText
        {
            get { return _opExpenses2TextBlockText; }
            set { SetProperty(ref _opExpenses2TextBlockText, value); }
        }

        public string NewBalance1TextBlockText
        {
            get { return _newBalance1TextBlockText; }
            set { SetProperty(ref _newBalance1TextBlockText, value); }
        }

        public string NewBalance2TextBlockText
        {
            get { return _newBalance2TextBlockText; }
            set { SetProperty(ref _newBalance2TextBlockText, value); }
        }

        /// <summary>
        ///     Applies the changes entered by user into forms in the dialog.
        /// </summary>
        internal void ApplyChange()
        {
            var newTaxRate = TaxRateEntry;
            var newRoadPct = RoadFundEntry;
            var newPolicePct = PoliceFundEntry;
            var newFirePct = FireFundEntry;

            _engine.CityTax = newTaxRate;
            _engine.RoadPercent = newRoadPct/100.0;
            _engine.PolicePercent = newPolicePct/100.0;
            _engine.FirePercent = newFirePct/100.0;

            LoadBudgetNumbers(false);
        }

        /// <summary>
        ///     Loads the budget numbers from the game engine and displays them in the dialog.
        /// </summary>
        /// <param name="updateEntries">if set to <c>true</c>, entries get updated at beginning.</param>
        private void LoadBudgetNumbers(bool updateEntries)
        {
            var b = _engine.GenerateBudget();
            if (updateEntries)
            {
                TaxRateEntry = b.TaxRate;
                RoadFundEntry = (int) Math.Round(b.RoadPercent*100.0);
                PoliceFundEntry = (int) Math.Round(b.PolicePercent*100.0);
                FireFundEntry = (int) Math.Round(b.FirePercent*100.0);
            }

            TaxRevenue = MainGamePageViewModel.FormatFunds(b.TaxIncome);

            RoadFundRequest = MainGamePageViewModel.FormatFunds(b.RoadRequest);
            RoadFundAlloc = MainGamePageViewModel.FormatFunds(b.RoadFunded);

            PoliceFundRequest = MainGamePageViewModel.FormatFunds(b.PoliceRequest);
            PoliceFundAlloc = MainGamePageViewModel.FormatFunds(b.PoliceFunded);

            FireFundRequest = MainGamePageViewModel.FormatFunds(b.FireRequest);
            FireFundAlloc = MainGamePageViewModel.FormatFunds(b.FireFunded);
        }

        /// <summary>
        ///     Setups the after basic initialize.
        /// </summary>
        /// <param name="mainPage">The main page.</param>
        /// <param name="engine">The engine.</param>
        public void SetupAfterBasicInit(MainGamePageViewModel mainPageViewModel, Engine.Micropolis engine)
        {
            _mainPageViewModel = mainPageViewModel;
            AutoBudgetButtonText = Strings.GetString("budgetdlg.auto_budget");
            PauseButtonText = Strings.GetString("budgetdlg.pause_game");

            TitleTextBlockText = Strings.GetString("budgetdlg.title");
            _engine = engine;
            _origTaxRate = engine.CityTax;
            _origRoadPct = engine.RoadPercent;
            _origFirePct = engine.FirePercent;
            _origPolicePct = engine.PolicePercent;


            ContinueButtonText = Strings.GetString("budgetdlg.continue");

            ResetButtonText = Strings.GetString("budgetdlg.reset");


            LoadBudgetNumbers(true);

            MakeFundingRatesPane();
            MakeOptionsPane();
            MakeTaxPane();
            MakeBalancePane();
        }

        public void SetEngine(Engine.Micropolis engine)
        {
            _engine = engine;
            LoadBudgetNumbers(true);

            MakeFundingRatesPane();
            MakeOptionsPane();
            MakeTaxPane();
            MakeBalancePane();
        }

        /// <summary>
        ///     Makes the funding rates pane.
        /// </summary>
        private void MakeFundingRatesPane()
        {
            FinancingTextBlockText = Strings.GetString("budgetdlg.funding_level_hdr");
            RequestedTextBlockText = Strings.GetString("budgetdlg.requested_hdr");
            PayedTextBlockText = Strings.GetString("budgetdlg.allocation_hdr");
            TrafficExpensesTextBlockText = Strings.GetString("budgetdlg.road_fund");
            PoliceExpensesTextBlockText = Strings.GetString("budgetdlg.police_fund");
            FireExpensesTextBlockText = Strings.GetString("budgetdlg.fire_fund");
        }

        /// <summary>
        ///     Makes the options pane.
        /// </summary>
        private void MakeOptionsPane()
        {
            IsAutoBudget = (_engine.AutoBudget);
            IsPause = (_engine.SimSpeed == Speeds.Speed["PAUSED"]);
        }

        /// <summary>
        ///     Makes the tax pane.
        /// </summary>
        private void MakeTaxPane()
        {
            TayTextBlockText = Strings.GetString("budgetdlg.tax_rate_hdr");
            YearlyIncomeTextBlockText = Strings.GetString("budgetdlg.annual_receipts_hdr");
            TaxIncomeTextBlockText = Strings.GetString("budgetdlg.tax_revenue");
        }

        /// <summary>
        ///     Gets called when user clicks continue button. Checks for changes in the dialog and forwards them in the game.
        /// </summary>
        private void Continue()
        {
            try
            {
                _telemetry.TrackEvent("BudgetDialogContinueClicked");
            }
            catch (Exception)
            {
            }

            if (IsAutoBudget != _engine.AutoBudget)
            {
                _engine.ToggleAutoBudget();
            }
            if (IsPause && _engine.SimSpeed != Speeds.Speed["PAUSED"])
            {
                _engine.SetSpeed(Speeds.Speed["PAUSED"]);
            }
            else if (!IsPause &&
                     _engine.SimSpeed == Speeds.Speed["PAUSED"])
            {
                _engine.SetSpeed(Speeds.Speed["NORMAL"]);
            }

            _mainPageViewModel.StartTimer();

            _mainPageViewModel.HideBudgetDialog();
        }

        /// <summary>
        ///     Gets called when user clicks reset button. Resets possible changes in dialog to their previous state.
        /// </summary>
        private void Reset()
        {
            try
            {
                _telemetry.TrackEvent("BudgetDialogResetClicked");
            }
            catch (Exception)
            {
            }


            _engine.CityTax = _origTaxRate;
            _engine.RoadPercent = _origRoadPct;
            _engine.FirePercent = _origFirePct;
            _engine.PolicePercent = _origPolicePct;
            LoadBudgetNumbers(true);
        }

        /// <summary>
        ///     Makes the balance pane.
        /// </summary>
        private void MakeBalancePane()
        {
            YearlyReportTextBlockText = Strings.GetString("budgetdlg.period_ending");
            MoneyAtBeginningOfYearTextBlockText = Strings.GetString("budgetdlg.cash_begin");
            TaxesMadeTextBlockText = Strings.GetString("budgetdlg.taxes_collected");
            InvestionsTextBlockText = Strings.GetString("budgetdlg.capital_expenses");
            CostsTextBlockText = Strings.GetString("budgetdlg.operating_expenses");
            MoneyAtEndOfYearTextBlockText = Strings.GetString("budgetdlg.cash_end");


            if (1 >= _engine.FinancialHistory.Count)
            {
                return;
            }

            var f = _engine.FinancialHistory[0];
            var fPrior = _engine.FinancialHistory[1];
            var cashFlow = f.TotalFunds - fPrior.TotalFunds;
            var capExpenses = -(cashFlow - f.TaxIncome + f.OperatingExpenses);


            Th1TextBlockText = MainGamePageViewModel.FormatGameDate(f.CityTime - 1);

            PreviousBalance1TextBlockText = MainGamePageViewModel.FormatFunds(fPrior.TotalFunds);
            TaxIncome1TextBlockText = MainGamePageViewModel.FormatFunds(f.TaxIncome);
            CapExpenses1TextBlockText = MainGamePageViewModel.FormatFunds(capExpenses);
            OpExpenses1TextBlockText = MainGamePageViewModel.FormatFunds(f.OperatingExpenses);
            NewBalance1TextBlockText = MainGamePageViewModel.FormatFunds(f.TotalFunds);

            if (2 >= _engine.FinancialHistory.Count)
            {
                return;
            }
            var f2 = _engine.FinancialHistory[1];
            var fPrior2 = _engine.FinancialHistory[2];
            var cashFlow2 = f2.TotalFunds - fPrior2.TotalFunds;
            var capExpenses2 = -(cashFlow2 - f2.TaxIncome + f2.OperatingExpenses);


            Th2TextBlockText = MainGamePageViewModel.FormatGameDate(f2.CityTime - 1);

            PreviousBalance2TextBlockText = MainGamePageViewModel.FormatFunds(fPrior2.TotalFunds);
            TaxIncome2TextBlockText = MainGamePageViewModel.FormatFunds(f2.TaxIncome);
            CapExpenses2TextBlockText = MainGamePageViewModel.FormatFunds(capExpenses2);
            OpExpenses2TextBlockText = MainGamePageViewModel.FormatFunds(f2.OperatingExpenses);
            NewBalance2TextBlockText = MainGamePageViewModel.FormatFunds(f2.TotalFunds);
        }
    }
}