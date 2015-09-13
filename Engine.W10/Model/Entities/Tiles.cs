using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Engine.Libs;

namespace Engine
{
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
    ///     Provides global methods for loading tile specifications.
    ///     Specified tiles available in the game
    /// </summary>
    public class Tiles
    {
        //static Charset UTF8 = Charset.forName("UTF-8");
        private static TileSpec[] _tiles;
        private static readonly Dictionary<String, TileSpec> TilesByName = new Dictionary<String, TileSpec>();
        /*public Tiles() {
            try
            {
                InitClass();
            }
            catch (IOException e)
            {
                throw;
            }
        }*/

        /// <summary>
        ///     Initializes this instance on app startup.
        /// </summary>
        /// <returns></returns>
        public static async Task Initialize()
        {
            await ReadTiles();
        }

        /// <summary>
        ///     Reads the tiles from disk and adds them to the internal database.
        /// </summary>
        /// <returns></returns>
        private static async Task ReadTiles()
        {
            var tilesList = new List<TileSpec>();

            StorageFile file = await LoadFiles.GetPackagedFile("Assets/graphics", "tiles.rc");

            IList<string> lines = await FileIO.ReadLinesAsync(file);


            for (int i = 0;; i++)
            {
                String tileName = i.ToString();
                String rawSpec = lines.FirstOrDefault(s => s.StartsWith(tileName + "   "));
                if (String.IsNullOrEmpty(rawSpec))
                {
                    break;
                }

                TileSpec ts = TileSpec.Parse(i, rawSpec, lines);
                TilesByName.Add(tileName, ts);
                tilesList.Add(ts);
            }
            _tiles = tilesList.ToArray();

            for (int i = 0; i < _tiles.Length; i++)
            {
                _tiles[i].ResolveReferences(TilesByName);

                BuildingInfo bi = _tiles[i].GetBuildingInfo();
                if (bi != null)
                {
                    for (int j = 0; j < bi.Members.Length; j++)
                    {
                        int tid = bi.Members[j];
                        int offx = (bi.Width >= 3 ? -1 : 0) + j%bi.Width;
                        int offy = (bi.Height >= 3 ? -1 : 0) + j/bi.Width;

                        if (_tiles[tid].Owner == null &&
                            (offx != 0 || offy != 0)
                            )
                        {
                            _tiles[tid].Owner = _tiles[i];
                            _tiles[tid].OwnerOffsetX = offx;
                            _tiles[tid].OwnerOffsetY = offy;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the specified tile specification by index number.
        /// </summary>
        /// <param name="tileNumber">The tile number.</param>
        /// <returns>a tile specification, or null if there is no tile with the given number</returns>
        public static TileSpec Get(int tileNumber)
        {
            if (_tiles == null)
            {
                return null;
            }
            if (tileNumber >= 0 && tileNumber < _tiles.Length)
            {
                return _tiles[tileNumber];
            }
            return null;
        }
    }
}