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
    ///     The Bulldozer tool, used to remove existing structures on a tile
    /// </summary>
    public class Bulldozer : ToolStroke
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Bulldozer" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public Bulldozer(Micropolis city, int xpos, int ypos)
            : base(city, MicropolisTools.MicropolisTool["BULLDOZER"], xpos, ypos)
        {
        }

        /// <summary>
        ///     Applies the bulldozer to the area. If the bulldozer is applied to the center of a zone, the whole zone is removed
        ///     with all of its buildings.
        /// </summary>
        /// <param name="eff">The eff.</param>
        protected override void ApplyArea(IToolEffectIfc eff)
        {
            CityRect b = GetBounds();

            // scan selection area for rubble, forest, etc...
            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    IToolEffectIfc subEff = new TranslatedToolEffect(eff, b.X + x, b.Y + y);
                    if (City.IsTileDozeable(subEff))
                    {
                        DozeField(subEff);
                    }
                }
            }

            // scan selection area for zones...
            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    if (TileConstants.IsZoneCenter(eff.GetTile(b.X + x, b.Y + y)))
                    {
                        DozeZone(new TranslatedToolEffect(eff, b.X + x, b.Y + y));
                    }
                }
            }
        }

        /// <summary>
        ///     Dozes the zone and makes effects such as sounds happen in the game.
        /// </summary>
        /// <param name="eff">The eff.</param>
        private void DozeZone(IToolEffectIfc eff)
        {
            int currTile = eff.GetTile(0, 0);

            // zone center bit is set
            //assert isZoneCenter(currTile);

            CityDimension dim = TileConstants.GetZoneSizeFor(currTile);
            //assert dim != null;
            //assert dim.width >= 3;
            //assert dim.height >= 3;

            eff.Spend(1);

            // make explosion sound;
            // bigger zones => bigger explosions

            if (dim.Width*dim.Height < 16)
            {
                eff.MakeSound(0, 0, Sounds.Sound["EXPLOSION_HIGH"]);
            }
            else if (dim.Width*dim.Height < 36)
            {
                eff.MakeSound(0, 0, Sounds.Sound["EXPLOSION_LOW"]);
            }
            else
            {
                eff.MakeSound(0, 0, Sounds.Sound["EXPLOSION_BOTH"]);
            }

            PutRubble(new TranslatedToolEffect(eff, -1, -1), dim.Width, dim.Height);
        }

        /// <summary>
        ///     Dozes the field.
        /// </summary>
        /// <param name="eff">The eff.</param>
        private void DozeField(IToolEffectIfc eff)
        {
            int tile = eff.GetTile(0, 0);

            if (TileConstants.IsOverWater(tile))
            {
                // dozing over water, replace with water.
                eff.SetTile(0, 0, TileConstants.RIVER);
            }
            else
            {
                // dozing on land, replace with land. Simple, eh?
                eff.SetTile(0, 0, TileConstants.DIRT);
            }

            FixZone(eff);
            eff.Spend(1);
        }

        /// <summary>
        ///     Puts the rubble into the IToolEffectIfc eff.
        /// </summary>
        /// <param name="eff">The eff.</param>
        /// <param name="w">The width.</param>
        /// <param name="h">The height.</param>
        private void PutRubble(IToolEffectIfc eff, int w, int h)
        {
            for (int yy = 0; yy < h; yy++)
            {
                for (int xx = 0; xx < w; xx++)
                {
                    int tile = eff.GetTile(xx, yy);
                    if (tile == TileConstants.CLEAR)
                        continue;

                    if (tile != TileConstants.RADTILE && tile != TileConstants.DIRT)
                    {
                        int z = InPreview ? 0 : City.Prng.Next(3);
                        int nTile = TileConstants.TINYEXP + z;
                        eff.SetTile(xx, yy, nTile);
                    }
                }
            }
            FixBorder(eff, w, h);
        }
    }
}