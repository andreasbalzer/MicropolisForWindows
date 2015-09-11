using System;
using System.Globalization;
using Windows.UI;

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
    ///     Parses a string representing a color.
    ///     Color can be specified either as #RRGGBB (hex) or as rgba(r,g,b,a) (int)
    /// </summary>
    public static class ColorParser
    {
        /// <summary>
        ///     Parses the color.
        ///     Color can be specified either as #RRGGBB (hex) or as rgba(r,g,b,a) (int)
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        /// <exception cref="Exception">invalid color format:  + str</exception>
        public static Color ParseColor(String str)
        {
            // format: #RRGGBB
            if (str.StartsWith("#") && str.Length == 7)
            {
                return Color.FromArgb(
                    255,
                    (byte) int.Parse(str.Substring(1, 2), NumberStyles.HexNumber),
                    (byte) int.Parse(str.Substring(3, 2), NumberStyles.HexNumber),
                    (byte) int.Parse(str.Substring(5, 2), NumberStyles.HexNumber));
            }

            // format: rgba(RRGGBBAA)
            if (str.StartsWith("rgba(") && str.EndsWith(")"))
            {
                String[] parts = str.Substring(5, str.Length - 1 - 5).Split(',');
                int r = Convert.ToInt32(parts[0]);
                int g = Convert.ToInt32(parts[1]);
                int b = Convert.ToInt32(parts[2]);
                double aa = Convert.ToDouble(parts[3], CultureInfo.InvariantCulture);
                int a = Math.Min(255, (int) Math.Floor(aa*256.0));
                return Color.FromArgb((byte) a, (byte) r, (byte) g, (byte) b);
            }
            throw new Exception("invalid color format: " + str);
        }
    }
}