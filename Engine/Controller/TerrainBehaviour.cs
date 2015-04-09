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
    ///     A terrain behaviour that changes the way a tile reacts, e.g. to tools.
    /// </summary>
    public class TerrainBehavior : TileBehavior
    {
        private static readonly int[] TrafficDensityTab =
        {
            TileConstants.ROADBASE, TileConstants.LTRFBASE,
            TileConstants.HTRFBASE
        };

        private readonly BTerrainBehavior _behavior;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TerrainBehavior" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="behavior">The behavior.</param>
        public TerrainBehavior(Micropolis city, BTerrainBehavior behavior)
            : base(city)
        {
            _behavior = behavior;
        }

        /// <summary>
        ///     Activate the tile identified by xpos and ypos properties.
        /// </summary>
        public override void Apply()
        {
            switch (_behavior)
            {
                case BTerrainBehavior.FIRE:
                    DoFire();
                    return;
                case BTerrainBehavior.FLOOD:
                    DoFlood();
                    return;
                case BTerrainBehavior.RADIOACTIVE:
                    DoRadioactiveTile();
                    return;
                case BTerrainBehavior.ROAD:
                    DoRoad();
                    return;
                case BTerrainBehavior.RAIL:
                    DoRail();
                    return;
                case BTerrainBehavior.EXPLOSION:
                    DoExplosion();
                    return;
                    //assert false;
            }
        }

        /// <summary>
        ///     Does the fire.
        /// </summary>
        private void DoFire()
        {
            City.FirePop++;

            // one in four times
            if (PRNG.Next(4) != 0)
            {
                return;
            }

            int[] dx = {0, 1, 0, -1};
            int[] dy = {-1, 0, 1, 0};

            for (int dir = 0; dir < 4; dir++)
            {
                if (PRNG.Next(8) == 0)
                {
                    int xtem = Xpos + dx[dir];
                    int ytem = Ypos + dy[dir];
                    if (!City.TestBounds(xtem, ytem))
                        continue;

                    int c = City.GetTile(xtem, ytem);
                    if (TileConstants.IsCombustible(c))
                    {
                        if (TileConstants.IsZoneCenter(c))
                        {
                            City.KillZone(xtem, ytem, c);
                            if (c > TileConstants.IZB)
                            {
                                //explode
                                City.MakeExplosion(xtem, ytem);
                            }
                        }
                        City.SetTile(xtem, ytem, (char) (TileConstants.FIRE + PRNG.Next(4)));
                    }
                }
            }

            int cov = City.GetFireStationCoverage(Xpos, Ypos);
            int rate = cov > 100
                ? 1
                : cov > 20
                    ? 2
                    : cov != 0 ? 3 : 10;

            if (PRNG.Next(rate + 1) == 0)
            {
                City.SetTile(Xpos, Ypos, (char) (TileConstants.RUBBLE + PRNG.Next(4)));
            }
        }


        /// <summary>
        ///     Called when the current tile is a flooding tile.
        /// </summary>
        private void DoFlood()
        {
            int[] dx = {0, 1, 0, -1};
            int[] dy = {-1, 0, 1, 0};

            if (City.FloodCnt != 0)
            {
                for (int z = 0; z < 4; z++)
                {
                    if (PRNG.Next(8) == 0)
                    {
                        int xx = Xpos + dx[z];
                        int yy = Ypos + dy[z];
                        if (City.TestBounds(xx, yy))
                        {
                            int t = City.GetTile(xx, yy);
                            if (TileConstants.IsCombustible(t)
                                || t == TileConstants.DIRT
                                || (t >= TileConstants.WOODS5 && t < TileConstants.FLOOD))
                            {
                                if (TileConstants.IsZoneCenter(t))
                                {
                                    City.KillZone(xx, yy, t);
                                }
                                City.SetTile(xx, yy, (char) (TileConstants.FLOOD + PRNG.Next(3)));
                            }
                        }
                    }
                }
            }
            else
            {
                if (PRNG.Next(16) == 0)
                {
                    City.SetTile(Xpos, Ypos, TileConstants.DIRT);
                }
            }
        }


        /// <summary>
        ///     Called when the current tile is a radioactive tile.
        /// </summary>
        private void DoRadioactiveTile()
        {
            if (PRNG.Next(4096) == 0)
            {
                // radioactive decay
                City.SetTile(Xpos, Ypos, TileConstants.DIRT);
            }
        }

        /// <summary>
        ///     Called when the current tile is a road tile.
        /// </summary>
        private void DoRoad()
        {
            City.RoadTotal++;

            if (City.RoadEffect < 30)
            {
                // deteriorating roads
                if (PRNG.Next(512) == 0)
                {
                    if (!TileConstants.IsConductive(Tile))
                    {
                        if (City.RoadEffect < PRNG.Next(32))
                        {
                            if (TileConstants.IsOverWater(Tile))
                                City.SetTile(Xpos, Ypos, TileConstants.RIVER);
                            else
                                City.SetTile(Xpos, Ypos, (char) (TileConstants.RUBBLE + PRNG.Next(4)));
                            return;
                        }
                    }
                }
            }

            if (!TileConstants.IsCombustible(Tile)) //bridge
            {
                City.RoadTotal += 4;
                if (DoBridge())
                    return;
            }

            int tden;
            if (Tile < TileConstants.LTRFBASE)
                tden = 0;
            else if (Tile < TileConstants.HTRFBASE)
                tden = 1;
            else
            {
                City.RoadTotal++;
                tden = 2;
            }

            int trafficDensity = City.GetTrafficDensity(Xpos, Ypos);
            int newLevel = trafficDensity < 64
                ? 0
                : trafficDensity < 192 ? 1 : 2;

            //assert newLevel >= 0 && newLevel < TRAFFIC_DENSITY_TAB.Length;

            if (tden != newLevel)
            {
                int z = (((RawTile & TileConstants.LOMASK) - TileConstants.ROADBASE) & 15) +
                        TrafficDensityTab[newLevel];
                z += RawTile & TileConstants.ALLBITS;

                City.SetTile(Xpos, Ypos, (char) z);
            }
        }

        /// <summary>
        ///     Called when the current tile is railroad.
        /// </summary>
        private void DoRail()
        {
            City.RailTotal++;
            City.GenerateTrain(Xpos, Ypos);

            if (City.RoadEffect < 30)
            {
                // deteriorating rail
                if (PRNG.Next(512) == 0)
                {
                    if (!TileConstants.IsConductive(Tile))
                    {
                        if (City.RoadEffect < PRNG.Next(32))
                        {
                            if (TileConstants.IsOverWater(Tile))
                            {
                                City.SetTile(Xpos, Ypos, TileConstants.RIVER);
                            }
                            else
                            {
                                City.SetTile(Xpos, Ypos, (char) (TileConstants.RUBBLE + PRNG.Next(4)));
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        ///     Called when the current tile is a road bridge over water.
        ///     Handles the draw bridge. For the draw bridge to appear,
        ///     there must be a boat on the water, the boat must be
        ///     within a certain distance of the bridge, it must be where
        ///     the map generator placed 'channel' tiles (these are tiles
        ///     that look just like regular river tiles but have a different
        ///     numeric value), and you must be a little lucky.
        /// </summary>
        /// <returns>true if the draw bridge is open; false otherwise</returns>
        private bool DoBridge()
        {
            int[] hdX = {-2, 2, -2, -1, 0, 1, 2};
            int[] hdY = {-1, -1, 0, 0, 0, 0, 0};
            int[] hbrtab =
            {
                TileConstants.HBRDG1, TileConstants.HBRDG3,
                TileConstants.HBRDG0, TileConstants.RIVER,
                TileConstants.BRWH, TileConstants.RIVER,
                TileConstants.HBRDG2
            };
            int[] hbrtab2 =
            {
                TileConstants.RIVER, TileConstants.RIVER,
                TileConstants.HBRIDGE, TileConstants.HBRIDGE,
                TileConstants.HBRIDGE, TileConstants.HBRIDGE,
                TileConstants.HBRIDGE
            };

            int[] vDx = {0, 1, 0, 0, 0, 0, 1};
            int[] vDy = {-2, -2, -1, 0, 1, 2, 2};
            int[] vbrtab =
            {
                TileConstants.VBRDG0, TileConstants.VBRDG1,
                TileConstants.RIVER, TileConstants.BRWV,
                TileConstants.RIVER, TileConstants.VBRDG2,
                TileConstants.VBRDG3
            };
            int[] vbrtab2 =
            {
                TileConstants.VBRIDGE, TileConstants.RIVER,
                TileConstants.VBRIDGE, TileConstants.VBRIDGE,
                TileConstants.VBRIDGE, TileConstants.VBRIDGE,
                TileConstants.RIVER
            };

            if (Tile == TileConstants.BRWV)
            {
                // vertical bridge, open
                if (PRNG.Next(4) == 0 && GetBoatDis() > 340/16)
                {
                    //close the bridge
                    ApplyBridgeChange(vDx, vDy, vbrtab, vbrtab2);
                }
                return true;
            }
            if (Tile == TileConstants.BRWH)
            {
                // horizontal bridge, open
                if (PRNG.Next(4) == 0 && GetBoatDis() > 340/16)
                {
                    // close the bridge
                    ApplyBridgeChange(hdX, hdY, hbrtab, hbrtab2);
                }
                return true;
            }

            if (GetBoatDis() < 300/16 && PRNG.Next(8) == 0)
            {
                if ((Tile & 1) != 0)
                {
                    // vertical bridge
                    if (Xpos < City.GetWidth() - 1)
                    {
                        // look for CHANNEL tile to right of
                        // bridge. the CHANNEL tiles are only
                        // found in the very center of the
                        // river
                        if (City.GetTile(Xpos + 1, Ypos) == TileConstants.CHANNEL)
                        {
                            // vertical bridge, open it up
                            ApplyBridgeChange(vDx, vDy, vbrtab2, vbrtab);
                            return true;
                        }
                    }
                    return false;
                }
                // horizontal bridge
                if (Ypos > 0)
                {
                    // look for CHANNEL tile just above
                    // bridge. the CHANNEL tiles are only
                    // found in the very center of the
                    // river
                    if (City.GetTile(Xpos, Ypos - 1) == TileConstants.CHANNEL)
                    {
                        // open it up
                        ApplyBridgeChange(hdX, hdY, hbrtab2, hbrtab);
                        return true;
                    }
                }
                return false;
            }

            return false;
        }


        /// <summary>
        ///     Helper function for doBridge- it toggles the draw-bridge.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="fromTab">From tab.</param>
        /// <param name="toTab">To tab.</param>
        private void ApplyBridgeChange(int[] dx, int[] dy, int[] fromTab, int[] toTab)
        {
            //FIXME- a closed bridge with traffic on it is not
            // correctly handled by this subroutine, because the
            // the tiles representing traffic on a bridge do not match
            // the expected tile values of fromTab

            for (int z = 0; z < 7; z++)
            {
                int x = Xpos + dx[z];
                int y = Ypos + dy[z];
                if (City.TestBounds(x, y))
                {
                    if ((City.GetTile(x, y) == fromTab[z]) ||
                        (City.GetTile(x, y) == TileConstants.CHANNEL)
                        )
                    {
                        City.SetTile(x, y, toTab[z]);
                    }
                }
            }
        }


        /// <summary>
        ///     Calculate how far away the boat currently is from the current tile.
        /// </summary>
        /// <returns></returns>
        private int GetBoatDis()
        {
            int dist = 99999;
            foreach (Sprite s in City.Sprites)
            {
                if (s.IsVisible() && s.Kind == SpriteKinds.SpriteKind["SHI"])
                {
                    int x = s.X/16;
                    int y = s.Y/16;
                    int d = Math.Abs(Xpos - x) + Math.Abs(Ypos - y);
                    dist = Math.Min(d, dist);
                }
            }
            return dist;
        }


        /// <summary>
        ///     Does the explosion.
        /// </summary>
        private void DoExplosion()
        {
            // clear AniRubble
            City.SetTile(Xpos, Ypos, (char) (TileConstants.RUBBLE + PRNG.Next(4)));
        }
    }
}