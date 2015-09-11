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

    using Windows.UI;
    using Engine;

    /// <summary>
    ///     ToolCursor used when a tool is selected and mouse hovers over map
    /// </summary>
    public class ToolCursor
    {
        /// <summary>
        ///     The border color of the rectangle displayed on top of map
        /// </summary>
        public Color BorderColor;

        /// <summary>
        ///     The fill color of the rectangle displayed on top of map
        /// </summary>
        public Color FillColor;

        /// <summary>
        ///     The rect displayed on top of map
        /// </summary>
        public CityRect Rect;
    }
}