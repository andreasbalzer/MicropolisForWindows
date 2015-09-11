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
    ///     The listener interface for receiving miscellaneous events that occur
    ///     in the Micropolis city.
    ///     Use the Micropolis class's AddListener interface to register an object
    ///     that implements this interface.
    /// </summary>
    public interface IListener
    {
        void CityMessage(MicropolisMessage message, CityLocation loc);
        void CitySound(Sound sound, CityLocation loc);

        /// <summary>
        ///     Fired whenever the "census" is taken, and the various historical
        ///     counters have been updated. (Once a month in game.)
        /// </summary>
        void CensusChanged();


        /// <summary>
        ///     Fired whenever resValve, comValve, or indValve changes.
        ///     (Twice a month in game.)
        /// </summary>
        void DemandChanged();


        /// <summary>
        ///     Fired whenever the city evaluation is recalculated. (Once a year.)
        /// </summary>
        void EvaluationChanged();

        /// <summary>
        ///     Fired whenever the mayor's money changes.
        /// </summary>
        void FundsChanged();


        /// <summary>
        ///     Fired whenever autoBulldoze, autoBudget, noDisasters, or simSpeed change.
        /// </summary>
        void OptionsChanged();
    }
}