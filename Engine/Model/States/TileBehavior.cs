using System;

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


    public abstract class TileBehavior
    {
        /// <summary>
        ///     The random number generator
        /// </summary>
        protected Random PRNG;

        /// <summary>
        ///     The city associated with this tile
        /// </summary>
        protected Micropolis City;

        /// <summary>
        ///     The raw tile content
        /// </summary>
        protected int RawTile;

        /// <summary>
        ///     The tile content
        /// </summary>
        protected int Tile;

        /// <summary>
        ///     The x position
        /// </summary>
        protected int Xpos;

        /// <summary>
        ///     The y position
        /// </summary>
        protected int Ypos;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TileBehavior" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        protected TileBehavior(Micropolis city)
        {
            City = city;
            PRNG = city.Prng;
        }

        /// <summary>
        ///     Processes the tile.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public void ProcessTile(int xpos, int ypos)
        {
            Xpos = xpos;
            Ypos = ypos;
            RawTile = City.GetTileRaw(xpos, ypos);
            Tile = RawTile & TileConstants.LOMASK;
            Apply();
        }


        /// <summary>
        ///     Activate the tile identified by xpos and ypos properties.
        /// </summary>
        public abstract void Apply();
    }
}