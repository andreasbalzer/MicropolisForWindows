using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class Book : IBook
    {
        private List<IAuthor> _authors;
        private string _title;
        private string _location;
        private string _publisher;
        private int _year;
        private string _country;
        private string _language;
        private string _amazonID;

        /// <summary>
        /// Authors of the book
        /// </summary>
        public List<IAuthor> Authors
        {
            get { return _authors; }
        }

        /// <summary>
        /// Title of the book
        /// </summary>
        public string Title
        {
            get { return _title; }
        }

        /// <summary>
        /// Location of publishing
        /// </summary>
        public string Location
        {
            get { return _location; }
        }

        /// <summary>
        /// Publisher of the book
        /// </summary>
        public string Publisher
        {
            get { return _publisher; }
        }

        /// <summary>
        /// Year of publishing
        /// </summary>
        public int Year
        {
            get { return _year; }
        }

        /// <summary>
        /// Country in which book was published
        /// </summary>
        public string Country
        {
            get { return _country; }
        }

        /// <summary>
        /// Language of book
        /// </summary>
        public string Language
        {
            get { return _language; }
        }

        /// <summary>
        /// Amazon ID of the book
        /// </summary>
        public string AmazonID
        {
            get { return _amazonID; }
        }

        /// <summary>
        /// Initiates a new instance of the book class.
        /// </summary>
        /// <param name="authors">Authors of the book.</param>
        /// <param name="title">Title of the book.</param>
        /// <param name="location">Location of the book publishing.</param>
        /// <param name="publisher">Publisher of the book.</param>
        /// <param name="country">Country in which the book was published.</param>
        /// <param name="language">Language in which the book was published.</param>
        /// <param name="year">Year in which the book was published.</param>
        /// <param name="amazonID">Amazon ID of the book.</param>
        public Book(List<IAuthor> authors, string title, string location, string publisher, string country, string language, int year, string amazonID)
        {
            this._authors = authors;
            this._title = title;
            this._location = location;
            this._publisher = publisher;
            this._country = country;
            this._language = language;
            this._amazonID = amazonID;
            this._year = year;
        }

        /// <summary>
        /// Converts this book instance into a string representation for on screen output.
        /// </summary>
        /// <returns>string for onscreen output</returns>
        public override string ToString()
        {
            string authorString = "";
            if (_authors.Count == 1) {
                authorString = _authors[0].ToString();
            }
            if (_authors.Count > 1)
            {
                for (int i = 0; i < _authors.Count - 1; i++)
                {
                    authorString += authorString[i].ToString() + " and ";
                }
                authorString += _authors[_authors.Count - 1].ToString();
            }

            return authorString + ". " + _title + ". " + _publisher + ". " + _location + ". " + _year;
        }
    }
}
