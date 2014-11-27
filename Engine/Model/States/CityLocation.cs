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
    ///     Coordinates of a location (x,y) in the city.
    /// </summary>
    public class CityLocation : IEquatable<CityLocation>
    {
        /// <summary>
        ///     The X coordinate of this location
        ///     Increasing X coordinates correspond to East,
        ///     and decreasing X coordinates correspond to West.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///     The Y coordinate of this location.
        ///     Increasing Y coordinates correspond to South,
        ///     and decreasing Y coordinates correspond to North.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CityLocation" /> class.
        ///     Constructs and initializes city coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public CityLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Gets the HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return X*33 + Y;
        }

        /// <summary>
        ///     Checks for equality between this CityLocation and the CityLocation specified.
        /// </summary>
        /// <param name="rhs">The object.</param>
        /// <returns></returns>
        public bool Equals(CityLocation rhs)
        {
            return X == rhs.X && Y == rhs.Y;
        }

        /// <summary>
        ///     Returns this location as a string.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "(" + X + "," + Y + ")";
        }
    }
}