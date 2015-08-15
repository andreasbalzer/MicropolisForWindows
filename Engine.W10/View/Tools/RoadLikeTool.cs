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
    ///     The road like tool is used to create roads, power wires and train rails.
    /// </summary>
    public class RoadLikeTool : ToolStroke
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoadLikeTool" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="tool">The tool.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public RoadLikeTool(Micropolis city, MicropolisTool tool, int xpos, int ypos)
            : base(city, tool, xpos, ypos)
        {
        }

        /// <summary>
        ///     Applies the tool to the effect.
        /// </summary>
        /// <param name="eff">The eff.</param>
        protected override void ApplyArea(IToolEffectIfc eff) // bug: check whether override or new
        {
            for (;;)
            {
                if (!ApplyForward(eff))
                {
                    break;
                }
                if (!ApplyBackward(eff))
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Applies the effect backward.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        private bool ApplyBackward(IToolEffectIfc eff)
        {
            bool anyChange = false;

            CityRect b = GetBounds();
            for (int i = b.Height - 1; i >= 0; i--)
            {
                for (int j = b.Width - 1; j >= 0; j--)
                {
                    var tte = new TranslatedToolEffect(eff, b.X + j, b.Y + i);
                    anyChange = anyChange || ApplySingle(tte);
                }
            }
            return anyChange;
        }

        /// <summary>
        ///     Applies the effect forward.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        private bool ApplyForward(IToolEffectIfc eff)
        {
            bool anyChange = false;

            CityRect b = GetBounds();
            for (int i = 0; i < b.Height; i++)
            {
                for (int j = 0; j < b.Width; j++)
                {
                    var tte = new TranslatedToolEffect(eff, b.X + j, b.Y + i);
                    anyChange = anyChange || ApplySingle(tte);
                }
            }
            return anyChange;
        }

        /// <summary>
        ///     Gets the bounds.
        /// </summary>
        /// <returns></returns>
        public override CityRect GetBounds() // bug: check whether override or new
        {
            // constrain bounds to be a rectangle with
            // either width or height equal to one.

            //assert tool.getWidth() == 1;
            //assert tool.getHeight() == 1;

            if (Math.Abs(Xdest - Xpos) >= Math.Abs(Ydest - Ypos))
            {
                // horizontal line
                var r = new CityRect
                {
                    X = Math.Min(Xpos, Xdest),
                    Width = Math.Abs(Xdest - Xpos) + 1,
                    Y = Ypos,
                    Height = 1
                };
                return r;
            }
            else
            {
                // vertical line
                var r = new CityRect
                {
                    X = Xpos,
                    Width = 1,
                    Y = Math.Min(Ypos, Ydest),
                    Height = Math.Abs(Ydest - Ypos) + 1
                };
                return r;
            }
        }

        /// <summary>
        ///     Applies the single.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Unexpected tool:  + tool</exception>
        private bool ApplySingle(IToolEffectIfc eff)
        {
            if (Tool == MicropolisTools.MicropolisTool["RAIL"])
            {
                return ApplyRailTool(eff);
            }
            if (Tool == MicropolisTools.MicropolisTool["ROADS"])
            {
                return ApplyRoadTool(eff);
            }
            if (Tool == MicropolisTools.MicropolisTool["WIRE"])
            {
                return ApplyWireTool(eff);
            }
            throw new Exception("Unexpected tool: " + Tool);
        }

        /// <summary>
        ///     Applies the rail tool.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        private bool ApplyRailTool(IToolEffectIfc eff)
        {
            if (LayRail(eff))
            {
                FixZone(eff);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Applies the road tool.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        private bool ApplyRoadTool(IToolEffectIfc eff)
        {
            if (LayRoad(eff))
            {
                FixZone(eff);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Applies the wire tool.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        private bool ApplyWireTool(IToolEffectIfc eff)
        {
            if (LayWire(eff))
            {
                FixZone(eff);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Lays the rail.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        private bool LayRail(IToolEffectIfc eff)
        {
            const int railCost = 20;
            const int tunnelCost = 100;

            int cost = railCost;

            var tile = (char) (eff.GetTile(0, 0) & TileConstants.LOMASK);
            tile = TileConstants.NeutralizeRoad(tile);

            bool continueCheck = true;
            // rail on water
            if (tile == TileConstants.RIVER || tile == TileConstants.REDGE || tile == TileConstants.CHANNEL)
            {
                cost = tunnelCost;

                // check east
                if (continueCheck)
                {
                    char eTile = TileConstants.NeutralizeRoad(eff.GetTile(1, 0));
                    if (eTile == TileConstants.RAILHPOWERV ||
                        eTile == TileConstants.HRAIL ||
                        (eTile >= TileConstants.LHRAIL && eTile <= TileConstants.HRAILROAD))
                    {
                        eff.SetTile(0, 0, TileConstants.HRAIL);
                        continueCheck = false;
                    }
                }

                // check west
                if (continueCheck)
                {
                    char wTile = TileConstants.NeutralizeRoad(eff.GetTile(-1, 0));
                    if (wTile == TileConstants.RAILHPOWERV ||
                        wTile == TileConstants.HRAIL ||
                        (wTile > TileConstants.VRAIL && wTile < TileConstants.VRAILROAD))
                    {
                        eff.SetTile(0, 0, TileConstants.HRAIL);
                        continueCheck = false;
                    }
                }

                // check south
                if (continueCheck)
                {
                    char sTile = TileConstants.NeutralizeRoad(eff.GetTile(0, 1));
                    if (sTile == TileConstants.RAILVPOWERH ||
                        sTile == TileConstants.VRAILROAD ||
                        (sTile > TileConstants.HRAIL && sTile < TileConstants.HRAILROAD))
                    {
                        eff.SetTile(0, 0, TileConstants.VRAIL);
                        continueCheck = false;
                    }
                }

                // check north
                if (continueCheck)
                {
                    char nTile = TileConstants.NeutralizeRoad(eff.GetTile(0, -1));
                    if (nTile == TileConstants.RAILVPOWERH ||
                        nTile == TileConstants.VRAILROAD ||
                        (nTile > TileConstants.HRAIL && nTile < TileConstants.HRAILROAD))
                    {
                        eff.SetTile(0, 0, TileConstants.VRAIL);
                        continueCheck = false;
                    }
                }

                // cannot do road here
                if (continueCheck)
                {
                    return false;
                }
            }
            else if (tile == TileConstants.LHPOWER)
            {
                // rail on power
                eff.SetTile(0, 0, TileConstants.RAILVPOWERH);
            }
            else if (tile == TileConstants.LVPOWER)
            {
                // rail on power
                eff.SetTile(0, 0, TileConstants.RAILHPOWERV);
            }

            else if (tile == TileConstants.ROADS)
            {
                // rail on road (case 1)
                eff.SetTile(0, 0, TileConstants.VRAILROAD);
            }

            else if (tile == TileConstants.ROADS2)
            {
                // rail on road (case 2)
                eff.SetTile(0, 0, TileConstants.HRAILROAD);
            }
            else
            {
                if (tile != TileConstants.DIRT)
                {
                    if (City.AutoBulldoze && TileConstants.CanAutoBulldozeRrw(tile))
                    {
                        cost += 1; //autodoze cost
                    }
                    else
                    {
                        // cannot do rail here
                        return false;
                    }
                }

                //rail on dirt
                eff.SetTile(0, 0, TileConstants.LHRAIL);
            }

            eff.Spend(cost);
            return true;
        }


        /// <summary>
        ///     Lays the road.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        private bool LayRoad(IToolEffectIfc eff)
        {
            const int roadCost = 10;
            const int bridgeCost = 50;

            int cost = roadCost;

            var tile = (char) (eff.GetTile(0, 0) & TileConstants.LOMASK);

            bool continueCheck = true;
            //road on water // check how to build bridges, if possible
            if (tile == TileConstants.RIVER || tile == TileConstants.REDGE || tile == TileConstants.CHANNEL)
            {
                cost = bridgeCost;

                // check east
                if (continueCheck)
                {
                    char eTile = TileConstants.NeutralizeRoad(eff.GetTile(1, 0));
                    if (eTile == TileConstants.VRAILROAD ||
                        eTile == TileConstants.HBRIDGE ||
                        (eTile >= TileConstants.ROADS && eTile <= TileConstants.HROADPOWER))
                    {
                        eff.SetTile(0, 0, TileConstants.HBRIDGE);
                        continueCheck = false;
                    }
                }

                // check west
                if (continueCheck)
                {
                    char wTile = TileConstants.NeutralizeRoad(eff.GetTile(-1, 0));
                    if (wTile == TileConstants.VRAILROAD ||
                        wTile == TileConstants.HBRIDGE ||
                        (wTile >= TileConstants.ROADS && wTile <= TileConstants.INTERSECTION))
                    {
                        eff.SetTile(0, 0, TileConstants.HBRIDGE);
                        continueCheck = false;
                    }
                }

                // check south
                if (continueCheck)
                {
                    char sTile = TileConstants.NeutralizeRoad(eff.GetTile(0, 1));
                    if (sTile == TileConstants.HRAILROAD ||
                        sTile == TileConstants.VROADPOWER ||
                        (sTile >= TileConstants.VBRIDGE && sTile <= TileConstants.INTERSECTION))
                    {
                        eff.SetTile(0, 0, TileConstants.VBRIDGE);
                        continueCheck = false;
                    }
                }

                // check north
                if (continueCheck)
                {
                    char nTile = TileConstants.NeutralizeRoad(eff.GetTile(0, -1));
                    if (nTile == TileConstants.HRAILROAD ||
                        nTile == TileConstants.VROADPOWER ||
                        (nTile >= TileConstants.VBRIDGE && nTile <= TileConstants.INTERSECTION))
                    {
                        eff.SetTile(0, 0, TileConstants.VBRIDGE);
                        continueCheck = false;
                    }
                }

                // cannot do road here
                if (continueCheck)
                {
                    return false;
                }
            }
            else if (tile == TileConstants.LHPOWER)
            {
                //road on power
                eff.SetTile(0, 0, TileConstants.VROADPOWER);
            }
            else if (tile == TileConstants.LVPOWER)
            {
                //road on power #2
                eff.SetTile(0, 0, TileConstants.HROADPOWER);
            }
            else if (tile == TileConstants.LHRAIL)
            {
                //road on rail
                eff.SetTile(0, 0, TileConstants.HRAILROAD);
            }
            else if (tile == TileConstants.LVRAIL)
            {
                //road on rail #2
                eff.SetTile(0, 0, TileConstants.VRAILROAD);
            }
            else
            {
                if (tile != TileConstants.DIRT)
                {
                    if (City.AutoBulldoze && TileConstants.CanAutoBulldozeRrw(tile))
                    {
                        cost += 1; //autodoze cost
                    }
                    else
                    {
                        // cannot do road here
                        return false;
                    }
                }

                // road on dirt;
                // just build a plain road, fixZone will fix it.
                eff.SetTile(0, 0, TileConstants.ROADS);
            }

            eff.Spend(cost);
            return true;
        }

        /// <summary>
        ///     Lays the wire.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        private bool LayWire(IToolEffectIfc eff)
        {
            const int wireCost = 5;
            const int underwaterWireCost = 25;

            int cost = wireCost;

            var tile = (char) (eff.GetTile(0, 0) & TileConstants.LOMASK);
            tile = TileConstants.NeutralizeRoad(tile);

            bool continueCheck = true;

            // wire on water
            if (tile == TileConstants.RIVER || tile == TileConstants.REDGE || tile == TileConstants.CHANNEL)
            {
                cost = underwaterWireCost;

                // check east
                if (continueCheck)
                {
                    int tmp = eff.GetTile(1, 0);
                    char tmpn = TileConstants.NeutralizeRoad(tmp);

                    if (TileConstants.IsConductive(tmp) &&
                        tmpn != TileConstants.HROADPOWER &&
                        tmpn != TileConstants.RAILHPOWERV &&
                        tmpn != TileConstants.HPOWER)
                    {
                        eff.SetTile(0, 0, TileConstants.VPOWER);
                        continueCheck = false;
                    }
                }

                // check west
                if (continueCheck)
                {
                    int tmp = eff.GetTile(-1, 0);
                    char tmpn = TileConstants.NeutralizeRoad(tmp);

                    if (TileConstants.IsConductive(tmp) &&
                        tmpn != TileConstants.HROADPOWER &&
                        tmpn != TileConstants.RAILHPOWERV &&
                        tmpn != TileConstants.HPOWER)
                    {
                        eff.SetTile(0, 0, TileConstants.VPOWER);
                        continueCheck = false;
                    }
                }

                // check south
                if (continueCheck)
                {
                    int tmp = eff.GetTile(0, 1);
                    char tmpn = TileConstants.NeutralizeRoad(tmp);

                    if (TileConstants.IsConductive(tmp) &&
                        tmpn != TileConstants.VROADPOWER &&
                        tmpn != TileConstants.RAILVPOWERH &&
                        tmpn != TileConstants.VPOWER)
                    {
                        eff.SetTile(0, 0, TileConstants.HPOWER);
                        continueCheck = false;
                    }
                }

                // check north
                if (continueCheck)
                {
                    int tmp = eff.GetTile(0, -1);
                    char tmpn = TileConstants.NeutralizeRoad(tmp);

                    if (TileConstants.IsConductive(tmp) &&
                        tmpn != TileConstants.VROADPOWER &&
                        tmpn != TileConstants.RAILVPOWERH &&
                        tmpn != TileConstants.VPOWER)
                    {
                        eff.SetTile(0, 0, TileConstants.HPOWER);
                        continueCheck = false;
                    }
                }


                // cannot do wire here
                if (continueCheck)
                {
                    return false;
                }
            }
            else if (tile == TileConstants.ROADS)
            {
                // wire on E/W road
                eff.SetTile(0, 0, TileConstants.HROADPOWER);
            }
            else if (tile == TileConstants.ROADS2)
            {
                // wire on N/S road
                eff.SetTile(0, 0, TileConstants.VROADPOWER);
            }
            else if (tile == TileConstants.LHRAIL)
            {
                // wire on E/W railroad tracks
                eff.SetTile(0, 0, TileConstants.RAILHPOWERV);
            }
            else if (tile == TileConstants.LVRAIL)
            {
                // wire on N/S railroad tracks
                eff.SetTile(0, 0, TileConstants.RAILVPOWERH);
            }
            else
            {
                if (tile != TileConstants.DIRT)
                {
                    if (City.AutoBulldoze && TileConstants.CanAutoBulldozeRrw(tile))
                    {
                        cost += 1; //autodoze cost
                    }
                    else
                    {
                        //cannot do wire here
                        return false;
                    }
                }

                //wire on dirt
                eff.SetTile(0, 0, TileConstants.LHPOWER);
            }

            eff.Spend(cost);
            return true;
        }
    }
}