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
    ///     Enumerates the various sounds that the city may produce.
    ///     The engine is not responsible for actually playing the sound. That task
    ///     belongs to the front-end (i.e. the user interface).
    /// </summary>
    public static class Sounds
    {
        /// <summary>
        ///     The sounds
        /// </summary>
        public static Dictionary<string, Sound> Sound;

        /// <summary>
        ///     Initializes the <see cref="Sounds" /> class.
        /// </summary>
        static Sounds()
        {
            Sound = new Dictionary<string, Sound>();
            Sound.Add("EXPLOSION_LOW", new Sound("explosion-low"));
            Sound.Add("EXPLOSION_HIGH", new Sound("explosion-high"));
            Sound.Add("EXPLOSION_BOTH", new Sound("explosion-lw"));
            Sound.Add("UHUH", new Sound("bop"));
            Sound.Add("SORRY", new Sound("bop"));
            Sound.Add("BUILD", new Sound("layzone"));
            Sound.Add("BULLDOZE", new Sound(null));
            Sound.Add("HONKHONK_LOW", new Sound("honkhonk-low"));
            Sound.Add("HONKHONK_MED", new Sound("honkhonk-med"));
            Sound.Add("HONKHONK_HIGH", new Sound("hinkhonk-high"));
            Sound.Add("HONKHONK_HI", new Sound("honkhonk-hi"));
            Sound.Add("SIREN", new Sound("siren"));
            Sound.Add("HEAVYTRAFFIC", new Sound("heavytraffic"));
            Sound.Add("MONSTER", new Sound("zombie-roar-5"));
        }
    }
}