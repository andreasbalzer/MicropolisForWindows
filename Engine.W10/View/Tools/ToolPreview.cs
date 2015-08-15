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
    ///     Preview that sticks to mouse cursor when tool is selected and moved across the map
    /// </summary>
    public class ToolPreview : IToolEffectIfc
    {
        /// <summary>
        ///     The cost of this tool
        /// </summary>
        public int Cost;

        /// <summary>
        ///     The offset x to top left origin of map
        /// </summary>
        public int OffsetX;

        /// <summary>
        ///     The offset y to top left origin of map
        /// </summary>
        public int OffsetY;

        /// <summary>
        ///     The sounds of this tool
        /// </summary>
        public List<SoundInfo> Sounds;

        /// <summary>
        ///     The tiles affected
        /// </summary>
        public int[][] Tiles;

        /// <summary>
        ///     The tool result field
        /// </summary>
        public ToolResult ToolResultField;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToolPreview" /> class.
        /// </summary>
        public ToolPreview()
        {
            Tiles = new int[0][]; //0 0

            Sounds = new List<SoundInfo>();
            ToolResultField = Engine.ToolResult.NONE;
        }


        /// <summary>
        ///     Gets the tile at a relative location.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <returns>
        ///     a non-negative tile identifier
        /// </returns>
        /// <remarks>implements IToolEffectIfc</remarks>
        public int GetTile(int dx, int dy)
        {
            if (InRange(dx, dy))
            {
                return Tiles[OffsetY + dy][OffsetX + dx];
            }
            return TileConstants.CLEAR;
        }

        /// <summary>
        ///     Makes the sound.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="sound">The sound.</param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void MakeSound(int dx, int dy, Sound sound)
        {
            Sounds.Add(new SoundInfo(dx, dy, sound));
        }

        /// <summary>
        ///     Sets the tile value at a relative location.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="tileValue">The tile value.</param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void SetTile(int dx, int dy, int tileValue)
        {
            ExpandTo(dx, dy);
            Tiles[OffsetY + dy][OffsetX + dx] = (short) tileValue;
        }


        /// <summary>
        ///     Deduct an amount from the controller's cash funds.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void Spend(int amount)
        {
            Cost += amount;
        }


        /// <summary>
        ///     Tools the result.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void ToolResult(ToolResult tr)
        {
            ToolResultField = tr;
        }

        /// <summary>
        ///     Gets the bounds.
        /// </summary>
        /// <returns></returns>
        public CityRect GetBounds()
        {
            return new CityRect(
                -OffsetX,
                -OffsetY,
                GetWidth(),
                GetHeight()
                );
        }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            return Tiles.Length != 0 ? Tiles[0].Length : 0;
        }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            return Tiles.Length;
        }

        /// <summary>
        ///     Ins the range.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <returns></returns>
        private bool InRange(int dx, int dy)
        {
            return OffsetY + dy >= 0 &&
                   OffsetY + dy < GetHeight() &&
                   OffsetX + dx >= 0 &&
                   OffsetX + dx < GetWidth();
        }

        /// <summary>
        ///     Expands to.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        private void ExpandTo(int dx, int dy)
        {
            if (Tiles == null || Tiles.Length == 0)
            {
                Tiles = new int[1][];
                Tiles[0] = new int[1];

                Tiles[0][0] = TileConstants.CLEAR;
                OffsetX = -dx;
                OffsetY = -dy;
                return;
            }

            // expand each existing row as needed
            for (int i = 0; i < Tiles.Length; i++)
            {
                int[] a = Tiles[i];
                if (OffsetX + dx >= a.Length)
                {
                    int newLen = OffsetX + dx + 1;
                    var aa = new int[newLen];
                    Array.Copy(a, 0, aa, 0, a.Length);
                    Arrays.Fill(aa, a.Length, newLen, TileConstants.CLEAR);
                    Tiles[i] = aa;
                }
                else if (OffsetX + dx < 0)
                {
                    int addl = -(OffsetX + dx);
                    int newLen = a.Length + addl;
                    var aa = new int[newLen];
                    Array.Copy(a, 0, aa, addl, a.Length);
                    Arrays.Fill(aa, 0, addl, TileConstants.CLEAR);
                    Tiles[i] = aa;
                }
            }

            if (OffsetX + dx < 0)
            {
                int addl = -(OffsetX + dx);
                OffsetX += addl;
            }

            int width = Tiles[0].Length;
            if (OffsetY + dy >= Tiles.Length)
            {
                int newLen = OffsetY + dy + 1;
                var newTiles = new int[newLen][];
                for (int i = 0; i < newTiles.Length; i++)
                {
                    newTiles[i] = new int[width];
                }
                Array.Copy(Tiles, 0, newTiles, 0, Tiles.Length);
                for (int i = Tiles.Length; i < newLen; i++)
                {
                    Arrays.Fill(newTiles[i], TileConstants.CLEAR);
                }
                Tiles = newTiles;
            }
            else if (OffsetY + dy < 0)
            {
                int addl = -(OffsetY + dy);
                int newLen = Tiles.Length + addl;
                var newTiles = new int[newLen][];
                for (int i = 0; i < newTiles.Length; i++)
                {
                    newTiles[i] = new int[width];
                }

                Array.Copy(Tiles, 0, newTiles, addl, Tiles.Length);
                for (int i = 0; i < addl; i++)
                {
                    Arrays.Fill(newTiles[i], TileConstants.CLEAR);
                }
                Tiles = newTiles;
                OffsetY += addl;
            }
        }
    }
}