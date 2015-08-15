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
    ///     Enumerates the various tools that can be applied to the map by the user. Call the tool's apply() method to actually
    ///     use the tool on the map.
    /// </summary>
    public static class MicropolisTools
    {
        /// <summary>
        ///     The micropolis tools
        /// </summary>
        public static Dictionary<string, MicropolisTool> MicropolisTool;

        /// <summary>
        ///     Initializes the <see cref="MicropolisTools" /> class.
        /// </summary>
        static MicropolisTools()
        {
            MicropolisTool = new Dictionary<string, MicropolisTool>();
            MicropolisTool.Add("EMPTY", new MicropolisTool("EMPTY", 0, 0));
            MicropolisTool.Add("BULLDOZER", new MicropolisTool("BULLDOZER", 1, 1));
            MicropolisTool.Add("WIRE", new MicropolisTool("WIRE", 1, 5)); //const=25 for underwater
            MicropolisTool.Add("ROADS", new MicropolisTool("ROADS", 1, 10)); //cost=50 for over water
            MicropolisTool.Add("RAIL", new MicropolisTool("RAIL", 1, 20)); //cost=100 for underwater
            MicropolisTool.Add("RESIDENTIAL", new MicropolisTool("RESIDENTIAL", 3, 100));
            MicropolisTool.Add("COMMERCIAL", new MicropolisTool("COMMERCIAL", 3, 100));
            MicropolisTool.Add("INDUSTRIAL", new MicropolisTool("INDUSTRIAL", 3, 100));
            MicropolisTool.Add("FIRE", new MicropolisTool("FIRE", 3, 500));
            MicropolisTool.Add("POLICE", new MicropolisTool("POLICE", 3, 500));
            MicropolisTool.Add("STADIUM", new MicropolisTool("STADIUM", 4, 5000));
            MicropolisTool.Add("PARK", new MicropolisTool("PARK", 1, 10));
            MicropolisTool.Add("SEAPORT", new MicropolisTool("SEAPORT", 4, 3000));
            MicropolisTool.Add("POWERPLANT", new MicropolisTool("POWERPLANT", 4, 3000));
            MicropolisTool.Add("NUCLEAR", new MicropolisTool("NUCLEAR", 4, 5000));
            MicropolisTool.Add("AIRPORT", new MicropolisTool("AIRPORT", 6, 10000));
            MicropolisTool.Add("QUERY", new MicropolisTool("QUERY", 1, 0));
        }
    }
}