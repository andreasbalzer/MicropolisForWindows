namespace Engine
{
    /*
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
    */

    /// <summary>
    ///     Budget numbers specified in BudgetDialog by user or automatically by engine.
    /// </summary>
    public class BudgetNumbers
    {
        #region tax

        /// <summary>
        ///     The new balance of this period
        /// </summary>
        public int NewBalance;

        /// <summary>
        ///     The operating expenses the user has spent
        /// </summary>
        public int OperatingExpenses;

        /// <summary>
        ///     The balance of the previous period
        /// </summary>
        public int PreviousBalance;

        /// <summary>
        ///     The tax income of the user
        /// </summary>
        public int TaxIncome;

        /// <summary>
        ///     The tax rate citizens have to pay
        /// </summary>
        public int TaxRate;

        #endregion

        #region roads

        /// <summary>
        ///     The money the user wants to spend on road repairs
        /// </summary>
        public int RoadFunded;

        /// <summary>
        ///     The required money to repair roads
        /// </summary>
        public int RoadRequest;

        /// <summary>
        ///     The percentage of roads budget between paid and required
        /// </summary>
        public double RoadPercent;

        #endregion

        #region fire

        /// <summary>
        ///     The money the user wants to spend on firebrigades
        /// </summary>
        public int FireFunded;

        /// <summary>
        ///     The percentage of fire brigade budget between paid and required
        /// </summary>
        public double FirePercent;

        /// <summary>
        ///     The required money to pay for firebrigades
        /// </summary>
        public int FireRequest;

        #endregion

        #region police

        /// <summary>
        ///     The money the user wants to spend on police services
        /// </summary>
        public int PoliceFunded;

        /// <summary>
        ///     The percentage of the police budget between paid and required
        /// </summary>
        public double PolicePercent;

        /// <summary>
        ///     The required money to pay for police services
        /// </summary>
        public int PoliceRequest;

        #endregion
    }
}