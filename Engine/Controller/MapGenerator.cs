using System;

namespace Engine
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
    ///     Contains the code for generating a random map terrain.
    /// </summary>
    public class MapGenerator
    {
        private static readonly int[][] BR_MATRIX =
        {
            new[] {0, 0, 0, 3, 3, 3, 0, 0, 0},
            new[] {0, 0, 3, 2, 2, 2, 3, 0, 0},
            new[] {0, 3, 2, 2, 2, 2, 2, 3, 0},
            new[] {3, 2, 2, 2, 2, 2, 2, 2, 3},
            new[] {3, 2, 2, 2, 4, 2, 2, 2, 3},
            new[] {3, 2, 2, 2, 2, 2, 2, 2, 3},
            new[] {0, 3, 2, 2, 2, 2, 2, 3, 0},
            new[] {0, 0, 3, 2, 2, 2, 3, 0, 0},
            new[] {0, 0, 0, 3, 3, 3, 0, 0, 0}
        };

        private static readonly int[][] SR_MATRIX =
        {
            new[] {0, 0, 3, 3, 0, 0},
            new[] {0, 3, 2, 2, 3, 0},
            new[] {3, 2, 2, 2, 2, 3},
            new[] {3, 2, 2, 2, 2, 3},
            new[] {0, 3, 2, 2, 3, 0},
            new[] {0, 0, 3, 3, 0, 0}
        };

        private static readonly int[] RED_TAB =
        {
            TileConstants.RIVEDGE + 8, TileConstants.RIVEDGE + 8, TileConstants.RIVEDGE + 12, TileConstants.RIVEDGE + 10,
            TileConstants.RIVEDGE + 0, TileConstants.RIVER, TileConstants.RIVEDGE + 14, TileConstants.RIVEDGE + 12,
            TileConstants.RIVEDGE + 4, TileConstants.RIVEDGE + 6, TileConstants.RIVER, TileConstants.RIVEDGE + 8,
            TileConstants.RIVEDGE + 2, TileConstants.RIVEDGE + 4, TileConstants.RIVEDGE + 0, TileConstants.RIVER
        };

        private static readonly int[] DIRECTION_TAB_X = {0, 1, 1, 1, 0, -1, -1, -1};
        private static readonly int[] DIRECTION_TAB_Y = {-1, -1, 0, 1, 1, 1, 0, -1};
        private static readonly int[] DX = {-1, 0, 1, 0};
        private static readonly int[] DY = {0, 1, 0, -1};

        private static readonly int[] TED_TAB = //bug: was int
        {
            0, 0, 0, 34,
            0, 0, 36, 35,
            0, 32, 0, 33,
            30, 31, 29, 37
        };

        private readonly Micropolis _engine;
        private readonly char[][] _map;
        private int _dir;
        private int _lastDir;
        private int _mapX;
        private int _mapY;
        private Random _prng;
        private int _xStart;
        private int _yStart;
        private CreateIsland createIsland = CreateIsland.SELDOM;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MapGenerator" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public MapGenerator(Micropolis engine)
        {
            //assert engine != null;
            _engine = engine;
            _map = engine.Map;
        }

        /// <summary>
        ///     Gets the width of the map.
        /// </summary>
        /// <returns></returns>
        private int GetWidth()
        {
            return _map[0].Length;
        }

        /// <summary>
        ///     Gets the height of the map.
        /// </summary>
        /// <returns></returns>
        private int GetHeight()
        {
            return _map.Length;
        }

        /// <summary>
        ///     Generate a random map terrain.
        /// </summary>
        public void GenerateNewCity()
        {
            int r = Micropolis.DEFAULT_PRNG.Next();
            GenerateSomeCity(r);
        }

        /// <summary>
        ///     Generates some city specified by random number generator seed r.
        /// </summary>
        /// <param name="r">The random number generator seed r.</param>
        public void GenerateSomeCity(int r)
        {
            GenerateMap(r);
            _engine.FireWholeMapChanged();
        }

        /// <summary>
        ///     Generates the map via random number generator seed.
        /// </summary>
        /// <param name="r">The seed for random number generator.</param>
        private void GenerateMap(int r)
        {
            _prng = new Random(r);

            if (createIsland == CreateIsland.SELDOM)
            {
                if (_prng.Next(100) < 10) //chance that island is generated
                {
                    MakeIsland();
                    return;
                }
            }

            if (createIsland == CreateIsland.ALWAYS)
            {
                MakeNakedIsland();
            }
            else
            {
                ClearMap();
            }

            GetRandStart();

            if (_curveLevel != 0)
            {
                DoRivers();
            }

            if (_lakeLevel != 0)
            {
                MakeLakes();
            }

            SmoothRiver();

            if (_treeLevel != 0)
            {
                DoTrees();
            }
        }

        /// <summary>
        ///     Makes an island.
        /// </summary>
        private void MakeIsland()
        {
            MakeNakedIsland();
            SmoothRiver();
            DoTrees();
        }

        /// <summary>
        ///     Erands the specified limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        private int Erand(int limit)
        {
            return Math.Min(
                _prng.Next(limit),
                _prng.Next(limit)
                );
        }

        /// <summary>
        ///     Makes a naked island.
        /// </summary>
        private void MakeNakedIsland()
        {
            int islandRadius = 18;
            int worldX = GetWidth();
            int worldY = GetHeight();

            for (int y = 0; y < worldY; y++)
            {
                for (int x = 0; x < worldX; x++)
                {
                    _map[y][x] = (char) TileConstants.RIVER;
                }
            }

            for (int y = 5; y < worldY - 5; y++)
            {
                for (int x = 5; x < worldX - 5; x++)
                {
                    _map[y][x] = (char) TileConstants.DIRT;
                }
            }

            for (int x = 0; x < worldX - 5; x += 2)
            {
                _mapX = x;
                _mapY = Erand(islandRadius + 1);
                BRivPlop();
                _mapY = (worldY - 10) - Erand(islandRadius + 1);
                BRivPlop();
                _mapY = 0;
                SRivPlop();
                _mapY = worldY - 6;
                SRivPlop();
            }

            for (int y = 0; y < worldY - 5; y += 2)
            {
                _mapY = y;
                _mapX = Erand(islandRadius + 1);
                BRivPlop();
                _mapX = (worldX - 10) - Erand(islandRadius + 1);
                BRivPlop();
                _mapX = 0;
                SRivPlop();
                _mapX = (worldX - 6);
                SRivPlop();
            }
        }

        /// <summary>
        ///     Clears the map by putting dirt on every field.
        /// </summary>
        private void ClearMap()
        {
            for (int y = 0; y < _map.Length; y++)
            {
                for (int x = 0; x < _map[y].Length; x++)
                {
                    _map[y][x] = (char) TileConstants.DIRT;
                }
            }
        }

        private void GetRandStart()
        {
            _xStart = 40 + _prng.Next(GetWidth() - 79);
            _yStart = 33 + _prng.Next(GetHeight() - 66);

            _mapX = _xStart;
            _mapY = _yStart;
        }

        /// <summary>
        ///     Creates the lakes.
        /// </summary>
        private void MakeLakes()
        {
            int lim1;
            if (_lakeLevel < 0)
                lim1 = _prng.Next(11);
            else
                lim1 = _lakeLevel/2;

            for (int t = 0; t < lim1; t++)
            {
                int x = _prng.Next(GetWidth() - 20) + 10;
                int y = _prng.Next(GetHeight() - 19) + 10;
                int lim2 = _prng.Next(13) + 2;

                for (int z = 0; z < lim2; z++)
                {
                    _mapX = x - 6 + _prng.Next(13);
                    _mapY = y - 6 + _prng.Next(13);

                    if (_prng.Next(5) != 0)
                        SRivPlop();
                    else
                        BRivPlop();
                }
            }
        }

        /// <summary>
        ///     Creates the rivers.
        /// </summary>
        private void DoRivers()
        {
            _dir = _lastDir = _prng.Next(4);
            DoBRiv();

            _mapX = _xStart;
            _mapY = _yStart;
            _dir = _lastDir = _lastDir ^ 4;
            DoBRiv();

            _mapX = _xStart;
            _mapY = _yStart;
            _lastDir = _prng.Next(4);
            DoSRiv();
        }

        private void DoBRiv()
        {
            int r1, r2;
            if (_curveLevel < 0)
            {
                r1 = 100;
                r2 = 200;
            }
            else
            {
                r1 = _curveLevel + 10;
                r2 = _curveLevel + 100;
            }

            while (_engine.TestBounds(_mapX + 4, _mapY + 4))
            {
                BRivPlop();
                if (_prng.Next(r1 + 1) < 10)
                {
                    _dir = _lastDir;
                }
                else
                {
                    if (_prng.Next(r2 + 1) > 90)
                    {
                        _dir++;
                    }
                    if (_prng.Next(r2 + 1) > 90)
                    {
                        _dir--;
                    }
                }
                MoveMap(_dir);
            }
        }

        private void DoSRiv()
        {
            int r1, r2;
            if (_curveLevel < 0)
            {
                r1 = 100;
                r2 = 200;
            }
            else
            {
                r1 = _curveLevel + 10;
                r2 = _curveLevel + 100;
            }

            while (_engine.TestBounds(_mapX + 3, _mapY + 3))
            {
                SRivPlop();
                if (_prng.Next(r1 + 1) < 10)
                {
                    _dir = _lastDir;
                }
                else
                {
                    if (_prng.Next(r2 + 1) > 90)
                    {
                        _dir++;
                    }
                    if (_prng.Next(r2 + 1) > 90)
                    {
                        _dir--;
                    }
                }
                MoveMap(_dir);
            }
        }

        private void BRivPlop()
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    PutOnMap(BR_MATRIX[y][x], x, y);
                }
            }
        }

        private void SRivPlop()
        {
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    PutOnMap(SR_MATRIX[y][x], x, y);
                }
            }
        }

        private void PutOnMap(int mapChar, int xoff, int yoff)
        {
            if (mapChar == 0)
                return;

            int xloc = _mapX + xoff;
            int yloc = _mapY + yoff;

            if (!_engine.TestBounds(xloc, yloc))
                return;

            char tmp = _map[yloc][xloc];
            if (tmp != TileConstants.DIRT)
            {
                tmp &= (char) TileConstants.LOMASK;
                if (tmp == TileConstants.RIVER && mapChar != TileConstants.CHANNEL)
                    return;
                if (tmp == TileConstants.CHANNEL)
                    return;
            }
            _map[yloc][xloc] = (char) mapChar;
        }

        private void SmoothRiver()
        {
            for (int mapY = 0; mapY < _map.Length; mapY++)
            {
                for (int mapX = 0; mapX < _map[mapY].Length; mapX++)
                {
                    if (_map[mapY][mapX] == TileConstants.REDGE)
                    {
                        int bitindex = 0;

                        for (int z = 0; z < 4; z++)
                        {
                            bitindex <<= 1;
                            int xtem = mapX + DX[z];
                            int ytem = mapY + DY[z];
                            if (_engine.TestBounds(xtem, ytem) &&
                                ((_map[ytem][xtem] & TileConstants.LOMASK) != TileConstants.DIRT) &&
                                (((_map[ytem][xtem] & TileConstants.LOMASK) < TileConstants.WOODS_LOW) ||
                                 ((_map[ytem][xtem] & TileConstants.LOMASK) > TileConstants.WOODS_HIGH)))
                            {
                                bitindex |= 1;
                            }
                        }

                        var temp = (char) RED_TAB[bitindex & 15];
                        if ((temp != TileConstants.RIVER) && _prng.Next(2) != 0)
                            temp++;
                        _map[mapY][mapX] = temp;
                    }
                }
            }
        }

        private void DoTrees()
        {
            int amount;

            if (_treeLevel < 0)
            {
                amount = _prng.Next(101) + 50;
            }
            else
            {
                amount = _treeLevel + 3;
            }

            for (int x = 0; x < amount; x++)
            {
                int xloc = _prng.Next(GetWidth());
                int yloc = _prng.Next(GetHeight());
                TreeSplash(xloc, yloc);
            }

            SmoothTrees();
            SmoothTrees();
        }

        private void TreeSplash(int xloc, int yloc)
        {
            int dis;
            if (_treeLevel < 0)
            {
                dis = _prng.Next(151) + 50;
            }
            else
            {
                dis = _prng.Next(101 + (_treeLevel*2)) + 50;
            }

            _mapX = xloc;
            _mapY = yloc;

            for (int z = 0; z < dis; z++)
            {
                int dir = _prng.Next(8);
                MoveMap(dir);

                if (!_engine.TestBounds(_mapX, _mapY))
                    return;

                if ((_map[_mapY][_mapX] & TileConstants.LOMASK) == TileConstants.DIRT)
                {
                    _map[_mapY][_mapX] = (char) TileConstants.WOODS;
                }
            }
        }

        private void MoveMap(int dir)
        {
            dir = dir & 7;
            _mapX += DIRECTION_TAB_X[dir];
            _mapY += DIRECTION_TAB_Y[dir];
        }

        /// <summary>
        ///     Smoothes the trees to avoid sharp corners.
        /// </summary>
        private void SmoothTrees()
        {
            for (int mapY = 0; mapY < _map.Length; mapY++)
            {
                for (int mapX = 0; mapX < _map[mapY].Length; mapX++)
                {
                    if (TileConstants.IsTree(_map[mapY][mapX]))
                    {
                        int bitindex = 0;
                        for (int z = 0; z < 4; z++)
                        {
                            bitindex <<= 1;
                            int xtem = mapX + DX[z];
                            int ytem = mapY + DY[z];
                            if (_engine.TestBounds(xtem, ytem) &&
                                TileConstants.IsTree(_map[ytem][xtem]))
                            {
                                bitindex |= 1;
                            }
                        }
                        int temp = TED_TAB[bitindex & 15];
                        if (temp != 0)
                        {
                            if (temp != TileConstants.WOODS)
                            {
                                if (((mapX + mapY) & 1) != 0)
                                {
                                    temp -= 8;
                                }
                            }
                            _map[mapY][mapX] = (char) temp;
                        }
                        else
                        {
                            _map[mapY][mapX] = (char) temp;
                        }
                    }
                }
            }
        }

        #region level

        /// <summary>
        ///     level for river curviness; -1==auto, 0==none, >0==level
        /// </summary>
        private int _curveLevel = -1;

        /// <summary>
        ///     level for lake creation; -1==auto, 0==none, >0==level
        /// </summary>
        private int _lakeLevel = -1;

        /// <summary>
        ///     Level for tree creation
        /// </summary>
        /// <remarks>
        ///     Level for tree creation.
        ///     If positive, this is (roughly) the number of trees to randomly place.
        ///     If negative, then the number of trees is randomly chosen.
        ///     If zero, then no trees are generated.
        /// </remarks>
        private int _treeLevel = -1;

        #endregion
    }
}