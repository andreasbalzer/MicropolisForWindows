using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Micropolis.NonGamePages
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
    ///     Author of Books.
    /// </summary>
    public interface IAuthor
    {
        /// <summary>
        ///     First name of author
        /// </summary>
        string Firstname { get; }

        /// <summary>
        ///     Last name of author.
        /// </summary>
        string Lastname { get; }

        /// <summary>
        ///     Middle name of author
        /// </summary>
        string Middlename { get; }
    }
}
