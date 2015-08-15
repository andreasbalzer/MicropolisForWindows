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
    ///     Enumeration of various city problems that the citizens complain about.
    /// </summary>
    public enum CityProblem
    {
        /// <summary>
        ///     The crime problem can be solved by police
        /// </summary>
        CRIME,

        /// <summary>
        ///     The pollution problem can be solved with forrests and parks (hopefully)
        /// </summary>
        POLLUTION,

        /// <summary>
        ///     The housing problem can be solved by adding residential zones.
        /// </summary>
        HOUSING,

        /// <summary>
        ///     The taxes problem can be solved by lowering taxes
        /// </summary>
        TAXES,

        /// <summary>
        ///     The traffic problem can be solved by more transportation options or by rezoning
        /// </summary>
        TRAFFIC,

        /// <summary>
        ///     The unemployment problem can be solved by adding commercial and industrial zones and hopefully by offering better
        ///     transportation
        /// </summary>
        UNEMPLOYMENT,

        /// <summary>
        ///     The fire problem can be solved by adding more fire brigades
        /// </summary>
        FIRE
    }
}