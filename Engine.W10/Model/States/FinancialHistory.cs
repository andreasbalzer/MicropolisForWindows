namespace Engine
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
    ///     Represents game state at specific time
    /// </summary>
    public class FinancialHistory
    {
        /// <summary>
        ///     The city time at the point of this historic state
        /// </summary>
        public int CityTime;

        /// <summary>
        ///     The operating expenses at this point in time
        /// </summary>
        public int OperatingExpenses;

        /// <summary>
        ///     The tax income at this point in time
        /// </summary>
        public int TaxIncome;

        /// <summary>
        ///     The total funds at this point in time
        /// </summary>
        public int TotalFunds;
    }
}