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
    ///     The listener interface for receiving notifications whenever a tile on the city map changes, or when a sprite moves
    ///     or changes.
    /// </summary>
    public interface IMapListener
    {
        /// <summary>
        ///     Called whenever data for a specific overlay has changed.
        /// </summary>
        /// <param name="overlayDataType">Type of the overlay data.</param>
        void MapOverlayDataChanged(MapState overlayDataType);

        /// <summary>
        ///     Called when a sprite moves.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        void SpriteMoved(Sprite sprite);

        /// <summary>
        ///     Called when a map tile changes, including for animations.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        void TileChanged(int xpos, int ypos);

        /// <summary>
        ///     Called when the entire map should be reread and rendered.
        /// </summary>
        void WholeMapChanged();
    }
}