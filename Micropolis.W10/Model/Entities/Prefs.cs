using Windows.Storage;

namespace Micropolis
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
    ///     User preferences to load from and store in local data
    /// </summary>
    public class Prefs
    {
        /// <summary>
        ///     Gets the boolean value of a preference, defaultValue if the preference does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        internal static bool GetBoolean(string key, bool defaultValue)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, defaultValue);
            }
            return (bool) ApplicationData.Current.LocalSettings.Values[key];
        }

        /// <summary>
        ///     Gets the boolean value of a preference, defaultValue if the preference does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        internal static string GetString(string key, string defaultValue)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, defaultValue);
            }
            return (string) ApplicationData.Current.LocalSettings.Values[key];
        }

        /// <summary>
        ///     Puts the boolean preference into the system.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal static void PutBoolean(string key, bool value)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

        /// <summary>
        ///     Puts the boolean preference into the system.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal static void PutString(string key, string value)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

        /// <summary>
        ///     Determines whether preferences contain a preference identified by key provided.
        /// </summary>
        /// <param name="key">Identifier of preference.</param>
        /// <returns>Preference identified by key stored (true), otherwise false.</returns>
        internal static bool ContainsKey(string key)
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(key);
        }
    }
}