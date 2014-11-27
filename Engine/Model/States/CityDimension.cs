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
    ///     Encapsulates the width and height of a rectangular section of a Micropolis city.
    /// </summary>
    public class CityDimension : IEquatable<CityDimension>
    {
        /// <summary>
        ///     The height of the city
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     The width of the city
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CityDimension" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public CityDimension(int width, int height)
        {
            Width = width;
            Height = height;
        }


        /// <summary>
        ///     Gets the HashCode of this city
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Width*33 + Height;
        }


        /// <summary>
        ///     Checks for equality between this CityDimension and the CityDimension provided
        /// </summary>
        /// <param name="rhs">The object.</param>
        /// <returns></returns>
        public bool Equals(CityDimension rhs)
        {
                return Width == rhs.Width && Height == rhs.Height;
        }


        /// <summary>
        ///     Returns this CityDimension as a string
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "width: "+Width + ", height:" + Height;
        }
    }
}