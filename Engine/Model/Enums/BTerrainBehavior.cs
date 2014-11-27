namespace Engine
{
/*
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
*/

    /// <summary>
    ///     The terrain behaviors
    /// </summary>
    public enum BTerrainBehavior
    {
        /// <summary>
        ///     There is a spreading Fire on this tile and nothing can be built here. Fire brigade needed.
        /// </summary>
        FIRE,

        /// <summary>
        ///     There is a Flood on the tile and at the moment nothing can be built here.
        /// </summary>
        FLOOD,

        /// <summary>
        ///     The tile is radioactive and nothing can be built here.
        /// </summary>
        RADIOACTIVE,

        /// <summary>
        ///     There is a road on this tile
        /// </summary>
        ROAD,

        /// <summary>
        ///     There is a rail on this tile
        /// </summary>
        RAIL,

        /// <summary>
        ///     There is an explosion on this tile
        /// </summary>
        EXPLOSION
    }
}