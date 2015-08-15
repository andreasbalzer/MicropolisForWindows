using System;

namespace Engine
{
/*
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
*/

    /// <summary>
    ///     Methods to fill arrays
    /// </summary>
    public static class Arrays
    {
        // 

        /// <summary>
        ///     Fills the specified array between bounds with a specific value.
        ///     Note: start is inclusive, end is exclusive (as is conventional in computer science)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array to be filled.</param>
        /// <param name="start">The start index in the array (inclusive index position).</param>
        /// <param name="end">The end index in the array (exclusive index position).</param>
        /// <param name="value">The value to be inserted into the array.</param>
        /// <exception cref="System.ArgumentNullException">Array is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     fromIndex or toIndex have wrong bounds
        /// </exception>
        /// <remarks>
        ///     source: http://stackoverflow.com/questions/6828154/c-sharp-equivalent-to-javas-arrays-fill-method
        /// </remarks>
        /// <author>
        ///     external
        /// </author>
        public static void Fill<T>(T[] array, int start, int end, T value)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (start < 0 || start >= end)
            {
                throw new ArgumentOutOfRangeException("array");
            }
            if (end > array.Length)
            {
                throw new ArgumentOutOfRangeException("array");
            }
            for (int i = start; i < end; i++)
            {
                array[i] = value;
            }
        }

        /// <summary>
        ///     Fills the specified array with the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array to be filled.</param>
        /// <param name="value">The value to be inserted into the array.</param>
        public static void Fill<T>(T[] array, T value)
        {
            Fill(array, 0, array.Length, value);
        }
    }
}