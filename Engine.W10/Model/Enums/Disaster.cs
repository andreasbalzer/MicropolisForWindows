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
    ///     Lists the disasters that the user can invoke.
    /// </summary>
    public enum Disaster
    {
        /// <summary>
        ///     A monster will go over land and destroy everything along its path.
        /// </summary>
        MONSTER,

        /// <summary>
        ///     A fire will destroy everything surrounding it. It spreads in all directions.
        /// </summary>
        FIRE,

        /// <summary>
        ///     A river or pond will flood some fields around it.
        /// </summary>
        FLOOD,

        /// <summary>
        ///     A meltdown will cause a nuclear power plant to no longer provide electricity.
        /// </summary>
        MELTDOWN,

        /// <summary>
        ///     A tornado will go over land and water and will destroy everything along its path.
        /// </summary>
        TORNADO,

        /// <summary>
        ///     An earthquake will destroy several buildings on the whole map.
        /// </summary>
        EARTHQUAKE
    }
}