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
    ///     Specifies a sound location
    /// </summary>
    public class SoundInfo
    {
        /// <summary>
        ///     The sound
        /// </summary>
        public Sound Sound;

        /// <summary>
        ///     The x-coordinate of the sound
        /// </summary>
        public int X;

        /// <summary>
        ///     The y-coordinate of the sound
        /// </summary>
        public int Y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SoundInfo" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="sound">The sound.</param>
        public SoundInfo(int x, int y, Sound sound)
        {
            X = x;
            Y = y;
            Sound = sound;
        }

        public override string ToString()
        {
            return "X: " + X + ", Y: " + Y;
        }
    }
}