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
    ///     City budget with currently available funds, prepaid maintenance and taxes
    /// </summary>
    public class CityBudget
    {
        /// <summary>
        ///     Reference to the city of this object
        /// </summary>
        private Micropolis _city;

        /// <summary>
        ///     Amount of prepaid fire station maintenance (in 1/TAXFREQ's).
        /// </summary>
        public int FireFundEscrow;

        /// <summary>
        ///     Amount of prepaid police station maintenance (in 1/TAXFREQ's).
        /// </summary>
        public int PoliceFundEscrow;

        /// <summary>
        ///     Amount of prepaid road maintenance (in 1/TAXFREQ's).
        /// </summary>
        public int RoadFundEscrow;

        /// <summary>
        ///     Amount of taxes collected so far in the current financial period (in 1/TAXFREQ's).
        /// </summary>
        public int TaxFund;

        /// <summary>
        ///     The amount of cash on hand.
        /// </summary>
        public int TotalFunds;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CityBudget" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        public CityBudget(Micropolis city)
        {
            _city = city;
        }
    }
}