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
    ///     TranslatedToolEffect describes effects caused by a tool usage
    /// </summary>
    public class TranslatedToolEffect : IToolEffectIfc
    {
        private readonly IToolEffectIfc _baseEffect;
        private readonly int _dx;
        private readonly int _dy;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TranslatedToolEffect" /> class.
        /// </summary>
        /// <param name="baseEffect">The base effect.</param>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        public TranslatedToolEffect(IToolEffectIfc baseEffect, int dx, int dy)
        {
            _baseEffect = baseEffect;
            _dx = dx;
            _dy = dy;
        }


        /// <summary>
        ///     Gets the tile.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        /// <remarks>implements IToolEffectIfc</remarks>
        public int GetTile(int x, int y)
        {
            return _baseEffect.GetTile(x + _dx, y + _dy);
        }

        /// <summary>
        ///     Makes the sound.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="sound">The sound.</param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void MakeSound(int x, int y, Sound sound)
        {
            _baseEffect.MakeSound(x + _dx, y + _dy, sound);
        }


        /// <summary>
        ///     Sets the tile.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="tileValue">The tile value.</param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void SetTile(int x, int y, int tileValue)
        {
            _baseEffect.SetTile(x + _dx, y + _dy, tileValue);
        }


        /// <summary>
        ///     Deduct an amount from the controller's cash funds.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void Spend(int amount)
        {
            _baseEffect.Spend(amount);
        }


        /// <summary>
        ///     Tools the result.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <remarks>implements IToolEffectIfc</remarks>
        public void ToolResult(ToolResult tr)
        {
            _baseEffect.ToolResult(tr);
        }
    }
}