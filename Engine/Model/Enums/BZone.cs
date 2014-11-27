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
    ///     Specified the zone types
    /// </summary>
    public enum BZone
    {
        /// <summary>
        ///     A residential zone with inhabitants
        /// </summary>
        RESIDENTIAL,

        /// <summary>
        ///     A hospital church providing health or religious services
        /// </summary>
        HOSPITAL_CHURCH,

        /// <summary>
        ///     A commercial complex providing goods for residents
        /// </summary>
        COMMERCIAL,

        /// <summary>
        ///     An industrial complex providing goods for commercial zones
        /// </summary>
        INDUSTRIAL,

        /// <summary>
        ///     A coal power plant providing electricity
        /// </summary>
        COAL,

        /// <summary>
        ///     A nuclear power plant providing electricity
        /// </summary>
        NUCLEAR,

        /// <summary>
        ///     A firestation to fight fires
        /// </summary>
        FIRESTATION,

        /// <summary>
        ///     A policestation to fight crime
        /// </summary>
        POLICESTATION,

        /// <summary>
        ///     An empty stadium without a game
        /// </summary>
        STADIUM_EMPTY,

        /// <summary>
        ///     A full stadium with a game at the moment
        /// </summary>
        STADIUM_FULL,

        /// <summary>
        ///     An airport with planes and helicopters
        /// </summary>
        AIRPORT,

        /// <summary>
        ///     A seaport with ships that travel in water
        /// </summary>
        SEAPORT
    }
}