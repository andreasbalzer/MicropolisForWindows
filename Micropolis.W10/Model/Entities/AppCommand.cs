using System;
using Windows.Storage;

namespace Micropolis.Model.Entities
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
    /// App command to instruct the app to take specific action at various points. e.g. skip menu page, update game, load file, etc.
    /// </summary>
    public class AppCommand
    {
        /// <summary>
        /// Initiates a new instance of the AppCommand class.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        public AppCommand(AppCommands instruction)
        {
            Instruction = instruction;
        }

        /// <summary>
        /// Initiates a new instance of the AppCommand class.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <param name="value">The value of the command.</param>
        public AppCommand(AppCommands instruction, string value)
        {
            Instruction = instruction;
            Value = value;
            Difficulty = -1;
            File = null;
        }

        public AppCommand(AppCommands instruction, Tuple<Engine.Micropolis, IStorageFile, int> gameSpecification)
        {
            Instruction = instruction;
            Engine = gameSpecification.Item1;
            File = gameSpecification.Item2;
            Difficulty = gameSpecification.Item3;
        }

        /// <summary>
        /// Initiates a new instance of the AppCommand class.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <param name="file">The file to execute on.</param>
        public AppCommand(AppCommands instruction, IStorageItem file)
        {
            Instruction = instruction;
            File = file;
            Difficulty = -1;
            Engine = null;
        }

        /// <summary>
        /// The instruction to execute
        /// </summary>
        public AppCommands Instruction { get; set; }

        /// <summary>
        /// The value to execute with.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The file to execute on.
        /// </summary>
        public IStorageItem File { get; set; }

        /// <summary>
        /// Difficulty of game to start
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// Engine to start
        /// </summary>
        public Engine.Micropolis Engine { get; set; }
    }
}