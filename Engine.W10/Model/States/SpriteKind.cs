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

    /// <summary>
    ///     SpriteKind
    /// </summary>
    public class SpriteKind
    {
        /// <summary>
        ///     The number of animation frames of this sprite kind
        /// </summary>
        public int NumFrames;

        /// <summary>
        ///     The object identifier of this sprite kind
        /// </summary>
        public int ObjectId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteKind" /> class.
        /// </summary>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="numFrames">The number frames.</param>
        public SpriteKind(int objectId, int numFrames)
        {
            ObjectId = objectId;
            NumFrames = numFrames;
        }

        public override string ToString()
        {
            return "OId: " + ObjectId + ", NumFrames: " + NumFrames;
        }
    }
}