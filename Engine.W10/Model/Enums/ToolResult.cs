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
    ///     Lists the various results that may occur when applying a tool.
    /// </summary>
    public enum ToolResult
    {
        /// <summary>
        ///     The success (1)
        /// </summary>
        SUCCESS,

        /// <summary>
        ///     The none (0)
        /// </summary>
        NONE,

        /// <summary>
        ///     The u h_ oh (-1; invalid position)
        /// </summary>
        UH_OH,

        /// <summary>
        ///     The insufficien t_ funds (-2)
        /// </summary>
        INSUFFICIENT_FUNDS
    }
}