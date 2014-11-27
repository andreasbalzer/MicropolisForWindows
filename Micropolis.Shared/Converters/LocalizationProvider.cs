using System;
using Windows.UI.Xaml.Data;

namespace Micropolis.Converters
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
    /// Loads translated string identified by value.
    /// </summary>
    public class LocalizationProvider : IValueConverter
    {
        /// <summary>
        /// Converts an identifier into the translated string identified by it.
        /// </summary>
        /// <param name="value">The identifier</param>
        /// <param name="targetType">The target type. Unused.</param>
        /// <param name="parameter">The parameter. Unused.</param>
        /// <param name="language">The language. Unused.</param>
        /// <returns>translated string</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return new ArgumentNullException("value");

            return Strings.GetString(value.ToString());
        }

        /// <summary>
        /// Converts a translated string into its identifier. Not Implemented.
        /// </summary>
        /// <param name="value">The string for which to get the identifier. Unused.</param>
        /// <param name="targetType">The target type. Unused.</param>
        /// <param name="parameter">The parameter. Unused.</param>
        /// <param name="language">The language. Unused.</param>
        /// <returns>The identifier.</returns>
        /// <remarks>Not Implemented.</remarks>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
