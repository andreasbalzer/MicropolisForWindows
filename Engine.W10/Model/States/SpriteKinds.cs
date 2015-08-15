using System.Collections.Generic;

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
    ///     Enumeration of the various kinds of sprites that may appear in the city.
    /// </summary>
    public static class SpriteKinds
    {
        /// <summary>
        ///     The sprite kinds
        /// </summary>
        public static Dictionary<string, SpriteKind> SpriteKind = new Dictionary<string, SpriteKind>();

        /// <summary>
        ///     Initializes the <see cref="SpriteKinds" /> class.
        /// </summary>
        static SpriteKinds()
        {
            SpriteKind.Add("TRA", new SpriteKind(1, 5));
            SpriteKind.Add("COP", new SpriteKind(2, 8));
            SpriteKind.Add("AIR", new SpriteKind(3, 11));
            SpriteKind.Add("SHI", new SpriteKind(4, 8));
            SpriteKind.Add("GOD", new SpriteKind(5, 16));
            SpriteKind.Add("TOR", new SpriteKind(6, 3));
            SpriteKind.Add("EXP", new SpriteKind(7, 6));
            SpriteKind.Add("BUS", new SpriteKind(8, 4));
        }
    }
}