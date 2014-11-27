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
    ///     A sound that can be played in the game
    /// </summary>
    public class Sound
    {
        private readonly String _wavName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Sound" /> class.
        /// </summary>
        /// <param name="wavName">Name of the wav.</param>
        public Sound(String wavName)
        {
            _wavName = wavName;
        }

        /// <summary>
        ///     Gets the uri of this audio file.
        /// </summary>
        /// <returns></returns>
        public Uri GetAudioFile()
        {
            if (_wavName == null)
            {
                return null;
            }

            String n2 = "ms-appx:///resources/sounds/" + _wavName + ".wav";
            var u = new Uri(n2, UriKind.RelativeOrAbsolute);
            return u;
        }

        public override string ToString()
        {
            return "wavName: "+_wavName;
        }
    }
}