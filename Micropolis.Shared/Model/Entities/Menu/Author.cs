using System;

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
    public class Author : IAuthor
    {
        /// <summary>
        ///     First name of author
        /// </summary>
        private readonly string _firstname;

        /// <summary>
        ///     Last name of author.
        /// </summary>
        private readonly string _lastname;

        /// <summary>
        ///     Middle name of author
        /// </summary>
        private readonly string _middlename;

        /// <summary>
        ///     Initiates a new instance of Author class.
        /// </summary>
        /// <param name="firstname">Firstname</param>
        /// <param name="middlename">Middle name</param>
        /// <param name="lastname">Lastname</param>
        public Author(string firstname, string middlename, string lastname)
        {
            _firstname = firstname;
            _middlename = middlename;
            _lastname = lastname;
        }

        /// <summary>
        ///     Initiates a new instance of Author class.
        /// </summary>
        /// <param name="firstname">Firstname</param>
        /// <param name="lastname">Lastname</param>
        public Author(string firstname, string lastname)
        {
            _firstname = firstname;
            _middlename = String.Empty;
            _lastname = lastname;
        }

        /// <summary>
        ///     Initiates a new instance of Author class.
        /// </summary>
        /// <param name="lastname">Lastname</param>
        public Author(string lastname)
        {
            _firstname = String.Empty;
            _middlename = String.Empty;
            _lastname = lastname;
        }

        /// <summary>
        ///     Firstname of author.
        /// </summary>
        public string Firstname
        {
            get { return _firstname; }
        }

        /// <summary>
        ///     Lastname of author.
        /// </summary>
        public string Lastname
        {
            get { return _lastname; }
        }

        /// <summary>
        ///     Middle name of author.
        /// </summary>
        public string Middlename
        {
            get { return _middlename; }
        }

        /// <summary>
        ///     Converts this author instance into a string representation for on screen outputs.
        /// </summary>
        /// <returns>string for onscreen output</returns>
        public override string ToString()
        {
            if (_middlename != String.Empty)
            {
                return _firstname + " " + _middlename + " " + _lastname;
            }
            if (_firstname != String.Empty)
            {
                return _firstname + " " + _lastname;
            }
            return _lastname;
        }
    }
}