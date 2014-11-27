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
    ///     Effect a tool has when it is used
    /// </summary>
    public class ToolEffect : IToolEffectIfc
    {
        private readonly Micropolis _city;
        private readonly int _originX;
        private readonly int _originY;
        public ToolPreview Preview;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToolEffect" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        public ToolEffect(Micropolis city)
            : this(city, 0, 0)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToolEffect" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public ToolEffect(Micropolis city, int xpos, int ypos)
        {
            _city = city;
            Preview = new ToolPreview();
            _originX = xpos;
            _originY = ypos;
        }


        /// <summary>
        ///     Gets the tile at a relative location.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns>a non-negative tile identifier</returns>
        /// <remarks>implements IToolEffectIfc</remarks>
        public int GetTile(int dx, int dy)
        {
            int c = Preview.GetTile(dx, dy);
            if (c != TileConstants.CLEAR)
            {
                return c;
            }

            if (_city.TestBounds(_originX + dx, _originY + dy))
            {
                return _city.GetTile(_originX + dx, _originY + dy);
            }
            // tiles outside city's boundary assumed to be
            // tile #0 (dirt).
            return 0;
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
            Preview.MakeSound(dx, dy, sound);
        }


        /// <summary>
        ///     Sets the tile value at a relative location.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="tileValue"></param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void SetTile(int dx, int dy, int tileValue)
        {
            Preview.SetTile(dx, dy, tileValue);
        }


        /// <summary>
        ///     Deduct an amount from the controller's cash funds.
        /// </summary>
        /// <param name="amount"></param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void Spend(int amount)
        {
            Preview.Spend(amount);
        }


        /// <summary>
        ///     Tools the result.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void ToolResult(ToolResult tr)
        {
            Preview.ToolResult(tr);
        }

        /// <summary>
        ///     Applies this instance.
        /// </summary>
        /// <returns></returns>
        public ToolResult Apply()
        {
            if (_originX - Preview.OffsetX < 0 ||
                _originX - Preview.OffsetX + Preview.GetWidth() > _city.GetWidth() ||
                _originY - Preview.OffsetY < 0 ||
                _originY - Preview.OffsetY + Preview.GetHeight() > _city.GetHeight())
            {
                return Engine.ToolResult.UH_OH;
            }

            if (_city.Budget.TotalFunds < Preview.Cost)
            {
                return Engine.ToolResult.INSUFFICIENT_FUNDS;
            }

            bool anyFound = false;
            for (int y = 0; y < Preview.Tiles.Length; y++)
            {
                for (int x = 0; x < Preview.Tiles[y].Length; x++)
                {
                    int c = Preview.Tiles[y][x];
                    if (c != TileConstants.CLEAR)
                    {
                        _city.SetTile(_originX + x - Preview.OffsetX, _originY + y - Preview.OffsetY, (char) c);
                        anyFound = true;
                    }
                }
            }

            foreach (SoundInfo si in Preview.Sounds)
            {
                _city.MakeSound(si.X, si.Y, si.Sound);
            }

            if (anyFound && Preview.Cost != 0)
            {
                _city.Spend(Preview.Cost);
                return Engine.ToolResult.SUCCESS;
            }
            return Preview.ToolResultField;
        }
    }
}