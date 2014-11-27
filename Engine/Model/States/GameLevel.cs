using System;

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
    ///     A GameLevel difficulty, specifing available funds at start.
    /// </summary>
    public static class GameLevel
    {
        /// <summary>
        ///     The easiest difficulty available.
        /// </summary>
        public static readonly int MIN_LEVEL = 0;

        /// <summary>
        ///     The most difficult difficulty.
        /// </summary>
        public static readonly int MAX_LEVEL = 2;

        /// <summary>
        ///     Determines whether the specified difficulty is valid.
        /// </summary>
        /// <param name="lev">The difficulty.</param>
        /// <returns></returns>
        public static bool IsValid(int lev)
        {
            return lev >= MIN_LEVEL && lev <= MAX_LEVEL;
        }

        /// <summary>
        ///     Gets the starting funds available to the player.
        /// </summary>
        /// <param name="lev">The difficulty.</param>
        /// <returns></returns>
        /// <exception cref="Exception">unexpected game level:  + lev</exception>
        public static int GetStartingFunds(int lev)
        {
            switch (lev)
            {
                case 0:
                    return 20000;
                case 1:
                    return 10000;
                case 2:
                    return 5000;
                default:
                    throw new Exception("unexpected game level: " + lev);
            }
        }
    }
}