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
    ///     Interface for effects caused by tool usage
    /// </summary>
    public interface IToolEffectIfc
    {
        /// <summary>
        ///     Gets the tile at a relative location.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <returns>a non-negative tile identifier</returns>
        int GetTile(int dx, int dy);

        /// <summary>
        ///     Makes the sound.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="sound">The sound.</param>
        void MakeSound(int dx, int dy, Sound sound);

        /// <summary>
        ///     Sets the tile value at a relative location.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="tileValue">The tile value.</param>
        void SetTile(int dx, int dy, int tileValue);

        /// <summary>
        ///     Deduct an amount from the controller's cash funds.
        /// </summary>
        /// <param name="amount">The amount.</param>
        void Spend(int amount);

        /// <summary>
        ///     Tools the result.
        /// </summary>
        /// <param name="tr">The tr.</param>
        void ToolResult(ToolResult tr);
    }
}