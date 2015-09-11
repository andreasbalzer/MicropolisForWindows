using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System.UserProfile;

namespace Micropolis
{
    using System.Diagnostics;
    using System.Threading;

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
    ///     Manages language strings. Class needs to be initialized prior to first use by calling CreateCityFolderAndThumbnails().
    /// </summary>
    public static class Strings
    {
        /// <summary>
        ///     Stores all strings of one single language. Unlike Java version we do not split strings into categories
        /// </summary>
        private static readonly Dictionary<string, string> _strings = new Dictionary<string, string>();

        /// <summary>
        ///     Initializes this class.
        /// </summary>
        public static async Task Initialize(CancellationToken cancelToken)
        {
            Micropolis.Utils.ThreadCancellation.CheckCancellation(cancelToken);
            await LoadFiles(cancelToken);
        }


        /// <summary>
        ///     Parses the file containing language strings.
        ///     Syntax of the file is
        ///     bla bla
        ///     name = value
        ///     name = value
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private static async Task ParseFile(StorageFile file)
        {
            IList<string> contents = await FileIO.ReadLinesAsync(file);
            foreach (string line in contents)
            {
                if (line.Contains("="))
                {
                    int seperator = line.IndexOf("=", StringComparison.Ordinal);
                    string name = line.Substring(0, seperator);
                    string value = line.Substring(seperator + 1);

                    name = name.Trim();
                    value = value.Trim();
                    if (!_strings.ContainsKey(name))
                    {
                        _strings.Add(name, value);
                    }
                    if (_strings[name] == "" || _strings[name] == null)
                    {
                        _strings[name] = value;
                    }
                }
            }
        }

        /// <summary>
        ///     Loads the strings of the prefered language.
        /// </summary>
        /// <returns></returns>
        private static async Task LoadFiles(CancellationToken cancelToken)
        {
            string languageModifier = String.Empty;
            
            StorageFolder installFolder = Package.Current.InstalledLocation;
            StorageFolder folder = await installFolder.GetFolderAsync("strings");

            try
            {
                if (!Prefs.ContainsKey("Language")
                    || Prefs.GetString("Language", "automatic") == "automatic")
                {
                    // language should be determined automatically
                    IReadOnlyList<string> languages = GlobalizationPreferences.Languages;
                    Micropolis.Utils.ThreadCancellation.CheckCancellation(cancelToken);
                    foreach (string language in languages)
                    {
                        // get language identifier

                        string possibleLanguageModifier = "";
                        int dashSeperatorPosition = language.IndexOf('-');
                        if (dashSeperatorPosition > 0)
                        {
                            possibleLanguageModifier = language.Substring(0, dashSeperatorPosition);
                        }
                        else
                        {
                            possibleLanguageModifier = language;
                        }

                        // try to see whether language files for language are available
                        IStorageFile file = await folder.TryGetItemAsync("CityMessages_"+possibleLanguageModifier+".properties") as IStorageFile;

                        if (file != null) // the language is available in language files
                        {
                            languageModifier = "_" + possibleLanguageModifier;
                            break;
                        }
                    }
                    if (languageModifier == String.Empty)
                    {
                        languageModifier = "";
                    }
                }
                else if (Prefs.ContainsKey("Language"))
                {
                    languageModifier = (string) Prefs.GetString("Language","en");
                }
                if (languageModifier != "en")
                    // a language different from english should be loaded (english is loaded below as default and fall back)
                {
                    Micropolis.Utils.ThreadCancellation.CheckCancellation(cancelToken);
                    await LoadAndParseFile("strings", "CityMessages_" + languageModifier + ".properties");
                    await LoadAndParseFile("strings", "CityStrings_" + languageModifier + ".properties");
                    await LoadAndParseFile("strings", "GuiStrings_" + languageModifier + ".properties");
                    await LoadAndParseFile("strings", "StatusMessages_" + languageModifier + ".properties");
                }
            }
            catch
            {
            }
            Micropolis.Utils.ThreadCancellation.CheckCancellation(cancelToken);
            // load english items in case something is missing in language packs. also loads generic filenames
            await LoadAndParseFile("strings", "CityMessages.properties");
            await LoadAndParseFile("strings", "CityStrings.properties");
            await LoadAndParseFile("strings", "GuiStrings.properties");
            await LoadAndParseFile("strings", "StatusMessages.properties");
        }

        private static async Task LoadAndParseFile(string folder, string file)
        {
            StorageFile fileObj = await Engine.Libs.LoadFiles.GetPackagedFile(folder, file);
            await ParseFile(fileObj);
        }

        /// <summary>
        ///     Gets the string identified via name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The translated string identfied by name</returns>
        public static string GetString(string name)
        {
            if (!_strings.ContainsKey(name))
            {
                Debug.WriteLine(name+" was not found in strings");
                throw new KeyNotFoundException("The string "+name+" could not been found in language files.");
            }
            return _strings[name];
        }

        /// <summary>
        ///     Determines whether the strings contain the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The translated strings contain one identified by key (true), otherwise false.</returns>
        internal static bool ContainsKey(string key)
        {
            return _strings.ContainsKey(key);
        }
    }
}