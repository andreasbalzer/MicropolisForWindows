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
    /// Converts text into its upper case.
    /// </summary>
    public class UppercaseTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts string into its upper case.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <param name="targetType">Target type. Unused.</param>
        /// <param name="parameter">Parameter. Unused.</param>
        /// <param name="language">Language. Unused.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";

            var text = value.ToString();
            return text.ToUpper();
        }

        /// <summary>
        /// Converts uppercase string into its original string.
        /// </summary>
        /// <param name="value">Value. Unused.</param>
        /// <param name="targetType">Target type. Unused.</param>
        /// <param name="parameter">Parameter. Unused.</param>
        /// <param name="language">Language. Unused.</param>
        /// <returns>Original spelling of string.</returns>
        /// <remarks>Not implemented.</remarks>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
