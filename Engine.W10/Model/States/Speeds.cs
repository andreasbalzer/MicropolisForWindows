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
    ///     Lists the simulation speeds available.
    ///     Contains properties identifying how often the animation timer fires,
    ///     and how many animation steps are fired at each interval.
    ///     Note: for every 2 animation steps, one simulation step is triggered.
    /// </summary>
    public static class Speeds
    {
        /// <summary>
        ///     The speeds
        /// </summary>
        public static Dictionary<string, Speed> Speed;

        /// <summary>
        ///     Initializes the <see cref="Speeds" /> class.
        /// </summary>
        static Speeds()
        {
            Speed = new Dictionary<string, Speed>();
            Speed.Add("PAUSED", new Speed(999, 0));
            Speed.Add("SLOW", new Speed(625, 1)); //one sim step every 1250 ms
            Speed.Add("NORMAL", new Speed(125, 1)); //one sim step every 250 ms
            Speed.Add("FAST", new Speed(25, 1)); //one sim step every 50 ms
            Speed.Add("SUPER_FAST", new Speed(25, 5)); //one sim step every 10 ms
        }
    }
}