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
    /// App commands to execute.
    /// </summary>
    public enum AppCommands
    {
        /// <summary>
        /// Load a game file.
        /// </summary>
        LOADFILE,

        /// <summary>
        /// Loads a game file but forgets about its origin so save does not overwrite that file.
        /// </summary>
        LOADFILEASNEWCITY,
        
        /// <summary>
        /// Loads a game file but then deletes it. Used to load Autosave.
        /// </summary>
        LOADFILEASNEWCITYANDDELETE,

        /// <summary>
        /// Skip menu page and directly go to MainGamePage after game has been loaded.
        /// </summary>
        SKIPMENU,

        /// <summary>
        /// Update game.
        /// </summary>
        UPDATEDVERSION,

        /// <summary>
        /// Loads a scenario file but forgets about its origin so save does not overwrite that file
        /// </summary>
        LOADSCENARIOASNEWCITY
    }
}
