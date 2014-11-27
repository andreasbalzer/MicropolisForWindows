using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
    ///     A series of book recommendations for the game
    /// </summary>
    public class Books
    {
        /// <summary>
        ///     Recommendation of books
        /// </summary>
        public ObservableCollection<BookSection> BookRecommendations;

        /// <summary>
        ///     Loads books recommendations.
        /// </summary>
        public Books()
        {
            //Book b1 = new Book(new List<IAuthor> { new Author("first", "last"), new Author("first", "last") }, "title", "location", "publisher", "country", "language", year, "");

            BookRecommendations = new ObservableCollection<BookSection>();

            var cityPlanningBibliographySection = new BookSection();
            cityPlanningBibliographySection.SectionName = "City Planning Bibliography";

            var generalGroup = new BookGroup();
            generalGroup.GroupName = "General";

            var b1 = new Book(new List<IAuthor> {new Author("R.", "Boyer"), new Author("D.", "Savageau")},
                "Places Rated Almanac", "Chicaco", "Rand McNally & Co.", "USA", "English", 1986, "");
            var b2 = new Book(new List<IAuthor> {new Author("Earnest", "Callenbach")}, " Ecotopia", "Berkeley",
                " Banyan Tree Books", "country", "language", 1975, "");
            var b3 = new Book(new List<IAuthor> {new Author("Francoise", "Choay")},
                "The Modern City: Planning in the 19th Century", "New York", "George Braziller", "country", "language",
                1969, "");
            var b4 = new Book(new List<IAuthor> {new Author("David", "Clark")}, "Geography", "Baltimore",
                "The Johns Hopkins University Press", "country", "language", 1982, "");
            var b5 = new Book(new List<IAuthor> {new Author("Grady", "Clay")},
                " Close-Up, How to Read the American City", "Chicago", "The University of Chicago Press", "country",
                "language", 1980, "");
            var b6 = new Book(new List<IAuthor> {new Author("A.", "Gallion"), new Author("S.", "Eisner")},
                "The Urban Pattern", "New York", "Van Nostrand Reinhold Company", "country", "language", 1986, "");
            var b7 =
                new Book(
                    new List<IAuthor>
                    {
                        new Author("M.", "Greenburg"),
                        new Author("D.", "Krueckeberg"),
                        new Author("C.", "Michaelson")
                    }, " Local population and Employment Projection Techniques",
                    " New Brunswick", "Center for Urban Policy Research", "country", "language", 1987, "");
            var b8 = new Book(new List<IAuthor> {new Author("Frank", "P.", "Hoskin")}, "The Language of Cities",
                "Cambridge", "Schenkman Publishing Company", "country", "language", 1972, "");
            var b9 = new Book(new List<IAuthor> {new Author("Jane", "Jacobs"), new Author("first", "last")},
                " The Death and Life of Great American Cities", "New York", "John Wiley & Sons", "country", "language",
                1974, "");
            var b10 = new Book(new List<IAuthor> {new Author("Le", "Corbusier")},
                " The City of Tomorrow and Its Planning", "New York", " Dover Publications, Inc", "country", "language",
                1987, "");
            var b11 = new Book(new List<IAuthor> {new Author("Kevin", "Lynch")}, "A Theory of Good City Form",
                "Cambridge", "MIT Press", "country", "language", 1981, "");
            var b12 = new Book(new List<IAuthor> {new Author("Richard", "Register")}, "Ecocity Berkeley", "Berkeley",
                "publisher", "country", "language", 1987, "");
            var b13 = new Book(new List<IAuthor>(), "Planning: The magazine of the American Planning Association",
                "1313 E. 60th St. Chicago, IL 60637", "American Planning Association", "country", "language", 0, "");

            generalGroup.Books.Add(b1);
            generalGroup.Books.Add(b2);
            generalGroup.Books.Add(b3);
            generalGroup.Books.Add(b4);
            generalGroup.Books.Add(b5);
            generalGroup.Books.Add(b6);
            generalGroup.Books.Add(b7);
            generalGroup.Books.Add(b8);
            generalGroup.Books.Add(b9);
            generalGroup.Books.Add(b10);
            generalGroup.Books.Add(b11);
            generalGroup.Books.Add(b12);
            generalGroup.Books.Add(b13);

            cityPlanningBibliographySection.BookGroups.Add(generalGroup);

            var childrenSection = new BookSection();
            childrenSection.SectionName = "Related Reading for Children";

            var fiction = new BookGroup();
            fiction.GroupName = "Fiction";

            childrenSection.BookGroups.Add(fiction);

            var b14 = new Book(new List<IAuthor> {new Author("Virginia", "Lee", "Burton")}, "The Little House", "Boston",
                "Houghton Mifflin", "country", "language", 1969, "");
            var b15 = new Book(new List<IAuthor> {new Author("Shirley", "Murphy"), new Author("Pat", "Murphy")},
                " Tortino's Return to the Sun", String.Empty, "Shepard Books", "country", "language", 1980, "");
            var b16 = new Book(new List<IAuthor> {new Author("Dr. Seuss")}, "The Lorax", "New York", " Random House",
                "country", "language", 1971, "");

            fiction.Books.Add(b14);
            fiction.Books.Add(b15);
            fiction.Books.Add(b16);

            var nonfiction = new BookGroup();
            nonfiction.GroupName = "Nonfiction";
            childrenSection.BookGroups.Add(nonfiction);

            var b17 = new Book(new List<IAuthor> {new Author("Albert", "Barker")}, "From Settlement to City", "New York",
                "Julian Messner", "country", "language", 1978, "");
            var b18 = new Book(new List<IAuthor> {new Author("James", "A.", "Eichner")},
                "The First Book of Local Government", "New York", "Franklin Watts", "country", "language", 1976, "");
            var b19 = new Book(new List<IAuthor> {new Author("David", "Macaulay")},
                "City: A Story of Roman Planning and Construction", "Boston", "Houghton Mifflin", "country", "language",
                1974, "");
            var b20 = new Book(new List<IAuthor> {new Author("David", "Macaulay")}, " Underground", "Boston",
                "Houghton Mifflin", "country", "language", 1976, "");
            var b21 = new Book(new List<IAuthor> {new Author("Roxie", "Monroe")},
                "Artchitects Make Zigzags: Looking at Architecture from A to Z", "Washington D.C.",
                "National Trust for Historic Preservation", "country", "language", 1986, "");
            var b22 = new Book(new List<IAuthor> {new Author("Dorthy", "Rhodes")}, "How to Read a City Map", "Chicago",
                "Elk Grove Press", "country", "language", 1967, "");

            nonfiction.Books.Add(b17);
            nonfiction.Books.Add(b18);
            nonfiction.Books.Add(b19);
            nonfiction.Books.Add(b20);
            nonfiction.Books.Add(b21);
            nonfiction.Books.Add(b22);

            BookRecommendations.Add(cityPlanningBibliographySection);
            BookRecommendations.Add(childrenSection);
        }
    }
}