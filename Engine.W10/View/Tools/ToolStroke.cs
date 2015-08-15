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
    ///     The graphical representation of a tool
    /// </summary>
    public class ToolStroke
    {
        protected Micropolis City;

        /// <summary>
        ///     The in preview flag specifies whether this stroke is used for a preview or whether it is placed on the map
        /// </summary>
        protected bool InPreview;

        /// <summary>
        ///     The tool of this stroke
        /// </summary>
        public MicropolisTool Tool;

        /// <summary>
        ///     The xdest
        /// </summary>
        public int Xdest;

        /// <summary>
        ///     The xpos
        /// </summary>
        public int Xpos;

        /// <summary>
        ///     The ydest
        /// </summary>
        public int Ydest;

        /// <summary>
        ///     The ypos
        /// </summary>
        public int Ypos;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToolStroke" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="tool">The tool.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public ToolStroke(Micropolis city, MicropolisTool tool, int xpos, int ypos)
        {
            City = city;
            Tool = tool;
            Xpos = xpos;
            Ypos = ypos;
            Xdest = xpos;
            Ydest = ypos;
        }

        /// <summary>
        ///     Gets the preview.
        /// </summary>
        /// <returns></returns>
        public ToolPreview GetPreview()
        {
            var eff = new ToolEffect(City);
            InPreview = true;
            try
            {
                ApplyArea(eff);
            }
            finally
            {
                InPreview = false;
            }
            return eff.Preview;
        }

        /// <summary>
        ///     Applies this instance.
        /// </summary>
        /// <returns></returns>
        public ToolResult Apply()
        {
            var eff = new ToolEffect(City);
            ApplyArea(eff);
            return eff.Apply();
        }

        /// <summary>
        ///     Applies the area.
        /// </summary>
        /// <param name="eff">The eff.</param>
        protected virtual void ApplyArea(IToolEffectIfc eff)
        {
            CityRect r = GetBounds();

            for (int i = 0; i < r.Height; i += Tool.GetHeight())
            {
                for (int j = 0; j < r.Width; j += Tool.GetWidth())
                {
                    Apply1(new TranslatedToolEffect(eff, r.X + j, r.Y + i));
                }
            }
        }

        /// <summary>
        ///     Apply1s the specified eff.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        /// <exception cref="Exception">unexpected tool:  + tool</exception>
        public bool Apply1(IToolEffectIfc eff)
        {
            if (Tool == MicropolisTools.MicropolisTool["PARK"])
            {
                return ApplyParkTool(eff);
            }
            if (Tool == MicropolisTools.MicropolisTool["RESIDENTIAL"])
            {
                return ApplyZone(eff, TileConstants.RESCLR);
            }
            if (Tool == MicropolisTools.MicropolisTool["COMMERCIAL"])
            {
                return ApplyZone(eff, TileConstants.COMCLR);
            }
            if (Tool == MicropolisTools.MicropolisTool["INDUSTRIAL"])
            {
                return ApplyZone(eff, TileConstants.INDCLR);
            }
            if (Tool == MicropolisTools.MicropolisTool["FIRE"])
            {
                return ApplyZone(eff, TileConstants.FIRESTATION);
            }
            if (Tool == MicropolisTools.MicropolisTool["POLICE"])
            {
                return ApplyZone(eff, TileConstants.POLICESTATION);
            }
            if (Tool == MicropolisTools.MicropolisTool["POWERPLANT"])
            {
                return ApplyZone(eff, TileConstants.POWERPLANT);
            }
            if (Tool == MicropolisTools.MicropolisTool["STADIUM"])
            {
                return ApplyZone(eff, TileConstants.STADIUM);
            }
            if (Tool == MicropolisTools.MicropolisTool["SEAPORT"])
            {
                return ApplyZone(eff, TileConstants.PORT);
            }
            if (Tool == MicropolisTools.MicropolisTool["NUCLEAR"])
            {
                return ApplyZone(eff, TileConstants.NUCLEAR);
            }
            if (Tool == MicropolisTools.MicropolisTool["AIRPORT"])
            {
                return ApplyZone(eff, TileConstants.AIRPORT);
            }
            // not expected
            throw new Exception("unexpected tool: " + Tool);
        }


        /// <summary>
        ///     Drag to specified coordinate.
        /// </summary>
        /// <param name="xdest">The xdest.</param>
        /// <param name="ydest">The ydest.</param>
        public void DragTo(int xdest, int ydest)
        {
            Xdest = xdest;
            Ydest = ydest;
        }

        /// <summary>
        ///     Gets the bounds of the associated tool.
        /// </summary>
        /// <returns></returns>
        public virtual CityRect GetBounds()
        {
            if (Tool.GetWidth() == 0 || Tool.GetHeight() == 0) // in case we have no tool selected.
            {
                return new CityRect(0,0,0,0);
            }

            var r = new CityRect {X = Xpos};

            if (Tool.GetWidth() >= 3)
            {
                r.X--;
            }
            if (Xdest >= Xpos)
            {
                r.Width = ((Xdest - Xpos)/Tool.GetWidth() + 1)*Tool.GetWidth();
            }
            else
            {
                r.Width = ((Xpos - Xdest)/Tool.GetWidth() + 1)*Tool.GetHeight();
                r.X += Tool.GetWidth() - r.Width;
            }

            r.Y = Ypos;
            if (Tool.GetHeight() >= 3)
            {
                r.Y--;
            }
            if (Ydest >= Ypos)
            {
                r.Height = ((Ydest - Ypos)/Tool.GetHeight() + 1)*Tool.GetHeight();
            }
            else
            {
                r.Height = ((Ypos - Ydest)/Tool.GetHeight() + 1)*Tool.GetHeight();
                r.Y += Tool.GetHeight() - r.Height;
            }

            return r;
        }

        /// <summary>
        ///     Gets the location.
        /// </summary>
        /// <returns></returns>
        public CityLocation GetLocation()
        {
            return new CityLocation(Xpos, Ypos);
        }

        /// <summary>
        ///     Applies the zone.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <param name="baseZone">The base zone.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Cannot applyZone to #+baseZone</exception>
        private bool ApplyZone(IToolEffectIfc eff, int baseZone)
        {
            //assert isZoneCenter(base);

            BuildingInfo bi = Tiles.Get(baseZone).GetBuildingInfo();
            if (bi == null)
            {
                throw new Exception("Cannot applyZone to #" + baseZone);
            }

            int cost = Tool.GetToolCost();
            bool canBuild = true;
            for (int rowNum = 0; rowNum < bi.Height; rowNum++)
            {
                for (int columnNum = 0; columnNum < bi.Width; columnNum++)
                {
                    int tileValue = eff.GetTile(columnNum, rowNum);
                    tileValue = tileValue & TileConstants.LOMASK;

                    if (tileValue != TileConstants.DIRT)
                    {
                        if (City.AutoBulldoze && TileConstants.CanAutoBulldozeZ((char) tileValue))
                        {
                            cost++;
                        }
                        else
                        {
                            canBuild = false;
                        }
                    }
                }
            }
            if (!canBuild)
            {
                eff.ToolResult(ToolResult.UH_OH);
                return false;
            }

            eff.Spend(cost);

            int i = 0;
            for (int rowNum = 0; rowNum < bi.Height; rowNum++)
            {
                for (int columnNum = 0; columnNum < bi.Width; columnNum++)
                {
                    eff.SetTile(columnNum, rowNum, (char) bi.Members[i]);
                    i++;
                }
            }

            FixBorder(eff, bi.Width, bi.Height);
            return true;
        }

        #region compatible function

        /// <summary>
        ///     Fixes the border.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        private void FixBorder(int left, int top, int right, int bottom)
        {
            var eff = new ToolEffect(City, left, top);
            FixBorder(eff, right + 1 - left, bottom + 1 - top);
            eff.Apply();
        }

        /// <summary>
        ///     Fixes the border.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void FixBorder(IToolEffectIfc eff, int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                FixZone(new TranslatedToolEffect(eff, x, 0));
                FixZone(new TranslatedToolEffect(eff, x, height - 1));
            }
            for (int y = 1; y < height - 1; y++)
            {
                FixZone(new TranslatedToolEffect(eff, 0, y));
                FixZone(new TranslatedToolEffect(eff, width - 1, y));
            }
        }

        /// <summary>
        ///     Applies the park tool.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <returns></returns>
        private bool ApplyParkTool(IToolEffectIfc eff)
        {
            int cost = Tool.GetToolCost();

            if (eff.GetTile(0, 0) != TileConstants.DIRT)
            {
                // some sort of bulldozing is necessary
                if (!City.AutoBulldoze)
                {
                    eff.ToolResult(ToolResult.UH_OH);
                    return false;
                }

                //FIXME- use a canAutoBulldoze-style function here
                if (TileConstants.IsRubble(eff.GetTile(0, 0)))
                {
                    // this tile can be auto-bulldozed
                    cost++;
                }
                else
                {
                    // cannot be auto-bulldozed
                    eff.ToolResult(ToolResult.UH_OH);
                    return false;
                }
            }

            int z = InPreview ? 0 : City.Prng.Next(5);
            int tile;
            if (z < 4)
            {
                tile = TileConstants.WOODS2 + z;
            }
            else
            {
                tile = TileConstants.FOUNTAIN;
            }

            eff.Spend(cost);
            eff.SetTile(0, 0, tile);

            return true;
        }

        /// <summary>
        ///     Fixes the zone.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        protected void FixZone(int xpos, int ypos)
        {
            var eff = new ToolEffect(City, xpos, ypos);
            FixZone(eff);
            eff.Apply();
        }

        /// <summary>
        ///     Fixes the zone.
        /// </summary>
        /// <param name="eff">The eff.</param>
        protected void FixZone(IToolEffectIfc eff)
        {
            fixSingle(eff);

            // "fix" the cells to the north, west, east, and south
            fixSingle(new TranslatedToolEffect(eff, 0, -1));
            fixSingle(new TranslatedToolEffect(eff, -1, 0));
            fixSingle(new TranslatedToolEffect(eff, 1, 0));
            fixSingle(new TranslatedToolEffect(eff, 0, 1));
        }

        /// <summary>
        ///     Fixes the single.
        /// </summary>
        /// <param name="eff">The eff.</param>
        private void fixSingle(IToolEffectIfc eff)
        {
            int tile = eff.GetTile(0, 0);

            if (TileConstants.IsRoadDynamic(tile))
            {
                // cleanup road
                int adjTile = 0;

                // check road to north
                if (TileConstants.RoadConnectsSouth(eff.GetTile(0, -1)))
                {
                    adjTile |= 1;
                }

                // check road to east
                if (TileConstants.RoadConnectsWest(eff.GetTile(1, 0)))
                {
                    adjTile |= 2;
                }

                // check road to south
                if (TileConstants.RoadConnectsNorth(eff.GetTile(0, 1)))
                {
                    adjTile |= 4;
                }

                // check road to west
                if (TileConstants.RoadConnectsEast(eff.GetTile(-1, 0)))
                {
                    adjTile |= 8;
                }

                eff.SetTile(0, 0, TileConstants.RoadTable[adjTile]);
            } //endif on a road tile

            else if (TileConstants.IsRailDynamic(tile))
            {
                // cleanup Rail
                int adjTile = 0;

                // check rail to north
                if (TileConstants.RailConnectsSouth(eff.GetTile(0, -1)))
                {
                    adjTile |= 1;
                }

                // check rail to east
                if (TileConstants.RailConnectsWest(eff.GetTile(1, 0)))
                {
                    adjTile |= 2;
                }

                // check rail to south
                if (TileConstants.RailConnectsNorth(eff.GetTile(0, 1)))
                {
                    adjTile |= 4;
                }

                // check rail to west
                if (TileConstants.RailConnectsEast(eff.GetTile(-1, 0)))
                {
                    adjTile |= 8;
                }

                eff.SetTile(0, 0, TileConstants.RailTable[adjTile]);
            } //end if on a rail tile

            else if (TileConstants.IsWireDynamic(tile))
            {
                // Cleanup Wire
                int adjTile = 0;

                // check wire to north
                if (TileConstants.WireConnectsSouth(eff.GetTile(0, -1)))
                {
                    adjTile |= 1;
                }

                // check wire to east
                if (TileConstants.WireConnectsWest(eff.GetTile(1, 0)))
                {
                    adjTile |= 2;
                }

                // check wire to south
                if (TileConstants.WireConnectsNorth(eff.GetTile(0, 1)))
                {
                    adjTile |= 4;
                }

                // check wire to west
                if (TileConstants.WireConnectsEast(eff.GetTile(-1, 0)))
                {
                    adjTile |= 8;
                }

                eff.SetTile(0, 0, TileConstants.WireTable[adjTile]);
            } //end if on a rail tile
        }

        #endregion
    }
}