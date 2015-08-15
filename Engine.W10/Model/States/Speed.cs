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
    ///     A speed available to the user
    /// </summary>
    public class Speed
    {
        /// <summary>
        ///     The animation speed, expressed as an interval in milliseconds.
        /// </summary>
        public int AnimationDelay;

        /// <summary>
        ///     For faster speeds, how many simulation steps should occur for every update to the screen.
        /// </summary>
        public int SimStepsPerUpdate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Speed" /> class.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="simSteps">The sim steps.</param>
        public Speed(int delay, int simSteps)
        {
            AnimationDelay = delay;
            SimStepsPerUpdate = simSteps;
        }

        public override string ToString()
        {
            return "AniDelay: " + AnimationDelay + ", simStepsPerUpdate: " + SimStepsPerUpdate;
        }
    }
}