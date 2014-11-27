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
    ///     Lists the various map overlay options that are available.
    /// </summary>
    public enum MapState
    {
        /// <summary>
        ///     ALMAP
        /// </summary>
        ALL,

        /// <summary>
        ///     REMAP
        /// </summary>
        RESIDENTIAL,

        /// <summary>
        ///     COMAP
        /// </summary>
        COMMERCIAL,

        /// <summary>
        ///     INMAP
        /// </summary>
        INDUSTRIAL,

        /// <summary>
        ///     RDMAP
        /// </summary>
        TRANSPORT,

        /// <summary>
        ///     PDMAP
        /// </summary>
        POPDEN_OVERLAY,

        /// <summary>
        ///     RGMAP
        /// </summary>
        GROWTHRATE_OVERLAY,

        /// <summary>
        ///     LVMAP
        /// </summary>
        LANDVALUE_OVERLAY,

        /// <summary>
        ///     CRMAP
        /// </summary>
        CRIME_OVERLAY,

        /// <summary>
        ///     PLMAP
        /// </summary>
        POLLUTE_OVERLAY,

        /// <summary>
        ///     TDMAP
        /// </summary>
        TRAFFIC_OVERLAY,

        /// <summary>
        ///     PRMAP
        /// </summary>
        POWER_OVERLAY,

        /// <summary>
        ///     FIMAP
        /// </summary>
        FIRE_OVERLAY,

        /// <summary>
        ///     POMAP
        /// </summary>
        POLICE_OVERLAY
    }
}