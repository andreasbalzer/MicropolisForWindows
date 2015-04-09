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
    ///     Implements an explosion. 
    ///     An explosion occurs when certain sprites collide, 
    ///     or when a zone is demolished by fire. 
    /// </summary>
    public class ExplosionSprite : Sprite
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExplosionSprite" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public ExplosionSprite(Micropolis engine, int x, int y) : base(engine, SpriteKinds.SpriteKind["EXP"])
        {
            this.X = x;
            this.Y = y;
            Width = 48;
            Height = 48;
            Offx = -24;
            Offy = -24;
            Frame = 1;
        }


        /// <summary>
        ///     Actually does the movement and animation
        ///     of this explosion sprite. Setting this.frame to zero will cause the
        ///     sprite to be unallocated. Causing fire around the explosion.
        /// </summary>
        protected override void MoveImpl()
        {
            if (City.Acycle%2 == 0)
            {
                if (Frame == 1)
                {
                    City.MakeSound(X/16, Y/16, Sounds.Sound["EXPLOSION_HIGH"]);
                    City.SendMessageAt(MicropolisMessages.EXPLOSION_REPORT, X/16, Y/16);
                }
                Frame++;
            }

            if (Frame > 6)
            {
                Frame = 0;

                startFire(X/16, Y/16);
                startFire(X/16 - 1, Y/16 - 1);
                startFire(X/16 + 1, Y/16 - 1);
                startFire(X/16 - 1, Y/16 + 1);
                startFire(X/16 + 1, Y/16 + 1);
            }
        }

        /// <summary>
        ///     Starts a fire at specified coordinates.
        /// </summary>
        /// <param name="xpos">The xpos of the fire.</param>
        /// <param name="ypos">The ypos of the fire.</param>
        private void startFire(int xpos, int ypos)
        {
            if (!City.TestBounds(xpos, ypos))
                return;

            int t = City.GetTile(xpos, ypos);
            if (!TileConstants.IsCombustible(t) && t != TileConstants.DIRT)
                return;
            if (TileConstants.IsZoneCenter(t))
                return;
            City.SetTile(xpos, ypos, (char) (TileConstants.FIRE + City.Prng.Next(4)));
        }

        public override string ToString()
        {
            return ": " + X + ", Y: " + Y;
        }
    }
}