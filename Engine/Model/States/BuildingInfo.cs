using System.Linq;

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
    ///     Describes a building
    /// </summary>
    public class BuildingInfo
    {
        /// <summary>
        ///     The height of the building
        /// </summary>
        public int Height;

        /// <summary>
        ///     The members of the building
        /// </summary>
        public short[] Members;

        /// <summary>
        ///     The width of the building
        /// </summary>
        public int Width;

        public override string ToString()
        {
            return "width: "+Width+", height: " + Height + ", members.C: " + Members.Count();
        }
    }
}