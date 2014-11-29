using System;
using Windows.UI.Xaml.Controls;
using Engine;

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
    ///     The budget dialog appeas every year. It offers the user the possibility to inspect current budget, compare to last
    ///     budget and make tax changes as well as changes in maintenance funding.
    /// </summary>
    public sealed partial class BudgetDialog
    {
        private TextBlock[] _capExpensesLbl;
        private Engine.Micropolis _engine;
        private MainGamePage _mainPage;
        private TextBlock[] _newBalanceLbl;
        private TextBlock[] _opExpensesLbl;

        private double _origFirePct;
        private double _origPolicePct;
        private double _origRoadPct;
        private int _origTaxRate;
        private TextBlock[] _previousBalanceLbl;
        private TextBlock[] _taxIncomeLbl;
        private TextBlock[] _thLbl;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BudgetDialog" /> class.
        /// </summary>
        public BudgetDialog()
        {
            InitializeComponent();
            SetupTextBlocks();
        }

        public bool EnableTimerWhenClosing { get; set; }

        /// <summary>
        ///     Applies the changes entered by user into forms in the dialog.
        /// </summary>
        private void ApplyChange()
        {
            var newTaxRate = (int) taxRateEntry.Value;
            var newRoadPct = (int) roadFundEntry.Value;
            var newPolicePct = (int) policeFundEntry.Value;
            var newFirePct = (int) fireFundEntry.Value;

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
            BudgetNumbers b = _engine.GenerateBudget();
            if (updateEntries)
            {
                taxRateEntry.Value = b.TaxRate;
                roadFundEntry.Value = (int) Math.Round(b.RoadPercent*100.0);
                policeFundEntry.Value = (int) Math.Round(b.PolicePercent*100.0);
                fireFundEntry.Value = (int) Math.Round(b.FirePercent*100.0);
            }

            taxRevenueLbl.Text = MainGamePage.FormatFunds(b.TaxIncome);

            roadFundRequest.Text = MainGamePage.FormatFunds(b.RoadRequest);
            roadFundAlloc.Text = MainGamePage.FormatFunds(b.RoadFunded);

            policeFundRequest.Text = MainGamePage.FormatFunds(b.PoliceRequest);
            policeFundAlloc.Text = MainGamePage.FormatFunds(b.PoliceFunded);

            fireFundRequest.Text = MainGamePage.FormatFunds(b.FireRequest);
            fireFundAlloc.Text = MainGamePage.FormatFunds(b.FireFunded);
        }

        /// <summary>
        ///     Setups the after basic initialize.
        /// </summary>
        /// <param name="mainPage">The main page.</param>
        /// <param name="engine">The engine.</param>
        public void SetupAfterBasicInit(MainGamePage mainPage, Engine.Micropolis engine)
        {
         

            _mainPage = mainPage;
            autoBudgetBtn.Content = Strings.GetString("budgetdlg.auto_budget");
            pauseBtn.Content = Strings.GetString("budgetdlg.pause_game");

            titleTbl.Text = Strings.GetString("budgetdlg.title");
            _engine = engine;
            _origTaxRate = engine.CityTax;
            _origRoadPct = engine.RoadPercent;
            _origFirePct = engine.FirePercent;
            _origPolicePct = engine.PolicePercent;

            taxRateEntry.ValueChanged += delegate { ApplyChange(); };
            roadFundEntry.ValueChanged += delegate { ApplyChange(); };
            fireFundEntry.ValueChanged += delegate { ApplyChange(); };
            policeFundEntry.ValueChanged += delegate { ApplyChange(); };


            continueBtn.Content = Strings.GetString("budgetdlg.continue");
            continueBtn.Click += delegate { OnContinueClicked(); };


            resetBtn.Content = Strings.GetString("budgetdlg.reset");
            resetBtn.Click += delegate { OnResetClicked(); };


            LoadBudgetNumbers(true);

            MakeFundingRatesPane();
            MakeOptionsPane();
            MakeTaxPane();
            MakeBalancePane();
        }

        public void SetEngine(Engine.Micropolis engine)
        {
            this._engine = engine;
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
            financingTbl.Text = Strings.GetString("budgetdlg.funding_level_hdr");
            requestedTbl.Text = Strings.GetString("budgetdlg.requested_hdr");
            payedTbl.Text = Strings.GetString("budgetdlg.allocation_hdr");
            trafficExpensesTbl.Text = Strings.GetString("budgetdlg.road_fund");
            policeExpensesTbl.Text = Strings.GetString("budgetdlg.police_fund");
            fireExpensesTbl.Text = Strings.GetString("budgetdlg.fire_fund");
        }

        /// <summary>
        ///     Makes the options pane.
        /// </summary>
        private void MakeOptionsPane()
        {
            autoBudgetBtn.IsChecked = (_engine.AutoBudget);
            pauseBtn.IsChecked = (_engine.SimSpeed == Speeds.Speed["PAUSED"]);
        }

        /// <summary>
        ///     Makes the tax pane.
        /// </summary>
        private void MakeTaxPane()
        {
            tayTbl.Text = Strings.GetString("budgetdlg.tax_rate_hdr");
            yearlyIncomeTbl.Text = Strings.GetString("budgetdlg.annual_receipts_hdr");
            taxIncomeTbl.Text = Strings.GetString("budgetdlg.tax_revenue");
        }

        /// <summary>
        ///     Gets called when user clicks continue button. Checks for changes in the dialog and forwards them in the game.
        /// </summary>
        private void OnContinueClicked()
        {
            if (autoBudgetBtn.IsChecked != _engine.AutoBudget)
            {
                _engine.ToggleAutoBudget();
            }
            if (pauseBtn.IsChecked.HasValue && pauseBtn.IsChecked.Value && _engine.SimSpeed != Speeds.Speed["PAUSED"])
            {
                _engine.SetSpeed(Speeds.Speed["PAUSED"]);
            }
            else if (pauseBtn.IsChecked.HasValue && !pauseBtn.IsChecked.Value &&
                     _engine.SimSpeed == Speeds.Speed["PAUSED"])
            {
                _engine.SetSpeed(Speeds.Speed["NORMAL"]);
            }
            
                _mainPage.StartTimer();
            
            _mainPage.HideBudgetDialog();
        }

        /// <summary>
        ///     Gets called when user clicks reset button. Resets possible changes in dialog to their previous state.
        /// </summary>
        private void OnResetClicked()
        {
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
            yearlyReportTbl.Text = Strings.GetString("budgetdlg.period_ending");
            moneyAtBeginningOfYearTbl.Text = Strings.GetString("budgetdlg.cash_begin");
            taxesMadeTbl.Text = Strings.GetString("budgetdlg.taxes_collected");
            investionsTbl.Text = Strings.GetString("budgetdlg.capital_expenses");
            costsTbl.Text = Strings.GetString("budgetdlg.operating_expenses");
            moneyAtEndOfYearTbl.Text = Strings.GetString("budgetdlg.cash_end");

            for (int i = 0; i < 2; i++)
            {
                if (i + 1 >= _engine.FinancialHistory.Count)
                {
                    break;
                }

                FinancialHistory f = _engine.FinancialHistory[i];
                FinancialHistory fPrior = _engine.FinancialHistory[i + 1];
                int cashFlow = f.TotalFunds - fPrior.TotalFunds;
                int capExpenses = -(cashFlow - f.TaxIncome + f.OperatingExpenses);


                _thLbl[i].Text = MainGamePage.FormatGameDate(f.CityTime - 1);

                _previousBalanceLbl[i].Text = MainGamePage.FormatFunds(fPrior.TotalFunds);
                _taxIncomeLbl[i].Text = MainGamePage.FormatFunds(f.TaxIncome);
                _capExpensesLbl[i].Text = MainGamePage.FormatFunds(capExpenses);
                _opExpensesLbl[i].Text = MainGamePage.FormatFunds(f.OperatingExpenses);
                _newBalanceLbl[i].Text = MainGamePage.FormatFunds(f.TotalFunds);
            }
        }

        /// <summary>
        /// Sets up textblocks for year-related data.
        /// </summary>
        private void SetupTextBlocks()
        {
            _thLbl = new TextBlock[2];
            _thLbl[0] = thLbl1;
            _thLbl[1] = thLbl2;

            _previousBalanceLbl = new TextBlock[2];
            _previousBalanceLbl[0] = previousBalanceLbl1;
            _previousBalanceLbl[1] = previousBalanceLbl2;

            _taxIncomeLbl = new TextBlock[2];
            _taxIncomeLbl[0] = taxIncomeLbl1;
            _taxIncomeLbl[1] = taxIncomeLbl2;

            _capExpensesLbl = new TextBlock[2];
            _capExpensesLbl[0] = capExpensesLbl1;
            _capExpensesLbl[1] = capExpensesLbl2;

            _opExpensesLbl = new TextBlock[2];
            _opExpensesLbl[0] = opExpensesLbl1;
            _opExpensesLbl[1] = opExpensesLbl2;

            _newBalanceLbl = new TextBlock[2];
            _newBalanceLbl[0] = newBalanceLbl1;
            _newBalanceLbl[1] = newBalanceLbl2;
        }
    }
}