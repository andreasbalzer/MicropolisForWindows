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
    ///     A message to be shown to the user.
    /// </summary>
    public class MicropolisMessage
    {
        /// <summary>
        ///     The name of this message
        /// </summary>
        public string Name;

        /// <summary>
        ///     Whether the message should be displayed in the notification pane.
        /// </summary>
        public bool UseNotificationPane = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MicropolisMessage" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MicropolisMessage(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}