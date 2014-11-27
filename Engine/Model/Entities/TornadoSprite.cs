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
    ///     A tornado moved across land and destroys everything in its path. This is the graphical representation.
    /// </summary>
    public class TornadoSprite : Sprite
    {
        private static readonly int[] CDx = {2, 3, 2, 0, -2, -3};
        private static readonly int[] CDy = {-2, 0, 2, 3, 2, 0};

        public int Count;
        public bool Flag;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TornadoSprite" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public TornadoSprite(Micropolis engine, int xpos, int ypos)
            : base(engine, SpriteKinds.SpriteKind["TOR"])
        {
            X = xpos*16 + 8;
            Y = ypos*16 + 8;
            Width = 48;
            Height = 48;
            Offx = -24;
            Offy = -40;

            Frame = 1;
            Count = 200;
        }

        /// <summary>
        ///     For subclasses to override.
        ///     Actually does the movement and animation of this particular sprite.
        ///     Setting this.frame to zero will cause the sprite to be unallocated.
        /// </summary>
        protected override void MoveImpl()
        {
            int z = Frame;

            if (z == 2)
            {
                //cycle animation

                if (Flag)
                    z = 3;
                else
                    z = 1;
            }
            else
            {
                Flag = (z == 1);
                z = 2;
            }

            if (Count > 0)
            {
                Count--;
            }

            Frame = z;

            foreach (Sprite s in City.AllSprites())
            {
                if (CheckSpriteCollision(s) &&
                    (s.Kind == SpriteKinds.SpriteKind["AIR"] ||
                     s.Kind == SpriteKinds.SpriteKind["COP"] ||
                     s.Kind == SpriteKinds.SpriteKind["SHI"] ||
                     s.Kind == SpriteKinds.SpriteKind["TRA"])
                    )
                {
                    s.ExplodeSprite();
                }
            }

            int zz = City.Prng.Next(CDx.Length);
            X += CDx[zz];
            Y += CDy[zz];

            if (!City.TestBounds(X/16, Y/16))
            {
                // out of bounds
                Frame = 0;
                return;
            }

            if (Count == 0 && City.Prng.Next(501) == 0)
            {
                // early termination
                Frame = 0;
                return;
            }

            DestroyTile(X/16, Y/16);
        }

        public override string ToString()
        {
            return "X: " + X + ", Y: " + Y;
        }
    }
}