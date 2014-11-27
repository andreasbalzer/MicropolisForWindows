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


    public class CityRect : IEquatable<CityRect>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CityRect" /> class.
        /// </summary>
        public CityRect()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CityRect" /> class.
        /// </summary>
        /// <param name="x">The x-coordinate of the rectangle.</param>
        /// <param name="y">The y-coordinate of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public CityRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     The height of the rectangle
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     The width of the rectangle
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     The X coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///     The Y coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int Y { get; set; }


        /// <summary>
        ///     Checks for equality between this rectangle and the rectangle specified.
        /// </summary>
        /// <param name="rhs">The rectangle to check for equality with.</param>
        /// <returns></returns>
        public bool Equals(CityRect rhs)
        {
            return X == rhs.X &&
                   Y == rhs.Y &&
                   Width == rhs.Width &&
                   Height == rhs.Height;
        }


        /// <summary>
        ///     Returns this rectangle as a string.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "x: " + X + ", y: " + Y + ", width:" + Width + ", height: " + Height + "]";
        }
    }
}