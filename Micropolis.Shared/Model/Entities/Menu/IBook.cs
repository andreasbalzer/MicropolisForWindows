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
    /// Represents a book written by authors.
    /// </summary>
    public interface IBook
    {
        /// <summary>
        /// Authors of the book
        /// </summary>
        List<IAuthor> Authors { get; }

        /// <summary>
        /// Title of the book
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Location of publishing
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Publisher of the book
        /// </summary>
        string Publisher { get; }

        /// <summary>
        /// Year of publishing
        /// </summary>
        int Year { get; }

        /// <summary>
        /// Country in which book was published
        /// </summary>
        string Country { get; }

        /// <summary>
        /// Language of book
        /// </summary>
        string Language { get; }

        /// <summary>
        /// Amazon ID of the book
        /// </summary>
        string AmazonID { get; }
    }
}
