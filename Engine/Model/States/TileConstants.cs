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
    ///     Contains symbolic names of certain tile values,
    ///     and helper functions to test tile attributes.
    ///     Attributes of tiles that are interesting:
    ///     <ul>
    ///         <li>
    ///             ZONE - the special tile for a zone
    ///         </li>
    ///         <li>
    ///             ANIM - the tile animates
    ///         </li>
    ///         <li>
    ///             BULL - is bulldozable
    ///         </li>
    ///         <li>
    ///             BURN - is combustible
    ///         </li>
    ///         <li>
    ///             COND - can conduct power
    ///         </li>
    ///         <li>
    ///             Road - traffic
    ///         </li>
    ///         <li>
    ///             Rail - railroad
    ///         </li>
    ///         <li>
    ///             Floodable - subject to floods
    ///         </li>
    ///         <li>
    ///             Wet
    ///         </li>
    ///         <li>
    ///             Rubble
    ///         </li>
    ///         <li>
    ///             Tree
    ///         </li>
    ///         <li>
    ///             OverWater
    ///         </li>
    ///         <li>
    ///             Arsonable
    ///         </li>
    ///         <li>
    ///             Vulnerable - vulnerable to earthquakes
    ///         </li>
    ///         <li>
    ///             Bridge
    ///         </li>
    ///         <li>
    ///             AutoDozeRRW - automatically bulldoze when
    ///             placing Road/Rail/Wire
    ///         </li>
    ///         <li>AutoDozeZ - automatically bulldoze when placing Zone</li>
    ///     </ul>
    /// </summary>
    public class TileConstants
    {
        //
        // terrain mapping
        //
// ReSharper disable InconsistentNaming
        public static short CLEAR = -1;
        public static int DIRT = 0;
        public static int RIVER = 2;
        public static int REDGE = 3;
        public static int CHANNEL = 4;
        public static int RIVEDGE = 5;
        public static int FIRSTRIVEDGE = 5;
        public static int LASTRIVEDGE = 20;
        public static int TREEBASE = 21;
        public static int WOODS_LOW = TREEBASE;
        public static int WOODS = 37;
        public static int WOODS_HIGH = 39;
        public static int WOODS2 = 40;
        public static int WOODS5 = 43;
        public static int RUBBLE = 44;
        public static int LASTRUBBLE = 47;
        public static int FLOOD = 48;
        public static int LASTFLOOD = 51;
        public static int RADTILE = 52;
        public static int FIRE = 56;
        public static int ROADBASE = 64;
        public static int HBRIDGE = 64;
        public static int VBRIDGE = 65;
        public static int ROADS = 66;
        public static int ROADS2 = 67;
        public static int ROADS3 = 68;
        public static int ROADS4 = 69;
        public static int ROADS5 = 70;
        public static int ROADS6 = 71;
        public static int ROADS7 = 72;
        public static int ROADS8 = 73;
        public static int ROADS9 = 74;
        public static int ROADS10 = 75;
        public static int INTERSECTION = 76;
        public static int HROADPOWER = 77;
        public static int VROADPOWER = 78;
        public static int BRWH = 79; //horz bridge, open
        public static int LTRFBASE = 80;
        public static int BRWV = 95; //vert bridge, open
        public static int HTRFBASE = 144;
        public static int LASTROAD = 206;
        public static int POWERBASE = 208;
        public static int HPOWER = 208; //underwater power-line
        public static int VPOWER = 209;
        public static int LHPOWER = 210;
        public static int LVPOWER = 211;
        public static int LVPOWER2 = 212;
        public static int LVPOWER3 = 213;
        public static int LVPOWER4 = 214;
        public static int LVPOWER5 = 215;
        public static int LVPOWER6 = 216;
        public static int LVPOWER7 = 217;
        public static int LVPOWER8 = 218;
        public static int LVPOWER9 = 219;
        public static int LVPOWER10 = 220;
        public static int RAILHPOWERV = 221;
        public static int RAILVPOWERH = 222;
        public static int LASTPOWER = 222;
        public static int RAILBASE = 224;
        public static int HRAIL = 224; //underwater rail (horz)
        public static int VRAIL = 225; //underwater rail (vert)
        public static int LHRAIL = 226;
        public static int LVRAIL = 227;
        public static int LVRAIL2 = 228;
        public static int LVRAIL3 = 229;
        public static int LVRAIL4 = 230;
        public static int LVRAIL5 = 231;
        public static int LVRAIL6 = 232;
        public static int LVRAIL7 = 233;
        public static int LVRAIL8 = 234;
        public static int LVRAIL9 = 235;
        public static int LVRAIL10 = 236;
        public static int HRAILROAD = 237;
        public static int VRAILROAD = 238;
        public static int LASTRAIL = 238;
        public static int RESBASE = 240;
        public static int RESCLR = 244;
        public static int HOUSE = 249;
        public static int LHTHR = 249; //12 house tiles
        public static int HHTHR = 260;
        public static int RZB = 265; //residential zone base
        public static int HOSPITAL = 409;
        public static int CHURCH = 418;
        public static int COMBASE = 423;
        public static int COMCLR = 427;
        public static int CZB = 436; //commercial zone base
        public static int INDBASE = 612;
        public static int INDCLR = 616;
        public static int IZB = 625;
        public static int PORTBASE = 693;
        public static int PORT = 698;
        public static int AIRPORT = 716;
        public static int POWERPLANT = 750;
        public static int FIRESTATION = 765;
        public static int POLICESTATION = 774;
        public static int STADIUM = 784;
        public static int FULLSTADIUM = 800;
        public static int NUCLEAR = 816;
        public static int LASTZONE = 826;
        public static int LIGHTNINGBOLT = 827;
        public static int HBRDG0 = 828; //draw bridge tiles (horz)
        public static int HBRDG1 = 829;
        public static int HBRDG2 = 830;
        public static int HBRDG3 = 831;
        public static int FOUNTAIN = 840;
        public static int TINYEXP = 860;
        public static int LASTTINYEXP = 867;
        public static int FOOTBALLGAME1 = 932;
        public static int FOOTBALLGAME2 = 940;
        public static int VBRDG0 = 948; //draw bridge tiles (vert)
        public static int VBRDG1 = 949;
        public static int VBRDG2 = 950;
        public static int VBRDG3 = 951;
        public static int LAST_TILE = 956;
        
        public static int[] RoadTable =
        {
            ROADS,
            ROADS2,
            ROADS,
            ROADS3,
            ROADS2,
            ROADS2,
            ROADS4,
            ROADS8,
            ROADS,
            ROADS6,
            ROADS,
            ROADS7,
            ROADS5,
            ROADS10,
            ROADS9,
            INTERSECTION
        };

        public static int[] RailTable =
        {
            LHRAIL,
            LVRAIL,
            LHRAIL,
            LVRAIL2,
            LVRAIL,
            LVRAIL,
            LVRAIL3,
            LVRAIL7,
            LHRAIL,
            LVRAIL5,
            LHRAIL,
            LVRAIL6,
            LVRAIL4,
            LVRAIL9,
            LVRAIL8,
            LVRAIL10
        };

        public static int[] WireTable =
        {
            LHPOWER,
            LVPOWER,
            LHPOWER,
            LVPOWER2,
            LVPOWER,
            LVPOWER,
            LVPOWER3,
            LVPOWER7,
            LHPOWER,
            LVPOWER5,
            LHPOWER,
            LVPOWER6,
            LVPOWER4,
            LVPOWER9,
            LVPOWER8,
            LVPOWER10
        };


        /// <summary>
        ///     status bits
        ///     bit 15 ... currently powered
        ///     bit 14 ... unused
        ///     bit 13 ... unused
        ///     bit 12 ... unused
        ///     bit 11 ... unused
        ///     bit 10 ... unused
        /// </summary>
        public static int PWRBIT = 32768;


        /// <summary>
        ///     mask for upper 6 bits
        /// </summary>
        public static int ALLBITS = 64512;

        /// <summary>
        ///     mask for low 10 bits
        /// </summary>
        public static int LOMASK = 1023;

        // ReSharper restore InconsistentNaming

        /// <summary>
        ///     Prevents a default instance of the <see cref="TileConstants" /> class from being created.
        /// </summary>
        private TileConstants()
        {
        }


        /// <summary>
        ///     Checks whether the tile can be auto-bulldozed for placement of road, rail, or wire.
        /// </summary>
        /// <param name="tileValue">The tile value.</param>
        /// <returns>true, if tile can be auto bulldozed, otherwise false</returns>
        public static bool CanAutoBulldozeRrw(int tileValue)
        {
            // can we autobulldoze this tile?
            return (
                (tileValue >= FIRSTRIVEDGE && tileValue <= LASTRUBBLE) ||
                (tileValue >= TINYEXP && tileValue <= LASTTINYEXP)
                );
        }


        /// <summary>
        ///     Checks whether the tile can be auto-bulldozed for placement of a zone.
        /// </summary>
        /// <param name="tileValue">The tile value.</param>
        /// <returns>true, if tile can be auto bulldozed, otherwise false</returns>
        public static bool CanAutoBulldozeZ(char tileValue)
        {
            //FIXME- what is significance of POWERBASE+2 and POWERBASE+12 ?

            // can we autobulldoze this tile?
            if ((tileValue >= FIRSTRIVEDGE && tileValue <= LASTRUBBLE) ||
                (tileValue >= POWERBASE + 2 && tileValue <= POWERBASE + 12) ||
                (tileValue >= TINYEXP && tileValue <= LASTTINYEXP))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        ///     Gets the tile behavior.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>tile behavior</returns>
        /// <remarks>used by scanTile</remarks>
        /// <see cref="ScanTile" />
        public static String GetTileBehavior(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec ts = Tiles.Get(tile);
            return ts != null ? ts.GetAttribute("behavior") : null;
        }


        /// <summary>
        ///     Gets the description number.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>description number</returns>
        /// <remarks>used by queryZoneStatus</remarks>
        /// <see cref="QueryZoneStatus" />
        public static int GetDescriptionNumber(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec ts = Tiles.Get(tile);
            if (ts != null)
            {
                return ts.GetDescriptionNumber();
            }
            return -1;
        }

        /// <summary>
        ///     Gets the pollution value.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>pollution value</returns>
        public static int GetPollutionValue(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec spec = Tiles.Get(tile);
            return spec != null ? spec.GetPollutionValue() : 0;
        }

        /// <summary>
        ///     Determines whether the specified tile is animated.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if animated, otherwise false</returns>
        public static bool IsAnimated(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec spec = Tiles.Get(tile);
            return spec != null && spec.AnimNext != null;
        }


        /// <summary>
        ///     Determines whether the specified tile is arsonable.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if arsonable, otherwise false</returns>
        /// <remarks>used by setFire()</remarks>
        /// <see cref="SetFire" />
        public static bool IsArsonable(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return (
                !IsZoneCenter(tile) &&
                tile >= LHTHR &&
                tile <= LASTZONE
                );
        }


        /// <summary>
        ///     Determines whether the specified tile is bridge.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if bridge, otherwise false</returns>
        /// <remarks>used by Sprite::destroyTile</remarks>
        public static bool IsBridge(int tile)
        {
            return IsRoad(tile) && !IsCombustible(tile);
        }

        /// <summary>
        ///     Determines whether the specified tile is combustible.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if combustible, otherwise false</returns>
        public static bool IsCombustible(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec spec = Tiles.Get(tile);
            return spec != null && spec.CanBurn;
        }

        /// <summary>
        ///     Determines whether the specified tile is conductive.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if conductive, otherwise false</returns>
        public static bool IsConductive(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec spec = Tiles.Get(tile);
            return spec != null && spec.CanConduct;
        }

        /// <summary>
        ///     Determines whether the specified tile is indestructible.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns></returns>
        /// <remarks>Used in repairZone().</remarks>
        /// <see cref="RepairZone" />
        public static bool IsIndestructible(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return tile >= RUBBLE && tile < ROADBASE;
        }

        /// <summary>
        ///     Determines whether the specified tile is indestructible2.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns></returns>
        /// <remarks>Used in zonePlop().</remarks>
        /// <see cref="ZonePlop" />
        public static bool IsIndestructible2(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return tile >= FLOOD && tile < ROADBASE;
        }

        /// <summary>
        ///     Determines whether [is over water] [the specified tile].
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if over water, otherwise false</returns>
        public static bool IsOverWater(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec spec = Tiles.Get(tile);
            return spec != null && spec.OverWater;
        }

        /// <summary>
        ///     Determines whether the specified tile is rubble.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if rubble, otherwise false</returns>
        public static bool IsRubble(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return ((tile >= RUBBLE) &&
                    (tile <= LASTRUBBLE));
        }

        /// <summary>
        ///     Determines whether the specified tile is a tree.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if tree, otherwise false</returns>
        public static bool IsTree(char tile)
        {
            //assert (tile & LOMASK) == tile;

            return ((tile >= WOODS_LOW) &&
                    (tile <= WOODS_HIGH));
        }


        /// <summary>
        ///     Determines whether the specified tile is vulnerable to an earthquake.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if vulnerable, otherwise false</returns>
        public static bool IsVulnerable(int tile)
        {
            //assert (tile & LOMASK) == tile;

            if (tile < RESBASE ||
                tile > LASTZONE ||
                IsZoneCenter(tile)
                )
            {
                return false;
            }
            return true;
        }

        public static bool CheckWet(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return (tile == POWERBASE ||
                    tile == POWERBASE + 1 ||
                    tile == RAILBASE ||
                    tile == RAILBASE + 1 ||
                    tile == BRWH ||
                    tile == BRWV);
        }

        /// <summary>
        ///     Gets the zone size for the specified tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>zone size</returns>
        public static CityDimension GetZoneSizeFor(int tile)
        {
            //assert isZoneCenter(tile);
            //assert (tile & LOMASK) == tile;

            TileSpec spec = Tiles.Get(tile);
            return spec != null ? spec.GetBuildingSize() : null;
        }

        /// <summary>
        ///     Determines whether the specified tile has a construction.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if tile has construction on it, otherwise false</returns>
        public static bool IsConstructed(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return tile >= 0 && tile >= ROADBASE;
        }

        /// <summary>
        ///     Determines whether the tile is a river edge.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns></returns>
        public static bool IsRiverEdge(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return tile >= FIRSTRIVEDGE && tile <= LASTRIVEDGE;
        }

        /// <summary>
        ///     Determines whether the specified tile is dozeable.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if dozable, otherwise false</returns>
        public static bool IsDozeable(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec spec = Tiles.Get(tile);
            return spec != null && spec.CanBulldoze;
        }

        /// <summary>
        ///     Determines whether the specified tile is floodable.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if floodable, otherwise false</returns>
        public static bool IsFloodable(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return (tile == DIRT || (IsDozeable(tile) && IsCombustible(tile)));
        }


        /// <summary>
        ///     Determines whether the specified tile is road.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Note: does not include rail/road tiles.
        /// </remarks>
        /// <see cref="IsRoadAny" />
        public static bool IsRoad(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return (tile >= ROADBASE && tile < POWERBASE);
        }

        public static bool IsRoadAny(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return (tile >= ROADBASE && tile < POWERBASE)
                   || (tile == HRAILROAD)
                   || (tile == VRAILROAD);
        }


        /// <summary>
        ///     Checks whether the tile is a road that will automatically change to connect to neighboring roads.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns></returns>
        public static bool IsRoadDynamic(int tile)
        {
            int tmp = NeutralizeRoad(tile);
            return (tmp >= ROADS && tmp <= INTERSECTION);
        }

        /// <summary>
        ///     Checks whether road connects east.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if road connects east, otherwise false</returns>
        public static bool RoadConnectsEast(int tile)
        {
            tile = NeutralizeRoad(tile);
            return (((tile == VRAILROAD) ||
                     (tile >= ROADBASE && tile <= VROADPOWER)
                ) &&
                    (tile != VROADPOWER) &&
                    (tile != HRAILROAD) &&
                    (tile != VBRIDGE));
        }

        /// <summary>
        ///     Checks whether road connects north.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if road connects north, otherwise false</returns>
        public static bool RoadConnectsNorth(int tile)
        {
            tile = NeutralizeRoad(tile);
            return (((tile == HRAILROAD) ||
                     (tile >= ROADBASE && tile <= VROADPOWER)
                ) &&
                    (tile != HROADPOWER) &&
                    (tile != VRAILROAD) &&
                    (tile != ROADBASE));
        }

        /// <summary>
        ///     Checks whether road connects south.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if road connects south, otherwise false</returns>
        public static bool RoadConnectsSouth(int tile)
        {
            tile = NeutralizeRoad(tile);
            return (((tile == HRAILROAD) ||
                     (tile >= ROADBASE && tile <= VROADPOWER)
                ) &&
                    (tile != HROADPOWER) &&
                    (tile != VRAILROAD) &&
                    (tile != ROADBASE));
        }

        /// <summary>
        ///     Checks whether road connects west.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if road connects west, otherwise false</returns>
        public static bool RoadConnectsWest(int tile)
        {
            tile = NeutralizeRoad(tile);
            return (((tile == VRAILROAD) ||
                     (tile >= ROADBASE && tile <= VROADPOWER)
                ) &&
                    (tile != VROADPOWER) &&
                    (tile != HRAILROAD) &&
                    (tile != VBRIDGE));
        }

        public static bool IsRail(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return (tile >= RAILBASE && tile < RESBASE);
        }

        public static bool IsRailAny(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return (tile >= RAILBASE && tile < RESBASE)
                   || (tile == RAILHPOWERV)
                   || (tile == RAILVPOWERH);
        }

        public static bool IsRailDynamic(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return (tile >= LHRAIL && tile <= LVRAIL10);
        }

        /// <summary>
        ///     Checks whether rail connects east.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if rail connects east, otherwise false</returns>
        public static bool RailConnectsEast(int tile)
        {
            tile = NeutralizeRoad(tile);
            return (tile >= RAILHPOWERV && tile <= VRAILROAD &&
                    tile != RAILVPOWERH &&
                    tile != VRAILROAD &&
                    tile != VRAIL);
        }

        /// <summary>
        ///     Checks whether rail connects north.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if rail connects north, otherwise false</returns>
        public static bool RailConnectsNorth(int tile)
        {
            tile = NeutralizeRoad(tile);
            return (tile >= RAILHPOWERV && tile <= VRAILROAD &&
                    tile != RAILHPOWERV &&
                    tile != HRAILROAD &&
                    tile != HRAIL);
        }

        /// <summary>
        ///     Checks whether rail connects south.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if rail connects south, otherwise false</returns>
        public static bool RailConnectsSouth(int tile)
        {
            tile = NeutralizeRoad(tile);
            return (tile >= RAILHPOWERV && tile <= VRAILROAD &&
                    tile != RAILHPOWERV &&
                    tile != HRAILROAD &&
                    tile != HRAIL);
        }

        /// <summary>
        ///     Checks whether rail connects west.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if rail connects west, otherwise false</returns>
        public static bool RailConnectsWest(int tile)
        {
            tile = NeutralizeRoad(tile);
            return (tile >= RAILHPOWERV && tile <= VRAILROAD &&
                    tile != RAILVPOWERH &&
                    tile != VRAILROAD &&
                    tile != VRAIL);
        }

        public static bool IsWireDynamic(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return (tile >= LHPOWER && tile <= LVPOWER10);
        }

        /// <summary>
        ///     Checks whether wires connects east.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if wire connects east, otherwise false</returns>
        public static bool WireConnectsEast(int tile)
        {
            int ntile = NeutralizeRoad(tile);
            return (IsConductive(tile) &&
                    ntile != HPOWER &&
                    ntile != HROADPOWER &&
                    ntile != RAILHPOWERV);
        }

        /// <summary>
        ///     Checks whether wires connects north.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if wire connects north, otherwise false</returns>
        public static bool WireConnectsNorth(int tile)
        {
            int ntile = NeutralizeRoad(tile);
            return (IsConductive(tile) &&
                    ntile != VPOWER &&
                    ntile != VROADPOWER &&
                    ntile != RAILVPOWERH);
        }

        /// <summary>
        ///     Checks whether wires connects south.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if wire connects south, otherwise false</returns>
        public static bool WireConnectsSouth(int tile)
        {
            int ntile = NeutralizeRoad(tile);
            return (IsConductive(tile) &&
                    ntile != VPOWER &&
                    ntile != VROADPOWER &&
                    ntile != RAILVPOWERH);
        }

        /// <summary>
        ///     Checks whether wires connects west.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if wire connects west, otherwise false</returns>
        public static bool WireConnectsWest(int tile)
        {
            int ntile = NeutralizeRoad(tile);
            return (IsConductive(tile) &&
                    ntile != HPOWER &&
                    ntile != HROADPOWER &&
                    ntile != RAILHPOWERV);
        }

        /// <summary>
        ///     Determines whether the tile is commercial zone.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if commercial zone, otherwise false</returns>
        public static bool IsCommercialZone(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec ts = Tiles.Get(tile);
            if (ts != null)
            {
                if (ts.Owner != null)
                {
                    ts = ts.Owner;
                }
                return ts.GetboolAttribute("commercial-zone");
            }
            return false;
        }

        /// <summary>
        ///     Determines whether the tile is hospital or church.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if hospital or church, otherwise false</returns>
        public static bool IsHospitalOrChurch(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return tile >= HOSPITAL &&
                   tile < COMBASE;
        }


        /// <summary>
        ///     Checks whether the tile is defined with the "industrial-zone" attribute.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if industrial zone, otherwise false</returns>
        /// <remarks>
        ///     Note: the old version of this function erroneously included the coal power
        ///     plant smoke as an industrial zone.
        /// </remarks>
        public static bool IsIndustrialZone(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec ts = Tiles.Get(tile);
            if (ts != null)
            {
                if (ts.Owner != null)
                {
                    ts = ts.Owner;
                }
                return ts.GetboolAttribute("industrial-zone");
            }
            return false;
        }

        /// <summary>
        ///     Determines whether the tile is residential clear.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if clear, otherwise false</returns>
        public static bool IsResidentialClear(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return tile >= RESBASE && tile <= RESBASE + 8;
        }

        /// <summary>
        ///     Determines whether the tile is residential zone.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if residential zone, otherwise false</returns>
        /// <remarks>
        ///     Note: does not include hospital/church.
        /// </remarks>
        /// <see cref="IsHospitalOrChurch" />
        public static bool IsResidentialZone(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return tile >= RESBASE &&
                   tile < HOSPITAL;
        }


        /// <summary>
        ///     Determines whether the tile is residential zone.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>true, if residential zone, otherwise false</returns>
        /// <remarks>
        ///     includes hospital/church.
        /// </remarks>
        public static bool IsResidentialZoneAny(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec ts = Tiles.Get(tile);
            if (ts != null)
            {
                if (ts.Owner != null)
                {
                    ts = ts.Owner;
                }
                return ts.GetboolAttribute("residential-zone");
            }
            return false;
        }


        /// <summary>
        ///     Tile represents a part of any sort of building.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns></returns>
        public static bool IsZoneAny(int tile)
        {
            //assert (tile & LOMASK) == tile;

            return tile >= RESBASE;
        }

        /// <summary>
        ///     Determines whether the tile is zone center.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns></returns>
        public static bool IsZoneCenter(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec spec = Tiles.Get(tile);
            return spec != null && spec.Zone;
        }

        /// <summary>
        ///     Converts a road tile value with traffic to the equivalent road tile without traffic.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>road tile without traffic</returns>
        public static char NeutralizeRoad(int tile)
        {
            //assert (tile & LOMASK) == tile;

            if (tile >= ROADBASE && tile <= LASTROAD)
            {
                tile = ((tile - ROADBASE) & 0xf) + ROADBASE;
            }
            return (char) tile;
        }


        /// <summary>
        ///     Determine the population level of a Residential zone tile. Note: the input tile MUST be a full-size res zone, it
        ///     cannot be an empty zone.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>int multiple of 8 between 16 and 40.</returns>
        public static int ResidentialZonePop(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec ts = Tiles.Get(tile);
            return ts.GetPopulation();
        }


        /// <summary>
        ///     Determine the population level of a Commercial zone tile. The input tile MAY be an empty zone.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>int between 0 and 5.</returns>
        public static int CommercialZonePop(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec ts = Tiles.Get(tile);
            return ts.GetPopulation()/8;
        }


        /// <summary>
        ///     Determine the population level of an Industrial zone tile.
        ///     The input tile MAY be an empty zone.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>int between 0 and 4.</returns>
        public static int IndustrialZonePop(int tile)
        {
            //assert (tile & LOMASK) == tile;

            TileSpec ts = Tiles.Get(tile);
            return ts.GetPopulation()/8;
        }
    }
}