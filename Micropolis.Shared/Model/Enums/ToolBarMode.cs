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
    ///     Specifies the mode of the toolbar.
    ///     It can be normal, optimal for mouse usage (supports scaling),
    ///     wide, optimal for touch usage
    ///     flyout, optimal for touch usage on small devices
    /// </summary>
    public enum ToolBarMode
    {
        /// <summary>
        ///     The normal mode features small toolbar icons selectable with mouse and touch; supports scaling.
        /// </summary>
        NORMAL,

        /// <summary>
        ///     The wide mode features wide toolbar icons selectable with mouse and touch; does not support scaling.
        /// </summary>
        WIDE,

        /// <summary>
        ///     The flyout mode features wide toolbar icons selectable with mouse and touch, hidden by a big selection icon; does
        ///     not support scaling.
        /// </summary>
        FLYOUT
    }
}