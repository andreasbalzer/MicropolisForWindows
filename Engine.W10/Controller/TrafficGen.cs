using System;
using System.Collections.Generic;

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
    ///     Contains the code for generating city traffic.
    /// </summary>
    public class TrafficGen
    {
        private const int MaxTrafficDistance = 30;
        private static readonly int[] PerimX = {-1, 0, 1, 2, 2, 2, 1, 0, -1, -2, -2, -2};
        private static readonly int[] PerimY = {-2, -2, -2, -1, 0, 1, 2, 2, 2, 1, 0, -1};
        private static readonly int[] DX = {0, 1, 0, -1};
        private static readonly int[] DY = {-1, 0, 1, 0};
        private readonly Micropolis _city;
        private readonly Stack<CityLocation> _positions = new Stack<CityLocation>();
        private int _lastdir;

        /// <summary>
        ///     The map x-coordinate
        /// </summary>
        public int MapX;

        /// <summary>
        ///     The map y-coordinate
        /// </summary>
        public int MapY;

        /// <summary>
        ///     The source zone
        /// </summary>
        public ZoneType SourceZone;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrafficGen" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        public TrafficGen(Micropolis city)
        {
            _city = city;
        }

        /// <summary>
        ///     Makes the traffic.
        /// </summary>
        /// <returns></returns>
        public int MakeTraffic()
        {
            if (FindPerimeterRoad()) //look for road on this zone's perimeter
            {
                if (TryDrive()) //attempt to drive somewhere
                {
                    // success; incr trafdensity
                    SetTrafficMem();
                    return 1;
                }

                return 0;
            }
            // no road found
            return -1;
        }

        private void SetTrafficMem()
        {
            while (_positions.Count != 0)
            {
                CityLocation pos = _positions.Pop();
                MapX = pos.X;
                MapY = pos.Y;
                //assert city.testBounds(mapX, mapY);

                // check for road/rail
                int tile = _city.GetTile(MapX, MapY);
                if (tile >= TileConstants.ROADBASE && tile < TileConstants.POWERBASE)
                {
                    _city.AddTraffic(MapX, MapY, 50);
                }
            }
        }

        /// <summary>
        ///     Finds the perimeter road.
        /// </summary>
        /// <returns></returns>
        public bool FindPerimeterRoad()
        {
            for (int z = 0; z < 12; z++)
            {
                int tx = MapX + PerimX[z];
                int ty = MapY + PerimY[z];

                if (RoadTest(tx, ty))
                {
                    MapX = tx;
                    MapY = ty;
                    return true;
                }
            }
            return false;
        }

        private bool RoadTest(int tx, int ty)
        {
            if (!_city.TestBounds(tx, ty))
            {
                return false;
            }

            char c = _city.GetTile(tx, ty);

            if (c < TileConstants.ROADBASE)
                return false;
            if (c > TileConstants.LASTRAIL)
                return false;
            if (c >= TileConstants.POWERBASE && c < TileConstants.LASTPOWER)
                return false;
            return true;
        }

        private bool TryDrive()
        {
            _lastdir = 5;
            _positions.Clear();

            for (int z = 0; z < MaxTrafficDistance; z++) //maximum distance to try
            {
                if (TryGo(z))
                {
                    // got a road
                    if (DriveDone())
                    {
                        // destination reached
                        return true;
                    }
                }
                else
                {
                    // deadend, try backing up
                    if (_positions.Count != 0)
                    {
                        _positions.Pop();
                        z += 3;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // gone maxdis
            return false;
        }

        private bool TryGo(int z)
        {
            // random starting direction
            int rdir = _city.Prng.Next(4);

            for (int d = rdir; d < rdir + 4; d++)
            {
                int realdir = d%4;
                if (realdir == _lastdir)
                    continue;

                if (RoadTest(MapX + DX[realdir], MapY + DY[realdir]))
                {
                    MapX += DX[realdir];
                    MapY += DY[realdir];
                    _lastdir = (realdir + 2)%4;

                    if (z%2 == 1)
                    {
                        // save pos every other move
                        _positions.Push(new CityLocation(MapX, MapY));
                    }

                    return true;
                }
            }

            return false;
        }

        private bool DriveDone()
        {
            int low, high;
            switch (SourceZone)
            {
                case ZoneType.RESIDENTIAL:
                    low = TileConstants.COMBASE;
                    high = TileConstants.NUCLEAR;
                    break;
                case ZoneType.COMMERCIAL:
                    low = TileConstants.LHTHR;
                    high = TileConstants.PORT;
                    break;
                case ZoneType.INDUSTRIAL:
                    low = TileConstants.LHTHR;
                    high = TileConstants.COMBASE;
                    break;
                default:
                    throw new Exception("unreachable");
            }

            if (MapY > 0)
            {
                int tile = _city.GetTile(MapX, MapY - 1);
                if (tile >= low && tile <= high)
                    return true;
            }
            if (MapX + 1 < _city.GetWidth())
            {
                int tile = _city.GetTile(MapX + 1, MapY);
                if (tile >= low && tile <= high)
                    return true;
            }
            if (MapY + 1 < _city.GetHeight())
            {
                int tile = _city.GetTile(MapX, MapY + 1);
                if (tile >= low && tile <= high)
                    return true;
            }
            if (MapX > 0)
            {
                int tile = _city.GetTile(MapX - 1, MapY);
                if (tile >= low && tile <= high)
                    return true;
            }
            return false;
        }
    }
}