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
    ///     Three settings on whether to generate a new map as an island.
    /// </summary>
    public enum CreateIsland
    {
        /// <summary>
        ///     Never creates an island
        /// </summary>
        NEVER,

        /// <summary>
        ///     Always creates an island
        /// </summary>
        ALWAYS,

        /// <summary>
        ///     Seldomly, creates an island (seldom == 10% of the time)
        /// </summary>
        SELDOM
    }
}