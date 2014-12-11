using Micropolis.ViewModels;

namespace Micropolis
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
    /// EarthquakeStepper shakes the map during an earthquake.
    /// </summary>
    public class EarthquakeStepper
    {
        /// <summary>
        /// reference to the MainGameGame
        /// </summary>
        private readonly MicropolisDrawingAreaViewModel _drawingAreaViewModel;

        /// <summary>
        /// Count of earthquake steps
        /// </summary>
        public int Count = 0;

        /// <summary>
        /// Initiates a new instance of the EarthquakeStepper class.
        /// </summary>
        /// <param name="drawingAreaViewModel"></param>
        public EarthquakeStepper(MicropolisDrawingAreaViewModel drawingAreaViewModel)
        {
            _drawingAreaViewModel = drawingAreaViewModel;
        }

        /// <summary>
        /// Performs one earthquake shake step.
        /// </summary>
        public void OneStep()
        {
            Count = (Count + 1)%MicropolisDrawingAreaViewModel.SHAKE_STEPS;
            _drawingAreaViewModel.Shake(Count);
        }
    }
}